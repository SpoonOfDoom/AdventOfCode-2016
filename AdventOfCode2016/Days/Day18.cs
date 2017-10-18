using System;
using System.Linq;
using System.Text;

namespace AdventOfCode2016.Days
{
    // ReSharper disable once UnusedMember.Global
    class Day18 : Day
    {
        public Day18() : base(18) {}
        
        private bool[,] InitializeTiles(string input, int rows)
        {
            bool[,] tiles = new bool[input.Length, rows];

            for (int index = 0; index < input.Length; index++)
            {
                tiles[index, 0] = input[index] == '^';
            }

            return tiles;
        }

        private bool IsTileTrap(bool left, bool center, bool right)
        {
            /*
             * Its left and center tiles are traps, but its right tile is not.
                    Its center and right tiles are traps, but its left tile is not.
                    Only its left tile is a trap.
                    Only its right tile is a trap.
             */
            return (left && center && !right)
                   || (center && right && !left)
                   || (left && !(center || right))
                   || (right && !(left || center));
        }

        private void FillTileGrid(bool[,] grid)
        {
            bool left, center, right;
            int width = grid.GetUpperBound(0) + 1;
            int height = grid.GetUpperBound(1) + 1;
                //..^^.
                //.^^^^
                //^^..^
            for (int y = 1; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    left = x != 0 && grid[x - 1, y - 1];
                    center = grid[x, y - 1];
                    right = x != width - 1 && grid[x + 1, y - 1];

                    grid[x, y] = IsTileTrap(left, center, right);
                }
            }
        }

        private string TileGridToString(bool[,] grid)
        {
            int width = grid.GetUpperBound(0) + 1;
            int height = grid.GetUpperBound(1) + 1;

            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < height; y++)
            {
                if (y > 0)
                {
                    sb.Append("\n");
                }
                for (int x = 0; x < width; x++)
                {
                    sb.Append(grid[x, y] ? '^' : '.');
                }
            }
            return sb.ToString();
        }

        protected override object GetSolutionPart1()
        {
            #region Description
            /*
             * --- Day 18: Like a Rogue ---

                As you enter this room, you hear a loud click! Some of the tiles in the floor here seem to be pressure plates for traps, and the trap you just triggered has run out of...
                whatever it tried to do to you. You doubt you'll be so lucky next time.

                Upon closer examination, the traps and safe tiles in this room seem to follow a pattern. The tiles are arranged into rows that are all the same width;
                you take note of the safe tiles (.) and traps (^) in the first row (your puzzle input).

                The type of tile (trapped or safe) in each row is based on the types of the tiles in the same position, and to either side of that position, in the previous row.
                (If either side is off either end of the row, it counts as "safe" because there isn't a trap embedded in the wall.)

                For example, suppose you know the first row (with tiles marked by letters) and want to determine the next row (with tiles marked by numbers):

                ABCDE
                12345

                The type of tile 2 is based on the types of tiles A, B, and C; the type of tile 5 is based on tiles D, E, and an imaginary "safe" tile. Let's call these
                three tiles from the previous row the left, center, and right tiles, respectively. Then, a new tile is a trap only in one of the following situations:

                    Its left and center tiles are traps, but its right tile is not.
                    Its center and right tiles are traps, but its left tile is not.
                    Only its left tile is a trap.
                    Only its right tile is a trap.

                In any other situation, the new tile is safe.

                Then, starting with the row ..^^., you can determine the next row by applying those rules to each new tile:

                    The leftmost character on the next row considers the left (nonexistent, so we assume "safe"), center (the first ., which means "safe"),
                    and right (the second ., also "safe") tiles on the previous row. Because all of the trap rules require a trap in at least one of the previous three tiles,
                    the first tile on this new row is also safe, ..
                    The second character on the next row considers its left (.), center (.), and right (^) tiles from the previous row. This matches the fourth rule:
                    only the right tile is a trap. Therefore, the next tile in this new row is a trap, ^.
                    The third character considers .^^, which matches the second trap rule: its center and right tiles are traps, but its left tile is not.
                    Therefore, this tile is also a trap, ^.
                    The last two characters in this new row match the first and third rules, respectively, and so they are both also traps, ^.

                After these steps, we now know the next row of tiles in the room: .^^^^. Then, we continue on to the next row, using the same rules, and get ^^..^.
                After determining two new rows, our map looks like this:

                ..^^.
                .^^^^
                ^^..^

                Here's a larger example with ten tiles per row and ten rows:

                .^^.^.^^^^
                ^^^...^..^
                ^.^^.^.^^.
                ..^^...^^^
                .^^^^.^^.^
                ^^..^.^^..
                ^^^^..^^^.
                ^..^^^^.^^
                .^^^..^.^^
                ^^.^^^..^^

                In ten rows, this larger example has 38 safe tiles.

                Starting with the map in your puzzle input, in a total of 40 rows (including the starting row), how many safe tiles are there?

             */
            #endregion

            #region TestRuns

            string expectedGrid = "..^^.\n.^^^^\n^^..^";
            var grid = InitializeTiles("..^^.", 3);
            FillTileGrid(grid);
            string gridResult = TileGridToString(grid);
            if (gridResult != expectedGrid)
            {
                Console.WriteLine(gridResult);
                throw new Exception($"Test 1 failed. Expected: {expectedGrid}, actual: {gridResult}");
            }

            expectedGrid = ".^^.^.^^^^\n^^^...^..^\n^.^^.^.^^.\n..^^...^^^\n.^^^^.^^.^\n^^..^.^^..\n^^^^..^^^.\n^..^^^^.^^\n.^^^..^.^^\n^^.^^^..^^";
            grid = InitializeTiles(".^^.^.^^^^", 10);
            FillTileGrid(grid);
            gridResult = TileGridToString(grid);
            if (gridResult != expectedGrid)
            {
                Console.WriteLine(gridResult);
                throw new Exception($"Test 2 failed. Expected: {expectedGrid}, actual: {gridResult}");
            }
            int safeTiles = grid.Cast<bool>().Count(tile => !tile);

            if (safeTiles != 38)
            {
                throw new Exception($"Test 2 failed! Wrong number of safe tiles. Expected: 38, actual: {safeTiles}");
            }

            #endregion
            grid = InitializeTiles(Input, 40);
            FillTileGrid(grid);
            gridResult = TileGridToString(grid);
            Console.WriteLine(gridResult);
            safeTiles = grid.Cast<bool>().Count(tile => !tile);

            return safeTiles; //1974
        }

        protected override object GetSolutionPart2()
        {
            /*
             * How many safe tiles are there in a total of 400000 rows?
             */

            bool[,] grid = InitializeTiles(Input, 400000);
            FillTileGrid(grid);
            string gridResult = TileGridToString(grid);
            Console.WriteLine(gridResult);
            int safeTiles = grid.Cast<bool>().Count(tile => !tile);

            return safeTiles; //19991126
        }
    }
}
