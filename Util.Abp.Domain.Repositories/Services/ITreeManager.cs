using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Util.Abp.Doamin.Entities;

namespace Util.Abp.Domain.Repositories.Services
{
    public interface ITreeManager<TEntity, TPrimaryKey> where TEntity : class, ITree<TEntity, TPrimaryKey> where TPrimaryKey : struct
    {
        Task CreateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default(CancellationToken));
        Task UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default(CancellationToken));
        Task<string> GetNextChildCodeAsync(TPrimaryKey? parentId);
        TEntity GetLastChildOrNull(TPrimaryKey? parentId);
        Task<string> GetCodeAsync(TPrimaryKey id);
        Task DeleteAsync(TPrimaryKey id, bool autoSave = false, CancellationToken cancellationToken = default(CancellationToken));
        Task MoveAsync(TPrimaryKey id, TPrimaryKey? parentId);
        Task<List<TEntity>> FindChildrenAsync(TPrimaryKey? parentId, bool recursive = false);

        /// <summary>
        /// 获取根节点的code
        /// </summary>
        /// <param name="id">子节点id</param>
        /// <returns></returns>
        Task<string> GetRootCode(TPrimaryKey id);

        /// <summary>
        /// 获取根节点
        /// </summary>
        /// <param name="id">子节点id</param>
        /// <returns></returns>
        Task<TEntity> GetRootAsync(TPrimaryKey id);

        /// <summary>
        /// 获取所有父节点
        /// </summary>
        /// <param name="id">子节点id</param>
        /// <param name="hasSelf">是否包含子节点</param>
        /// <returns></returns>
        Task<List<TEntity>> GetAllParentListAsync(TPrimaryKey id, bool hasSelf = false);
    }
}
