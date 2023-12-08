using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Days
{
    internal static class Day4
    {
        static Regex numberRgx = new Regex(@"\d+");

        public static void Run(string input)
        {
            Console.WriteLine(Part1(input));
        }

        static int Part1(string input)
        {
            string[][] lines = input.Split("\r\n").Select(x => x.Split('|')).ToArray();
            int totalSum = 0;

            foreach (string[] line in lines)
            {
                line[0] = line[0].Substring(line[0].IndexOf(':'));
                int count = 0;
                Match[] matches = numberRgx.Matches(line[0]).ToArray();
                foreach (Match match in numberRgx.Matches(line[1]))
                {
                    if (matches.Where(m => m.Value == match.Value).Count() >= 1)
                        count++;
                }
                if (count == 0)
                    continue;

                int sum = 1;
                for (int i = 1; i < count; i++)
                {
                    sum *= 2;
                }
                totalSum += sum;
            }

            return totalSum;
        }
    }
}
