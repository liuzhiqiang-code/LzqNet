namespace LzqNet.Common.Utils;

public static class StringExtensions
{
    public static long? ToInt64(this string value)
    {
        return long.TryParse(value, out long result) ? result : null;
    }

    public static long ToInt64(this string value, long defaultValue = 0)
    {
        return long.TryParse(value, out long result) ? result : defaultValue;
    }

    public static DateTime? ToDateTime(this string value)
    {
        try
        {
            return DateTime.Parse(value);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
