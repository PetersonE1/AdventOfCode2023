using System;
using System.Collections;
using System.Text;

namespace AdventOfCode2023.Days
{
    internal static class Day14
    {
        public static void Run(string input)
        {
            Console.WriteLine(Part1(input));
        }

        // Each iteration runs top-to-bottom, generating a checksum as it goes.
        // Loop iteration until checksum matches the previous one.

        private static int Part1(string input)
        {
            (uint[] maps, uint[] unmoveableMaps) = ToBits(input, out int length);

            uint[] cached_maps = new uint[maps.Length];
            do
            {
                maps.CopyTo(cached_maps, 0);
                IterationStep(maps, unmoveableMaps, length);
            }
            while (!maps.SequenceEqual(cached_maps));

            return CalculateNorthStrain(maps, unmoveableMaps, length);
        }

        private static (uint[] maps, uint[] unmoveableMaps) ToBits(string input, out int length)
        {
            string[] lines = input.Split("\r\n");
            length = lines[0].Length;
            uint[] maps = new uint[lines.Length];
            uint[] unmoveableMaps = new uint[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                uint map = 0;
                uint unmoveableMap = 0;

                foreach (char c in line)
                {
                    map <<= 1;
                    unmoveableMap <<= 1;

                    if (c != '.')
                        map += 1;
                    if (c == '#')
                        unmoveableMap += 1;
                }

                maps[i] = map;
                unmoveableMaps[i] = unmoveableMap;
            }

            return (maps, unmoveableMaps);
        }

        private static void IterationStep(uint[] maps, uint[] unmoveableMaps, int length)
        {
            for (int i = 0; i < maps.Length - 1; i++)
            {
                (uint a, uint b) = Roll(maps[i], maps[i+1], unmoveableMaps[i+1], length);
                maps[i] = a;
                maps[i+1] = b;
            }
        }

        // Row 0         : 0010010 ..O..O.
        // Row 1         : 0100110 .O..#O.
        // AND 1+~0 [2]  : 0100100
        // Square Map [3]: 0000100
        // AND ~3+2 [4]  : 0100000
        // OR 4+0 [5]    : 0110010 .OO..O.
        // AND 1+~4 [6]  : 0000110 ....#O.
        private static (uint top, uint bottom) Roll(uint top, uint bottom, uint squareMap, int length)
        {
            uint mask = uint.MaxValue >> (32 - length);
            uint hasOpenSpace = bottom & ~top;
            uint rollable = ~squareMap & hasOpenSpace;
            uint newTop = rollable | top;
            uint newBottom = bottom & ~rollable;

            return (newTop & mask, newBottom & mask);
        }

        private static int CalculateNorthStrain(uint[] maps, uint[] unmoveableMaps, int length)
        {
            int strain = 0;
            for (int i = 0; i < maps.Length; i++)
            {
                uint map = maps[i];
                uint unmoveableMap = unmoveableMaps[i];
                for (int j = 0; j < length; j++)
                {
                    uint a = (map >> j) & 1;
                    uint b = (unmoveableMap >> j) & 1;
                    if (a == 1 && b == 0)
                        strain += maps.Length - i;
                }
            }
            return strain;
        }

        private static void PrintMaps(uint[] maps, uint[] unmoveableMaps, int length)
        {
            for (int i = 0; i < maps.Length; i++)
            {
                uint map = maps[i];
                uint unmoveableMap = unmoveableMaps[i];
                StringBuilder sb = new();
                for (int j = 0; j < length; j++)
                {
                    uint a = (map >> j) & 1;
                    uint b = (unmoveableMap >> j) & 1;
                    sb.Insert(0, a == 1 ? (b == 1 ? '#' : 'O') : '.');
                }
                Console.WriteLine(sb.ToString());
            }
        }
    }
}