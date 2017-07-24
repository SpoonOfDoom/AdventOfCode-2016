using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2016.Extensions;

namespace AdventOfCode2016.Days
{
    // ReSharper disable once UnusedMember.Global
    class Day15 : Day
	{
	    class Disc
	    {
	        public readonly int startPos;
	        public int positions;

	        public Disc(int startPos, int positions)
	        {
	            this.startPos = startPos;
	            this.positions = positions;
	        }
	    }

		public Day15() : base(15) {}

	    private Regex regex = new Regex(@"#\d has (\d+).+ (\d+)\.$");

	    private List<Disc> ParseDiscs()
	    {
	        List<Disc> discs = new List<Disc>();

            foreach (string line in InputLines)
	        {
	            Match match = regex.Match(line);

                Disc d = new Disc(match.Groups[2].Value.ToInt(), match.Groups[1].Value.ToInt());
                discs.Add(d);
	        }
	        return discs;
	    }

	    private bool FallsThrough(List<Disc> discs, int timepoint)
	    {
	        for (int i = 0; i < discs.Count; i++)
	        {
	            if ((timepoint+(i+1) + discs[i].startPos)%discs[i].positions != 0)
	            {
	                return false;
	            }
	        }

	        return true;
	    }

	    private int FindFirstPossibleTime(int positions, int startPos, int discNumber)
	    {
	        for (int i = 0; i < positions; i++)
	        {
	            if ((startPos + discNumber + i)%positions == 0)
	            {
	                return i;
	            }
	        }
	        return -1;
	    }
        

        private int FindTimePoint(List<Disc> discs)
	    {
	        int maxPositions = -1;
	        int startPos = -1;
	        int discNumber = -1;
	        for (int i = 0; i < discs.Count; i++)
	        {
	            Disc disc = discs[i];
	            if (disc.positions > maxPositions)
	            {
	                maxPositions = disc.positions;
	                startPos = disc.startPos;
	                discNumber = i+1;
	            }
	        }

	        for (int t = FindFirstPossibleTime(maxPositions, startPos, discNumber); t < int.MaxValue - maxPositions; t += maxPositions)
	        {
	            if (FallsThrough(discs, t))
	            {
	                return t;
	            }
	        }

	        return -1;
	    }

	    protected override object GetSolutionPart1()
	    {
            /*
             * The halls open into an interior plaza containing a large kinetic sculpture. The sculpture is in a sealed enclosure and seems to involve a set of identical spherical capsules that are carried to the top and allowed to bounce through the maze of spinning pieces.

                Part of the sculpture is even interactive! When a button is pressed, a capsule is dropped and tries to fall through slots in a set of rotating discs to finally go through a little hole at the bottom and come out of the sculpture. If any of the slots aren't aligned with the capsule as it passes, the capsule bounces off the disc and soars away. You feel compelled to get one of those capsules.

                The discs pause their motion each second and come in different sizes; they seem to each have a fixed number of positions at which they stop. You decide to call the position with the slot 0, and count up for each position it reaches next.

                Furthermore, the discs are spaced out so that after you push the button, one second elapses before the first disc is reached, and one second elapses as the capsule passes from one disc to the one below it. So, if you push the button at time=100, then the capsule reaches the top disc at time=101, the second disc at time=102, the third disc at time=103, and so on.

                The button will only drop a capsule at an integer time - no fractional seconds allowed.

                For example, at time=0, suppose you see the following arrangement:

                Disc #1 has 5 positions; at time=0, it is at position 4.
                Disc #2 has 2 positions; at time=0, it is at position 1.

                If you press the button exactly at time=0, the capsule would start to fall; it would reach the first disc at time=1. Since the first disc was at position 4 at time=0, by time=1 it has ticked one position forward. As a five-position disc, the next position is 0, and the capsule falls through the slot.

                Then, at time=2, the capsule reaches the second disc. The second disc has ticked forward two positions at this point: it started at position 1, then continued to position 0, and finally ended up at position 1 again. Because there's only a slot at position 0, the capsule bounces away.

                If, however, you wait until time=5 to push the button, then when the capsule reaches each disc, the first disc will have ticked forward 5+1 = 6 times (to position 0), and the second disc will have ticked forward 5+2 = 7 times (also to position 0). In this case, the capsule would fall through the discs and come out of the machine.

                However, your situation has more than two discs; you've noted their positions in your puzzle input. What is the first time you can press the button to get a capsule?
             */

            List<Disc> testDiscs = new List<Disc>
            {
                new Disc(4, 5),
                new Disc(1, 2)
            };
	        int testResult = FindTimePoint(testDiscs);
	        if (testResult != 5)
	        {
	            throw new Exception("Test failed! Expected: 5, actual: " + testResult);
	        }
	        
            List<Disc> discs = ParseDiscs();
	        var result = FindTimePoint(discs);
	        if (result == -1)
	        {
	            throw new Exception("No solution found!");
	        }
	        return result; //203660
        }

	    protected override object GetSolutionPart2()
	    {
            /*
             * After getting the first capsule (it contained a star! what great fortune!), the machine detects your success and begins to rearrange itself.

                When it's done, the discs are back in their original configuration as if it were time=0 again, but a new disc with 11 positions and starting at position 0 has appeared exactly one second below the previously-bottom disc.

                With this new disc, and counting again starting from time=0 with the configuration in your puzzle input, what is the first time you can press the button to get another capsule?

                Although it hasn't changed, you can still get your puzzle input.
             */

	        List<Disc> discs = ParseDiscs();
            discs.Add(new Disc(0, 11));
	        int result = FindTimePoint(discs);
            return result; //2408135
        }
	}
}
