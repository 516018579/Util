using System.ComponentModel;

namespace Util.Domain.Entities
{
    public interface IHasName
    {
        [DisplayName("名称")]
        string Name { get; set; }
    }
}
