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
            Console.WriteLine(Part2(input));
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

        static ulong Part2(string input)
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
            List<(string, int)> currentNodes = nodes.Keys.Where(key => key.EndsWith("A")).Select(x => (x, 0)).ToList();
            for (int i = 0; i < currentNodes.Count; i++) currentNodes[i] = (currentNodes[i].Item1, i);
            ulong[] stepsToComplete = new ulong[currentNodes.Count];
            while (currentNodes.Count > 0)
            {
                foreach (char c in instructions)
                {
                    if (c == 'R')
                        currentNodes = currentNodes.Select(node => (nodes[node.Item1].Item2, node.Item2)).ToList();
                    else if (c == 'L')
                        currentNodes = currentNodes.Select(node => (nodes[node.Item1].Item1, node.Item2)).ToList();
                    steps++;
                    
                    for (int i = 0; i < currentNodes.Count; i++)
                    {
                        if (currentNodes[i].Item1.EndsWith("Z"))
                        {
                            stepsToComplete[currentNodes[i].Item2] = steps;
                            currentNodes.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }

            return LeastCommonMultiple(stepsToComplete.ToList());
        }

        static ulong LeastCommonMultiple(List<ulong> inputs)
        {
            if (inputs.Count == 1)
                return inputs[0];
            if (inputs.Count > 2)
            {
                List<ulong> tempInputs = inputs.Take(inputs.Count - 1).ToList();
                inputs.RemoveRange(0, inputs.Count - 1);
                inputs.Add(LeastCommonMultiple(tempInputs));
            }

            ulong multiple = inputs[0] * inputs[1];
            ulong gcf = GreatestCommonFactor(inputs[0], inputs[1]);
            return multiple / gcf;
        }

        static ulong GreatestCommonFactor(ulong a, ulong b)
        {
            while (a != b)
            {
                if (a > b)
                    a -= b;
                else
                    b -= a;
            }
            if (a < 0)
                return 0;
            return a;
        }
    }
}
