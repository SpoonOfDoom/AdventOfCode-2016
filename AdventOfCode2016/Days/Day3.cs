using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Extensions;

namespace AdventOfCode2016.Days
{
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
			foreach (var line in inputLines)
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
			List<int> TriangleA = new List<int>();
			List<int> TriangleB = new List<int>();
			List<int> TriangleC = new List<int>();

			foreach (var line in inputLines)
			{
				var s = line;
				while (s.Contains("  ")) //normalize spaces without regex
				{
					s = s.Replace("  ", " ");
				}

				var sides = s.Trim().Split(' ').Select(x => x.ToInt()).ToList();
				TriangleA.Add(sides[0]);
				TriangleB.Add(sides[1]);
				TriangleC.Add(sides[2]);
				if (TriangleA.Count == 3)
				{
					Triangles.Add(new Triangle(TriangleA));
					Triangles.Add(new Triangle(TriangleB));
					Triangles.Add(new Triangle(TriangleC));

					TriangleA.Clear();
					TriangleB.Clear();
					TriangleC.Clear();
				}
			}
		}

		public override string GetSolutionPart1()
		{
			ParseLines();
			var count = Triangles.Count(t => t.IsPossible);
			return count.ToString();
		}

		public override string GetSolutionPart2()
		{
			Triangles.Clear();
			ParseLinesVertically();
			var count = Triangles.Count(t => t.IsPossible);
			return count.ToString();
		}
	}
}
