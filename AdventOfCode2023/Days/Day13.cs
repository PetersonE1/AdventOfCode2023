using System;
using System.Collections;
using System.Text;

namespace AdventOfCode2023.Days
{
    internal static class Day13
    {
        public static void Run(string input)
        {
            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        // Perfect reflection => One side of the reflection must touch a border

        private static ulong Part1(string input)
        {
            string[][] grids = input.Split("\r\n\r\n").Select(s => s.Split("\r\n")).ToArray();

            ulong count = 0;
            
            foreach (string[] grid in grids)
            {
                for (int i = 1; i < grid[0].Length; i++)
                    if (grid.IsMirror(GetColumn, i - 1, true)) count += (uint)i;
                
                for (int i = 1; i < grid.Length; i++)
                    if (grid.IsMirror(GetRow, i - 1)) count += 100 * (uint)i;
            }

            return count;
        }

        private static ulong Part2(string input)
        {
            throw new NotImplementedException();
        }

        private static string GetColumn(this string[] grid, int index) => grid.Aggregate(string.Empty, (result, current) => result += current[index]);

        private static string GetRow(this string[] grid, int index) => grid[index];

        private delegate string Selector(string[] grid, int index);
        private static bool IsMirror(this string[] grid, Selector selector, int index, bool isCol = false, int gap = 0)
        {
            if (index - gap < 0 || index + gap + 1 >= (isCol ? grid[0].Length : grid.Length)) return true;
            if (selector(grid, index - gap) == selector(grid, index + 1 + gap))
                return grid.IsMirror(selector, index, isCol, gap + 1);
            return false;
        }
    }
}