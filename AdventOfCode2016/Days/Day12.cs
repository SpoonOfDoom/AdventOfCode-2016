using System.Collections.Generic;
using AdventOfCode2016.Extensions;

namespace AdventOfCode2016.Days
{
    // ReSharper disable once UnusedMember.Global
    class Day12 : Day
    {
        public Day12() : base(12) {}

        private Dictionary<string, int> registers = new Dictionary<string, int>
        {
            {"a", 0 },
            {"b", 0 },
            {"c", 0 },
            {"d", 0 },
        };

        private void ExecuteInstructions()
        {
            for (int index = 0; index < InputLines.Count; index++)
            {
                string line = InputLines[index];
                var parts = line.Split(' ');
                switch (parts[0])
                {
                    case "cpy":
                        string v = parts[1];
                        int value = v.IsNumeric() ? v.ToInt() : registers[v];
                        string target = parts[2];
                        registers[target] = value;
                        break;

                    case "inc":
                        string incRegister = parts[1];
                        registers[incRegister]++;
                        break;

                    case "dec":
                        string decRegister = parts[1];
                        registers[decRegister]--;
                        break;

                    case "jnz":
                        int x = parts[1].IsNumeric()? parts[1].ToInt() : registers[parts[1]];
                        if (x != 0)
                        {
                            int amount = parts[2].ToInt();
                            index += amount -1;
                        }
                        break;
                }
            }
        }

        protected override object GetSolutionPart1()
        {
            ExecuteInstructions();
            return registers["a"].ToString();
        }

        protected override object GetSolutionPart2()
        {
            registers = new Dictionary<string, int>
            {
                {"a", 0 },
                {"b", 0 },
                {"c", 1 },
                {"d", 0 },
            };
            ExecuteInstructions();
            return  registers["a"].ToString();
        }
    }
}
