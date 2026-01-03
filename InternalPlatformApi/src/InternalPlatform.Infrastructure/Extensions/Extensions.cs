using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace InternalPlatform.Infrastructure.Extensions;

public static class Extensions
{
    public static string ToThaiMonthName(this int month)
    {
        return month switch
        {
            1 => "มกราคม",
            2 => "กุมภาพันธ์",
            3 => "มีนาคม",
            4 => "เมษายน",
            5 => "พฤษภาคม",
            6 => "มิถุนายน",
            7 => "กรกฎาคม",
            8 => "สิงหาคม",
            9 => "กันยายน",
            10 => "ตุลาคม",
            11 => "พฤศจิกายน",
            12 => "ธันวาคม",
            _ => throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.")
        };
    }

    public static string Decrypt(string value)
    {
        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(value);
        string _decrypted = string.Empty;
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SecretKey") ?? throw new InvalidOperationException("SecretKey environment variable is not set."));
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        _decrypted = streamReader.ReadToEnd();
                    }
                }
            }
        }
        return _decrypted;
    }

    public static string Encrypt(string value)
    {
        byte[] iv = new byte[16];
        byte[] array;
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SecretKey") ?? throw new InvalidOperationException("SecretKey environment variable is not set."));
            aes.IV = iv;
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(value);
                    }
                    array = memoryStream.ToArray();
                }
            }
        }
        return Convert.ToBase64String(array);
    }

    public static string ToLikeSQL(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return "%%";
        else
            return $"%{value}%";
    }
}