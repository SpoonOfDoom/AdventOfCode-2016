using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2016.Extensions;

namespace AdventOfCode2016.Days
{
    class Day10 : Day
    {
        public Day10() : base(10) {}

        private static int winNumber1 = 61;
        private static int winNumber2 = 17;

        private Regex botRegex = new Regex(@"bot (\d+) gives low to (\w+) (\d+) and high to (\w+) (\d+)");
        private Regex inputRegex = new Regex(@"value (\d+) goes to bot (\d+)");

        private static Dictionary<int, Bot> bots = new Dictionary<int, Bot>();
        private static Dictionary<int, int> outputs = new Dictionary<int, int>();
        private static Dictionary<int, int> inputAndTargets = new Dictionary<int, int>();
        
        private class Bot
        {
            public int Number;
            private List<int> Chips = new List<int>();
            public int LowTarget, HighTarget;
            public bool LowTargetOutput = false;
            public bool HighTargetOutput = false;
            

            public bool ReadyToWork => Chips.Count == 2;
            public bool IsWinner => Chips.Count == 2 && Chips.Contains(winNumber1) && Chips.Contains(winNumber2);

            public void GiveChips()
            {
                if (!ReadyToWork)
                {
                    throw new Exception("No two chips available to do things. This shouldn't have been called yet.");
                }

                if (Chips[0] < Chips[1])
                {
                    GiveTo(LowTarget, Chips[0], LowTargetOutput);
                    GiveTo(HighTarget, Chips[0], HighTargetOutput);
                }
                else
                {
                    GiveTo(LowTarget, Chips[1], LowTargetOutput);
                    GiveTo(HighTarget, Chips[0], HighTargetOutput);
                }
            }


            public void GiveTo(int targetNumber, int chip, bool output = false)
            {
                if (output)
                {
                    outputs[targetNumber] = chip;
                }
                else
                {
                    Chips.Remove(chip);
                    bots[targetNumber].Receive(chip);
                }
            }

            public void Receive(int chip)
            {
                if (Chips.Count >= 2)
                {
                    throw new Exception("Too many chips!");
                }

                Chips.Add(chip);
            }
        }

        private void ParseCommand(string line)
        {
            if (line.StartsWith("bot"))
            {
                var groups = botRegex.Match(line).Groups;
                //private Regex botRegex = new Regex(@"bot (\d+) gives low to (\w+) (\d+) and high to (\w+) (\d+)");
                var bot = new Bot
                          {
                              Number = groups[1].Value.ToInt(),
                              LowTarget = groups[3].Value.ToInt(),
                              HighTarget = groups[5].Value.ToInt(),
                              LowTargetOutput = groups[2].Value == "output",
                              HighTargetOutput = groups[4].Value == "output"
                          };
                if (bots.ContainsKey(bot.Number))
                {
                    throw new Exception("Duplicate bot rule!");
                }
                bots[bot.Number] = bot;
            }
            else
            {
                var groups = inputRegex.Match(line).Groups;
                int chip = groups[1].Value.ToInt();
                int target = groups[2].Value.ToInt();
                if (inputAndTargets.ContainsKey(chip))
                {
                    throw new Exception("Duplicate input rule!");
                }
                inputAndTargets[chip] = target;
            }
        }

        private void SeedInitial()
        {
            foreach (KeyValuePair<int, int> inputAndTarget in inputAndTargets)
            {
                bots[inputAndTarget.Value].Receive(inputAndTarget.Key);
            }
        }

        private int Iterate()
        {
            Bot winBot = bots.Values.SingleOrDefault(b => b.IsWinner);
            if (winBot != default(Bot))
            {
                return winBot.Number;
            }
            var readyBots = bots.Values.Where(b => b.ReadyToWork).ToList();
            if (!readyBots.Any())
            {
                return -2;
            }
            foreach (Bot bot in readyBots)
            {
                bot.GiveChips();
            }
            return -1;
        }

        public override object GetSolutionPart1()
        {
            /*
             * You come upon a factory in which many robots are zooming around handing small microchips to each other.

                Upon closer examination, you notice that each bot only proceeds when it has two microchips, and once it does, it gives each one to a different bot or puts it
                in a marked "output" bin. Sometimes, bots take microchips from "input" bins, too.

                Inspecting one of the microchips, it seems like they each contain a single number; the bots must use some logic to decide what to do with each chip.
                You access the local control computer and download the bots' instructions (your puzzle input).

                Some of the instructions specify that a specific-valued microchip should be given to a specific bot; the rest of the instructions indicate what
                a given bot should do with its lower-value or higher-value chip.

                For example, consider the following instructions:

                value 5 goes to bot 2
                bot 2 gives low to bot 1 and high to bot 0
                value 3 goes to bot 1
                bot 1 gives low to output 1 and high to bot 0
                bot 0 gives low to output 2 and high to output 0
                value 2 goes to bot 2

                    Initially, bot 1 starts with a value-3 chip, and bot 2 starts with a value-2 chip and a value-5 chip.
                    Because bot 2 has two microchips, it gives its lower one (2) to bot 1 and its higher one (5) to bot 0.
                    Then, bot 1 has two microchips; it puts the value-2 chip in output 1 and gives the value-3 chip to bot 0.
                    Finally, bot 0 has two microchips; it puts the 3 in output 2 and the 5 in output 0.

                In the end, output bin 0 contains a value-5 microchip, output bin 1 contains a value-2 microchip, and output bin 2 contains a value-3 microchip.
                In this configuration, bot number 2 is responsible for comparing value-5 microchips with value-2 microchips.

                Based on your instructions, what is the number of the bot that is responsible for comparing value-61 microchips with value-17 microchips?
                */

            #region TEST
            
            var testInstructions = new List<string>
                                   {
                                       "value 5 goes to bot 2",
                                       "bot 2 gives low to bot 1 and high to bot 0",
                                       "value 3 goes to bot 1",
                                       "bot 1 gives low to output 1 and high to bot 0",
                                       "bot 0 gives low to output 2 and high to output 0",
                                       "value 2 goes to bot 2"
                                   };
            foreach (string line in testInstructions)
            {
                ParseCommand(line);
            }

            winNumber1 = 5;
            winNumber2 = 2;

            SeedInitial();
            int winBotNumber = -1;
            while (winBotNumber == -1)
            {
                winBotNumber = Iterate();
            }
            if (winBotNumber != 2)
            {
                throw new Exception("FAIL!");
            }
            winNumber1 = 61;
            winNumber2 = 17;
            bots.Clear();
            inputAndTargets.Clear();
            outputs.Clear();
            #endregion

            foreach (string inputLine in inputLines)
            {
                ParseCommand(inputLine);
            }
            SeedInitial();

            winBotNumber = -1;
            while (winBotNumber == -1)
            {
                winBotNumber = Iterate();
            }
            
            return winBotNumber; //56
        }

        public override object GetSolutionPart2()
        {
            return base.GetSolutionPart2();
        }
    }
}
