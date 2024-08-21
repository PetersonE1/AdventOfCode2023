using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Days
{
    internal static class Day12
    {
        public static void Run(string input)
        {
            Console.WriteLine(Part1(input));
        }

        private static int Part1(string input)
        {
            // Remove confirmed sequences of damaged springs from the input list
            // Attempt to logically confirm unknowns next to confirmed damages
            // Attempt to confirm list numbers to unknown regions
            // Iterate over regions, starting leftmost and moving right, nesting for multiple list numbers
            // Generate count of possibilities per region, multiply all together to get possibilities per line

            input = "???.### 1,1,3";
            (string sequence, int[] lengths)[] lines = input.Split("\r\n").Select(n => {
                var arr = n.Split(' ');
                return (arr[0], arr[1].Split(',').Select(r => int.Parse(r)).ToArray());
            }).ToArray();

            IdentifyConfirmedDamagedSequences(ref lines[0]);

            return 0;
        }

        private static void IdentifyConfirmedDamagedSequences(ref (string sequence, int[] lengths) input)
        {
            StringBuilder builder = new();

            List<int> lengths = input.lengths.ToList();

            for (int index = 0; index < lengths.Count; index++)
            {
                int length = lengths[index];
                builder.Clear();
                for (int i = 0; i < length; i++){
                    builder.Append('#');
                }
                string checker = builder.ToString();

                if (Contains(input.sequence, checker))
                {
                    input.sequence = input.sequence.Replace(checker, string.Empty);
                    lengths.RemoveAt(index);
                    index--;
                }
            }
        }

        private static bool Contains(string source, string substring)
        {
            if (source.Contains('.' + substring + '.'))
                return true;
            if (string.Concat(source.Take(substring.Length + 1)) == substring + '.')
                return true;
            if (string.Concat(source.TakeLast(substring.Length + 1)) == '.' + substring)
                return true;
            return false;
        }
    }
}
