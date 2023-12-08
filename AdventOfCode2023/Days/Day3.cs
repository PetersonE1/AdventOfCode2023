using System;
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
        static Regex gearRgx = new Regex(@"([*])");
        static Regex intRgx = new Regex(@"([0-9])");

        public static void Run(string input)
        {
            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        static int Part1(string input)
        {
            string grid = input.Replace("\r\n", string.Empty);
            int width = input.IndexOf("\r\n");
            int sum = 0;
            
            foreach (Match match in symbolRgx.Matches(grid))
                sum += GetSymbolSum(grid, width, match.Index);

            return sum;
        }

        static int Part2(string input)
        {
            string grid = input.Replace("\r\n", string.Empty);
            int width = input.IndexOf("\r\n");
            int sum = 0;

            foreach (Match match in gearRgx.Matches(grid))
                sum += GetGearSum(grid, width, match.Index);

            return sum;
        }

        static int GetGearSum(string grid, int width, int index)
        {
            List<int> nums = new List<int>();
            for (int i = index - width; i <= index + width; i += width)
                for (int j = -1; j <= 1; j++)
                    if (intRgx.IsMatch(grid[i + j].ToString()))
                        nums.Add(GetNum(grid, width, i + j));

            nums = nums.Distinct().ToList();
            if (nums.Count == 2)
                return nums[0] * nums[1];
            return 0;
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
            int activeIndex = index;
            while (activeIndex-- % width != 0)
            {
                if (!intRgx.IsMatch(grid[activeIndex].ToString()))
                    break;
                chars = chars.Prepend(grid[activeIndex]).ToList();
            }
            activeIndex = index;
            while (++activeIndex % width != 0)
            {
                if (!intRgx.IsMatch(grid[activeIndex].ToString()))
                    break;
                chars = chars.Append(grid[activeIndex]).ToList();
            }

            return int.Parse(string.Join("", chars));
        }
    }
}
