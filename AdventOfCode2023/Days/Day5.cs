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
        public static void Run(string input)
        {
            Console.WriteLine(Part1(input));
        }

        static ulong Part1(string input)
        {
            string[] sections = input.Split("\r\n\r\n");
            ulong[] seeds = sections[0].Split(' ').Skip(1).Select(x => ulong.Parse(x)).ToArray();

            List<ulong> locations = new List<ulong>();
            foreach (ulong seed in seeds)
                locations.Add(GetLocation(seed, sections));

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

        static ulong GetLocation(ulong seed, string[] sections)
        {
            foreach (string section in sections.Skip(1))
            {
                (ulong, ulong, ulong)[] maps = section.Split("\r\n").Skip(1).Select(m => m.Split(' ').Select(x => ulong.Parse(x)).ToArray()).Select(x => (x[0], x[1], x[2])).ToArray();
                seed = Lookup(seed, maps);
            }
            return seed;
        }
    }
}
