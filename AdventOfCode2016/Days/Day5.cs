using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AdventOfCode2016.Extensions;

namespace AdventOfCode2016.Days
{
	class Day5 : Day
	{
		public Day5() : base(5) {}

		private static Random rand = new Random();
		private const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_/*\\+-|?^°!\"§$%&()=~";
		private static StringBuilder guessBuilder = new StringBuilder();
		private static MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

		private static byte[] GetHashBytes(string s)
		{
			var bytes = s.Select(Convert.ToByte).ToArray();
			var hashedBytes = md5.ComputeHash(bytes);

			return hashedBytes;
		}

		private static string GetGuessString(StringBuilder passwordBuilder)
		{
			guessBuilder.Clear();
			for (int j = 0; j < 8; j++)
			{
				if (passwordBuilder.Length <= j || passwordBuilder[j] == '_')
				{
					if (guessBuilder.Length <= j)
					{
						guessBuilder.Append(characters[rand.Next(characters.Length)]);
					}
					else
					{
						guessBuilder[j] = characters[rand.Next(characters.Length)];
					}
				}
				else
				{
					if (guessBuilder.Length <= j)
					{
						guessBuilder.Append(passwordBuilder[j]);
					}
					else
					{
						guessBuilder[j] = passwordBuilder[j];
					}
				}
			}
			return guessBuilder.ToString();
		}

		private string GetPassword()
		{
			Console.WriteLine("Initialize the HACKING!!!!!11!");
			Console.WriteLine();
			StringBuilder passwordBuilder = new StringBuilder();
			int i = 0;
			while (passwordBuilder.Length < 8)
			{
				if (i%10000 == 0)
				{
					Console.Write("Hacking... {0}\r", GetGuessString(passwordBuilder));
				}
				string word = input + i;
				var hashBytes = GetHashBytes(word);
				if (hashBytes[0] == 0 && hashBytes[1] == 0)
				{
					string s1 = hashBytes[2].ToString("x2");
					if (s1[0] == '0')
					{
						passwordBuilder.Append(s1[1]);
					}
				}
				i++;
			}
			Console.Write("Hakcing... {0}\r", GetGuessString(passwordBuilder));
			Console.WriteLine();
			return passwordBuilder.ToString();
		}

		private string GetPassword2(string doorId = null)
		{
			Console.WriteLine("Initialize the h4ck1ng... AGAIN!!!!!11!");
			Console.WriteLine();
			if (doorId == null)
			{
				doorId = input;
			}
			StringBuilder passwordBuilder = new StringBuilder("________");
			int i = 0;
			
			while (passwordBuilder.ToString().Contains("_"))
			{
				if (i%5000 == 0)
				{
					Console.Write("Hakcing... {0}\r", GetGuessString(passwordBuilder));
				}
				string word = doorId + i;
				var hashBytes = GetHashBytes(word);
				if (hashBytes[0] == 0 && hashBytes[1] == 0)
				{
					string s1 = hashBytes[2].ToString("x2");
					if (s1[0] == '0')
					{
						if (s1[1].IsNumeric())
						{
							var position = s1[1].ToInt();
							if (position < 8 && passwordBuilder[position] == '_')
							{
								passwordBuilder[position] = hashBytes[3].ToString("x2")[0];
							}
						}
						
					}
				}
				i++;
			}
			Console.Write("Hakcing... {0}\r", GetGuessString(passwordBuilder));
			Console.WriteLine();
			return passwordBuilder.ToString();
		}

		public override object GetSolutionPart1()
		{
			//"801b56a7";
			return GetPassword();
		}

		public override object GetSolutionPart2()
		{
			//424a0197
			return GetPassword2();
		}
	}
}
