namespace BankMore.Core.Utils.Extensions;

public static class StringExtensions
{
    public static string OnlyNumbers(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var chr = value.Where(char.IsDigit);

        return new string([.. chr]);
    }
}