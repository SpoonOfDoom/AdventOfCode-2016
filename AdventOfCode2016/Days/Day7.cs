using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2016.Days
{
    class Day7 : Day
    {
        public Day7() : base(7) {}
        Regex regexAbbaInBrackets = new Regex(@"\[\w*((\w)(\w)\3\2)\w*\]");
        Regex regexAbba = new Regex(@"\w*((\w)(\w)\3\2)\w*");

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

        public override string GetSolutionPart1()
        {
            int ipsWithTls = inputLines.Count(SupportsTls);
            return ipsWithTls.ToString(); //118
        }

        public override string GetSolutionPart2()
        {
            return base.GetSolutionPart2();
        }
    }
}
