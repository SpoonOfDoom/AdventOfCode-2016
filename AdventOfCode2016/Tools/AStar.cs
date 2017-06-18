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

    public abstract class SearchNode
    {
        protected abstract void CreateNumericHash();
        protected abstract void CreateStringHash();
        public abstract HashSet<ExpandActionNew> ExpandNode();

        private bool hashCreated = false;
        protected string stringHash;
        protected long numericHash;
        public int Cost { get; set; }
        public List<object> Actions { get; set; } //todo: change implementation to avoid having a full list in every node ("predecessor" variable, and then walk along the predecessors of every node)
        public virtual string VerboseInfo => "";

        public string StringHash
        {
            get
            {
                if (!hashCreated)
                {
                    CreateHash();
                }
                return stringHash;
            }
        }

        public long NumericHash
        {
            get
            {
                if (!hashCreated)
                {
                    CreateHash();
                }
                return numericHash;
            }
        }

        /// <summary>Bestimmt, ob das angegebene Objekt mit dem aktuellen Objekt identisch ist.</summary>
        /// <returns>true, wenn das angegebene Objekt und das aktuelle Objekt gleich sind, andernfalls false.</returns>
        /// <param name="obj">Das Objekt, das mit dem aktuellen Objekt verglichen werden soll. </param>
        public override bool Equals(object obj)
        {
            if (!(obj is SearchNode))
            {
                return false;
            }
            switch (AStar.HashingMode)
            {
                case AStar.NodeHashMode.String:
                    return StringHash == ((SearchNode)obj).StringHash;
                case AStar.NodeHashMode.Numeric:
                    return NumericHash == ((SearchNode)obj).NumericHash;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public virtual bool IsGoalState(SearchNode goalState = null)
        {
            return this.Equals(goalState);
        }

        public virtual float GetHeuristic(SearchNode goalState = null)
        {
            return 0;
        }

        public void CreateHash()
        {
            hashCreated = true;
            switch (AStar.HashingMode)
            {
                case AStar.NodeHashMode.String:
                    CreateStringHash();
                    return;
                case AStar.NodeHashMode.Numeric:
                    CreateNumericHash();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public struct ExpandAction
    {
        public ISearchNode Result;
        public object Action; //todo: make "Action" interface/abstract class so we can grab the cost and other data from them
        public int Cost;
    }

    public struct ExpandActionNew
    {
        public SearchNode Result;
        public object Action; //todo: make "Action" interface/abstract class so we can grab the cost and other data from them
        public int Cost;
    }

    public class AStar
    {
        public enum NodeHashMode
        {
            String,
            Numeric
        }

        public static NodeHashMode HashingMode;

        private SimplePriorityQueue<ISearchNode> openQueue;
        private HashSet<ISearchNode> closedSet;

        private SimplePriorityQueue<SearchNode> openQueueNew;
        private HashSet<SearchNode> closedSetNew;

        public AStar()
        {
            HashingMode = NodeHashMode.String;
        }

        public AStar(NodeHashMode hashingMode)
        {
            HashingMode = hashingMode;
        }

        public int GetMinimumCost(ISearchNode startState, ISearchNode goalState = null, bool verbose = false)
        {
            Tuple<List<object>, int> path = GetOptimalPath(startState, goalState, verbose);
            return path.Item2;
        }

        public int GetMinimumCost(SearchNode startState, SearchNode goalState = null, bool verbose = false)
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

        public Tuple<List<object>, int> GetOptimalPath(SearchNode startState, SearchNode goalState = null, bool verbose = false)
        {
            if (verbose)
            {
                Console.Clear();
            }
            Stopwatch searchWatch = new Stopwatch();
            searchWatch.Start();
            openQueueNew = new SimplePriorityQueue<SearchNode>();
            closedSetNew = new HashSet<SearchNode>();

            openQueueNew.Enqueue(startState, 0);
            long step = 0;
            SearchNode current;
            HashSet<ExpandActionNew> expandActions;
            SearchNode newNode;
            SearchNode match;
            while (openQueueNew.Count > 0)
            {
                step++;
                current = openQueueNew.Dequeue();

                if (current.IsGoalState(current))
                {
                    return Tuple.Create(current.Actions, current.Cost);
                }
                closedSetNew.Add(current);

                expandActions = current.ExpandNode();

                if (verbose)
                {
                    OutputVerboseInfo(goalState, searchWatch, step, current);
                }


                foreach (ExpandActionNew expandAction in expandActions)
                {
                    newNode = expandAction.Result;
                    newNode.Cost = current.Cost + expandAction.Cost;
                    newNode.Actions.Add(expandAction.Action);
                    if (closedSetNew.Contains(newNode))
                    {
                        continue;
                    }
                    match = openQueueNew.SingleOrDefault(x => x.Equals(newNode));
                    if (match != null)
                    {
                        if (match.Cost > newNode.Cost)
                        {
                            openQueueNew.UpdatePriority(match, newNode.Cost + newNode.GetHeuristic(goalState));
                        }
                    }
                    else
                    {
                        openQueueNew.Enqueue(newNode, newNode.Cost + newNode.GetHeuristic(goalState));
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

        private void OutputVerboseInfo(SearchNode goalState, Stopwatch searchWatch, long step, SearchNode current)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Open list: {0}   ", openQueueNew.Count);
            Console.WriteLine("Closed list: {0}   ", closedSetNew.Count);
            if (openQueueNew.Count > 0)
            {
                Console.WriteLine("First cost until now: {0}   ", openQueueNew.First.Cost);
                Console.WriteLine("First tentative cost: {0}   ", openQueueNew.First.Cost + openQueueNew.First.GetHeuristic(goalState));
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