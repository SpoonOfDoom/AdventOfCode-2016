using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AdventOfCode2016.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNumeric(this string s)
        {
            long a;
            return long.TryParse(s, out a);
        }

        public static int ToInt(this string s)
        {
            return int.Parse(s);
        }

        public static long ToLong(this string s)
        {
            return long.Parse(s);
        }

        public static string Md5Hash(this string s, MD5CryptoServiceProvider md5 = null)
        {
	        if (md5 == null)
	        {
		        md5 = new MD5CryptoServiceProvider();
	        }
	        var bytes = s.Select(Convert.ToByte).ToArray();
	        var hashedBytes = md5.ComputeHash(bytes);
			StringBuilder sb = new StringBuilder();

	        foreach (byte b in hashedBytes)
	        {
		        sb.Append(b.ToString("x2"));
	        }
            return sb.ToString();
        }
    }
}
