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
#if DEBUG
        private const string Configuration = "Debug";
#else
        private const string Configuration = "Release";
#endif

        private const string TimeExportFolder = "exports";
        protected object solutionPart1, solutionPart2;
        protected TimeSpan solutionTime1, solutionTime2;
        private TimeSpan TotalTime => solutionTime1 + solutionTime2;
        protected string Input;
        protected List<string> InputLines;
        private readonly int number;

        private static Dictionary<int, List<Dictionary<string, TimeSpan>>> solutionTimes = new Dictionary<int, List<Dictionary<string, TimeSpan>>>();

        private object SolutionPart1
        {
            get
            {
                if (solutionPart1 == null)
                {
                    GetSolutionPart1();
                }
                return solutionPart1;
            }
        }

        private object SolutionPart2
        {
            get
            {
                if (solutionPart2 == null)
                {
                    GetSolutionPart2();
                }
                return solutionPart2;
            }
        }

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
            solutionPart1 = "not implemented.";
            solutionTime1 = TimeSpan.Zero;
            return SolutionPart1;
        }

        protected virtual object GetSolutionPart2()
        {
            solutionPart2 = "not implemented";
            solutionTime2 = TimeSpan.Zero;
            return SolutionPart2;
        }

        // ReSharper disable once UnusedMember.Global
        public static void RunAllDays(bool verbose = true)
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
            if (!Directory.Exists(TimeExportFolder))
            {
                Directory.CreateDirectory(TimeExportFolder);
            }
            if (filename == "solutionTimes")
            {
                filename += DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";
            }
            string filePath = TimeExportFolder + "\\" + filename;
            string fileContent = "Machine Name;Build Configuration;Day Number;Part 1;Part 2;Total\n";


            foreach (KeyValuePair<int, List<Dictionary<string, TimeSpan>>> solutionTime in solutionTimes)
            {
                foreach (Dictionary<string, TimeSpan> uniqueRun in solutionTime.Value)
                {
                    fileContent += $"{Environment.MachineName};{Configuration};{solutionTime.Key};{uniqueRun["Part1"]};{uniqueRun["Part2"]};{uniqueRun["Total"]}\n";
                }
            }
            File.WriteAllText(filePath, fileContent, Encoding.UTF8);
        }

        private void WriteToFile(int part, bool append = true)
        {
            if (!Directory.Exists(TimeExportFolder))
            {
                Directory.CreateDirectory(TimeExportFolder);
            }
            string filename = "day_" + number + ".log";

            string filePath = TimeExportFolder + "\\" + filename;

            string solution = part == 1 ? SolutionPart1.ToString() : SolutionPart2.ToString();
            TimeSpan solutionTime = part == 1 ? solutionTime1 : solutionTime2;

            string fileContent = $"{Environment.MachineName}|{Configuration}\\Day {number} - Part {part}: {solution} (solved in {solutionTime.TotalSeconds} seconds / {solutionTime}, saved at {DateTime.Now:yyyy-MM-dd_HH-mm-ss})\n";

            if (append)
            {
                File.AppendAllText(filePath, fileContent, Encoding.UTF8);
            }
            else
            {
                File.WriteAllText(filePath, fileContent, Encoding.UTF8);
            }


        }

        public static void RunDay(int number, Day dayInstance = null, bool batch = false, bool verbose = true, int times = 1)
        {
            if (dayInstance == null)
            {
                Type dayType = Type.GetType("AdventOfCode2016.Days.Day" + number);
                if (dayType == null)
                {
                    throw new Exception("Couldn't find type AdventOfCode2016.Days.Day" + number);
                }
                dayInstance = (Day)Activator.CreateInstance(dayType);
            }


            for (int i = 0; i < times; i++)
            {
                var sw = new Stopwatch();

                sw.Start();
                object solution1 = dayInstance.GetSolutionPart1();
                sw.Stop();
                if (dayInstance.solutionPart1 == null)
                {
                    dayInstance.solutionPart1 = solution1;
                    dayInstance.solutionTime1 = sw.Elapsed;
                }


                //dayInstance.WriteToFile();
                if (verbose)
                {
                    Console.WriteLine($"day {dayInstance.number} part 1 : {dayInstance.SolutionPart1} - solved in {dayInstance.solutionTime1.TotalSeconds} seconds ({dayInstance.solutionTime1.TotalMilliseconds} milliseconds)");
                }
                try
                {
                    dayInstance.WriteToFile(1);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                sw.Restart();
                object solution2 = dayInstance.GetSolutionPart2();
                sw.Stop();

                if (dayInstance.solutionPart2 == null)
                {
                    dayInstance.solutionPart2 = solution2;
                    dayInstance.solutionTime2 = sw.Elapsed;
                }


                if (verbose)
                {
                    Console.WriteLine($"day {dayInstance.number} part 2 : {dayInstance.SolutionPart2} - solved in {dayInstance.solutionTime2.TotalSeconds} seconds ({dayInstance.solutionTime2.TotalMilliseconds} milliseconds)");
                    Console.WriteLine($"total time: {dayInstance.TotalTime.TotalSeconds} seconds ({dayInstance.TotalTime.TotalMilliseconds} milliseconds)");
                }

                try
                {
                    dayInstance.WriteToFile(2);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                var run = new Dictionary<string, TimeSpan>
                {
                    {"Total", dayInstance.TotalTime},
                    {"Part1", dayInstance.solutionTime1},
                    {"Part2", dayInstance.solutionTime2}
                };
                if (!solutionTimes.ContainsKey(number))
                {
                    solutionTimes[number] = new List<Dictionary<string, TimeSpan>>();
                }
                solutionTimes[number].Add(run);
            }

            if (!batch)
            {
                Console.Read();
            }
        }
    }
}
