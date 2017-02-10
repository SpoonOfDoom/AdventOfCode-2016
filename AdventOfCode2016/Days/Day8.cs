using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Extensions;

namespace AdventOfCode2016.Days
{
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
                        for (int y = 0; y < screenHeight; y++)
                        {
                            if (display[X, y])
                            {
                                newYs.Add((y + Value)%screenHeight);
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
                        for (int x = 0; x < screenWidth; x++)
                        {
                            if (display[x, Y])
                            {
                                newXs.Add((x + Value) % screenWidth);
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

        private const int screenWidth = 50;
        private const int screenHeight = 6;
        private bool[,] display = new bool[screenWidth,screenHeight];

        private List<Command> Commands = new List<Command>();

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
        
        public override object GetSolutionPart1()
        {
            foreach (string line in inputLines)
            {
                Commands.Add(ParseCommand(line));
            }

            foreach (Command command in Commands)
            {
                command.ExecuteCommand(display);
            }

            int truthinessPixelCount = 0;
            for (int x = 0; x < screenWidth; x++)
            {
                for (int y = 0; y < screenHeight; y++)
                {
                    if (display[x,y])
                    {
                        truthinessPixelCount++;
                    }
                }
            }
            return truthinessPixelCount; //121
        }

        public override object GetSolutionPart2()
        {
            Console.WriteLine();
            for (int y = 0; y < screenHeight; y++)
            {
                for (int x = 0; x < screenWidth; x++)
                {
                    if (x%5 ==0)
                    {
                        Console.Write("|");
                    }
                    Console.Write(display[x,y] ? "X" : "_");
                }
                Console.Write("\n");
            }
            int i = 0;
            Console.Read();
            return "RURUCEOEIL"; //RURUCEOEIL
        }
    }
}
