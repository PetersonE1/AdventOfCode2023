using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Days
{
    internal static class Day2
    {
        public static void Run(string input)
        {
            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        static int Part1(string input)
        {
            string[] lines = input.Split("\r\n");
            int sum = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Substring((i + 1).ToString().Length + 7);
                string[] parts = line.Split("; ");

                bool isValid = true;
                foreach (string part in parts)
                {
                    string[] sections = part.Split(", ");
                    foreach (string section in sections)
                    {
                        if (section.Contains("red"))
                        {
                            if (int.Parse(section.Split(" ")[0]) > 12)
                                isValid = false;
                        }
                        if (section.Contains("blue"))
                        {
                            if (int.Parse(section.Split(" ")[0]) > 14)
                                isValid = false;
                        }
                        if (section.Contains("green"))
                        {
                            if (int.Parse(section.Split(" ")[0]) > 13)
                                isValid = false;
                        }
                    }
                    if (!isValid)
                        break;
                }
                if (isValid)
                {
                    sum += i + 1;
                }
            }
            return sum;
        }

        static int Part2(string input)
        {
            string[] lines = input.Split("\r\n");
            int sum = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Substring((i + 1).ToString().Length + 7);
                string[] parts = line.Split("; ");

                int[] count = [0, 0, 0];
                foreach (string part in parts)
                {
                    string[] sections = part.Split(", ");
                    foreach (string section in sections)
                    {
                        if (section.Contains("red"))
                        {
                            int red = int.Parse(section.Split(" ")[0]);
                            if (red > count[0])
                                count[0] = red;
                        }
                        if (section.Contains("blue"))
                        {
                            int blue = int.Parse(section.Split(" ")[0]);
                            if (blue > count[1])
                                count[1] = blue;
                        }
                        if (section.Contains("green"))
                        {
                            int green = int.Parse(section.Split(" ")[0]);
                            if (green > count[2])
                                count[2] = green;
                        }
                    }
                }
                sum += count[0] * count[1] * count[2];
            }
            return sum;
        }
    }
}
