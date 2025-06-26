using System.Security.Cryptography;
using System.Text;

namespace BankMore.Core.Utils.Extensions;

public static class StringExtensions
{
    public static string OnlyNumbers(this string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        var chr = input.Where(char.IsDigit);
        return new string([.. chr]);
    }

    public static string ComputeMd5Hash(this string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes);
    }
}