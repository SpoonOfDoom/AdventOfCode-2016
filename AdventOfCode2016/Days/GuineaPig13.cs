using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AdventOfCode2016.Extensions;
using AdventOfCode2016.Tools;
using Priority_Queue;

namespace AdventOfCode2016.Days
{
    class GuineaPig13 : Day
    {
        public GuineaPig13() : base(13) {}

        private class Coordinate : ISearchNode
        {
            enum Direction
            {
                Left,
                Right,
                Up,
                Down
            }

            public int x;
            public int y;

            public int Cost { get; set; }

            public List<object> Actions { get; set; }

            public HashSet<ExpandAction> ExpandNode()
            {
                HashSet<ExpandAction> actions = new HashSet<ExpandAction>();

                var nodeLeft = new Coordinate { x = x - 1, y = y };
                if (!IsWall(nodeLeft))
                {
                    actions.Add(new ExpandAction {action = Direction.Left, cost = 1, result = nodeLeft});
                }


                var nodeRight = new Coordinate { x = x + 1, y = y };
                if (!IsWall(nodeRight))
                {
                    actions.Add(new ExpandAction { action = Direction.Right, cost = 1, result = nodeRight });
                }


                var nodeUp = new Coordinate { x = x, y = y - 1 };
                if (!IsWall(nodeUp))
                {
                    actions.Add(new ExpandAction { action = Direction.Up, cost = 1, result = nodeUp });
                }


                var nodeDown = new Coordinate { x = x, y = y + 1 };
                if (!IsWall(nodeDown))
                {
                    actions.Add(new ExpandAction { action = Direction.Down, cost = 1, result = nodeDown });
                }

                return actions;
            }

            public bool Equals(ISearchNode goalState)
            {
                var target = goalState as Coordinate;
                return x == target.x && y == target.y;
            }

            public int GetHeuristic(ISearchNode goalState)
            {
                var target = goalState as Coordinate;
                if (nodeHeuristicLookup.ContainsKey(target))
                {
                    return nodeHeuristicLookup[target];
                }
                
                if (Equals(goalState))
                {
                    return 0;
                }
                return Math.Abs(x - target.x) + Math.Abs(y - target.y);
            }

            public int GetTentativeCost(ISearchNode goalState)
            {
                int heuristic = GetHeuristic(goalState as Coordinate);
                return Cost + heuristic;
            }

            public string VerboseInfo { get; }
            public bool IsGoalState(ISearchNode goalState = null)
            {
                throw new NotImplementedException();
            }

            float ISearchNode.GetHeuristic(ISearchNode goalState)
            {
                return GetHeuristic(goalState);
            }
        }

        private static Dictionary<Coordinate, bool> isWallLookup = new Dictionary<Coordinate, bool>();
        private static Dictionary<Coordinate, int> nodeCostLookup = new Dictionary<Coordinate, int>();
        private static Dictionary<Coordinate, int> nodeHeuristicLookup = new Dictionary<Coordinate, int>();
        static int number;
        
        
        private static bool IsWall(Coordinate tile)
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
            var dummy = new Coordinate
                            {x = int.MaxValue, y = int.MaxValue};
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
                    Coordinate c = new Coordinate
                                       {x = x, y = y};
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
            int result = new AStar().GetMinimumCost(start, target);
            return result;
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

                //DrawMap(start, current, openList, closed);
                
                int newCost = nodeCostLookup[current] + 1;
                var newNodes = current.ExpandNode().Select(x => x.result as Coordinate);

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
            number = input.ToInt();
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
            return base.GetSolutionPart2();
            var start = new Coordinate { x = 1, y = 1 };
            int steps = 50;
            int tiles = FindRange(start, steps, manual:false);
            return tiles.ToString();
        }
    }
}
