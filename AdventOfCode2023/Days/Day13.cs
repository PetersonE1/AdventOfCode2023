using System;
using System.Collections;
using System.Text;

namespace AdventOfCode2023.Days
{
    internal static class Day13
    {
        public static void Run(string input)
        {
            Console.WriteLine(Part1(input, out var cache));
            Console.WriteLine(Part2(input, cache));
        }

        // Perfect reflection => One side of the reflection must touch a border

        private static ulong Part1(string input, out (int index, bool isCol)[] cache)
        {
            string[][] grids = input.Split("\r\n\r\n").Select(s => s.Split("\r\n")).ToArray();

            ulong count = 0;
            
            cache = new(int index, bool isCol)[grids.Length];
            int index = 0;
            foreach (string[] grid in grids)
            {
                for (int i = 1; i < grid[0].Length; i++)
                    if (grid.IsMirror(GetColumn, i - 1, true)) { count += (uint)i; cache[index] = (i, true); }
                
                for (int i = 1; i < grid.Length; i++)
                    if (grid.IsMirror(GetRow, i - 1)) { count += 100 * (uint)i; cache[index] = (i, false); }
                index++;
            }

            return count;
        }

        private static ulong Part2(string input, (int index, bool isCol)[] cache)
        {
            string[][] grids = input.Split("\r\n\r\n").Select(s => s.Split("\r\n")).ToArray();
            ulong count = 0;

            int index = 0;
            foreach (string[] grid in grids)
            {
                for (int i = 1; i < grid[0].Length; i++)
                    if (cache[index] != (i, true) && grid.IsMirrorWithSmudge(GetColumn, i - 1, true)) {count += (uint)i; }
                
                for (int i = 1; i < grid.Length; i++)
                    if (cache[index] != (i, false) && grid.IsMirrorWithSmudge(GetRow, i - 1)) {count += 100 * (uint)i; }

                index++;
            }

            return count;
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

        private static bool IsMirrorWithSmudge(this string[] grid, Selector selector, int index, bool isCol = false, int gap = 0, bool smudgeFixed = false)
        {
            if (index - gap < 0 || index + gap + 1 >= (isCol ? grid[0].Length : grid.Length)) return true;
            string s1 = selector(grid, index - gap);
            string s2 = selector(grid, index + 1 + gap);
            if (s1 == s2)
                return grid.IsMirrorWithSmudge(selector, index, isCol, gap + 1, smudgeFixed);
            if (!smudgeFixed && IsOneDifferent(s1, s2))
            {
                return grid.IsMirrorWithSmudge(selector, index, isCol, gap + 1, true);
            }
            return false;
        }

        private static bool IsOneDifferent(string a, string b)
        {
            if (a.Length != b.Length) return false;

            int count = 0;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i]) count++;

            return count == 1;
        }
    }
}