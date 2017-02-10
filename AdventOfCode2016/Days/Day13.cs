using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AdventOfCode2016.Extensions;
using Priority_Queue;

namespace AdventOfCode2016.Days
{
	class Day13 : Day
	{
		public Day13() : base(13) {}

		private struct Coordinate
		{
			public int x;
			public int y;
		}

		private Dictionary<Coordinate, bool> isWallLookup = new Dictionary<Coordinate, bool>();
		private Dictionary<Coordinate, int> nodeCostLookup = new Dictionary<Coordinate, int>();
		private Dictionary<Coordinate, int> nodeHeuristicLookup = new Dictionary<Coordinate, int>();
		
		
		private bool IsWall(Coordinate tile, int number)
		{
			if (isWallLookup.ContainsKey(tile))
			{
				return isWallLookup[tile];
			}
			int x = tile.x;
			int y = tile.y;

			if (x < 0 || y < 0)
			{
				isWallLookup[tile] = true;
				return true;
			}

			int sum = x*x + 3*x + 2*x*y + y + y*y;
			sum += number;

			string binary = Convert.ToString(sum, 2);

			bool isWall = binary.Count(c => c == '1')%2 != 0;
			isWallLookup[tile] = isWall;
			return isWall;
		}

		private int GetTentativeCost(Coordinate node, Coordinate target)
		{
			int cost = nodeCostLookup[node];
			if (nodeHeuristicLookup.ContainsKey(node))
			{
				return cost + nodeHeuristicLookup[node];
			}
			int heuristic = GetHeuristic(node, target);
			nodeHeuristicLookup[node] = heuristic;
			return cost + heuristic;
		}

		private static int GetHeuristic(Coordinate node, Coordinate target)
		{
			if (node.Equals(target))
			{
				return 0;
			}
			return Math.Abs(node.x - target.x) + Math.Abs(node.y - target.y);
		}

		/// <summary>
		/// Returns all legal moves from the node.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		private List<Coordinate> ExpandNode(Coordinate node)
		{
			int number = input.ToInt();
			//number = 10; //testcase

			List<Coordinate> nodes = new List<Coordinate>();
			
			var nodeLeft = new Coordinate() {x = node.x-1, y = node.y};
			if (!IsWall(nodeLeft, number))
			{
				nodes.Add(nodeLeft);
			}
			

			var nodeRight = new Coordinate() {x = node.x+1, y = node.y};
			if (!IsWall(nodeRight, number))
			{
				nodes.Add(nodeRight);
			}

			
			var nodeUp = new Coordinate() {x = node.x, y = node.y -1};
			if (!IsWall(nodeUp, number))
			{
				nodes.Add(nodeUp);
			}
			

			var nodeDown = new Coordinate() {x = node.x, y = node.y +1};
			if (!IsWall(nodeDown, number))
			{
				nodes.Add(nodeDown);
			}
			
			return nodes;
		}

		private static void ConsoleWithColor(object content, ConsoleColor bgcolor = ConsoleColor.Black, ConsoleColor fgColor = ConsoleColor.White)
		{
			Console.BackgroundColor = bgcolor;
			Console.ForegroundColor = fgColor;

			Console.Write(content.ToString());

			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.White;
		}

		//Lazy way to draw without specifying a target Coordinate.
		private void DrawMap(Coordinate start, Coordinate current, SimplePriorityQueue<Coordinate> openList, HashSet<Coordinate> closed)
		{
			var dummy = new Coordinate() {x = int.MaxValue, y = int.MaxValue};
			DrawMap(start, dummy, current, openList, closed);
		}

		private void DrawMap(Coordinate start, Coordinate target, Coordinate current, SimplePriorityQueue<Coordinate> openList, HashSet<Coordinate> closed)
		{
			Console.CursorLeft = 0;
			Console.CursorTop = 0;
			Console.WriteLine("Legend - Free:. Wall:X Start:S Target:T Closed:c Open:o Current:G");

			int maxX = isWallLookup.Keys.Max(t => t.x);
			int maxY = isWallLookup.Keys.Max(t => t.y);
			for (int y = 0; y <= maxY; y++)
			{
				Console.WriteLine();
				for (int x = 0; x <= maxX; x++)
				{
					Coordinate c = new Coordinate() {x = x, y = y};
					if (c.Equals(current))
					{
						ConsoleWithColor("G", fgColor: ConsoleColor.Magenta);
					}
					else if (c.Equals(start))
					{
						ConsoleWithColor("S", fgColor: ConsoleColor.Cyan);
					}
					else if (c.Equals(target))
					{
						ConsoleWithColor("T", fgColor: ConsoleColor.Cyan);
					}
					else if (closed.Contains(c))
					{
						ConsoleWithColor("c", fgColor: ConsoleColor.DarkGreen);
					}
					else if (openList.Contains(c))
					{
						ConsoleWithColor("o", fgColor: ConsoleColor.Yellow);
					}
					else if (isWallLookup.ContainsKey(c))
					{
						if (isWallLookup[c])
						{
							ConsoleWithColor("X", fgColor: ConsoleColor.Red);
						}
						else
						{
							ConsoleWithColor(".");
						}
					}
					else
					{
						ConsoleWithColor("?");
					}

				}
			}
		}

		private int FindPath(Coordinate start, Coordinate target, bool manual = true, int autoStepTime = 200)
		{
			var closed = new HashSet<Coordinate>();
			var openList = new SimplePriorityQueue<Coordinate>();
			nodeCostLookup[start] = 0;
			openList.Enqueue(start, 0);
			isWallLookup[start] = false;
			
			while (openList.Count > 0)
			{

				var current = openList.Dequeue();
				closed.Add(current);
				if (current.Equals(target))
				{
					return nodeCostLookup[current];
				}

				DrawMap(start, target, current, openList, closed);
				var newNodes = ExpandNode(current);
				int newCost = nodeCostLookup[current] + 1;

				if (manual)
				{
					Console.ReadLine();
				}
				else
				{
					Thread.Sleep(autoStepTime);
				}
				
				foreach (var node in newNodes)
				{
					if (closed.Contains(node))
					{
						continue;
					}
					if (openList.Contains(node))
					{
						if (GetTentativeCost(node, target) > newCost + GetHeuristic(node, target))
						{
							nodeCostLookup[node] = newCost;
							openList.UpdatePriority(node, GetTentativeCost(node, target));
							continue;
						}
					}
					nodeCostLookup[node] = newCost;
					openList.Enqueue(node, GetTentativeCost(node, target));
				}
			}

			return -1;
		}

		private int FindRange(Coordinate start, int maxSteps, bool manual = true, int autoStepTime = 10)
		{
			var closed = new HashSet<Coordinate>();
			var openList = new SimplePriorityQueue<Coordinate>();
			HashSet<Coordinate> visitedLocations = new HashSet<Coordinate>();
			nodeCostLookup[start] = 0;
			openList.Enqueue(start, 0);
			isWallLookup[start] = false;
			
			while (openList.Count > 0)
			{

				var current = openList.Dequeue();
				closed.Add(current);
				if (nodeCostLookup[current] <= maxSteps)
				{
					visitedLocations.Add(current);
				}

				DrawMap(start, current, openList, closed);
				
				int newCost = nodeCostLookup[current] + 1;
				var newNodes = ExpandNode(current);

				if (manual)
				{
					Console.ReadLine();
				}
				else
				{
					Thread.Sleep(autoStepTime);
				}

				if (newCost > maxSteps)
				{
					continue;
				}
				foreach (var node in newNodes)
				{
					if (closed.Contains(node))
					{
						continue;
					}
					if (openList.Contains(node))
					{
						continue;
					}
					nodeCostLookup[node] = newCost;
					openList.Enqueue(node, 1);
				}
			}

			return visitedLocations.Count;
		}

		public override object GetSolutionPart1()
		{
			var start = new Coordinate { x = 1, y = 1 };
			var target = new Coordinate { x = 31, y = 39 };
			int pathCost = FindPath(start, target, manual:false);

			if (pathCost == -1)
			{
				Console.WriteLine("Fail - no path found :(");
			}

			return pathCost.ToString();
		}

		public override object GetSolutionPart2()
		{
			var start = new Coordinate { x = 1, y = 1 };
			int steps = 50;
			int tiles = FindRange(start, steps, manual:false);
			return tiles.ToString();
		}
	}
}
