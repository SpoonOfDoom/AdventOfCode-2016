using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AdventOfCode2016.Extensions;

namespace AdventOfCode2016.Days
{
    // ReSharper disable once UnusedMember.Global
    class Day5 : Day
	{
		public Day5() : base(5) {}

		private static readonly Random Rand = new Random();
		private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_/*\\+-|?^°!\"§$%&()=~";
		private static StringBuilder guessBuilder = new StringBuilder();
		private static MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

		private static byte[] GetHashBytes(string s)
		{
			byte[] bytes = s.Select(Convert.ToByte).ToArray();
			byte[] hashedBytes = md5.ComputeHash(bytes);

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
						guessBuilder.Append(Characters[Rand.Next(Characters.Length)]);
					}
					else
					{
						guessBuilder[j] = Characters[Rand.Next(Characters.Length)];
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
			var passwordBuilder = new StringBuilder();
			int i = 0;
			while (passwordBuilder.Length < 8)
			{
				if (i%10000 == 0)
				{
					Console.Write("Hacking... {0}\r", GetGuessString(passwordBuilder));
				}
				string word = Input + i;
				byte[] hashBytes = GetHashBytes(word);
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
				doorId = Input;
			}
			var passwordBuilder = new StringBuilder("________");
			int i = 0;
			
			while (passwordBuilder.ToString().Contains("_"))
			{
				if (i%5000 == 0)
				{
					Console.Write("Hakcing... {0}\r", GetGuessString(passwordBuilder));
				}
				string word = doorId + i;
				byte[] hashBytes = GetHashBytes(word);
				if (hashBytes[0] == 0 && hashBytes[1] == 0)
				{
					string s1 = hashBytes[2].ToString("x2");
					if (s1[0] == '0')
					{
						if (s1[1].IsNumeric())
						{
							int position = s1[1].ToInt();
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

	    protected override object GetSolutionPart1()
		{
			//"801b56a7";
			return GetPassword();
		}

	    protected override object GetSolutionPart2()
		{
			//424a0197
			return GetPassword2();
		}
	}
}
