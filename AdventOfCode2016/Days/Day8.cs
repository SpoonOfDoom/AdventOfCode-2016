using System;
using System.Collections.Generic;
using AdventOfCode2016.Extensions;

namespace AdventOfCode2016.Days
{
    // ReSharper disable once UnusedMember.Global
    class Day8 : Day
    {
        public Day8() : base(8) {}

        private enum CommandType
        {
            Rect,
            RotateColumn,
            RotateRow
        }

        private class Command
        {
            public CommandType CommandType;
            public int X, Y, Value;
            
            public void ExecuteCommand(bool[,] display)
            {
                switch (CommandType)
                {
                    case CommandType.Rect:
                        for (int x = 0; x < X; x++)
                        {
                            for (int y = 0; y < Y; y++)
                            {
                                display[x, y] = true;
                            }
                        }
                        break;

                    case CommandType.RotateColumn:
                        List<int> newYs = new List<int>();
                        for (int y = 0; y < ScreenHeight; y++)
                        {
                            if (display[X, y])
                            {
                                newYs.Add((y + Value)%ScreenHeight);
                                display[X, y] = false;
                            }
                        }
                        foreach (int y in newYs)
                        {
                            display[X, y] = true;
                        }

                        break;

                    case CommandType.RotateRow:
                        List<int> newXs = new List<int>();
                        for (int x = 0; x < ScreenWidth; x++)
                        {
                            if (display[x, Y])
                            {
                                newXs.Add((x + Value) % ScreenWidth);
                                display[x, Y] = false;
                            }
                        }
                        foreach (int x in newXs)
                        {
                            display[x, Y] = true;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private const int ScreenWidth = 50;
        private const int ScreenHeight = 6;
        private bool[,] display = new bool[ScreenWidth,ScreenHeight];

        private List<Command> commands = new List<Command>();

        private Command ParseCommand(string commandText)
        {
            Command command = new Command();
            string[] parts = commandText.Split(' ');
            switch (parts[0])
            {
                case "rect":
                    string[] dimensions = parts[1].Split('x');
                    command.CommandType = CommandType.Rect;
                    command.X = dimensions[0].ToInt();
                    command.Y = dimensions[1].ToInt();
                    return command;
                case "rotate":
                    
                    if (parts[2].StartsWith("x"))
                    {
                        command.CommandType = CommandType.RotateColumn;
                        command.X = parts[2].Substring(2).ToInt();
                        command.Y = -1;
                    }
                    else
                    {
                        command.CommandType = CommandType.RotateRow;
                        command.X = -1;
                        command.Y = parts[2].Substring(2).ToInt();
                    }
                    command.Value = parts[4].ToInt();
                    return command;

                default:
                    throw new ArgumentOutOfRangeException(parts[0]);
            }
        }

        protected override object GetSolutionPart1()
        {
            foreach (string line in InputLines)
            {
                commands.Add(ParseCommand(line));
            }

            foreach (Command command in commands)
            {
                command.ExecuteCommand(display);
            }

            int truthinessPixelCount = 0;
            for (int x = 0; x < ScreenWidth; x++)
            {
                for (int y = 0; y < ScreenHeight; y++)
                {
                    if (display[x,y])
                    {
                        truthinessPixelCount++;
                    }
                }
            }
            return truthinessPixelCount; //121
        }

        protected override object GetSolutionPart2()
        {
            Console.WriteLine();
            for (int y = 0; y < ScreenHeight; y++)
            {
                for (int x = 0; x < ScreenWidth; x++)
                {
                    if (x%5 ==0)
                    {
                        Console.Write("|");
                    }
                    Console.Write(display[x,y] ? "X" : "_");
                }
                Console.Write("\n");
            }
            Console.Read();
            //todo: read letters programmatically
            return "RURUCEOEIL"; //RURUCEOEIL
        }
    }
}
