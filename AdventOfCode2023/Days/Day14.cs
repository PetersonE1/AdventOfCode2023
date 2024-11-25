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
            (UInt128[] maps, UInt128[] unmoveableMaps) = ToBits(input, out int length);

            UInt128[] cached_maps = new UInt128[maps.Length];
            do
            {
                maps.CopyTo(cached_maps, 0);
                IterationStep(maps, unmoveableMaps, length);
            }
            while (!maps.SequenceEqual(cached_maps));

            return CalculateNorthStrain(maps, unmoveableMaps, length);
        }

        private static (UInt128[] maps, UInt128[] unmoveableMaps) ToBits(string input, out int length)
        {
            string[] lines = input.Split("\r\n");
            length = lines[0].Length;
            UInt128[] maps = new UInt128[lines.Length];
            UInt128[] unmoveableMaps = new UInt128[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                UInt128 map = 0;
                UInt128 unmoveableMap = 0;

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

        private static void IterationStep(UInt128[] maps, UInt128[] unmoveableMaps, int length)
        {
            for (int i = 0; i < maps.Length - 1; i++)
            {
                (UInt128 a, UInt128 b) = Roll(maps[i], maps[i+1], unmoveableMaps[i+1], length);
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
        private static (UInt128 top, UInt128 bottom) Roll(UInt128 top, UInt128 bottom, UInt128 squareMap, int length)
        {
            UInt128 mask = UInt128.MaxValue >> (128 - length);
            UInt128 hasOpenSpace = bottom & ~top;
            UInt128 rollable = ~squareMap & hasOpenSpace;
            UInt128 newTop = rollable | top;
            UInt128 newBottom = bottom & ~rollable;

            return (newTop & mask, newBottom & mask);
        }

        private static int CalculateNorthStrain(UInt128[] maps, UInt128[] unmoveableMaps, int length)
        {
            int strain = 0;
            for (int i = 0; i < maps.Length; i++)
            {
                UInt128 map = maps[i];
                UInt128 unmoveableMap = unmoveableMaps[i];
                for (int j = 0; j < length; j++)
                {
                    UInt128 a = (map >> j) & 1;
                    UInt128 b = (unmoveableMap >> j) & 1;
                    if (a == 1 && b == 0)
                        strain += maps.Length - i;
                }
            }
            return strain;
        }

        private static void PrintMaps(UInt128[] maps, UInt128[] unmoveableMaps, int length)
        {
            for (int i = 0; i < maps.Length; i++)
            {
                UInt128 map = maps[i];
                UInt128 unmoveableMap = unmoveableMaps[i];
                StringBuilder sb = new();
                for (int j = 0; j < length; j++)
                {
                    UInt128 a = (map >> j) & 1;
                    UInt128 b = (unmoveableMap >> j) & 1;
                    sb.Insert(0, a == 1 ? (b == 1 ? '#' : 'O') : '.');
                }
                Console.WriteLine(sb.ToString());
            }
        }
    }
}