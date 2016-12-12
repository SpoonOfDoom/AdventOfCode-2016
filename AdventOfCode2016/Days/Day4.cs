using System.Collections.Generic;
using System.Linq;
using System.Text;
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

			private static StringBuilder stringBuilder = new StringBuilder();

			private string CreateChecksum()
			{
				var lettersByOccurence = Name.Replace("-", "").GroupBy(c => c)
												.OrderByDescending(c => c.Count())
												.ThenBy(c => c.Key)
												.ToDictionary(grp => grp.Key, grp => grp.Count());
				var checksum = string.Join("", lettersByOccurence.Select(l => l.Key).ToList().Take(5));
				return checksum;
			}

			//^\w{5}-\w{4}-\w{7}

			private static char RotateLetter(char c, int amount)
			{
				if (c == 'z')
				{
					return 'a';
				}
				amount %= 26;

				for (int i = 0; i < amount; i++)
				{
					c = IncrementLetter(c);
				}
				return c;
			}

			private static char IncrementLetter(char c)
			{
				if (c == 'z')
				{
					return 'a';
				}
				c++;
				return c;
			}

			public string DecryptName()
			{
				stringBuilder.Clear();
				foreach (char c in Name)
				{
					if (c == '-')
					{
						stringBuilder.Append(c);
						continue;
					}
					stringBuilder.Append(RotateLetter(c, SectorId));
				}
				return stringBuilder.ToString(); //some names have a single wrong letter in them, not sure why yet. But it worked well enough to find the right room, so debugging is postponed.
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
			var decryptedNames = rooms.Select(r => r.DecryptName()).ToList();
			var sector = rooms.Single(r => r.DecryptName().StartsWith("northpole")).SectorId;

			
			return base.GetSolutionPart2();
		}
	}
}
