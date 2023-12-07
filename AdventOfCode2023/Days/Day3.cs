using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Days
{
    internal static class Day3
    {
        public static void Run(string input)
        {
            Console.WriteLine(Part1(input));
        }

        static int Part1(string input)
        {
            string grid = input.Replace("\r\n", string.Empty);
            int width = input.IndexOf("\r\n");
            Console.WriteLine(grid);
            Console.WriteLine(width);
            return 0;
        }
    }
}
