using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Priority_Queue;

namespace AdventOfCode2016.Tools
{
    public interface ISearchNode
    {
        int Cost { get; set; }
        List<object> Actions { get; set; }
        string VerboseInfo { get; }

        HashSet<ExpandAction> ExpandNode();
        bool Equals(ISearchNode otherState); //For checking if node is already in openQueue or closedSet
        bool IsGoalState(ISearchNode goalState = null); //Goal state ist not necessarily equal in every way
        float GetHeuristic(ISearchNode goalState = null);
    }

    public struct ExpandAction
    {
        public ISearchNode result;
        public object action;
        public int cost;
    }

    public class AStar
    {
        SimplePriorityQueue<ISearchNode> openQueue;
        HashSet<ISearchNode> closedSet;

        Dictionary<ISearchNode, int> nodeCost = new Dictionary<ISearchNode, int>();

        public int GetMinimumCost(ISearchNode startState, ISearchNode goalState = null, bool verbose = false)
        {
            Tuple<List<object>, int> path = GetOptimalPath(startState, goalState, verbose);
            return path.Item2;
        }

        public Tuple<List<object>, int> GetOptimalPath(ISearchNode startState, ISearchNode goalState = null, bool verbose = false)
        {
            if (verbose)
            {
                Console.Clear();
            }
            Stopwatch searchWatch = new Stopwatch();
            searchWatch.Start();
            openQueue = new SimplePriorityQueue<ISearchNode>();
            closedSet = new HashSet<ISearchNode>();

            openQueue.Enqueue(startState, 0);
            long step = 0;
            while (openQueue.Count > 0)
            {
                step++;
                ISearchNode current = openQueue.Dequeue();

                if (current.IsGoalState(current))
                {
                    return Tuple.Create(current.Actions, current.Cost);
                }
                closedSet.Add(current);


                HashSet<ExpandAction> expandActions = current.ExpandNode();

                if (verbose)
                {
                    OutputVerboseInfo(goalState, searchWatch, step, current);
                }

                foreach (ExpandAction expandAction in expandActions)
                {
                    ISearchNode newNode = expandAction.result;
                    newNode.Cost = current.Cost + expandAction.cost;
                    newNode.Actions.Add(expandAction.action);
                    if (closedSet.Any(x => x.Equals(newNode)))
                    {
                        continue;
                    }

                    if (openQueue.Any(x => x.Equals(newNode)))
                    {
                        if (openQueue.Single(x => x.Equals(newNode)).Cost > newNode.Cost)
                        {
                            openQueue.UpdatePriority(openQueue.Single(x => x.Equals(newNode)), newNode.Cost + newNode.GetHeuristic(goalState));
                        }
                    }
                    else
                    {
                        openQueue.Enqueue(newNode, newNode.Cost + newNode.GetHeuristic(goalState));
                    }
                }
            }

            return null;
        }

        private void OutputVerboseInfo(ISearchNode goalState, Stopwatch searchWatch, long step, ISearchNode current)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Open list: {0}   ", openQueue.Count);
            Console.WriteLine("Closed list: {0}   ", closedSet.Count);
            if (openQueue.Count > 0)
            {
                Console.WriteLine("First cost until now: {0}   ", openQueue.First.Cost);
                Console.WriteLine("First tentative cost: {0}   ", openQueue.First.Cost + openQueue.First.GetHeuristic(goalState));
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine();
            }
            Console.WriteLine($"Step: {step}   ");
            Console.WriteLine("Time: {0}:{1}:{2}.{3}   ", searchWatch.Elapsed.Hours, searchWatch.Elapsed.Minutes, searchWatch.Elapsed.Seconds, searchWatch.Elapsed.Milliseconds);

            Console.WriteLine(current.VerboseInfo);
        }
    }
}