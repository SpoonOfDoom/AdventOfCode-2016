using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Extensions;
using AdventOfCode2016.Tools;

namespace AdventOfCode2016.Days
{
    // ReSharper disable once UnusedMember.Global
    class Day24 : Day
    {
        public Day24() : base(24) {}

        private class SearchHelper
        {
            private enum SearchColors
            {
                Visited,
                Open,
                Current,
                Next,
                Path,
                Start,
                Goal
            }

            private static Dictionary<SearchColors, ConsoleColor> colorDict = new Dictionary<SearchColors, ConsoleColor>
            {
                {SearchColors.Visited, ConsoleColor.DarkBlue },
                {SearchColors.Open, ConsoleColor.Blue },
                {SearchColors.Current, ConsoleColor.Red },
                {SearchColors.Next, ConsoleColor.DarkMagenta },
                {SearchColors.Path, ConsoleColor.Green },
                {SearchColors.Start, ConsoleColor.Yellow },
                {SearchColors.Goal, ConsoleColor.DarkYellow },

            };
            private class GameState : ISearchNode
            {
                public static Maze maze;
                internal Coordinates robotCoordinates;

                private GameState CloneAndMove(Directions direction)
                {
                    GameState clone = new GameState
                    {
                        robotCoordinates = robotCoordinates.Move(direction),
                        Actions = new List<object>(Actions),
                        Cost = Cost,
                    };
                    return clone;
                }

                public int Cost { get; set; }
                public List<object> Actions { get; set; }

                public string VerboseInfo => "";

                public string StringHash => robotCoordinates.ToString();
                public long NumericHash { get; }

                public HashSet<ExpandAction> ExpandNode()
                {
                    HashSet<ExpandAction> actions = new HashSet<ExpandAction>();
                    Dictionary<Directions, TileType> adjacentTiles = maze.GetAdjacents(robotCoordinates);
                    foreach (KeyValuePair<Directions, TileType> pair in adjacentTiles)
                    {
                        if (pair.Value != TileType.Wall)
                        {
                            actions.Add(new ExpandAction
                            {
                                Action = pair.Key,
                                Cost = 1,
                                Result = CloneAndMove(pair.Key)
                            });
                        }
                    }
                    return actions;
                }

                public bool Equals(ISearchNode otherState)
                {
                    return StringHash == otherState.StringHash;
                }

                public bool IsGoalState(ISearchNode goalState = null)
                {
                    return Equals(goalState);
                }

                public float GetHeuristic(ISearchNode goalState = null)
                {
                    if (this.IsGoalState(goalState))
                    {
                        return 0;
                    }
                    int md = Toolbox.GetManhattanDistance(robotCoordinates.x,
                        robotCoordinates.y,
                        ((GameState) goalState).robotCoordinates.x,
                        ((GameState) goalState).robotCoordinates.y);
                    return md;
                }

                public void CreateHash()
                {
                    throw new NotImplementedException();
                }
            }

            private void PrintMaze(Maze maze, List<Coordinates> visited, List<Coordinates> open, Coordinates position, Coordinates goal, Coordinates consoleCursorStart, List<Coordinates> shortestPath = null)
            {
                Console.SetCursorPosition(consoleCursorStart.x, consoleCursorStart.y);
                Coordinates c = new Coordinates(0,0);
                bool colorDirty = false;
                for (int y = 0; y < maze.Height; y++)
                {
                    for (int x = 0; x < maze.Width; x++)
                    {
                        c.x = x;
                        c.y = y;
                        if (colorDirty)
                        {
                            Console.ResetColor();
                            colorDirty = false;
                        }
                        if (open.Contains(c))
                        {
                            Console.BackgroundColor = open.IndexOf(c) == 0 ? colorDict[SearchColors.Next] : colorDict[SearchColors.Open];
                            colorDirty = true;
                        }
                        if (visited.Contains(c))
                        {
                            Console.BackgroundColor = colorDict[SearchColors.Visited];
                            colorDirty = true;
                        }

                        if (c.Equals(goal))
                        {
                            Console.BackgroundColor = colorDict[SearchColors.Goal];
                            colorDirty = true;
                        }

                        if (x == position.x && y == position.y)
                        {
                            Console.BackgroundColor = colorDict[SearchColors.Current];
                            colorDirty = true;
                            Console.Write('R');
                            continue;
                        }
                        if (shortestPath != null && shortestPath.Contains(c))
                        {
                            Console.BackgroundColor = colorDict[SearchColors.Path];
                            colorDirty = true;
                        }

                        switch (maze[x, y])
                        {
                            case TileType.Wall:
                                Console.Write('#');
                                break;
                            case TileType.Path:
                                Console.Write('.');
                                break;
                            case TileType.Target:
                                Console.Write('T');
                                break;
                            case TileType.Start:
                                Console.BackgroundColor = colorDict[SearchColors.Start];
                                colorDirty = true;
                                Console.Write('0');
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    Console.Write("\n");
                }
            }
            
            public Dictionary<Tuple<Coordinates, Coordinates>, int> GetDistances(Maze maze, bool verbose = false)
            {
                if (verbose)
                {
                    Console.Clear();
                    Console.WriteLine("Getting distances from start...");

                }
                GameState.maze = maze;
                AStar aStar = new AStar(AStar.NodeHashMode.String);
                aStar.GeneralVerboseOutputPrinted += AStar_GeneralVerboseOutputPrinted;

                Dictionary<Tuple<Coordinates, Coordinates>, int> distances = new Dictionary<Tuple<Coordinates, Coordinates>, int>();

                List<Coordinates> targets = maze.GetTargCoordinateList();

                int t = 0;
                Coordinates mazeStartCoordinates = maze.GetStartCoordinates();
                foreach (Coordinates target in targets)
                {
                    t++;
                    if (verbose)
                    {
                        Console.SetCursorPosition(0, 1);
                        Console.WriteLine($"Searching start to target {t} of {targets.Count}...");
                    }

                    GameState startState = new GameState
                    {
                        robotCoordinates = mazeStartCoordinates,
                        Actions = new List<object>()
                    };

                    GameState goalState = new GameState
                    {
                        robotCoordinates = target,
                        Actions = new List<object>()
                    };
                    Tuple<List<object>, int> result = aStar.GetOptimalPath(startState, goalState);
                    int cost = result.Item2;
                    distances[new Tuple<Coordinates, Coordinates>(mazeStartCoordinates, target)] = cost;
                }

                if (verbose)
                {
                    Console.WriteLine("Start finished. moving on...");
                    Console.WriteLine($"Target count: {targets.Count}");
                }
                for (int i = 0; i < targets.Count; i++)
                {
                    for (int j = i+1; j < targets.Count; j++)
                    {
                        Coordinates startCoordinates = targets[i];
                        Coordinates target = targets[j];
                        if (verbose)
                        {
                            Console.SetCursorPosition(0,4);
                            Console.WriteLine($"i: {i}     ");
                            Console.WriteLine($"j: {j}     ");
                            Console.WriteLine($"start: {startCoordinates}     ");
                            Console.WriteLine($"target: {target}     ");
                        }
                        GameState startState = new GameState
                        {
                            robotCoordinates = startCoordinates,
                            Actions = new List<object>()
                        };

                        GameState goalState = new GameState
                        {
                            robotCoordinates = target,
                            Actions = new List<object>()
                        };
                        
                        int cost = aStar.GetMinimumCost(startState, goalState);
                        distances[new Tuple<Coordinates, Coordinates>(startCoordinates, target)] = cost;
                    }
                }
                if (verbose)
                {
                    Console.WriteLine("Finished getting distances.");
                    Console.WriteLine("Adding reversed distances...");
                }

                Dictionary<Tuple<Coordinates, Coordinates>, int> tempDict = new Dictionary<Tuple<Coordinates, Coordinates>, int>(distances);
                foreach (KeyValuePair<Tuple<Coordinates, Coordinates>, int> pair in tempDict)
                {
                    if (pair.Key.Item1.Equals(mazeStartCoordinates))
                    {
                        continue;
                    }
                    Tuple<Coordinates, Coordinates> flipped = pair.Key.Flip();
                    if (!distances.ContainsKey(flipped))
                    {
                        distances.Add(flipped, pair.Value);
                    }
                }

                if (verbose)
                {
                    Console.WriteLine("Distances finished!");
                }

                
                return distances;
            }

            private void AStar_GeneralVerboseOutputPrinted(object sender, EventArgs e)
            {
                SearchEventArgs se = e as SearchEventArgs;
                GameState current = se.CurrentNode as GameState;
                GameState goal = se.GoalNode as GameState;
                List<Coordinates> closedCoordinates = se.ClosedSet.Cast<GameState>().Select(x => x.robotCoordinates).ToList();
                List<Coordinates> openCoordinates = se.OpenQueue.Cast<GameState>().Select(x => x.robotCoordinates).ToList();
                PrintMaze(GameState.maze, closedCoordinates, openCoordinates, current.robotCoordinates, goal.robotCoordinates, new Coordinates(Console.CursorLeft, Console.CursorTop));
            }
        }

        enum Directions
        {
            Up,
            Down,
            Left,
            Right
        }

        private enum TileType
        {
            Wall,
            Path,
            Target,
            Start
        }

        private struct Coordinates
        {
            public int x, y;

            public Coordinates(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public Coordinates Move(Directions direction)
            {
                Coordinates newCoordinates = new Coordinates();
                switch (direction)
                {
                    case Directions.Up:
                        newCoordinates.x = x;
                        newCoordinates.y = y - 1;
                        break;
                    case Directions.Down:
                        newCoordinates.x = x;
                        newCoordinates.y = y + 1;
                        break;
                    case Directions.Left:
                        newCoordinates.x = x - 1;
                        newCoordinates.y = y;
                        break;
                    case Directions.Right:
                        newCoordinates.x = x + 1;
                        newCoordinates.y = y;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                }
                return newCoordinates;
            }

            public override string ToString()
            {
                return $"{x}|{y}";
            }

            /// <summary>Gibt an, ob diese Instanz und ein angegebenes Objekt gleich sind.</summary>
            /// <returns>true, wenn <paramref name="obj" /> und diese Instanz denselben Typ aufweisen und denselben Wert darstellen, andernfalls false. </returns>
            /// <param name="obj">Das Objekt, das mit der aktuellen Instanz verglichen werden soll. </param>
            public override bool Equals(object obj)
            {
                return x == ((Coordinates)obj).x && y == ((Coordinates)obj).y;
            }

            public bool Equals(Coordinates other)
            {
                return x == other.x && y == other.y;
            }

            /// <summary>Gibt den Hashcode für diese Instanz zurück.</summary>
            /// <returns>Eine 32-Bit-Ganzzahl mit Vorzeichen. Diese ist der Hashcode für die Instanz.</returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    return (x * 397) ^ y;
                }
            }
        }

        private class Maze
        {
            private TileType[,] tiles;
            public TileType this[int x, int y] => tiles[x, y];
            public TileType this[Coordinates c] => tiles[c.x, c.y];
            public int Width => tiles.GetUpperBound(0) + 1;
            public int Height => tiles.GetUpperBound(1) + 1;

            public Maze(List<string> lines)
            {
                tiles = ParseMaze(lines);
            }

            public Dictionary<Directions, TileType> GetAdjacents(Coordinates start)
            {
                Dictionary<Directions, TileType> dict = new Dictionary<Directions, TileType>();
                dict[Directions.Up] = GetAdjacent(start, Directions.Up);
                dict[Directions.Down] = GetAdjacent(start, Directions.Down);
                dict[Directions.Left] = GetAdjacent(start, Directions.Left);
                dict[Directions.Right] = GetAdjacent(start, Directions.Right);
                return dict;
            }

            public TileType GetAdjacent(Coordinates start, Directions direction)
            {
                switch (direction)
                {
                    case Directions.Up:
                        if (start.y <= 0)
                        {
                            return TileType.Wall;
                        }
                        return this[start.x, start.y - 1];
                    case Directions.Down:
                        if (start.y >= Height)
                        {
                            return TileType.Wall;
                        }
                        return this[start.x, start.y + 1];
                    case Directions.Left:
                        if (start.x <= 0)
                        {
                            return TileType.Wall;
                        }
                        return this[start.x - 1, start.y];
                    case Directions.Right:
                        if (start.x >= Width)
                        {
                            return TileType.Wall;
                        }
                        return this[start.x + 1, start.y];
                    default:
                        throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                }
            }

            //private Maze(Maze original)
            //{
            //    TileType[,] t = new TileType[original.Width, original.Height];

            //    for (int x = 0; x < original.Width; x++)
            //    {
            //        for (int y = 0; y < original.Height; y++)
            //        {
            //            t[x, y] = original[x, y];
            //        }
            //    }
            //    tiles = t;
            //}

            //public Maze Clone()
            //{
            //    return new Maze(this);
            //}

            public Coordinates GetStartCoordinates()
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        if (this[x, y] == TileType.Start)
                        {
                            return new Coordinates(x, y);
                        }
                    }
                }
                throw new Exception("Couldn't find start of the maze!");
            }

            public List<Coordinates> GetTargCoordinateList()
            {
                List<Coordinates> coordinateList = new List<Coordinates>();
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        if (this[x,y] == TileType.Target)
                        {
                            coordinateList.Add(new Coordinates(x,y));
                        }
                    }
                }
                return coordinateList;
            }
        }

        private static TileType[,] ParseMaze(List<string> lines)
        {
            int width = lines[0].Length;
            int height = lines.Count;
            TileType[,] tiles = new TileType[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    switch (lines[y][x])
                    {
                        case '#':
                            tiles[x, y] = TileType.Wall;
                            break;
                        case '.':
                            tiles[x, y] = TileType.Path;
                            break;
                        case '0':
                            tiles[x, y] = TileType.Start;
                            break;
                        default:
                                tiles[x, y] = TileType.Target;
                            break;
                    }
                }
            }

            return tiles;
        }


        private int GetShortestTotalDistance(Dictionary<Tuple<Coordinates, Coordinates>, int> distances, Coordinates startCoordinates, bool backToStart = false)
        {
            List<int> distanceInts = new List<int>();
            Dictionary<Tuple<Coordinates, Coordinates>, int> backToStartDistances = null;
            if (backToStart)
            {
                backToStartDistances = distances.Where(x => x.Key.Item1.Equals(startCoordinates)).ToDictionary(x => x.Key.Flip(), y => y.Value);
            }
            GetDistances(distanceInts, 0, startCoordinates, distances, backToStartDistances);



            int result = distanceInts.Where(d => d > 0).Min();

            return result;
        }

        //this whole function and how its results are used is goddamn dirty. I'm ashamed, but I also want to move on.
        private int GetDistances(List<int> distances, int distanceSoFar, Coordinates position, Dictionary<Tuple<Coordinates, Coordinates>, int> remainingDistances, Dictionary<Tuple<Coordinates, Coordinates>, int> backToStartDistances = null)
        {
            if (remainingDistances.Count == 0)
            {
                if (backToStartDistances is null) //this is a really dirty way to do part 2, even compared to the rest, but it's also the way in which I don't have to rewrite too much of it
                {
                    return distanceSoFar;
                }
                else
                {
                    foreach (KeyValuePair<Tuple<Coordinates, Coordinates>, int> pair in backToStartDistances.Where(x => x.Key.Item1.Equals(position)))
                    {
                        distances.Add(GetDistances(distances, distanceSoFar + pair.Value, pair.Key.Item2, remainingDistances, null));
                    }
                    return -1;
                }
            }

            foreach (KeyValuePair<Tuple<Coordinates, Coordinates>, int> pair in remainingDistances)
            {
                if (pair.Key.Item1.Equals(position))
                {
                    Dictionary<Tuple<Coordinates, Coordinates>, int> rd = remainingDistances.Where(x => !(x.Key.Item1.Equals(position) || x.Key.Item2.Equals(position)))
                        .ToDictionary(x => x.Key, y => y.Value);
                    distances.Add(GetDistances(distances, distanceSoFar + pair.Value, pair.Key.Item2, rd, backToStartDistances));
                    distances.RemoveAll(d => d < 0);
                }
                else
                {
                    continue;
                }
            }
            return -1;
        }

        protected override object GetSolutionPart1()
        {
            /*
             * --- Day 24: Air Duct Spelunking ---

                You've finally met your match; the doors that provide access to the roof are locked tight, and all of the controls and related electronics are
                inaccessible. You simply can't reach them.

                The robot that cleans the air ducts, however, can.

                It's not a very fast little robot, but you reconfigure it to be able to interface with some of the exposed wires that have been routed through
                the HVAC system. If you can direct it to each of those locations, you should be able to bypass the security controls.

                You extract the duct layout for this area from some blueprints you acquired and create a map with the relevant locations marked (your puzzle input).
                0 is your current location, from which the cleaning robot embarks; the other numbers are (in no particular order) the locations the robot needs to
                visit at least once each. Walls are marked as #, and open passages are marked as .. Numbers behave like open passages.

                For example, suppose you have a map like the following:

                ###########
                #0.1.....2#
                #.#######.#
                #4.......3#
                ###########

                To reach all of the points of interest as quickly as possible, you would have the robot take the following path:

                    0 to 4 (2 steps)
                    4 to 1 (4 steps; it can't move diagonally)
                    1 to 2 (6 steps)
                    2 to 3 (2 steps)

                Since the robot isn't very fast, you need to find it the shortest route. This path is the fewest steps (in the above example, a total of 14)
                required to start at 0 and then visit every other location at least once.

                Given your actual map, and starting from location 0, what is the fewest number of steps required to visit every non-0 number marked on the map
                at least once?

             */

            #region Testrun

            List<string> testInput = new List<string>
            {
                "###########",
                "#0.1.....2#",
                "#.#######.#",
                "#4.......3#",
                "###########",
            };

            Maze testMaze = new Maze(testInput);
            SearchHelper searchHelper = new SearchHelper();

            Dictionary<Tuple<Coordinates, Coordinates>, int> testDistances = searchHelper.GetDistances(testMaze);

            int testResult = GetShortestTotalDistance(testDistances, testMaze.GetStartCoordinates());

            if (testResult != 14)
            {
                throw new Exception($"Test failed! Expected: 14, Actual: {testResult}");
            }
            #endregion

            Maze maze = new Maze(InputLines);
            Dictionary<Tuple<Coordinates, Coordinates>, int> distances = searchHelper.GetDistances(maze, true);
            int result = GetShortestTotalDistance(distances, maze.GetStartCoordinates());

            return result; //500
        }

        protected override object GetSolutionPart2()
        {
            /*
             * Of course, if you leave the cleaning robot somewhere weird, someone is bound to notice.
                What is the fewest number of steps required to start at 0, visit every non-0 number marked on the map at least once, and then return to 0?
             */
            SearchHelper searchHelper = new SearchHelper();

            Maze maze = new Maze(InputLines);
            Dictionary<Tuple<Coordinates, Coordinates>, int> distances = searchHelper.GetDistances(maze, true);
            int result = GetShortestTotalDistance(distances, maze.GetStartCoordinates(), backToStart:true);

            return result; //748
        }
    }
}
