using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Days
{
    internal static class Day8
    {
        static Regex wordRgx = new Regex(@"\w+");

        public static void Run(string input)
        {
            Console.WriteLine(Part1(input));
        }

        static ulong Part1(string input)
        {
            string[] lines = input.Split("\r\n");
            string instructions = lines[0];

            Dictionary<string, (string, string)> nodes = new Dictionary<string, (string, string)>();
            for (int i = 2; i < lines.Length; i++)
            {
                MatchCollection matches = wordRgx.Matches(lines[i]);
                nodes.Add(matches[0].Value, (matches[1].Value, matches[2].Value));
            }

            ulong steps = 0;
            string currentNode = "AAA";
            while (currentNode != "ZZZ")
            {
                foreach (char c in instructions)
                {
                    if (c == 'R')
                        currentNode = nodes[currentNode].Item2;
                    else if (c == 'L')
                        currentNode = nodes[currentNode].Item1;
                    steps++;
                    if (currentNode == "ZZZ") break;
                }
            }

            return steps;
        }
    }
}
