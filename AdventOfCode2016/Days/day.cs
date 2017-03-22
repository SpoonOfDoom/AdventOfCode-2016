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
        protected string Input;
        protected List<string> InputLines;
        private readonly int number;

        private static Dictionary<int, Dictionary<string, TimeSpan>> solutionTimes = new Dictionary<int, Dictionary<string, TimeSpan>>();

        protected Day(int number)
        {
            this.number = number;
            GetInput();
        }

        /// <summary>
        /// Input will be entered in a seperate method so that it can be collapsed individually (for bigger inputs)
        /// </summary>
        /// <returns></returns>
        private void GetInput()
        {
            Input = File.ReadAllText("input\\day" + number + ".txt");
            InputLines = File.ReadAllLines("input\\day" + number + ".txt").ToList();
        }

        protected virtual object GetSolutionPart1()
        {
            return "not implemented.";
        }

        protected virtual object GetSolutionPart2()
        {
            return "not implemented.";
        }

        // ReSharper disable once UnusedMember.Global
        public static void RunAlDays(bool verbose = true)
        {
            var sw = new Stopwatch();
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
            foreach (KeyValuePair<int, Dictionary<string, TimeSpan>> solutionTime in solutionTimes)
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
                if (dayType == null)
                {
                    throw new Exception("Couldn't find type AdventOfCode2016.Days.Day" + number);
                }
                dayInstance = (Day) Activator.CreateInstance(dayType);
            }

            var sw = new Stopwatch();

            var totalTime = new TimeSpan();

            sw.Start();
            string solution1 = dayInstance.GetSolutionPart1().ToString();
            sw.Stop();
            TimeSpan part1Time = sw.Elapsed;
            totalTime += sw.Elapsed;
            
            if (verbose)
            {
                Console.WriteLine($"day {dayInstance.number} part 1 : {solution1} - solved in {sw.Elapsed.TotalSeconds} seconds ({sw.Elapsed.TotalMilliseconds} milliseconds)");
            }
            
            sw.Restart();
            string solution2 = dayInstance.GetSolutionPart2().ToString();
            sw.Stop();
            TimeSpan part2Time = sw.Elapsed;
            totalTime += sw.Elapsed;
            
            if (verbose)
            {
                Console.WriteLine($"day {dayInstance.number} part 2 : {solution2} - solved in {sw.Elapsed.TotalSeconds} seconds ({sw.Elapsed.TotalMilliseconds} milliseconds)");
                Console.WriteLine($"total time: {totalTime.TotalSeconds} seconds ({totalTime.TotalMilliseconds} milliseconds)");
            }

            solutionTimes[number] = new Dictionary<string, TimeSpan>
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
    }
}
