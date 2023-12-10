using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Days
{
    internal static class Day9
    {
        public static void Run(string input)
        {
            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        static long Part1(string input)
        {
            string[] lines = input.Split("\r\n");
            long sum = 0;
            foreach (string line in lines)
                sum += GetNext(line.Split(' ').Select(x => int.Parse(x)).ToArray());

            return sum;
        }

        static long Part2(string input)
        {
            string[] lines = input.Split("\r\n");
            long sum = 0;
            foreach (string line in lines)
                sum += GetPrevious(line.Split(' ').Select(x => int.Parse(x)).ToArray());

            return sum;
        }

        static int GetNext(int[] inputs)
        {
            int[] diffs = new int[inputs.Length - 1];
            for (int i = 0; i < inputs.Length - 1; i++)
                diffs[i] = inputs[i + 1] - inputs[i];
            if (diffs.All(x => x == 0))
                return inputs[^1];
            return inputs[^1] + GetNext(diffs);
        }

        static int GetPrevious(int[] inputs)
        {
            int[] diffs = new int[inputs.Length - 1];
            for (int i = 0; i < inputs.Length - 1; i++)
                diffs[i] = inputs[i + 1] - inputs[i];
            if (diffs.All(x => x == 0))
                return inputs[0];
            return inputs[0] - GetPrevious(diffs);
        }
    }
}
