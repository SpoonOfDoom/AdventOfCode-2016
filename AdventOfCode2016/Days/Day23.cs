using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Extensions;

namespace AdventOfCode2016.Days
{
    // ReSharper disable once UnusedMember.Global
    class Day23 : Day
    {
        public Day23() : base(23) {}

        public enum CommandTypes
        {
            inc,
            dec,
            jnz,
            cpy,
            tgl
        }

        private class Instruction
        {
            public CommandTypes Type;
            public List<string> Parameters = new List<string>();

            public bool IsValid()
            {
                switch (Type)
                {
                    case CommandTypes.inc:
                        if (Parameters[0].IsNumeric() || Parameters.Count > 1)
                        {
                            return false;
                        }
                        break;
                    case CommandTypes.dec:
                        if (Parameters[0].IsNumeric() || Parameters.Count > 1)
                        {
                            return false;
                        }
                        break;
                    case CommandTypes.jnz:
                        if (Parameters.Count < 2)
                        {
                            return false;
                        }
                        break;
                    case CommandTypes.cpy:
                        if (Parameters.Count < 2 || (Parameters[0].IsNumeric() && Parameters[1].IsNumeric()))
                        {
                            return false;
                        }
                        break;
                    case CommandTypes.tgl:
                        if (Parameters.Count > 1)
                        {
                            return false;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return true;
            }
        }

        private class Loop
        {
            public long EndIndex;
            public Dictionary<string, long> StartValues = new Dictionary<string, long>();
            public bool HitToggle;

        }

        private class ActiveLoopList
        {
            private Dictionary<long, Loop> activeLoops = new Dictionary<long, Loop>();

            public bool IsLooping(long index)
            {
                if (!activeLoops.ContainsKey(index))
                {
                    return false;
                }
                return !activeLoops[index].HitToggle;
            }

            public void SetLoop(long endIndex, Dictionary<string, long> registers)
            {
                activeLoops[endIndex] = new Loop
                {
                    EndIndex = endIndex,
                    HitToggle = false,
                    StartValues = registers.ToDictionary(x => x.Key, x => x.Value)
                };

            }

            public void HitToggle()
            {
                activeLoops.Clear();
            }

            public void RemoveToggle(long index)
            {
                activeLoops[index].HitToggle = false;
            }

            public Loop this[long index] => activeLoops[index];

            public void RemoveLoop(long index)
            {
                activeLoops.Remove(index);
            }

            public void RemoveAllBelow(long index)
            {
                List<long> keys = activeLoops.Keys.Where(k => k < index).ToList();
                foreach (long key in keys)
                {
                    RemoveLoop(key);
                }
            }
        }
        
        private void ExecuteInstructions(List<Instruction> instructions, Dictionary<string, long> registers)
        {
            var activeLoops = new ActiveLoopList();

            for (int index = 0; index < instructions.Count; index++)
            {
                activeLoops.RemoveAllBelow(index);
                Instruction currentInstruction = instructions[index];
                if (!currentInstruction.IsValid())
                {
                    continue;
                }

                switch (currentInstruction.Type)
                {
                    case CommandTypes.tgl:
                        activeLoops.HitToggle();
                        string tgl = currentInstruction.Parameters[0];
                        int tglTarget = (int) (tgl.IsNumeric() ? tgl.ToLong() : registers[tgl]);
                        if (index + tglTarget < 0 || index + tglTarget >= instructions.Count)
                        {
                            continue;
                        }
                        Instruction targetInstruction = instructions[index + tglTarget];

                        if (targetInstruction.Parameters.Count == 1)
                        {
                            if (targetInstruction.Type == CommandTypes.inc)
                            {
                                targetInstruction.Type = CommandTypes.dec;
                            }
                            else
                            {
                                targetInstruction.Type = CommandTypes.inc;
                            }
                        }
                        else
                        {
                            if (targetInstruction.Type == CommandTypes.jnz)
                            {
                                targetInstruction.Type = CommandTypes.cpy;
                            }
                            else
                            {
                                targetInstruction.Type = CommandTypes.jnz;
                            }
                        }

                        break;

                    case CommandTypes.cpy:
                        string v = currentInstruction.Parameters[0];
                        long value = v.IsNumeric() ? v.ToLong() : registers[v];
                        string target = currentInstruction.Parameters[1];
                        registers[target] = value;
                        break;

                    case CommandTypes.inc:
                        string incRegister = currentInstruction.Parameters[0];
                        registers[incRegister]++;
                        break;

                    case CommandTypes.dec:
                        string decRegister = currentInstruction.Parameters[0];
                        registers[decRegister]--;
                        break;

                    case CommandTypes.jnz:
                        string xs = currentInstruction.Parameters[0];
                        long x = xs.IsNumeric() ? xs.ToLong() : registers[xs];
                        if (x != 0)
                        {
                            
                            if (activeLoops.IsLooping(index))
                            {
                                Loop loop = activeLoops[index];
                                long diffA = registers["a"] - loop.StartValues["a"];
                                long diffB = registers["b"] - loop.StartValues["b"];
                                long diffC = registers["c"] - loop.StartValues["c"];
                                long diffD = registers["d"] - loop.StartValues["d"];

                                long steps = registers[xs];
                                switch (xs)
                                {
                                    case "a":
                                        steps /= diffA;
                                        break;
                                    case "b":
                                        steps /= diffB;
                                        break;
                                    case "c":
                                        steps /= diffC;
                                        break;
                                    case "d":
                                        steps /= diffD;
                                        break;
                                }
                                steps = Math.Abs(steps);
                                registers["a"] += diffA * steps;
                                registers["b"] += diffB * steps;
                                registers["c"] += diffC * steps;
                                registers["d"] += diffD * steps;
                                activeLoops.RemoveLoop(index);
                            }
                            else
                            {
                                string a = currentInstruction.Parameters[1];
                                int amount = (int)(a.IsNumeric() ? a.ToLong() : registers[a]);
                                if (amount < 0)
                                {
                                    activeLoops.SetLoop(index, registers);
                                }
                                index += amount - 1;
                            }
                        }
                        break;
                }
            }
        }

        private List<Instruction> ParseInstructions(List<string> stringInstructions)
        {
            List<Instruction> instructions = new List<Instruction>();
            foreach (string line in stringInstructions)
            {
                string[] parts = line.Split(' ');
                Instruction instruction = new Instruction();
                for (int i = 0; i < parts.Length; i++)
                {
                    if (i == 0)
                    {
                        Enum.TryParse(parts[i], out instruction.Type);
                    }
                    else
                    {
                        instruction.Parameters.Add(parts[i]);
                    }
                }
                instructions.Add(instruction);
            }
            return instructions;
        }

        protected override object GetSolutionPart1()
        {
            /*
             * --- Day 23: Safe Cracking ---

                    This is one of the top floors of the nicest tower in EBHQ. The Easter Bunny's private office is here, complete with a safe hidden behind a painting,
                    and who wouldn't hide a star in a safe behind a painting?

                    The safe has a digital screen and keypad for code entry. A sticky note attached to the safe has a password hint on it: "eggs". The painting is of a
                    large rabbit coloring some eggs. You see 7.

                    When you go to type the code, though, nothing appears on the display; instead, the keypad comes apart in your hands, apparently having been smashed.
                    Behind it is some kind of socket - one that matches a connector in your prototype computer! You pull apart the smashed keypad and extract the logic
                    circuit, plug it into your computer, and plug your computer into the safe.

                    Now, you just need to figure out what output the keypad would have sent to the safe. You extract the assembunny code from the logic chip (your
                    puzzle input).

                    The code looks like it uses almost the same architecture and instruction set that the monorail computer used! You should be able to use the same
                    assembunny interpreter for this as you did there, but with one new instruction:

                    tgl x toggles the instruction x away (pointing at instructions like jnz does: positive means forward; negative means backward):

                        For one-argument instructions, inc becomes dec, and all other one-argument instructions become inc.
                        For two-argument instructions, jnz becomes cpy, and all other two-instructions become jnz.
                        The arguments of a toggled instruction are not affected.
                        If an attempt is made to toggle an instruction outside the program, nothing happens.
                        If toggling produces an invalid instruction (like cpy 1 2) and an attempt is later made to execute that instruction, skip it instead.
                        If tgl toggles itself (for example, if a is 0, tgl a would target itself and become inc a), the resulting instruction is not executed until
                            the next time it is reached.

                    For example, given this program:

                    cpy 2 a
                    tgl a
                    tgl a
                    tgl a
                    cpy 1 a
                    dec a
                    dec a

                        cpy 2 a initializes register a to 2.
                        The first tgl a toggles an instruction a (2) away from it, which changes the third tgl a into inc a.
                        The second tgl a also modifies an instruction 2 away from it, which changes the cpy 1 a into jnz 1 a.
                        The fourth line, which is now inc a, increments a to 3.
                        Finally, the fifth line, which is now jnz 1 a, jumps a (3) instructions ahead, skipping the dec a instructions.

                    In this example, the final value in register a is 3.

                    The rest of the electronics seem to place the keypad entry (the number of eggs, 7) in register a, run the code, and then send the value left in
                    register a to the safe.

                    What value should be sent to the safe?

             */

            #region Testrun
            List<Instruction> testInstructions = ParseInstructions(new List<string>
            {
                "cpy 2 a",
                "tgl a",
                "tgl a",
                "tgl a",
                "cpy 1 a",
                "dec a",
                "dec a",
            });

            Dictionary<string, long> testRegisters = new Dictionary<string, long>
            {
                {"a", 0 },
                {"b", 0 },
                {"c", 0 },
                {"d", 0 },
            };

            ExecuteInstructions(testInstructions, testRegisters);
            if (testRegisters["a"] != 3)
            {
                throw new Exception($"Test failed! Expected: {3}, actual: {testRegisters["a"]}");
            }

            #endregion

            Dictionary<string, long> registers = new Dictionary<string, long>
            {
                {"a", 7 },
                {"b", 0 },
                {"c", 0 },
                {"d", 0 },
            };
            List<Instruction> instructions = ParseInstructions(InputLines);
            ExecuteInstructions(instructions, registers);

            return registers["a"]; //11662
        }

        protected override object GetSolutionPart2()
        {
            /*
             * --- Part Two ---

                The safe doesn't open, but it does make several angry noises to express its frustration.

                You're quite sure your logic is working correctly, so the only other thing is... you check the painting again. As it turns out, colored eggs are
                still eggs. Now you count 12.

                As you run the program with this new input, the prototype computer begins to overheat. You wonder what's taking so long, and whether the lack of
                any instruction more powerful than "add one" has anything to do with it. Don't bunnies usually multiply?

                Anyway, what value should actually be sent to the safe?
             */

            Dictionary<string, long> registers = new Dictionary<string, long>
            {
                {"a", 12 },
                {"b", 0 },
                {"c", 0 },
                {"d", 0 },
            };
            List<Instruction> instructions = ParseInstructions(InputLines);
            ExecuteInstructions(instructions, registers);

            return registers["a"]; //479008222
        }
    }
}
