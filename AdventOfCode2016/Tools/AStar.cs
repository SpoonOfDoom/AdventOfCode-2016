using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Priority_Queue;

namespace AdventOfCode2016.Tools
{
    public interface ISearchNode //todo: convert to abstract class and provide default implementations of methods
    {
        //todo: introduce bool numericHashMode to control whether to use string or numeric hash in default Equals implementation
        int Cost { get; set; }
        List<object> Actions { get; set; } //todo: change implementation to avoid having a full list in every node ("predecessor" variable, and then walk along the predecessors of every node)
        string VerboseInfo { get; }
        string StringHash { get; }
        long NumericHash { get; }

        HashSet<ExpandAction> ExpandNode();
        bool Equals(ISearchNode otherState); //For checking if node is already in openQueue or closedSet
        bool IsGoalState(ISearchNode goalState = null); //Goal state ist not necessarily equal in every way
        float GetHeuristic(ISearchNode goalState = null);

        void CreateHash();
    }

    public struct ExpandAction
    {
        public ISearchNode Result;
        public object Action; //todo: make "Action" interface/abstract class so we can grab the cost and other data from them
        public int Cost;
    }

    public class AStar
    {
        private SimplePriorityQueue<ISearchNode> openQueue;
        private HashSet<ISearchNode> closedSet;

        public int GetMinimumCost(ISearchNode startState, ISearchNode goalState = null, bool verbose = false)
        {
            Tuple<List<object>, int> path = GetOptimalPath(startState, goalState, verbose); 
            return path.Item2;
        }

        // ReSharper disable once MemberCanBePrivate.Global
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
            ISearchNode current;
            HashSet<ExpandAction> expandActions;
            ISearchNode newNode;
            ISearchNode match;
            while (openQueue.Count > 0)
            {
                step++;
                current = openQueue.Dequeue();

                if (current.IsGoalState(current))
                {
                    return Tuple.Create(current.Actions, current.Cost);
                }
                closedSet.Add(current);


                expandActions = current.ExpandNode();

                if (verbose)
                {
                    OutputVerboseInfo(goalState, searchWatch, step, current);
                }


                foreach (ExpandAction expandAction in expandActions)
                {
                    newNode = expandAction.Result;
                    newNode.Cost = current.Cost + expandAction.Cost;
                    newNode.Actions.Add(expandAction.Action);
                    if (closedSet.Any(x => x.Equals(newNode)))
                    {
                        continue;
                    }
                    match = openQueue.SingleOrDefault(x => x.Equals(newNode));
                    if (match != default(ISearchNode))
                    {
                        if (match.Cost > newNode.Cost)
                        {
                            openQueue.UpdatePriority(match, newNode.Cost + newNode.GetHeuristic(goalState));
                        }
                    }
                    else
                    {
                        openQueue.Enqueue(newNode, newNode.Cost + newNode.GetHeuristic(goalState));
                    }
                }
            }

            return Tuple.Create(new List<object>(), -1);
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