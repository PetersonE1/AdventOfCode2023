using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Days
{
    internal static class Day6
    {
        static Regex numRgx = new Regex(@"\d+");

        public static void Run(string input)
        {
            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        static int Part1(string input)
        {
            string[] lines = input.Split("\r\n");
            int[] times = numRgx.Matches(lines[0]).Select(x => int.Parse(x.Value)).ToArray();
            int[] distances = numRgx.Matches(lines[1]).Select(x => int.Parse(x.Value)).ToArray();

            int total = 1;

            for (int i = 0; i < times.Length; i++)
            {
                int distance = distances[i];
                int time = times[i];

                int min = 0;
                for (int j = 0; j < time; j++)
                {
                    if (j * (time - j) > distance)
                    {
                        min = j;
                        break;
                    }
                }
                total *= time - 2 * min + 1;
            }
            return total;
        }

        static ulong Part2(string input)
        {
            string[] lines = input.Split("\r\n");
            ulong time = ulong.Parse(string.Join("", numRgx.Matches(lines[0]).Select(s => s.Value).ToArray()));
            ulong distance = ulong.Parse(string.Join("", numRgx.Matches(lines[1]).Select(s => s.Value).ToArray()));

            ulong min = 0;
            for (ulong j = 0; j < time; j++)
            {
                if (j * (time - j) > distance)
                {
                    min = j;
                    break;
                }
            }

            return time - 2 * min + 1;
        }
    }
}
