using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Extensions;
using AdventOfCode2016.Tools;

namespace AdventOfCode2016.Days
{
    // ReSharper disable once UnusedMember.Global
    class Day11 : Day
    {
        public Day11() : base(11) {}

        private class GameState : ISearchNode
        {
            private struct Action
            {
                public int startFloor;
                public int endFloor;
                public List<string> items;
            }

            public int Cost { get; set; }
            public List<object> Actions { get; set; }
            public string VerboseInfo => $"Floor 1:{floors[1].Count}  \nFloor 2:{floors[2].Count}  \nFloor 3:{floors[3].Count}  \nFloor 4:{floors[4].Count}  \n";
            private const int MaxItemsInElevator = 2;

            public int elevatorPosition = 1;

            public Dictionary<int, List<string>> floors = new Dictionary<int, List<string>>();

            private void Move(int startFloor, int targetFloor, params string[] things)
            {
                if (things == null || things.Length == 0 || things.Length > MaxItemsInElevator)
                {
                    throw new Exception("Invalid item count for elevator movement - this shouldn't have happened.");
                }

                if (Math.Abs(targetFloor - startFloor) > 1)
                {
                    throw new Exception("Elevator can't move more than one floor at once!");
                }
                elevatorPosition = targetFloor;
                floors[startFloor].RemoveAll(things.Contains);
                floors[targetFloor].AddRange(things);
            }

            public GameState Clone()
            {
                var actions = new List<object>();
                var nFloors = new Dictionary<int, List<string>>();

                foreach (object action in Actions)
                {
                    var a = (Action)action;
                    var items = new List<string>(a.items);

                    actions.Add(new Action {startFloor = a.startFloor, endFloor = a.endFloor, items = items});
                }

                foreach (KeyValuePair<int, List<string>> keyValuePair in floors)
                {
                    nFloors[keyValuePair.Key] = new List<string>(keyValuePair.Value);
                }
                
                var clonedState = new GameState
                {
                    Cost = Cost,
                    elevatorPosition = elevatorPosition,
                    Actions = actions,
                    floors = nFloors

                };
                return clonedState;
            }

            public bool Fried()
            {
                for (int i = 1; i <= 4; i++)
                {
                    if (FloorChipsFried(floors[i]))
                    {
                        return true;
                    }
                }
                return false;
            }

            private bool FloorChipsFried(List<string> items)
            {
                if (items.Count <= 1)
                {
                    return false;
                }
                List<string> microChips = new List<string>();
                List<string> generators = new List<string>();

                foreach (string item in items)
                {
                    string type = item.Substring(item.Length - 1);
                    string element = item.Substring(0, item.Length - 1);

                    if (type == "G")
                    {
                        generators.Add(element);
                    }
                    else
                    {
                        microChips.Add(element);
                    }
                }
                return generators.Count != 0 && microChips.Any(m => !generators.Contains(m));
            }

            public HashSet<ExpandAction> ExpandNode()
            {
                int startFloor = elevatorPosition;
                int[] targetFloors = {elevatorPosition - 1, elevatorPosition + 1};
                HashSet<ExpandAction> actions = new HashSet<ExpandAction>();
                foreach (int targetFloor in targetFloors)
                {
                    if (targetFloor < 1 || targetFloor > 4)
                    {
                        continue;
                    }
                    
                    List<List<string>> itemCombos = floors[startFloor].DifferentCombinations(2).Select(c => c.ToList()).ToList();
                    itemCombos.AddRange(floors[startFloor].Select(s => new List<string> {s}));

                    foreach (List<string> itemCombo in itemCombos)
                    {
                        GameState newState = Clone();

                        newState.Move(startFloor, targetFloor, itemCombo.ToArray());
                        if (newState.Fried())
                        {
                            continue;
                        }
                        //if (newState.floors[newState.elevatorPosition].Count == 0)
                        //{
                        //    throw new Exception("This shouldn't happen, might be worth to investigate what lead here.");
                        //}
                        newState.floors[startFloor].Sort();
                        newState.floors[targetFloor].Sort();
                        actions.Add(new ExpandAction {Cost = 1, Action = new Action {startFloor = startFloor, endFloor = targetFloor, items = new List<string>(itemCombo)}, Result = newState});
                    }
                }
                return actions;
            }

            public bool Equals(ISearchNode otherState)
            {
                GameState other = otherState as GameState;

                if (other.elevatorPosition != elevatorPosition)
                {
                    return false;
                }
                for (int i = 1; i <= 4; i++)
                {
                    if (!other.floors[i].SequenceEqual(floors[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            public bool IsGoalState(ISearchNode goalState = null)
            {
                for (int i = 1; i <= 3; i++)
                {
                    if (floors[i].Count > 0)
                    {
                        return false;
                    }
                }
                return true;
            }

            public float GetHeuristic(ISearchNode goalState = null)
            {
                if (IsGoalState())
                {
                    return 0;
                }
                double maybeCost = 0;
                for (int i = 1; i <= 4; i++)
                {
                    maybeCost += (Math.Ceiling(floors[i].Count / 2f) * (6 - i));
                    int chipCount = floors[i].Count(item => item.EndsWith("M"));
                    int generatorCount = floors[i].Count(item => item.EndsWith("G"));
                    if (chipCount >= 3 && generatorCount < 3)
                    {
                        maybeCost += ((chipCount-2)/2f + i) / 10f;
                    }
                    if (i < 4)
                    {
                        int matching = floors[i].Select(item => item.Substring(0, item.Length - 1))
                            .Intersect(floors[i + 1]
                            .Select(item2 => item2.Substring(0, item2.Length - 1))).Count();
                        maybeCost -= (matching / 10f);
                    }
                }
                return (float)maybeCost;
            }
        }

        protected override object GetSolutionPart1()
        {
            #region description
            /*
                You come upon a column of four floors that have been entirely sealed off from the rest of the building except for a small dedicated lobby.
                There are some radiation warnings and a big sign which reads "Radioisotope Testing Facility".

                According to the project status board, this facility is currently being used to experiment with Radioisotope Thermoelectric Generators (RTGs, or simply "generators")
                that are designed to be paired with specially-constructed microchips. Basically, an RTG is a highly radioactive rock that generates electricity through heat.

                The experimental RTGs have poor radiation containment, so they're dangerously radioactive. The chips are prototypes and don't have normal radiation shielding,
                but they do have the ability to generate an electromagnetic radiation shield when powered. Unfortunately, they can only be powered by their corresponding RTG.
                An RTG powering a microchip is still dangerous to other microchips.

                In other words, if a chip is ever left in the same area as another RTG, and it's not connected to its own RTG, the chip will be fried. Therefore, it is assumed
                that you will follow procedure and keep chips connected to their corresponding RTG when they're in the same room, and away from other RTGs otherwise.

                These microchips sound very interesting and useful to your current activities, and you'd like to try to retrieve them. The fourth floor of the facility has
                an assembling machine which can make a self-contained, shielded computer for you to take with you - that is, if you can bring it all of the RTGs and microchips.

                Within the radiation-shielded part of the facility (in which it's safe to have these pre-assembly RTGs), there is an elevator that can move between the four floors.
                Its capacity rating means it can carry at most yourself and two RTGs or microchips in any combination. (They're rigged to some heavy diagnostic equipment -
                the assembling machine will detach it for you.) As a security measure, the elevator will only function if it contains at least one RTG or microchip.
                The elevator always stops on each floor to recharge, and this takes long enough that the items within it and the items on that floor can irradiate each other.
                (You can prevent this if a Microchip and its Generator end up on the same floor in this way, as they can be connected while the elevator is recharging.)

                You make some notes of the locations of each component of interest (your puzzle input). Before you don a hazmat suit and start moving things around, you'd like
                to have an idea of what you need to do.

                When you enter the containment area, you and the elevator will start on the first floor.

                For example, suppose the isolated area has the following arrangement:

                The first floor contains a hydrogen-compatible microchip and a lithium-compatible microchip.
                The second floor contains a hydrogen generator.
                The third floor contains a lithium generator.
                The fourth floor contains nothing relevant.

                As a diagram (F# for a Floor number, E for Elevator, H for Hydrogen, L for Lithium, M for Microchip, and G for Generator), the initial state looks like this:

                F4 .  .  .  .  .  
                F3 .  .  .  LG .  
                F2 .  HG .  .  .  
                F1 E  .  HM .  LM 

                Then, to get everything up to the assembling machine on the fourth floor, the following steps could be taken:

                    Bring the Hydrogen-compatible Microchip to the second floor, which is safe because it can get power from the Hydrogen Generator:

                    F4 .  .  .  .  .  
                    F3 .  .  .  LG .  
                    F2 E  HG HM .  .  
                    F1 .  .  .  .  LM 

                    Bring both Hydrogen-related items to the third floor, which is safe because the Hydrogen-compatible microchip is getting power from its generator:

                    F4 .  .  .  .  .  
                    F3 E  HG HM LG .  
                    F2 .  .  .  .  .  
                    F1 .  .  .  .  LM 

                    Leave the Hydrogen Generator on floor three, but bring the Hydrogen-compatible Microchip back down with you so you can still use the elevator:

                    F4 .  .  .  .  .  
                    F3 .  HG .  LG .  
                    F2 E  .  HM .  .  
                    F1 .  .  .  .  LM 

                    At the first floor, grab the Lithium-compatible Microchip, which is safe because Microchips don't affect each other:

                    F4 .  .  .  .  .  
                    F3 .  HG .  LG .  
                    F2 .  .  .  .  .  
                    F1 E  .  HM .  LM 

                    Bring both Microchips up one floor, where there is nothing to fry them:

                    F4 .  .  .  .  .  
                    F3 .  HG .  LG .  
                    F2 E  .  HM .  LM 
                    F1 .  .  .  .  .  

                    Bring both Microchips up again to floor three, where they can be temporarily connected to their corresponding generators while the elevator recharges,
                    preventing either of them from being fried:

                    F4 .  .  .  .  .  
                    F3 E  HG HM LG LM 
                    F2 .  .  .  .  .  
                    F1 .  .  .  .  .  

                    Bring both Microchips to the fourth floor:

                    F4 E  .  HM .  LM 
                    F3 .  HG .  LG .  
                    F2 .  .  .  .  .  
                    F1 .  .  .  .  .  

                    Leave the Lithium-compatible microchip on the fourth floor, but bring the Hydrogen-compatible one so you can still use the elevator; this is safe because
                    although the Lithium Generator is on the destination floor, you can connect Hydrogen-compatible microchip to the Hydrogen Generator there:

                    F4 .  .  .  .  LM 
                    F3 E  HG HM LG .  
                    F2 .  .  .  .  .  
                    F1 .  .  .  .  .  

                    Bring both Generators up to the fourth floor, which is safe because you can connect the Lithium-compatible Microchip to the Lithium Generator upon arrival:

                    F4 E  HG .  LG LM 
                    F3 .  .  HM .  .  
                    F2 .  .  .  .  .  
                    F1 .  .  .  .  .  

                    Bring the Lithium Microchip with you to the third floor so you can use the elevator:

                    F4 .  HG .  LG .  
                    F3 E  .  HM .  LM 
                    F2 .  .  .  .  .  
                    F1 .  .  .  .  .  

                    Bring both Microchips to the fourth floor:

                    F4 E  HG HM LG LM 
                    F3 .  .  .  .  .  
                    F2 .  .  .  .  .  
                    F1 .  .  .  .  .  

                In this arrangement, it takes 11 steps to collect all of the objects at the fourth floor for assembly. (Each elevator stop counts as one step, even if nothing
                is added to or removed from it.)

                In your situation, what is the minimum number of steps required to bring all of the objects to the fourth floor?
                */
#endregion

            var aStar = new AStar();
            
            var testState = new GameState
            {
                Actions = new List<object>(),
                Cost = 0,
                floors = new Dictionary<int, List<string>> {
                    { 1, new List<string> {"HM", "LM"} },
                    { 2, new List<string> {"HG"} },
                    { 3, new List<string> {"LG"} },
                    { 4, new List<string>()},
                },
                elevatorPosition = 1
            };

            int testCost = aStar.GetMinimumCost(testState);

            if (testCost != 11)
            {
               throw new Exception("Test failed! Expected: 11, Actual: " + testCost); 
            }

            /*
             * Input and manuel creation of list, because I'm not gonna parse that:
             * The first floor contains a promethium generator and a promethium-compatible microchip.
                The second floor contains a cobalt generator, a curium generator, a ruthenium generator, and a plutonium generator.
                The third floor contains a cobalt-compatible microchip, a curium-compatible microchip, a ruthenium-compatible microchip, and a plutonium-compatible microchip.
                The fourth floor contains nothing relevant.
             */
            var startState = new GameState
            {
                Actions = new List<object>(),
                Cost = 0,
                floors = new Dictionary<int, List<string>> {
                    { 1, new List<string> {"PrG", "PrM"} },
                    { 2, new List<string> {"CoG", "CuG", "RuG", "PlG"} },
                    { 3, new List<string> {"CoM", "CuM", "RuM", "PlM"} },
                    { 4, new List<string>()},
                },
                elevatorPosition = 1
            };
            int minCost = aStar.GetMinimumCost(startState, verbose:true);
            return minCost; //33, runtime 1:42:21
        }
    }
}
