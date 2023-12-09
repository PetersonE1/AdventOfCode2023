using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Days
{
    internal static class Day5
    {
        static List<(ulong, ulong, ulong)[]> maps = new();
        public static void Run(string input)
        {
            string[] sections = input.Split("\r\n\r\n");
            Console.WriteLine(Part1(sections[0]));
            Console.WriteLine(Part2(sections[0]));

            foreach (string section in sections.Skip(1))
            {
                maps.Add(section.Split("\r\n").Skip(1).Select(m => m.Split(' ').Select(x => ulong.Parse(x)).ToArray()).Select(x => (x[0], x[1], x[2])).ToArray());
            }
        }

        static ulong Part1(string section)
        {
            ulong[] seeds = section.Split(' ').Skip(1).Select(x => ulong.Parse(x)).ToArray();

            List<ulong> locations = new List<ulong>();
            foreach (ulong seed in seeds)
                locations.Add(GetLocation(seed));

            return locations.Min();
        }

        static ulong Part2(string section)
        {
            (ulong, ulong)[] seedPairs = section.Split(' ').Skip(1).Select(x => ulong.Parse(x)).Chunk(2).Select(x => (x[0], x[1])).ToArray();

            List<ulong> locations = new List<ulong>();
            foreach (var pair in seedPairs)
                for (ulong i = 0; i < pair.Item2; i++)
                    locations.Add(GetLocation(pair.Item1 + i));

            return locations.Min();
        }

        static ulong Lookup(ulong item, (ulong, ulong, ulong)[] lookup)
        {
            foreach (var check in lookup)
            {
                ulong diff = item - check.Item2;
                if (diff >= 0 && diff <= check.Item3)
                    return check.Item1 + diff;
            }
            return item;
        }

        static ulong GetLocation(ulong seed)
        {
            foreach (var map in maps)
            {
                seed = Lookup(seed, map);
            }
            return seed;
        }
    }
}
