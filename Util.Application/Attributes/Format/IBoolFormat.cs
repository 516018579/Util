namespace Util.Application.Attributes.Format
{
    public interface IBoolFormat
    {
        string GetText(bool value);

        bool GetValue(string value);
    }
}
