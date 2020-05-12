using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Util.Abp.EntityFrameworkCore
{
    public static class AbpEfCoreExtensions
    {
        #region Insert

        public static async Task InsertAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, IEnumerable<TEntity> list) where TEntity : class, IEntity<TPrimaryKey>
        {
            if (list != null)
            {
                foreach (var entity in list)
                {
                    await repository.InsertAsync(entity);
                }
            }
        }
        #endregion

        #region Update

        public static async Task UpdateAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, TPrimaryKey key, Action<TEntity> updateAction) where TEntity : class, IEntity<TPrimaryKey>
        {
            var entity = await repository.GetAsync(key);
            updateAction(entity);
            await repository.UpdateAsync(entity);
        }


        public static async Task UpdateAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, IEnumerable<TEntity> list) where TEntity : class, IEntity<TPrimaryKey>
        {
            foreach (var item in list)
            {
                await repository.UpdateAsync(item);
            }
        }


        public static async Task UpdateAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, IEnumerable<TPrimaryKey> keys, Action<TEntity> updateAction) where TEntity : class, IEntity<TPrimaryKey>
        {
            foreach (var key in keys)
            {
                await repository.UpdateAsync(key, updateAction);
            }
        }


        public static Task UpdateAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, Expression<Func<TEntity, bool>> where, Action<TEntity> updateAction) where TEntity : class, IEntity<TPrimaryKey>
        {
            var ids = repository.Where(where).Select(x => x.Id).ToList();
            return repository.UpdateAsync(ids, updateAction);
        }

        #endregion

        #region Delete


        public static async Task DeleteAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, IEnumerable<TEntity> list) where TEntity : class, IEntity<TPrimaryKey>
        {
            if (list != null)
            {
                foreach (var entity in list)
                {
                    await repository.DeleteAsync(entity);
                }
            }
        }
        #endregion
    }
}
