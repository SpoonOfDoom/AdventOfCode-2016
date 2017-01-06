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

        HashSet<ExpandAction> ExpandNode();
        bool Equals(ISearchNode goalState);
        int GetHeuristic(ISearchNode goalState);
        int GetTentativeCost(ISearchNode goalState);
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

        public int GetMinimumCost(ISearchNode startState, ISearchNode goalState)
        {
            Tuple<List<object>, int> path = GetOptimalPath(startState, goalState);
            return path.Item2;
        }

        public Tuple<List<object>, int> GetOptimalPath(ISearchNode startState, ISearchNode goalState, bool verbose = false)
        {
            Stopwatch searchWatch = new Stopwatch();
            searchWatch.Start();
            List<ExpandAction> actions = new List<ExpandAction>();
            openQueue = new SimplePriorityQueue<ISearchNode>();
            closedSet = new HashSet<ISearchNode>();

            openQueue.Enqueue(startState, 0);
            while (openQueue.Count > 0)
            {
                ISearchNode current = openQueue.Dequeue();

                if (current.Equals(goalState))
                {

                    return Tuple.Create(current.Actions, current.Cost);
                }

                closedSet.Add(current);


                HashSet<ExpandAction> expandActions = current.ExpandNode();

                if (verbose)
                {
                    Console.Clear();
                    Console.WriteLine("Open list: {0}", openQueue.Count);
                    Console.WriteLine("Closed list: {0}", closedSet.Count);
                    if (openQueue.Count > 0) Console.WriteLine("First tentative cost: {0}", openQueue.First.GetTentativeCost(goalState));

                    Console.WriteLine("Time: {0}:{1}:{2}.{3}", searchWatch.Elapsed.Hours, searchWatch.Elapsed.Minutes, searchWatch.Elapsed.Seconds, searchWatch.Elapsed.Milliseconds);

                }

                foreach (ExpandAction expandAction in expandActions)
                {
                    ISearchNode newNode = expandAction.result;
                    newNode.Cost = current.Cost + expandAction.cost;
                    if (closedSet.Any(x => x.Equals(newNode)))
                    {
                        continue;
                    }

                    if (openQueue.Any(x => x.Equals(newNode) && x.Cost > newNode.Cost))
                    {
                        openQueue.UpdatePriority(newNode, newNode.GetTentativeCost(goalState));
                    }
                    else
                    {
                        openQueue.Enqueue(newNode, newNode.GetTentativeCost(goalState));
                    }
                }
            }

            return null;
        }
    }
}