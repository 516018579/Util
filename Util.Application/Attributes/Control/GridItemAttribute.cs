using System;

namespace Util.Application.Attributes.Control
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GridItemAttribute : Attribute
    {
        public GridItemAttribute()
        {
        }

        public GridItemAttribute(int sort = 0, int rowSpan = 1, int colSpan = 1, string width = null, string align = null, string halign = null, string formatter = null, string editor = null, bool isEdit = false)
        {
            RowSpan = rowSpan;
            ColSpan = colSpan;
            Width = width;
            Align = align;
            Halign = halign;
            Formatter = formatter;
            Editor = editor;
            Sort = sort;
            IsEdit = isEdit;
        }

        public int RowSpan { get; set; }
        public int ColSpan { get; set; }
        public string Width { get; set; }
        public string Align { get; set; }
        public string Halign { get; set; }
        public string Formatter { get; set; }
        public string Editor { get; set; }
        public bool IsEdit { get; set; }
        public int? Sort { get; set; }
    }
}
