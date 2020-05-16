using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace Util.Abp.Doamin.Entities
{
    /// <summary>
    /// 树形结构实体
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public interface ITree<TEntity, TPrimaryKey> : IEntity<TPrimaryKey>
        where TPrimaryKey : struct
        where TEntity : class
    {
        string Code { get; set; }
        [ForeignKey(nameof(ParentId))]
        TEntity Parent { get; set; }
        ICollection<TEntity> Children { get; set; }
        TPrimaryKey? ParentId { get; set; }
    }
}
