using System;
using System.Linq;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Digests;

namespace AdventOfCode2016
{
    static class Toolbox
    {

        public static string GetHashString(string input, MD5Digest bouncyMd5)
        {
            //todo: convert to bouncy castle for better speed
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytes = input.Select(Convert.ToByte).ToArray();
            byte[] hashedBytes = md5.ComputeHash(bytes);

            return string.Join("", hashedBytes.Select(b => b.ToString("x2")));
        }
    }
}
