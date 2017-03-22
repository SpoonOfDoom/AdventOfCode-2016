﻿using System.Collections.Generic;
using Org.BouncyCastle.Crypto.Digests;

namespace AdventOfCode2016.Days
{
    // ReSharper disable once UnusedMember.Global
    class Day14 : Day
    {
        public Day14() : base(14) {}

        private Toolbox toolbox = new Toolbox();
        private MD5Digest md5 = new MD5Digest();

        private string GetHash(string input)
        {
            return toolbox.GetHashString(input, md5);
        }

        private List<int> FindKeyIndices(string input)
        {
            var indices = new List<int>();

            return indices;
        }

        protected override object GetSolutionPart1()
        {
            return base.GetSolutionPart1();
        }

        protected override object GetSolutionPart2()
        {
            return base.GetSolutionPart2();
        }
    }
}
