using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Extensions;

namespace AdventOfCode2016.Days
{
    // ReSharper disable once UnusedMember.Global
    class Day25 : Day
    {
        public Day25() : base(25) {}


        public enum CommandTypes
        {
            Inc,
            Dec,
            Jnz,
            Cpy,
            Tgl,
            Out
        }

        private class Instruction
        {
            public CommandTypes Type;
            public List<string> Parameters = new List<string>();

            public bool IsValid()
            {
                switch (Type)
                {
                    case CommandTypes.Inc:
                        if (Parameters[0].IsNumeric() || Parameters.Count > 1)
                        {
                            return false;
                        }
                        break;
                    case CommandTypes.Dec:
                        if (Parameters[0].IsNumeric() || Parameters.Count > 1)
                        {
                            return false;
                        }
                        break;
                    case CommandTypes.Jnz:
                        if (Parameters.Count < 2)
                        {
                            return false;
                        }
                        break;
                    case CommandTypes.Cpy:
                        if (Parameters.Count < 2 || (Parameters[0].IsNumeric() && Parameters[1].IsNumeric()))
                        {
                            return false;
                        }
                        break;
                    case CommandTypes.Tgl:
                        if (Parameters.Count > 1)
                        {
                            return false;
                        }
                        break;
                    case CommandTypes.Out:
                        if (Parameters.Count != 1)
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
                    case CommandTypes.Tgl:
                        activeLoops.HitToggle();
                        string tgl = currentInstruction.Parameters[0];
                        int tglTarget = (int)(tgl.IsNumeric() ? tgl.ToLong() : registers[tgl]);
                        if (index + tglTarget < 0 || index + tglTarget >= instructions.Count)
                        {
                            continue;
                        }
                        Instruction targetInstruction = instructions[index + tglTarget];

                        if (targetInstruction.Parameters.Count == 1)
                        {
                            if (targetInstruction.Type == CommandTypes.Inc)
                            {
                                targetInstruction.Type = CommandTypes.Dec;
                            }
                            else
                            {
                                targetInstruction.Type = CommandTypes.Inc;
                            }
                        }
                        else
                        {
                            if (targetInstruction.Type == CommandTypes.Jnz)
                            {
                                targetInstruction.Type = CommandTypes.Cpy;
                            }
                            else
                            {
                                targetInstruction.Type = CommandTypes.Jnz;
                            }
                        }

                        break;

                    case CommandTypes.Cpy:
                        string v = currentInstruction.Parameters[0];
                        long value = v.IsNumeric() ? v.ToLong() : registers[v];
                        string target = currentInstruction.Parameters[1];
                        registers[target] = value;
                        break;

                    case CommandTypes.Inc:
                        string incRegister = currentInstruction.Parameters[0];
                        registers[incRegister]++;
                        break;

                    case CommandTypes.Dec:
                        string decRegister = currentInstruction.Parameters[0];
                        registers[decRegister]--;
                        break;

                    case CommandTypes.Jnz:
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
                --- Day 25: Clock Signal ---

                You open the door and find yourself on the roof. The city sprawls away from you for miles and miles.

                There's not much time now - it's already Christmas, but you're nowhere near the North Pole, much too far to deliver these stars to the sleigh in time.

                However, maybe the huge antenna up here can offer a solution. After all, the sleigh doesn't need the stars, exactly; it needs the timing data they provide,
                and you happen to have a massive signal generator right here.

                You connect the stars you have to your prototype computer, connect that to the antenna, and begin the transmission.

                Nothing happens.

                You call the service number printed on the side of the antenna and quickly explain the situation. "I'm not sure what kind of equipment you have connected over there,"
                he says, "but you need a clock signal." You try to explain that this is a signal for a clock.

                "No, no, a clock signal - timing information so the antenna computer knows how to read the data you're sending it. An endless, alternating pattern
                of 0, 1, 0, 1, 0, 1, 0, 1, 0, 1...." He trails off.

                You ask if the antenna can handle a clock signal at the frequency you would need to use for the data from the stars. "There's no way it can! The only antenna
                we've installed capable of that is on top of a top-secret Easter Bunny installation, and you're definitely not-" You hang up the phone.

                You've extracted the antenna's clock signal generation assembunny code (your puzzle input); it looks mostly compatible with code you worked on just recently.

                This antenna code, being a signal generator, uses one extra instruction:

                    out x transmits x (either an integer or the value of a register) as the next value for the clock signal.

                The code takes a value (via register a) that describes the signal to generate, but you're not sure how it's used. You'll have to find the input to produce the
                right signal through experimentation.

                What is the lowest positive integer that can be used to initialize register a and cause the code to output a clock signal of 0, 1, 0, 1... repeating forever?

             */

            #region Testrun
            
            #endregion
            return base.GetSolutionPart1();
        }
    }
}
