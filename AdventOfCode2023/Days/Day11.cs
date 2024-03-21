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

        private static int Calculate(string input, int expansion)
        {
            List<List<char>> nodes = input.Split("\r\n").Select(x => x.ToCharArray().ToList()).ToList();
            nodes = ExpandUniverse(nodes, expansion - 1);

            (int x, int y)[] galaxies = FindGalaxies(nodes);
            ((int x, int y) a, (int x, int y) b)[] combinations = galaxies.Combinations();

            int sum = 0;
            foreach (var (a, b) in combinations)
            {
                sum += Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
            }

            return sum;
        }

        private static List<List<char>> ExpandUniverse(List<List<char>> nodes, int expansion)
        {
            int index = 0;
            while (index < nodes.Count)
            {
                if (!nodes.GetRow(index).Any(c => c != '.'))
                {
                    for (int e = 0; e < expansion; e++)
                    {
                        nodes.Insert(index, nodes.GetRow(index).ToList());
                        index++;
                    }
                }
                index++;
            }
            index = 0;
            while (index < nodes.GetRow(0).Count)
            {
                if (!nodes.GetColumn(index).Any(c => c != '.'))
                {
                    for (int e = 0; e < expansion; e++)
                    {
                        for (int i = 0; i < nodes.Count; i++)
                        {
                            nodes[i].Insert(index, '.');
                        }
                        index++;
                    }
                }
                index++;
            }
            return nodes;
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
