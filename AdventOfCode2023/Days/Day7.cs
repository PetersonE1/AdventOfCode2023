using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Days
{
    internal static class Day7
    {
        public static void Run(string input)
        {
            Console.WriteLine(Part1(input));
        }

        static int Part1(string input)
        {
            List<(string, int)> lines = input.Split("\r\n").Select(line => line.Split(' ')).Select(line => (line[0], int.Parse(line[1]))).ToList();
            lines.Sort(new HandComparer());

            int sum = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                sum += lines[i].Item2 * (i + 1);
            }

            return sum;
        }

        class HandComparer : IComparer<(string, int)>
        {
            public int Compare((string, int) x, (string, int) y)
            {
                int result = GetHandType(x.Item1).CompareTo(GetHandType(y.Item1));
                if (result != 0)
                    return result;

                for (int i = 0; i < x.Item1.Length; i++)
                {
                    result = Array.IndexOf(strengths, y.Item1[i]).CompareTo(Array.IndexOf(strengths, x.Item1[i]));
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

            HandType GetHandType(string hand)
            {
                hand = string.Join("", hand.ToCharArray().OrderBy(c => c));
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
                    if (temp == 3)
                        return HandType.FullHouse;
                    if (temp == 2)
                        return HandType.TwoPair;
                    if (temp == 1)
                    {
                        if (hand.Count(c => c == hand[3]) == 2)
                            return HandType.TwoPair;
                        return HandType.Pair;
                    }
                }
                if (count == 1)
                {
                    int temp = hand.Count(c => c == hand[1]);
                    if (temp == 4)
                        return HandType.FourOfAKind;
                    if (temp == 3)
                        return HandType.ThreeOfAKind;
                    if (temp == 2)
                    {
                        if (hand.Count(c => c == hand[3]) == 2)
                            return HandType.TwoPair;
                        return HandType.Pair;
                    }
                    if (temp == 1)
                    {
                        int temp2 = hand.Count(c => c == hand[2]);
                        if (temp2 == 3)
                            return HandType.ThreeOfAKind;
                        if (temp2 == 2)
                            return HandType.Pair;
                        if (temp2 == 1)
                        {
                            if (hand.Count(c => c == hand[3]) == 2)
                                return HandType.Pair;
                            return HandType.HighCard;
                        }
                    }
                }
                return HandType.HighCard;
            }
        }
    }
}
