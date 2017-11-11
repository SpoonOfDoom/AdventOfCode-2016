using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode2016.Extensions;
using AdventOfCode2016.Tools;

namespace AdventOfCode2016.Days
{
    // ReSharper disable once UnusedMember.Global
    class Day22 : Day
    {
        public Day22() : base(22) {}
        private Regex nodeRegex = new Regex(@"node-x(\d+)-y(\d+) +(\d+)T +(\d+)T");

        internal class GridNode
        {
            public int X, Y;
            public int Size;

            public int Available => Size - Used;

            public int Used
            {
                get => used;
                set
                {
                    used = value;
                    CreateHash();
                }
            }

            public string NodeHash
            {
                get
                {
                    if (nodeHash == null)
                    {
                        CreateHash();
                    }
                    return nodeHash;
                }
            }

            private string nodeHash = null;

            public GridNode[,] Grid;
            private int used;
            
            public void CreateHash()
            {
                nodeHash = $"{X}-{Y}-{Size}-{Used}";
            }

            public GridNode()
            {
                
            }

            public GridNode(GridNode[,] grid)
            {
                Grid = grid;
            }

            public GridNode(GridNode[,] grid, int x, int y)
            {
                Grid = grid;
                X = x;
                Y = y;
            }

            public List<GridNode> GetAdjacentNodes()
            {
                var adjacentNodes = new List<GridNode>();
                if (X > 0)
                {
                    adjacentNodes.Add(Grid[X - 1, Y]);
                }
                if (Y > 0)
                {
                    adjacentNodes.Add(Grid[X, Y - 1]);
                }
                if (X < Grid.GetUpperBound(0))
                {
                    adjacentNodes.Add(Grid[X + 1, Y]);
                }
                if (Y < Grid.GetUpperBound(1))
                {
                    adjacentNodes.Add(Grid[X, Y + 1]);
                }

                return adjacentNodes;
            }

            public bool CanReceive(int dataAmount)
            {
                return dataAmount <= Available;
            }

            public void MoveTo(int x, int y)
            {
                Grid[x, y].Used += Used;
                Used = 0;
            }

            /// <summary>Gibt eine Zeichenfolge zurück, die das aktuelle Objekt darstellt.</summary>
            /// <returns>Eine Zeichenfolge, die das aktuelle Objekt darstellt.</returns>
            public override string ToString()
            {
                return NodeHash;
            }
            
            internal GridNode Clone(GridNode[,] newGrid)
            {
                return new GridNode
                {
                    Grid = newGrid,
                    Size = Size,
                    Used = Used,
                    X = X,
                    Y = Y,
                };
            }
        }

        private List<GridNode> ParseInputToNodeList(List<string> inputLines)
        {
            List<GridNode> nodesFromInput = new List<GridNode>(inputLines.Count);

            for (int i = 2; i < inputLines.Count; i++)
            {
                GroupCollection groups = nodeRegex.Match(inputLines[i]).Groups;

                nodesFromInput.Add(new GridNode()
                {
                    X = groups[1].Value.ToInt(),
                    Y = groups[2].Value.ToInt(),
                    Size = groups[3].Value.ToInt(),
                    Used = groups[4].Value.ToInt()
                });
            }

            return nodesFromInput;
        }

        private static GridNode[,] MakeGridFromNodeList(List<GridNode> nodesFromInput)
        {
            GridNode[,] grid = new GridNode[nodesFromInput.Max(e => e.X) + 1, nodesFromInput.Max(e => e.Y) + 1];
            foreach (GridNode node in nodesFromInput)
            {
                node.Grid = grid;
                grid[node.X, node.Y] = node;
            }
            return grid;
        }

        private class GameState : ISearchNode
        {
            public int Cost { get; set; }
            public List<object> Actions { get; set; }

            public string VerboseInfo
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    for (int y = 0; y < Grid.GetUpperBound(0) + 1; y++)
                    {
                        for (int x = 0; x < Grid.GetUpperBound(1) + 1; x++)
                        {
                            if (x == GoalDataX && y == GoalDataY)
                            {
                                sb.Append("G");
                            }
                            else if (Grid[y, x].Used == 0)
                            {
                                sb.Append("_");
                            }
                            else
                            {
                                sb.Append(".");
                            }
                        }
                        sb.Append("\n");
                    }
                    sb.Append(StringHash);
                    return sb.ToString();
                }
            }

            public string StringHash
            {
                get
                {
                    if (stringHash == null)
                    {
                        CreateHash();
                    }
                    return stringHash;
                }
            }

            public long NumericHash { get; }

            public int GoalDataX, GoalDataY, EmptyX, EmptyY;

            public GridNode[,] Grid;

            private string stringHash = null;

            public HashSet<ExpandAction> ExpandNode()
            {
                if (false)
                {
                    return ExpandNodeGeneral();
                }
                else
                {
                    return ExpandNodeBasedOnFilthyAssumptionsAboutInputData();
                }
                
            }

            private HashSet<ExpandAction> ExpandNodeBasedOnFilthyAssumptionsAboutInputData()
            {
                HashSet<ExpandAction> actions = new HashSet<ExpandAction>();

                List<GridNode> neighbours = Grid[EmptyX, EmptyY].GetAdjacentNodes();
                foreach (GridNode neighbour in neighbours)
                {
                    if (Grid[EmptyX, EmptyY].CanReceive(neighbour.Used))
                    {
                        GameState newState = Clone();
                        newState.Grid[neighbour.X, neighbour.Y].MoveTo(EmptyX, EmptyY);
                        newState.EmptyX = neighbour.X;
                        newState.EmptyY = neighbour.Y;
                        if (GoalDataX == neighbour.X && GoalDataY == neighbour.Y)
                        {
                            newState.GoalDataX = EmptyX;
                            newState.GoalDataY = EmptyY;
                        }

                        newState.CreateHash();
                        actions.Add(new ExpandAction
                        {
                            Action = "x" + neighbour.X + "+" + (neighbour.X - EmptyX)+ "y" + neighbour.Y + "+" + (neighbour.Y - EmptyY),
                            Cost = 1,
                            Result = newState
                        });
                    }
                }
                
                return actions;
            }

            private HashSet<ExpandAction> ExpandNodeGeneral()
            {
                HashSet<ExpandAction> actions = new HashSet<ExpandAction>();
                for (int x = 0; x <= Grid.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= Grid.GetUpperBound(1); y++)
                    {
                        List<GridNode> neighbours = Grid[x, y].GetAdjacentNodes().Where(n => n.CanReceive(Grid[x, y].Used)).ToList();
                        foreach (GridNode neighbour in neighbours)
                        {
                            var newState = Clone();
                            newState.Grid[x, y].MoveTo(neighbour.X, neighbour.Y);
                            if (x == GoalDataX && y == GoalDataY)
                            {
                                newState.GoalDataX = neighbour.X;
                                newState.GoalDataY = neighbour.Y;
                            }
                            int xDiff = x - neighbour.X;
                            int yDiff = y - neighbour.Y;
                            newState.EmptyX = x;
                            newState.EmptyY = y;
                            newState.CreateHash();
                            actions.Add(new ExpandAction
                            {
                                Action = "x" + x + "+" + xDiff + "y" + y + "+" + yDiff,
                                Cost = 1,
                                Result = newState
                            });
                        }
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
                return GoalDataX == 0 && GoalDataY == 0;
            }

            public float GetHeuristic(ISearchNode goalState = null)
            {
                if (GoalDataX == 0 && GoalDataY == 0)
                {
                    return 0;
                }
                float value = 1f;
                value += ((GoalDataX + GoalDataY) * 4) + Math.Abs(GoalDataX - EmptyX) + Math.Abs(GoalDataY - EmptyY);
                
                return value;
            }

            public void CreateHash()
            {
                StringBuilder sb = new StringBuilder("G");
                sb.Append(GoalDataX);
                sb.Append("-");
                sb.Append(GoalDataY);
                sb.Append("E");
                sb.Append(EmptyX);
                sb.Append("-");
                sb.Append(EmptyY);
                //sb.Append("|");

                //foreach (GridNode node in Grid)
                //{
                //    sb.Append("#" + node.ToString());
                //}
                stringHash = sb.ToString();
            }

            private GameState Clone()
            {
                GridNode[,] newGrid = new GridNode[Grid.GetUpperBound(0) +1, Grid.GetUpperBound(1) +1];
                for (int y = 0; y <= Grid.GetUpperBound(1); y++)
                {
                    for (int x = 0; x <= Grid.GetUpperBound(0); x++)
                    {
                        newGrid[x, y] = Grid[x, y].Clone(newGrid);
                    }
                }
                GameState newState = new GameState
                {
                    GoalDataX = GoalDataX,
                    GoalDataY = GoalDataY,
                    EmptyX = EmptyX,
                    EmptyY = EmptyY,
                    Grid = newGrid,
                    Cost = Cost,
                    Actions = new List<object>(Actions),
                };
                return newState;
            }
        }

        private void AStarSearchNodeProcessed(object sender, EventArgs e)
        {
            GameState gameState = (GameState)((SearchEventArgs)e).CurrentNode;
            gameState.Actions = null;
            for (int y = 0; y < gameState.Grid.GetUpperBound(1); y++)
            {
                for (int x = 0; x < gameState.Grid.GetUpperBound(0); x++)
                {
                    gameState.Grid[x, y] = null;
                }
            }
            gameState.Grid = null;
        }

        protected override object GetSolutionPart1()
        {
            /*
             * --- Day 22: Grid Computing ---

                You gain access to a massive storage cluster arranged in a grid; each storage node is only connected to the four nodes directly adjacent to it
                (three if the node is on an edge, two if it's in a corner).

                You can directly access data only on node /dev/grid/node-x0-y0, but you can perform some limited actions on the other nodes:

                    You can get the disk usage of all nodes (via df). The result of doing this is in your puzzle input.
                    You can instruct a node to move (not copy) all of its data to an adjacent node (if the destination node has enough space to receive the data).
                    The sending node is left empty after this operation.

                Nodes are named by their position: the node named node-x10-y10 is adjacent to nodes node-x9-y10, node-x11-y10, node-x10-y9, and node-x10-y11.

                Before you begin, you need to understand the arrangement of data on these nodes. Even though you can only move data between directly connected nodes,
                you're going to need to rearrange a lot of the data to get access to the data you need. Therefore, you need to work out how you might be able to shift data around.

                To do this, you'd like to count the number of viable pairs of nodes. A viable pair is any two nodes (A,B), regardless of whether they are directly connected, such that:

                    Node A is not empty (its Used is not zero).
                    Nodes A and B are not the same node.
                    The data on node A (its Used) would fit on node B (its Avail).

                How many viable pairs of nodes are there?
             */
            
            List<GridNode> nodes = ParseInputToNodeList(InputLines);

            List<GridNode[]> pairs = nodes.DifferentCombinations(2).Select(p => p.ToArray()).ToList();
            List<GridNode[]> viablePairs = pairs.Where(p => (p[1].Used > 0 && p[0].CanReceive(p[1].Used)) || (p[0].Used > 0 && p[1].CanReceive(p[0].Used))).ToList();

            int result = viablePairs.Count;
            return result; //937
        }

        protected override object GetSolutionPart2()
        {
            /*
             * Now that you have a better understanding of the grid, it's time to get to work.

                Your goal is to gain access to the data which begins in the node with y=0 and the highest x (that is, the node in the top-right corner).

                For example, suppose you have the following grid:

                Filesystem            Size  Used  Avail  Use%
                /dev/grid/node-x0-y0   10T    8T     2T   80%
                /dev/grid/node-x0-y1   11T    6T     5T   54%
                /dev/grid/node-x0-y2   32T   28T     4T   87%
                /dev/grid/node-x1-y0    9T    7T     2T   77%
                /dev/grid/node-x1-y1    8T    0T     8T    0%
                /dev/grid/node-x1-y2   11T    7T     4T   63%
                /dev/grid/node-x2-y0   10T    6T     4T   60%
                /dev/grid/node-x2-y1    9T    8T     1T   88%
                /dev/grid/node-x2-y2    9T    6T     3T   66%

                In this example, you have a storage grid 3 nodes wide and 3 nodes tall. The node you can access directly, node-x0-y0, is almost full.
                The node containing the data you want to access, node-x2-y0 (because it has y=0 and the highest x value), contains 6 terabytes of data - enough to fit on your node,
                if only you could make enough space to move it there.

                Fortunately, node-x1-y1 looks like it has enough free space to enable you to move some of this data around. In fact, it seems like all of the nodes have enough space
                to hold any node's data (except node-x0-y2, which is much larger, very full, and not moving any time soon). So, initially, the grid's capacities and connections look like this:

                ( 8T/10T) --  7T/ 9T -- [ 6T/10T]
                    |           |           |
                  6T/11T  --  0T/ 8T --   8T/ 9T
                    |           |           |
                 28T/32T  --  7T/11T --   6T/ 9T

                The node you can access directly is in parentheses; the data you want starts in the node marked by square brackets.

                In this example, most of the nodes are interchangable: they're full enough that no other node's data would fit, but small enough that their data could be moved around.
                Let's draw these nodes as .. The exceptions are the empty node, which we'll draw as _, and the very large, very full node, which we'll draw as #. Let's also draw the goal
                data as G. Then, it looks like this:

                (.) .  G
                 .  _  .
                 #  .  .

                The goal is to move the data in the top right, G, to the node in parentheses. To do this, we can issue some commands to the grid and rearrange the data:

                    Move data from node-y0-x1 to node-y1-x1, leaving node node-y0-x1 empty:
                    (.) _  G
                     .  .  .
                     #  .  .

                    Move the goal data from node-y0-x2 to node-y0-x1:
                    (.) G  _
                     .  .  .
                     #  .  .

                    At this point, we're quite close. However, we have no deletion command, so we have to move some more data around. So, next, we move the data from node-y1-x2 to node-y0-x2:
                    (.) G  .
                     .  .  _
                     #  .  .

                    Move the data from node-y1-x1 to node-y1-x2:
                    (.) G  .
                     .  _  .
                     #  .  .

                    Move the data from node-y1-x0 to node-y1-x1:
                    (.) G  .
                     _  .  .
                     #  .  .

                    Next, we can free up space on our node by moving the data from node-y0-x0 to node-y1-x0:
                    (_) G  .
                     .  .  .
                     #  .  .

                    Finally, we can access the goal data by moving the it from node-y0-x1 to node-y0-x0:
                    (G) _  .
                     .  .  .
                     #  .  .

                So, after 7 steps, we've accessed the data we want. Unfortunately, each of these moves takes time, and we need to be efficient:

                What is the fewest number of steps required to move your goal data to node-x0-y0?
             */
            #region Testrun

            List<string> testInput = new List<string>
            {
                "norf",
                "narf",
                "/dev/grid/node-x0-y0   10T    8T     2T   80%",
                "/dev/grid/node-x0-y1   11T    6T     5T   54%",
                "/dev/grid/node-x0-y2   32T   28T     4T   87%",
                "/dev/grid/node-x1-y0    9T    7T     2T   77%",
                "/dev/grid/node-x1-y1    8T    0T     8T    0%",
                "/dev/grid/node-x1-y2   11T    7T     4T   63%",
                "/dev/grid/node-x2-y0   10T    6T     4T   60%",
                "/dev/grid/node-x2-y1    9T    8T     1T   88%",
                "/dev/grid/node-x2-y2    9T    6T     3T   66%"
            };

            var testNodes = ParseInputToNodeList(testInput);
            var testGrid = MakeGridFromNodeList(testNodes);
            var testStartState = new GameState
            {
                GoalDataX = testGrid.GetUpperBound(0),
                GoalDataY = 0,
                Grid = testGrid,
                Actions = new List<object>(),
            };
            var testEmptyNode = testNodes.Single(n => n.Used == 0);
            testStartState.EmptyX = testEmptyNode.X;
            testStartState.EmptyY = testEmptyNode.Y;
            AStar aStar = new AStar(AStar.NodeHashMode.String);

            int testResult = aStar.GetMinimumCost(testStartState);
            if (testResult != 7)
            {
                throw new Exception($"Test failed! Expected: 7, actual: {testResult}");
            }

            #endregion


            /* I usually try to find general solutions, without exploiting specific things about the input data. But in this case, search was taking forever, so I modified things to consider that
             * we only ever have exactly one empty node, and that we basically move that around like in a sliding puzzle, which is apparently true for this problem.
             * In theory though, the input could just as well contain other ways and states with multiple empty nodes etc.
             * Because of this, I'll add todo: find a more general solution that doesn't take frickin' ages.
            */
            List<GridNode> nodes = ParseInputToNodeList(InputLines);
            GridNode[,] grid = MakeGridFromNodeList(nodes);
            int goalDataX = grid.GetUpperBound(0);
            int goalDataY = 0;
            var startState = new GameState
            {
                GoalDataX = goalDataX,
                GoalDataY = goalDataY,
                Grid = grid,
                Actions = new List<object>()
            };
            var emptyNode = nodes.Single(n => n.Used == 0);
            startState.EmptyX = emptyNode.X;
            startState.EmptyY = emptyNode.Y;
            aStar.SearchNodeProcessed += AStarSearchNodeProcessed; //this is leftover from when I saved a whole lot more data and had much more runtime, and ran out of memory. Probably not needed anymore.
            return aStar.GetMinimumCost(startState, verbose:true); //188
        }
    }
}
