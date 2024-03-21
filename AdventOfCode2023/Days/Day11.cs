using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Days
{
    internal static class Day11
    {
        public static void Run(string input)
        {
            Console.WriteLine(Calculate(input, 2));
            Console.WriteLine(Calculate(input, 1000000));
        }

        private static long Calculate(string input, int expansion)
        {
            List<List<char>> nodes = input.Split("\r\n").Select(x => x.ToCharArray().ToList()).ToList();

            (int x, int y)[] galaxies = FindGalaxies(nodes);
            ((int x, int y) a, (int x, int y) b)[] combinations = galaxies.Combinations();
            
            return ExpandUniverse(nodes, combinations, expansion - 1);
        }

        private static long ExpandUniverse(List<List<char>> nodes, ((int x, int y) a, (int x, int y) b)[] combinations, int expansion)
        {
            long sum = 0;
            foreach (var (a, b) in combinations)
            {
                sum += Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
                int start_x = Math.Min(a.x, b.x);
                int end_x = Math.Max(a.x, b.x);
                for (int x = start_x; x <= end_x; x++)
                {
                    if (!nodes.GetRow(x).Contains('#'))
                    {
                        sum += expansion;
                    }
                }
                int start_y = Math.Min(a.y, b.y);
                int end_y = Math.Max(a.y, b.y);
                for (int y = start_y; y <= end_y; y++)
                {
                    if (!nodes.GetColumn(y).Contains('#'))
                    {
                        sum += expansion;
                    }
                }
            }
            return sum;
        }

        private static (int x, int y)[] FindGalaxies(List<List<char>> nodes)
        {
            List<(int x, int y)> galaxies = new List<(int x, int y)>();
            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes[i].Count; j++)
                {
                    if (nodes[i][j] == '#')
                    {
                        galaxies.Add((i, j));
                    }
                }
            }
            return galaxies.ToArray();
        }

        private static (T, T)[] Combinations<T>(this T[] array)
        {
            List<(T, T)> combinations = new List<(T, T)>();
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    combinations.Add((array[i], array[j]));
                }
            }
            return combinations.ToArray();
        }

        private static void Print(List<List<char>> nodes)
        {
            foreach (List<char> row in nodes)
            {
                Console.WriteLine(string.Join("", row));
            }
        }

        private static List<T> GetColumn<T>(this List<List<T>> array, int column)
        {
            return array.Select(x => x[column]).ToList();
        }

        private static List<T> GetRow<T>(this List<List<T>> array, int row)
        {
            return array[row];
        }
    }
}
