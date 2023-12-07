﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Days
{
    internal static class Day3
    {
        static Regex symbolRgx = new Regex(@"([^a-zA-Z0-9.])");
        static Regex intRgx = new Regex(@"([0-9])");

        public static void Run(string input)
        {
            Console.WriteLine(Part1(input));
        }

        static int Part1(string input)
        {
            string grid = input.Replace("\r\n", string.Empty);
            int width = input.IndexOf("\r\n");
            int sum = 0;
            
            foreach (Match match in symbolRgx.Matches(grid))
            {
                Console.WriteLine($"Detected {match.Value} at index {match.Index}");
                int x = GetSymbolSum(grid, width, match.Index);
                sum += x;
                Console.WriteLine($"Sum of symbols: {x}");
            }

            return sum;
        }

        static int GetSymbolSum(string grid, int width, int index)
        {
            List<int> nums = new List<int>();
            for (int i = index - width; i <= index + width; i += width)
                for (int j = -1; j <= 1; j++)
                    if (intRgx.IsMatch(grid[i + j].ToString()))
                        nums.Add(GetNum(grid, width, i + j));
            int sum = 0;
            foreach (int num in nums.Distinct())
                sum += num;
            return sum;
        }

        static int GetNum(string grid, int width, int index)
        {
            if (index < 0 || index >= grid.Length)
                return 0;

            List<char> chars = [grid[index]];
            Console.WriteLine($"Beginning number parsing with {grid[index]}");
            int activeIndex = index;
            Console.WriteLine($"Moving left, Modulus: [{activeIndex % (width - 1)}]");
            while (activeIndex-- % width != 0)
            {
                Console.WriteLine($"Char: [{grid[activeIndex]}], Modulus: [{activeIndex % (width - 1)}]");
                if (!intRgx.IsMatch(grid[activeIndex].ToString()))
                    break;
                chars = chars.Prepend(grid[activeIndex]).ToList();
            }
            activeIndex = index;
            Console.WriteLine($"Moving right, Modulus: [{activeIndex % width}]");
            while (++activeIndex % width != 0)
            {
                Console.WriteLine($"Char: [{grid[activeIndex]}], Modulus: [{activeIndex % width}]");
                if (!intRgx.IsMatch(grid[activeIndex].ToString()))
                    break;
                chars = chars.Append(grid[activeIndex]).ToList();
            }

            Console.WriteLine($"Parsed number: {string.Join("", chars)}");
            return int.Parse(string.Join("", chars));
        }
    }
}
