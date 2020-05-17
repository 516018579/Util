using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Util.Abp.Doamin.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace Util.Abp.Domain.Repositories.Services
{
    public class TreeManager
    {
        public static int DefaultCodeLength { get; set; } = 5;

        /// <summary>
        /// Creates code for given numbers.
        /// Example: if numbers are 4,2 then returns "00004.00002";
        /// </summary>
        /// <param name="numbers">Numbers</param>
        public static string CreateCode<TEntity>(params int[] numbers)
        {
            if (numbers.IsNullOrEmpty())
            {
                return null;
            }

            var codeLength = DefaultCodeLength;

            try
            {
                codeLength = (int)typeof(TEntity).GetField("CodeLength").GetRawConstantValue();
            }
            catch (Exception e)
            {
                // ignored
            }

            return numbers.Select(number => number.ToString(new string('0', codeLength))).JoinAsString(".");
        }

        /// <summary>
        /// Appends a child code to a parent code. 
        /// Example: if parentCode = "00001", childCode = "00042" then returns "00001.00042".
        /// </summary>
        /// <param name="parentCode">Parent code. Can be null or empty if parent is a root.</param>
        /// <param name="childCode">Child code.</param>
        public static string AppendCode(string parentCode, string childCode)
        {
            if (childCode.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(childCode), "childCode can not be null or empty.");
            }

            if (parentCode.IsNullOrEmpty())
            {
                return childCode;
            }

            return parentCode + "." + childCode;
        }

        /// <summary>
        /// Gets relative code to the parent.
        /// Example: if code = "00019.00055.00001" and parentCode = "00019" then returns "00055.00001".
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="parentCode">The parent code.</param>
        public static string GetRelativeCode(string code, string parentCode)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            if (parentCode.IsNullOrEmpty())
            {
                return code;
            }

            if (code.Length == parentCode.Length)
            {
                return null;
            }

            return code.Substring(parentCode.Length + 1);
        }

        /// <summary>
        /// Calculates next code for given code.
        /// Example: if code = "00019.00055.00001" returns "00019.00055.00002".
        /// </summary>
        /// <param name="code">The code.</param>
        public static string CalculateNextCode<TEntity>(string code)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            var parentCode = GetParentCode(code);
            var lastUnitCode = GetLastUnitCode(code);

            return AppendCode(parentCode, CreateCode<TEntity>(Convert.ToInt32(lastUnitCode) + 1));
        }

        /// <summary>
        /// Gets the last unit code.
        /// Example: if code = "00019.00055.00001" returns "00001".
        /// </summary>
        /// <param name="code">The code.</param>
        public static string GetLastUnitCode(string code)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            var splittedCode = code.Split('.');
            return splittedCode[splittedCode.Length - 1];
        }

        /// <summary>
        /// Gets parent code.
        /// Example: if code = "00019.00055.00001" returns "00019.00055".
        /// </summary>
        /// <param name="code">The code.</param>
        public static string GetParentCode(string code)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            var splittedCode = code.Split('.');
            if (splittedCode.Length == 1)
            {
                return null;
            }

            return splittedCode.Take(splittedCode.Length - 1).JoinAsString(".");
        }
    }

    public class TreeManager<TEntity, TPrimaryKey> : DomainService, ITreeManager<TEntity, TPrimaryKey>
        where TEntity : class, ITree<TEntity, TPrimaryKey>
        where TPrimaryKey : struct
    {
        protected IRepository<TEntity, TPrimaryKey> Repository { get; private set; }
        public TreeManager(IRepository<TEntity, TPrimaryKey> repository)
        {
            Repository = repository;
        }

        [UnitOfWork]
        public virtual async Task CreateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            entity.Code = await GetNextChildCodeAsync(entity.ParentId);

            await Repository.InsertAsync(entity, autoSave, cancellationToken);
        }

        public virtual async Task UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Repository.UpdateAsync(entity, autoSave, cancellationToken);
        }

        public virtual async Task<string> GetNextChildCodeAsync(TPrimaryKey? parentId)
        {
            var lastChild = GetLastChildOrNull(parentId);
            if (lastChild == null)
            {
                var parentCode = parentId != null ? await GetCodeAsync(parentId.Value) : null;
                return TreeManager.AppendCode(parentCode, TreeManager.CreateCode<TEntity>(1));
            }

            return TreeManager.CalculateNextCode<TEntity>(lastChild.Code);
        }

        public virtual TEntity GetLastChildOrNull(TPrimaryKey? parentId)
        {
            var children = Repository.Where(ou => ou.ParentId.Equals(parentId)).ToList();
            return children.OrderBy(c => c.Code).LastOrDefault();
        }

        public virtual async Task<string> GetCodeAsync(TPrimaryKey id)
        {
            return (await Repository.GetAsync(id)).Code;
        }

        [UnitOfWork]
        public virtual async Task DeleteAsync(TPrimaryKey id, bool autoSave = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var children = await FindChildrenAsync(id, true);

            foreach (var child in children)
            {
                await Repository.DeleteAsync(child, autoSave, cancellationToken);
            }

            await Repository.DeleteAsync(id, autoSave, cancellationToken);
        }

        [UnitOfWork]
        public virtual async Task MoveAsync(TPrimaryKey id, TPrimaryKey? parentId)
        {
            var organizationUnit = await Repository.GetAsync(id);
            if (organizationUnit.ParentId.Equals(parentId))
            {
                return;
            }

            //Should find children before Code change
            var children = await FindChildrenAsync(id, true);

            //Store old code of OU
            var oldCode = organizationUnit.Code;

            //Move OU
            organizationUnit.Code = await GetNextChildCodeAsync(parentId);
            organizationUnit.ParentId = parentId;

            //Update Children Codes
            foreach (var child in children)
            {
                child.Code = TreeManager.AppendCode(organizationUnit.Code, TreeManager.GetRelativeCode(child.Code, oldCode));
            }
        }

        public async Task<List<TEntity>> FindChildrenAsync(TPrimaryKey? parentId, bool recursive = false)
        {
            if (!recursive)
            {
                return Repository.Where(ou => ou.ParentId.Equals(parentId)).ToList();
            }

            if (!parentId.HasValue)
            {
                return await Repository.GetListAsync();
            }

            var code = await GetCodeAsync(parentId.Value);

            return Repository.Where(
                ou => ou.Code.StartsWith(code) && !ou.Id.Equals(parentId.Value)
            ).ToList();
        }


        public async Task<string> GetRootCode(TPrimaryKey id)
        {
            var child = await Repository.GetAsync(id);

            return child.Code.Split('.')[0];
        }

        public async Task<TEntity> GetRootAsync(TPrimaryKey id)
        {
            var rootCode = await GetRootCode(id);

            return Repository.First(x => x.Code == rootCode);
        }

        public async Task<List<TEntity>> GetAllParentListAsync(TPrimaryKey id, bool hasSelf = false)
        {
            var child = await Repository.GetAsync(id);

            return Repository.Where(x => child.Code.StartsWith(x.Code) && (hasSelf || !x.ParentId.Equals(child.ParentId))).ToList();
        }
    }


}
