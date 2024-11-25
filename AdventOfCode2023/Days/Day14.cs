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

        // Plan: Treat each row as a bit array; 0 = empty, 1 = occupied
        // Rollable spots are empty in above row: XOR (0 and 0, nothing to roll; 1 and 1, not rollable;
        // 1 and 0, nothing to roll but adding is fine; 0 and 1, rollable)
        // NAND with map of square rocks to prevent them from moving

        // Row 0         : 0010010
        // Row 1         : 0100110
        // XOR [2]       : 0110100
        // AND 2+1 [3]   : 0100100
        // Square Map [4]: 0000100
        // NAND 4+3 [5]  : 0100000
        // ADD 5+0 [6]   : 0110010

        // Each iteration runs top-to-bottom, generating a checksum as it goes.
        // Loop iteration until checksum matches the previous one.
        
        private static int Part1(string input)
        {
            throw new NotImplementedException();

            BitArray map = new();
            BitArray unmovableMap = new();

            return 0;
        }
    }
}