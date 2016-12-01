using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public abstract class Day
    {
        protected string input;
        protected List<string> inputLines;
        public readonly int Number;

        protected Day(int number)
        {
            Number = number;
            GetInput();
        }

        /// <summary>
        /// Input will be entered in a seperate method so that it can be collapsed individually (for bigger inputs)
        /// </summary>
        /// <returns></returns>
        private void GetInput()
        {
            input = File.ReadAllText("input\\day" + Number + ".txt");
            inputLines = File.ReadAllLines("input\\day" + Number + ".txt").ToList();
        }

        public virtual string GetSolutionPart1()
        {
            throw new NotImplementedException();
        }
        public virtual string GetSolutionPart2()
        {
            throw new NotImplementedException();
        }
    }
}
