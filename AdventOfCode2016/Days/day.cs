using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2016.Days
{
    public abstract class Day
    {
        protected string input;
        protected List<string> inputLines;
        public readonly int Number;

        public static Dictionary<int, Dictionary<string, TimeSpan>> SolutionTimes = new Dictionary<int, Dictionary<string, TimeSpan>>();

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

        public static void RunAllDays(bool verbose = true)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 1; i <= 25; i++)
            {
                RunDay(i, batch: true, verbose: verbose);
            }
            sw.Stop();

            WriteTimesToFile();
            Console.WriteLine($"Total time taken: {sw.Elapsed.Hours}:{sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}:{sw.Elapsed.Milliseconds}");
            Console.ReadLine();
        }

        private static void WriteTimesToFile(string filename = "solutionTimes")
        {
            const string timeExportFolder = "exports";
            if (!Directory.Exists(timeExportFolder))
            {
                Directory.CreateDirectory(timeExportFolder);
            }
            if (filename == "solutionTimes")
            {
                filename += DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";
            }
            string filePath = timeExportFolder + "\\" + filename;

            string fileContent = "Day Number;Part 1;Part 2;Total\n";
            foreach (KeyValuePair<int, Dictionary<string, TimeSpan>> solutionTime in SolutionTimes)
            {
                fileContent += $"{solutionTime.Key};{solutionTime.Value["Part1"]};{solutionTime.Value["Part2"]};{solutionTime.Value["Total"]}\n";
            }

            File.WriteAllText(filePath, fileContent, Encoding.UTF8);
        }

        public static void RunDay(int number, Day dayInstance = null, bool batch = false, bool verbose = true)
        {
            if (dayInstance == null)
            {
                Type dayType = Type.GetType("AdventOfCode2016.Days.Day" + number);
                dayInstance = (Day)Activator.CreateInstance(dayType);
            }

            Stopwatch sw = new Stopwatch();

            string solution1, solution2;
            TimeSpan totalTime = new TimeSpan();
            TimeSpan part1Time = new TimeSpan();
            TimeSpan part2Time = new TimeSpan();
            try
            {
                sw.Start();
                solution1 = dayInstance.GetSolutionPart1();
                sw.Stop();
                part1Time = sw.Elapsed;
                totalTime += sw.Elapsed;
            }
            catch (NotImplementedException)
            {
                sw.Stop();
                solution1 = "not implemented.";
            }
            if (verbose)
            {
                Console.WriteLine($"day {dayInstance.Number} part 1 : {solution1} - solved in {sw.Elapsed.TotalSeconds} seconds ({sw.Elapsed.TotalMilliseconds} milliseconds)");

            }

            try
            {
                sw.Restart();
                solution2 = dayInstance.GetSolutionPart2();
                sw.Stop();
                part2Time = sw.Elapsed;
                totalTime += sw.Elapsed;
            }
            catch (NotImplementedException)
            {
                sw.Stop();
                solution2 = "not implemented.";
            }
            if (verbose)
            {
                Console.WriteLine($"day {dayInstance.Number} part 2 : {solution2} - solved in {sw.Elapsed.TotalSeconds} seconds ({sw.Elapsed.TotalMilliseconds} milliseconds)");
                Console.WriteLine($"total time: {totalTime.TotalSeconds} seconds ({totalTime.TotalMilliseconds} milliseconds)");
            }

            SolutionTimes[number] = new Dictionary<string, TimeSpan>
                                    {
                                        {"Total", totalTime},
                                        {"Part1", part1Time},
                                        {"Part2", part2Time}
                                    };

            if (!batch)
            {
                Console.Read();
            }
        }

        public static Day GetDayInstance(int number)
        {
            Type dayType = Type.GetType("Day" + number);
            Day dayInstance = (Day)Activator.CreateInstance(dayType);
            return dayInstance;
        }
    }
}
