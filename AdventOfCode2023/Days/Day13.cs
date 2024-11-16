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
        }

        // Error: Checking for any reflection at all, need perfect reflection. Return to original list idea.
        // What counts as perfect reflection? In the first "proper" shape, the top three (3) rows are ignored. Longest reflection?
        private static ulong Part1(string input)
        {
            string[][] grids = input.Split("\r\n\r\n").Select(s => s.Split("\r\n")).ToArray();

            ulong count = 0;

            foreach (string[] grid in grids)
            {
                string cachedString = grid[0];
                bool doBreak = false;
                for (uint i = 1; i < grid.Length; i++)
                {
                    if (grid[i] == cachedString)
                    {
                        count += i;
                        doBreak = true;
                        break;
                    }
                    cachedString = grid[i];
                }

                if (doBreak) break;
                cachedString = grid.GetColumn(0);
                for (int i = 1; i < grid.Length; i++)
                {
                    string col = grid.GetColumn(i);
                    if (col == cachedString)
                    {
                        count += (uint)i * 100;
                        break;
                    }
                    cachedString = col;
                }
            }

            return count;
        }

        private static string GetColumn(this string[] grid, int index)
        {
            return grid.Aggregate(string.Empty, (result, current) => result + current[index]);
        }
    }
}