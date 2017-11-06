using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Extensions;

namespace AdventOfCode2016.Days
{
    // ReSharper disable once UnusedMember.Global
    class Day19 : Day
    {
        public Day19() : base(19) {}

        private int FindBiggestPowerOfTwo(int number)
        {
            int power = 2;

            while (power <= number)
            {
                power *= 2;
            }
            power /= 2;

            return power;
        }

        private int FindeOneBasedIndex(int positions)
        {
            return 2 * (positions - FindBiggestPowerOfTwo(positions)) + 1;
        }

        private int FindWinnerOppositeMode(List<int> startCandidates, int elf)
        {
            List<int> candidates = new List<int>(startCandidates);
            int index;
            int removeIndex;
            while (candidates.Count > 1)
            {
                index = candidates.IndexOf(elf);
                if (candidates.Count == 2)
                {
                    return candidates[index];
                }
                removeIndex = (index + candidates.Count / 2) % candidates.Count;
                candidates.RemoveAt(removeIndex);
                if (removeIndex <= index)
                {
                    index = candidates.IndexOf(elf);
                }
                elf = candidates[(index + 1) % candidates.Count];
            }
            return -1;
        }

        protected override object GetSolutionPart1()
        {
            /*
             * --- Day 19: An Elephant Named Joseph ---

                    The Elves contact you over a highly secure emergency channel. Back at the North Pole, the Elves are busy misunderstanding White Elephant parties.

                    Each Elf brings a present. They all sit in a circle, numbered starting with position 1. Then, starting with the first Elf, they take turns stealing all the presents from the Elf to their left. An Elf with no presents is removed from the circle and does not take turns.

                    For example, with five Elves (numbered 1 to 5):

                      1
                    5   2
                     4 3

                        Elf 1 takes Elf 2's present.
                        Elf 2 has no presents and is skipped.
                        Elf 3 takes Elf 4's present.
                        Elf 4 has no presents and is also skipped.
                        Elf 5 takes Elf 1's two presents.
                        Neither Elf 1 nor Elf 2 have any presents, so both are skipped.
                        Elf 3 takes Elf 5's three presents.

                    So, with five Elves, the Elf that sits starting in position 3 gets all the presents.

                    With the number of Elves given in your puzzle input, which Elf gets all the presents?
             */

            //This is a kid-friendly version of the suicide circle problem, which I happened to have seen a video about on Numberphile.
            //Basically, you want to find the biggest power of two below the amount of people in the circle, subtract it from the amount of people, and then multiply that by two and add one.
            //So: x = 2 * (n - p) + 1, where n is the number of people and p is the highest power of two below that.

            #region TestRuns
            int position = FindeOneBasedIndex(5);
            if (position != 3)
            {
                throw new Exception($"Test failed! Expected: 3, actual: {position}");
            }

            #endregion

            int positions = Input.ToInt();
            position = FindeOneBasedIndex(positions);
            return position;
        }

        protected override object GetSolutionPart2()
        {
            /*
             * Realizing the folly of their present-exchange rules, the Elves agree to instead steal presents from the Elf directly across the circle. If two Elves are across the circle, the one on the left (from the perspective of the stealer) is stolen from. The other rules remain unchanged: Elves with no presents are removed from the circle entirely, and the other elves move in slightly to keep the circle evenly spaced.

                For example, with five Elves (again numbered 1 to 5):

                    The Elves sit in a circle; Elf 1 goes first:

                      1
                    5   2
                     4 3

                    Elves 3 and 4 are across the circle; Elf 3's present is stolen, being the one to the left. Elf 3 leaves the circle, and the rest of the Elves move in:

                      1           1
                    5   2  -->  5   2
                     4 -          4

                    Elf 2 steals from the Elf directly across the circle, Elf 5:

                      1         1 
                    -   2  -->     2
                      4         4 

                    Next is Elf 4 who, choosing between Elves 1 and 2, steals from Elf 1:

                     -          2  
                        2  -->
                     4          4

                    Finally, Elf 2 steals from Elf 4:

                     2
                        -->  2  
                     -

                So, with five Elves, the Elf that sits starting in position 2 gets all the presents.

                With the number of Elves given in your puzzle input, which Elf now gets all the presents?
             */

            //I haven't been able to come up with a mathematical solution for this variation, so all that was left was iterating it through, which takes AGES.

            #region Testrun


            List<int> testCandidates = Enumerable.Range(1, 5).ToList();
            int testResult = FindWinnerOppositeMode(testCandidates, 1);
            if (testResult != 2)
            {
                throw new Exception($"Test failed! Expected: 2, actual: {testCandidates[testResult]}");
            }

            #endregion
            List<int> candidates = Enumerable.Range(1, Input.ToInt()).ToList();
            int result = FindWinnerOppositeMode(candidates, 1);
            return result; //1420064
        }
    }
}
