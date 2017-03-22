using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2016.Days
{
    // ReSharper disable once UnusedMember.Global
    class Day7 : Day
    {
        public Day7() : base(7) {}
        Regex regexAbbaInBrackets = new Regex(@"\[\w*((\w)(\w)\3\2)\w*\]");
        Regex regexAbba = new Regex(@"\w*((\w)(\w)\3\2)\w*");
        Regex stuffInBrackets = new Regex(@"\[(\w+)\]");
        Regex stuffOutsideBrackets = new Regex(@"(\w+)(\[|$)");
        Regex abaRegex = new Regex(@"(?=(\w)(\w)\1)");

        private bool SupportsTls(string ip)
        {
            MatchCollection resultBrackets = regexAbbaInBrackets.Matches(ip);
            if (resultBrackets.Count > 0)
            {
                foreach (Match match in resultBrackets)
                {
                    if (match.Groups[2].Value != match.Groups[3].Value)
                    {
                        return false;
                    }
                }
            }

            MatchCollection abbas = regexAbba.Matches(ip);
            if (abbas.Count == 0)
            {
                return false;
            }
            foreach (Match match in abbas)
            {
                if (match.Groups[2].Value != match.Groups[3].Value)
                {
                    return true;
                }
            }
            return false;
        }

        private bool SupportsSsl(string ip)
        {
            var stringsInsideBrackets = new List<string>();
            var stringsOutsideBrackets = new List<string>();

            var matchesInside = stuffInBrackets.Matches(ip);
            foreach (Match match in matchesInside)
            {
                stringsInsideBrackets.Add(match.Groups[1].Value);
            }

            var matchesOutside = stuffOutsideBrackets.Matches(ip);
            foreach (Match match in matchesOutside)
            {
                stringsOutsideBrackets.Add(match.Groups[1].Value);
            }

            HashSet<string> babsToSearch = new HashSet<string>();
            foreach (string outsideString in stringsOutsideBrackets)
            {
                var abaMatches = abaRegex.Matches(outsideString);
                foreach (Match match in abaMatches)
                {
                    string aba = outsideString.Substring(match.Index, 3);
                    if (aba[0] != aba[1])
                    {
                        string bab = ""+ aba[1] + aba[0] + aba[1];
                        babsToSearch.Add(bab);
                    }
                }
            }
            
            foreach (string bab in babsToSearch)
            {
                foreach (string insideString in stringsInsideBrackets)
                {
                    if (insideString.Contains(bab))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected override object GetSolutionPart1()
        {
            int ipsWithTls = InputLines.Count(SupportsTls);
            return ipsWithTls; //118
        }

        protected override object GetSolutionPart2()
        {
            int ipsWithSsl = InputLines.Count(SupportsSsl);
            return ipsWithSsl; //260
        }
    }
}
