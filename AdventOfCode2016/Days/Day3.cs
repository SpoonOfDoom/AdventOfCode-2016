using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Extensions;

namespace AdventOfCode2016.Days
{
    // ReSharper disable once UnusedMember.Global
    class Day3 : Day
	{
		private class Triangle
		{
			public int SideA;
			public int SideB;
			public int SideC;

			public Triangle(int sideA, int sideB, int sideC)
			{
				SideA = sideA;
				SideB = sideB;
				SideC = sideC;
			}

			public Triangle(List<int> sides)
			{
				if (sides.Count != 3)
				{
					throw new Exception("WTF not 3 sides OMGWTFBBQ!");
				}
				SideA = sides[0];
				SideB = sides[1];
				SideC = sides[2];
			}

			public bool IsPossible => SideA + SideB > SideC 
										&& SideA + SideC > SideB
										&& SideB + SideC > SideA;
		}
		public Day3() : base(3) {}
		
		private static List<Triangle> Triangles = new List<Triangle>();

		private void ParseLines()
		{
			foreach (var line in InputLines)
			{
				var s = line;
				while (s.Contains("  ")) //normalize spaces without regex
				{
					s = s.Replace("  ", " ");
				}

				var sides = s.Trim().Split(' ').Select(x => x.ToInt()).ToList();
				var t = new Triangle(sides[0], sides[1], sides[2]);
				Triangles.Add(t);
			}
		}

		private void ParseLinesVertically()
		{
			var triangleA = new List<int>();
			var triangleB = new List<int>();
			var triangleC = new List<int>();

			foreach (string line in InputLines)
			{
				string s = line;
				while (s.Contains("  ")) //normalize spaces without regex
				{
					s = s.Replace("  ", " ");
				}

				List<int> sides = s.Trim().Split(' ').Select(x => x.ToInt()).ToList();
				triangleA.Add(sides[0]);
				triangleB.Add(sides[1]);
				triangleC.Add(sides[2]);
				if (triangleA.Count == 3)
				{
					Triangles.Add(new Triangle(triangleA));
					Triangles.Add(new Triangle(triangleB));
					Triangles.Add(new Triangle(triangleC));

					triangleA.Clear();
					triangleB.Clear();
					triangleC.Clear();
				}
			}
		}

	    protected override object GetSolutionPart1()
		{
			ParseLines();
			int count = Triangles.Count(t => t.IsPossible);
			return count;
		}

	    protected override object GetSolutionPart2()
		{
			Triangles.Clear();
			ParseLinesVertically();
			int count = Triangles.Count(t => t.IsPossible);
			return count;
		}
	}
}
