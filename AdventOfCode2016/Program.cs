using System;
using System.Diagnostics;
using AdventOfCode2016.Days;

namespace AdventOfCode2016
{
	class Program
	{
		static void Main()
		{
			Day d;
			Stopwatch sw = new Stopwatch();
			try
			{
				d = new Day5();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("Press any key to exit...");
				Console.Read();
				return;
			}


			string solution1, solution2;
			TimeSpan totalTime = new TimeSpan();
			try
			{
				sw.Start();
				solution1 = d.GetSolutionPart1();
				sw.Stop();
				totalTime += sw.Elapsed;
			}
			catch (NotImplementedException)
			{
				solution1 = "not implemented.";
			}
			Console.WriteLine($"day {d.Number} part 1 : {solution1} - solved in {sw.Elapsed.TotalSeconds} seconds ({sw.Elapsed.TotalMilliseconds} milliseconds)");

			try
			{
				sw.Restart();
				solution2 = d.GetSolutionPart2();
				sw.Stop();
				totalTime += sw.Elapsed;
			}
			catch (NotImplementedException)
			{
				solution2 = "not implemented.";
			}
			Console.WriteLine($"day {d.Number} part 2 : {solution2} - solved in {sw.Elapsed.TotalSeconds} seconds ({sw.Elapsed.TotalMilliseconds} milliseconds)");
			Console.WriteLine($"total time: {totalTime.TotalSeconds} seconds ({totalTime.TotalMilliseconds} milliseconds)");

			Console.Read();
		}
	}
}
