using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using System.Text.Json;
using System.Security.Cryptography;

public static class ObjectExtensions
{
    public static T Clone<T>(this T obj)
    {
        return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(obj));
    }
}
public static class Tools
{
    private static SemaphoreSlim semaphoreGQLEndpoint = new SemaphoreSlim(1, 1);
    private static readonly SHA1 _sha1 = SHA1.Create();
    public static DateTime UnixTimeStampMsToDateTime(double unixTimeStamp)
    {
        // The unix timestamp is how many seconds since the epoch time
        // so just substract
        var epochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        var dateTime = epochDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }

    public static Dictionary<string, object> ObjectToDictionary(object obj)
    {
        Dictionary<string, object> ret = new Dictionary<string, object>();

        foreach (System.Reflection.PropertyInfo prop in obj.GetType().GetProperties())
        {
            string propName = prop.Name;
            var val = obj.GetType().GetProperty(propName).GetValue(obj, null);
            if (val != null)
            {
                ret.Add(propName, val);
            }
            else
            {
                ret.Add(propName, null);
            }
        }

        return ret;
    }

    public static string Truncate(this string value, int maxChars)
    {
        return value.Length <= maxChars ? value : value.Substring(0, maxChars) + (char)0x2026;
    }

    public static byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
            .Where(x => x % 2 == 0)
            .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
            .ToArray();
    }

    public static string? GetSubDomain(Uri url)
    {

        if (url.HostNameType == UriHostNameType.Dns)
        {
            var host = url.Host;
            if (host.Split('.').Length > 2)
            {
                var lastIndex = host.LastIndexOf(".");
                var index = host.LastIndexOf(".", lastIndex - 1);
                return host.Substring(0, index);
            }
        }

        return null;
    }

    public static byte[] GetHashSHA1(string inputString)
    {
        return _sha1.ComputeHash(Encoding.UTF8.GetBytes(inputString));
    }

    public static string GetHashSHA1String(string inputString)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in GetHashSHA1(inputString))
        {
            sb.Append(b.ToString("X2"));
        }

        return sb.ToString();
    }

    public static string ConvertHashrateToString(double hashrate)
    {
        string ret = "";

        if (hashrate > 1000000000000000.0) ret = String.Format("{0} PH/s", Math.Round(hashrate / 1000000000000000.0, 2));
        else if (hashrate > 1000000000000.0) ret = String.Format("{0} TH/s", Math.Round(hashrate / 1000000000000.0, 2));
        else if (hashrate > 1000000000.0) ret = String.Format("{0} GH/s", Math.Round(hashrate / 1000000000.0, 2));
        else if (hashrate > 1000000.0) ret = String.Format("{0} MH/s", Math.Round(hashrate / 1000000.0, 2));
        else if (hashrate > 1000.0) ret = String.Format("{0} KH/s", Math.Round(hashrate / 1000.0, 2));
        else ret = String.Format("{0} H/s", Math.Round(hashrate, 2));

        return ret;
    }

    public static string ConvertDifficultyToString(double difficulty)
    {
        string ret = "";

        if (difficulty > 1000000000000000000.0) ret = String.Format("{0} EH", Math.Round(difficulty / 1000000000000000000.0, 2));
        else if (difficulty > 1000000000000000.0) ret = String.Format("{0} PH", Math.Round(difficulty / 1000000000000000.0, 2));
        else if (difficulty > 1000000000000.0) ret = String.Format("{0} TH", Math.Round(difficulty / 1000000000000.0, 2));
        else if (difficulty > 1000000000.0) ret = String.Format("{0} GH", Math.Round(difficulty / 1000000000.0, 2));
        else if (difficulty > 1000000.0) ret = String.Format("{0} MH", Math.Round(difficulty / 1000000.0, 2));
        else if (difficulty > 1000.0) ret = String.Format("{0} KH", Math.Round(difficulty / 1000.0, 2));
        else ret = String.Format("{0} H", Math.Round(difficulty, 2));

        return ret;
    }
}
