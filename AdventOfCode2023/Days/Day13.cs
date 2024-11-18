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

        // Perfect reflection => One side of the reflection must touch a border

        // Reflections getting dropped in current implementation: An imperfect reflection gets caught, discarded,
        // and a subsequent perfect reflection is skipped

        private static ulong Part1(string input)
        {
            string[][] grids = input.Split("\r\n\r\n").Select(s => s.Split("\r\n")).ToArray();

            ulong count = 0;

            int index = 0;
            foreach (string[] grid in grids)
            {
                index++;
                var rowMirror = GetMirror(grid, GetRow);
                var colMirror = GetMirror(grid, GetColumn, true);

                count += 100 * (uint)rowMirror.index + (uint)colMirror.index;

                if (rowMirror.index - rowMirror.length > 0 && rowMirror.index + rowMirror.length < grid.GetColumn(0).Length)
                    throw new Exception($"ERROR ERROR IN ROW: [index={rowMirror.index},length={rowMirror.length},tot={grid.GetColumn(0).Length}]");
                if (colMirror.index - colMirror.length > 0 && colMirror.index + colMirror.length < grid[0].Length)
                    throw new Exception($"ERROR ERROR IN COL: [index={colMirror.index},length={colMirror.length},tot={grid[0].Length}]");

                Console.WriteLine("passed");

                // DEBUG DISPLAY
                if (index % 20 == 0)
                {
                    foreach (string s in grid) Console.WriteLine(s);
                    Console.WriteLine($"Horizontal Mirror: {(rowMirror.length > 0 ? $"Found at index {rowMirror.index}, length {rowMirror.length}" : "Not found.")}");
                    Console.WriteLine($"Vertical Mirror: {(colMirror.length > 0 ? $"Found at index {colMirror.index}, length {colMirror.length}" : "Not found.")}");
                    Console.WriteLine();
                    if (rowMirror.length == 0)
                    {
                        Console.WriteLine($"ZERO CHECK: {rowMirror.index}");
                    }
                }
            }

            return count;
        }

        /*private static ulong Part1B(string input)
        {
            BitArray[][] grids = input.Split("\r\n\r\n").Select(s => s.Split("\r\n").Select(s => {
                BitArray b = new(s.Length);
                for (int i = 0; i < s.Length; i++)
                    b.Set(i, s[i] == '#' ? true : false);
                return b;
            }).ToArray()).ToArray();

            ulong count = 0;
            foreach (BitArray[] grid in grids)
            {
                
            }

            return count;
        }*/

        private static string GetColumn(this string[] grid, int index) => grid.Aggregate(string.Empty, (result, current) => result + current[index]);

        private static string GetRow(this string[] grid, int index) => grid[index];

        private delegate string Selector(string[] grid, int index);
        private static (uint length, int index) GetMirror(string[] grid, Selector selector, bool isCol = false)
        {
            (uint length, int index) mirror = new();

            List<string> lines = [selector(grid, 0)];
            bool isReversing = false;
            int length = isCol ? grid[0].Length : grid.Length;
            for (int i = 1; i < length; i++)
            {
                if (lines.Count == 0) break;
                if (!isReversing)
                {
                    if (selector(grid, i) == lines[^1])
                    {
                        mirror.index = i;
                        mirror.length = 1;
                        lines.RemoveAt(lines.Count - 1);
                        isReversing = true;
                        continue;
                    }
                    lines.Add(selector(grid, i));
                }
                else
                {
                    if (selector(grid, i) != lines[^1])
                    {
                        mirror.index = 0;
                        mirror.length = 0;
                        break;
                    }
                    mirror.length++;
                    lines.RemoveAt(lines.Count - 1);
                }
            }

            return mirror;
        }

        private static bool HasOneSet(this BitArray arr)
        {
            int count = 0;
            foreach (bool b in arr)
                if (b) count++;

            return count == 1 ? true : false;
        }
    }
}