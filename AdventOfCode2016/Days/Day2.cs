using System;
using System.Collections.Generic;

namespace AdventOfCode2016.Days
{
	class Day2 : Day
	{
		public Day2() : base(2) {}
		private Dictionary<string, int> CoordinateKeys = new Dictionary<string, int>
		{
			["0-0"] = 1,
			["1-0"] = 2,
			["2-0"] = 3,

			["0-1"] = 4,
			["1-1"] = 5,
			["2-1"] = 6,

			["0-2"] = 7,
			["1-2"] = 8,
			["2-2"] = 9,
		};

		private Dictionary<string, char> CoordinateKeys2 = new Dictionary<string, char>
		{
			["2-0"] = '1',

			["1-1"] = '2',
			["2-1"] = '3',
			["3-1"] = '4',

			["0-2"] = '5',
			["1-2"] = '6',
			["2-2"] = '7',
			["3-2"] = '8',
			["4-2"] = '9',

			["1-3"] = 'A',
			["2-3"] = 'B',
			["3-3"] = 'C',

			["2-4"] = 'D'
		};


		private void Move<T>(char direction, ref int x, ref int y, Dictionary<string, T> dict )
		{
			switch (direction)
			{
				case 'U':
					y = dict.ContainsKey($"{x}-{y - 1}") ? y - 1 : y;
					break;
				case 'D':
					y = dict.ContainsKey($"{x}-{y + 1}") ? y + 1 : y;
					break;
				case 'R':
					x = dict.ContainsKey($"{x + 1}-{y}") ? x + 1 : x;
					break;
				case 'L':
					x = dict.ContainsKey($"{x - 1}-{y}") ? x - 1 : x;
					break;
				default:
					throw new Exception("Invalid direction!");
			}
		}


		public override object GetSolutionPart1()
		{
			int x = 1;
			int y = 1;

			var buttons = new List<int>();

			foreach (string line in inputLines)
			{
				foreach (char c in line)
				{
					Move(c, ref x, ref y, CoordinateKeys);
				}
				buttons.Add(CoordinateKeys[$"{x}-{y}"]);
			}

			return string.Join("", buttons);
		}

		public override object GetSolutionPart2()
		{
			int x = 0;
			int y = 2;
			
			var buttons = new List<char>();
			
			foreach (string line in inputLines)
			{
				foreach (char c in line)
				{
					Move(c, ref x, ref y, CoordinateKeys2);
				}
				buttons.Add(CoordinateKeys2[$"{x}-{y}"]);
			}

			return string.Join("", buttons);
		}
	}
}
