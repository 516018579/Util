using System;

namespace Util.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class UniqueAttribute : Attribute
    {
        public UniqueAttribute()
        {

        }

        public UniqueAttribute(string indexName)
        {
            IndexName = indexName;
        }

        public string IndexName { get; set; }
    }
}
