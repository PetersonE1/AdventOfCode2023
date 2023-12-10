using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Days
{
    internal static class Day7
    {
        public static void Run(string input)
        {
            Console.WriteLine(Solution(input, false));
            Console.WriteLine(Solution(input, true));
        }

        static int Solution(string input, bool isPart2)
        {
            List<(string, int)> lines = input.Split("\r\n").Select(line => line.Split(' ')).Select(line => (line[0], int.Parse(line[1]))).ToList();
            HandComparer comparer = new HandComparer();
            comparer.Part2 = isPart2;
            lines.Sort(comparer);

            int sum = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                sum += lines[i].Item2 * (i + 1);
            }

            return sum;
        }

        class HandComparer : IComparer<(string, int)>
        {
            public bool Part2 = false;

            public int Compare((string, int) x, (string, int) y)
            {
                int result = GetHandType(x.Item1).CompareTo(GetHandType(y.Item1));
                if (result != 0)
                    return result;

                char[] compareArray = Part2 ? strengths2 : strengths;

                for (int i = 0; i < x.Item1.Length; i++)
                {
                    result = Array.IndexOf(compareArray, y.Item1[i]).CompareTo(Array.IndexOf(compareArray, x.Item1[i]));
                    if (result != 0)
                        return result;
                }
                return 0;
            }

            enum HandType
            {
                HighCard,
                Pair,
                TwoPair,
                ThreeOfAKind,
                FullHouse,
                FourOfAKind,
                FiveOfAKind,
            }

            char[] strengths = ['A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2'];
            char[] strengths2 = ['A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J'];

            HandType GetHandType(string hand)
            {
                Dictionary<char, int> chars = new();
                foreach (char c in hand)
                {
                    if (!chars.ContainsKey(c))
                        chars.Add(c, 1);
                    else
                        chars[c]++;
                }
                SortedSet<(char, int)> charCounts = new(chars.Select(x => (x.Key, x.Value)), new CharCountComparer());
                int countJ = charCounts.Where(x => x.Item1 == 'J').Select(x => x.Item2).FirstOrDefault();
                if (Part2 && countJ > 0 && countJ < 5)
                {
                    (char, int) item = charCounts.First(c => c.Item1 == 'J');
                    (char, int) mostItem = charCounts.First(c => c.Item1 != 'J');
                    charCounts.Remove(item);
                    charCounts.Remove(mostItem);

                    mostItem.Item2 += item.Item2;
                    charCounts.Add(mostItem);
                }
                hand = string.Join("", charCounts.Select(x => string.Join("", new char[x.Item2].Select(i => x.Item1))));
                int count = hand.Count(c => c == hand[0]);
                if (count == 5)
                    return HandType.FiveOfAKind;
                if (count == 4)
                    return HandType.FourOfAKind;
                if (count == 3)
                {
                    if (hand.Count(c => c == hand[3]) == 2)
                        return HandType.FullHouse;
                    return HandType.ThreeOfAKind;
                }
                if (count == 2)
                {
                    int temp = hand.Count(c => c == hand[2]);
                    if (temp == 2)
                        return HandType.TwoPair;
                    if (temp == 1)
                        return HandType.Pair;
                }
                return HandType.HighCard;
            }
        }



        class CharCountComparer : IComparer<(char, int)>
        {
            public int Compare((char, int) x, (char, int) y)
            {
                int result = y.Item2.CompareTo(x.Item2);
                if (result != 0)
                    return result;
                return y.Item1.CompareTo(x.Item1);
            }
        }
    }
}
