using System.ComponentModel;

namespace Util.Domain.Entities
{
    public interface IHasRemark
    {
        [DisplayName("备注")]
        string Remark { get; set; }
    }
}
