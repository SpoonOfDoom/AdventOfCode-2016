using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Extensions;

namespace AdventOfCode2016.Days
{
    // ReSharper disable once UnusedMember.Global
    class Day20 : Day
    {
        public Day20() : base(20) {}

        class IpRange
        {
            public long Start;
            public long End;

            public long Count => End - Start;

            public bool Contains(long ip)
            {
                return ip >= Start && ip <= End;
            }
        }

        private List<IpRange> ParseBlockedIps(List<string> inputLines)
        {
            List<IpRange> ipRanges = new List<IpRange>(inputLines.Count);
            foreach (string line in inputLines)
            {
                string[] parts = line.Split('-');
                ipRanges.Add(new IpRange
                {
                    Start = parts[0].ToLong(),
                    End = parts[1].ToLong()
                });
            }
            return ipRanges;
        }

        private List<IpRange> CombineIpRanges(List<IpRange> ipRanges)
        {
            var ipRangeList = new List<IpRange>(ipRanges).ToList();
            List<IpRange> orderedIpRanges = ipRangeList.OrderBy(item => item.Start).ThenBy(item => item.End).ToList();
            //List<IpRange> combinedIpRanges = new List<IpRange>();
            int i = orderedIpRanges.Count - 1;
            IpRange a, b, c;
            while (i > 0)
            {
                b = orderedIpRanges[i];
                a = orderedIpRanges[i - 1];

                if (a.End >= b.Start-1)
                {
                    //combinedIpRanges.Add(new IpRange
                    //{
                    //    Start = Math.Min(a.Start, b.Start),
                    //    End = Math.Max(a.End, b.End)
                    //});
                    b.End = Math.Max(b.End, a.End);
                    b.Start = Math.Min(a.Start, b.Start);
                    orderedIpRanges.RemoveAt(i - 1);
                    i = Math.Max(0, orderedIpRanges.Count - 1);
                }
                else
                {
                    i--;
                }
            }
            
            return orderedIpRanges;
        }

        private long FindLowestFreeIp(List<string> inputLines)
        {
            List<IpRange> blockedIpRanges = ParseBlockedIps(inputLines);
            List<IpRange> combinedIpRanges = CombineIpRanges(blockedIpRanges);
            long maxIp = blockedIpRanges.Max(ip => ip.End);
            long lowestFreeIp = 0;
            foreach (IpRange ipRange in combinedIpRanges)
            {
                if (ipRange.Contains(lowestFreeIp))
                {
                    lowestFreeIp = ipRange.End + 1;
                    continue;
                }
                return lowestFreeIp;
            }
            return -1;
        }
        private long FindAllowedIpCount(List<string> inputLines, long maxValidIp)
        {
            List<IpRange> blockedIpRanges = ParseBlockedIps(inputLines);
            List<IpRange> combinedIpRanges = CombineIpRanges(blockedIpRanges);
            long freeCount = 0;
            long freeIp = 0;
            foreach (IpRange ipRange in combinedIpRanges)
            {
                freeCount += ipRange.Start - freeIp;
                freeIp = ipRange.End + 1;
            }
            long remainingIps = Math.Max(0, maxValidIp - freeIp);
            return freeCount + remainingIps;
        }


        protected override object GetSolutionPart1()
        {
            /*
             * --- Day 20: Firewall Rules ---

                You'd like to set up a small hidden computer here so you can use it to get back into the network later. However, the corporate firewall
                only allows communication with certain external IP addresses.

                You've retrieved the list of blocked IPs from the firewall, but the list seems to be messy and poorly maintained, and it's not clear which IPs are allowed.
                Also, rather than being written in dot-decimal notation, they are written as plain 32-bit integers, which can have any value from 0 through 4294967295, inclusive.

                For example, suppose only the values 0 through 9 were valid, and that you retrieved the following blacklist:

                5-8
                0-2
                4-7

                The blacklist specifies ranges of IPs (inclusive of both the start and end value) that are not allowed. Then, the only IPs that this firewall allows are 3 and 9,
                since those are the only numbers not in any range.

                Given the list of blocked IPs you retrieved from the firewall (your puzzle input), what is the lowest-valued IP that is not blocked?
             */

            #region Testrun
            var inputLines = new List<string> { "5-8", "0-2", "4-7" };
            long testLowestFreeIp = FindLowestFreeIp(inputLines);

            if (testLowestFreeIp != 3)
            {
                throw new Exception($"Test failed! Expected: 3, actual: {testLowestFreeIp}");
            }

            #endregion

            long lowestFreeIp = FindLowestFreeIp(InputLines);
            
            return lowestFreeIp; //23923783
        }

        protected override object GetSolutionPart2()
        {
            /*
             * How many IPs are allowed by the blacklist?
             */

            long maxValidIp = 4294967295;

            long allowedCount = FindAllowedIpCount(InputLines, maxValidIp);

            return allowedCount; //125
        }
    }
}
