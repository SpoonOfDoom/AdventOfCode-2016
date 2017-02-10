using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2016.Days
{
    class Day6 : Day
    {
        public Day6() : base(6) {}
        
        private Dictionary<int, Dictionary<char, int>> letterCount = new Dictionary<int, Dictionary<char, int>>();
        private int messageLength;

        public override object GetSolutionPart1()
        {
            messageLength = inputLines[0].Length;

            foreach (string line in inputLines)
            {
                for (int j = 0; j < messageLength; j++)
                {
                    if (!letterCount.ContainsKey(j))
                    {
                        letterCount[j] = new Dictionary<char, int>();
                    }

                    char c = line[j];
                    if (!letterCount[j].ContainsKey(c))
                    {
                        letterCount[j][c] = 0;
                    }
                    letterCount[j][c]++;
                }
            }
            Dictionary<int, char> correctChars = new Dictionary<int, char>();
            int most = 0;
            for (int i = 0; i < messageLength; i++)
            {
                foreach (KeyValuePair<char, int> keyValuePair in letterCount[i])
                {
                    if (keyValuePair.Value > most)
                    {
                        correctChars[i] = keyValuePair.Key;
                        most = keyValuePair.Value;
                    }
                }
                most = 0;
            }
            string result = string.Join("", correctChars.Values);
            return result; //qtbjqiuq
        }

        public override object GetSolutionPart2()
        {
            Dictionary<int, char> correctChars = new Dictionary<int, char>();
            int least = int.MaxValue;
            for (int i = 0; i < messageLength; i++)
            {
                foreach (KeyValuePair<char, int> keyValuePair in letterCount[i])
                {
                    if (keyValuePair.Value < least)
                    {
                        correctChars[i] = keyValuePair.Key;
                        least = keyValuePair.Value;
                    }
                }
                least = int.MaxValue;
            }
            string result = string.Join("", correctChars.Values);
            return result; //akothqli
        }
    }
}
