using System;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto.Digests;

namespace AdventOfCode2016.Days
{
    // ReSharper disable once UnusedMember.Global
    class Day14 : Day
    {
        public Day14() : base(14) {}

        private MD5Digest md5 = new MD5Digest();

        private string GetHash(string input)
        {
            return Toolbox.GetHashString(input, md5);
        }

        private List<int> FindKeyIndices(string input)
        {
            throw new NotImplementedException();
            var indices = new List<int>();

            return indices;
        }

        
    }
}
