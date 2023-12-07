using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Days
{
    internal static class Day1
    {
        private static string[] digits = [ "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" ];

        public static int Run(string input)
        {
            string[] lines = input.Split("\r\n");
            List<string> numbers = new List<string>();

            foreach (string line in lines)
            {
                Tuple<int, int, int>[] speltDigits = GetSpeltDigits(line);
                string number = string.Empty;
                for (int i = 0; i < line.Length; i++)
                    if (int.TryParse(line[i].ToString(), out int first))
                    {
                        bool isFirst = true;
                        foreach (var speltDigit in speltDigits)
                        {
                            if (speltDigit.Item1 < i)
                            {
                                isFirst = false;
                                break;
                            }
                        }
                        if (isFirst)
                            number += line[i];
                        break;
                    }
                if (number.Length == 0)
                {
                    Tuple<int, int, int> seed = new Tuple<int, int, int>(int.MaxValue, 0, 0);
                    number += speltDigits.Aggregate(seed, (min, x) => x.Item1 < min.Item1 ? x : min).Item3;
                }

                for (int i = line.Length - 1; i >= 0; i--)
                    if (int.TryParse(line[i].ToString(), out int last))
                    {
                        bool isLast = true;
                        foreach (var speltDigit in speltDigits)
                        {
                            if (speltDigit.Item2 > i)
                            {
                                isLast = false;
                                break;
                            }
                        }
                        if (isLast)
                            number += line[i];
                        break;
                    }
                if (number.Length == 1)
                {
                    Tuple<int, int, int> seed = new Tuple<int, int, int>(0, 0, 0);
                    number += speltDigits.Aggregate(seed, (max, x) => x.Item2 > max.Item2 ? x : max).Item3;
                }
                numbers.Add(number);
            }

            int result = 0;
            foreach (string number in numbers)
            {
                result += int.Parse(number);
            }
            return result;
        }

        public static Tuple<int, int, int>[] GetSpeltDigits(string input)
        {
            List<Tuple<int, int, int>> results = new List<Tuple<int, int, int>>();
            for (int i = 0; i < digits.Length; i++)
            {
                int? first = GetSpanContains(input, digits[i]);
                int? last = GetSpanContains(input, digits[i], true);
                if (first != null && last != null)
                {
                    results.Add(new Tuple<int, int, int>((int)first, (int)last, i + 1));
                }
            }

            return results.ToArray();
        }

        public static int? GetSpanContains(string input, string target, bool reverse = false)
        {
            int start = 0;
            int length = input.Length;
            if (!input.Contains(target))
                return null;

            if (!reverse)
            {
                while (input.Substring(start, length - start).Contains(target))
                {
                    length--;
                }
                length++;

                while (input.Substring(start, length - start).Contains(target))
                {
                    start++;
                }
                start--;

                return start;
            }
            else
            {
                while (input.Substring(start, length - start).Contains(target))
                {
                    start++;
                }
                start--;

                while (input.Substring(start, length - start).Contains(target))
                {
                    length--;
                }
                length++;

                return start;
            }
        }
    }
}
