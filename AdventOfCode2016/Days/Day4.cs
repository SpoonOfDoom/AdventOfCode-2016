using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using AdventOfCode2016.Extensions;

namespace AdventOfCode2016.Days
{
	class Day4 : Day
	{
		private class Room
		{
			public string Name;
			public int SectorId;
			public string Checksum;

			private string CreateChecksum()
			{
				var lettersByOccurence = Name.Replace("-", "").GroupBy(c => c)
												.OrderByDescending(c => c.Count())
												.ThenBy(c => c.Key)
												.ToDictionary(grp => grp.Key, grp => grp.Count());
				var checksum = string.Join("", lettersByOccurence.Select(l => l.Key).ToList().Take(5));
				return checksum;
			}

			public bool IsValid()
			{
				return CreateChecksum() == Checksum;
			}
		}
		public Day4() : base(4) {}

		private Regex regex = new Regex(@"(.+)-(\d+)\[(\w+)\]");
		private List<Room> rooms = new List<Room>();

		private void ParseLines()
		{
			foreach (string line in inputLines)
			{
				var groups = regex.Match(line).Groups;

				Room r = new Room()
				{
					Name = groups[1].Value,
					SectorId = groups[2].Value.ToInt(),
					Checksum = groups[3].Value
				};

				rooms.Add(r);
			}
		}
		
		public override string GetSolutionPart1()
		{
			ParseLines();
			var realRooms = rooms.Where(r => r.IsValid());
			return realRooms.Sum(r => r.SectorId).ToString();
		}

		public override string GetSolutionPart2()
		{
			return base.GetSolutionPart2();
		}
	}
}
