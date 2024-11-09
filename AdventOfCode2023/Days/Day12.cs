using System;
using System.Collections;
using System.Text;

namespace AdventOfCode2023.Days
{
    internal static class Day12
    {
        public static void Run(string input)
        {
            //Console.WriteLine(BruteForce(input));
            Console.WriteLine(StateMachine(input));

            StringBuilder sb = new();
            string[] lines = input.Split("\r\n");
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] sides = line.Split();
                
                sb.Append(sides[0] + '?');
                sb.Append(sides[0] + '?');
                sb.Append(sides[0] + '?');
                sb.Append(sides[0] + '?');
                sb.Append(sides[0] + ' ');

                sb.Append(sides[1] + ',');
                sb.Append(sides[1] + ',');
                sb.Append(sides[1] + ',');
                sb.Append(sides[1] + ',');
                sb.Append(sides[1]);

                if (i != lines.Length - 1)
                    sb.AppendLine();
            }
            Console.WriteLine(StateMachine(sb.ToString(), true));
        }

        private static ulong BruteForce(string input)
        {
            // Remove confirmed sequences of damaged springs from the input list
            // Attempt to logically confirm unknowns next to confirmed damages
            // Attempt to confirm list numbers to unknown regions
            // Iterate over regions, starting leftmost and moving right, nesting for multiple list numbers
            // Generate count of possibilities per region, multiply all together to get possibilities per line

            // Method 2: Treat as binary, count from 0 up and perform a bitwise AND to check if it fits the pattern
            // If it fits, group and check if it matches the numbers. If so, increment counter by 1

            // Method 2 works for part 1. Will not work for part 2. Need to be smarter.

            (Boolish[] sequence, int[] lengths)[] lines = input.Split("\r\n").Select(n => {
                var arr = n.Split(' ');
                return (ToBoolish(arr[0]), arr[1].Split(',').Select(r => int.Parse(r)).ToArray());
            }).ToArray();

            int lineCount = 0;
            ulong totalCount = 0;
            foreach (var line in lines)
            {
                lineCount++;
                if (lineCount % 100 == 0)
                    Console.WriteLine($"Processing line {lineCount}");

                BitArray bitArray = new BitArray(line.sequence.Length);

                int topBound = (int)Math.Pow(2, bitArray.Count) - 1;

                uint count = 0;
                for (int i = 0; i < topBound; i++)
                {
                    Increment(bitArray);
                    if (BitwiseANDCheck(bitArray, line.sequence))
                    {
                        bool[] bools = new bool[bitArray.Count];
                        bitArray.CopyTo(bools, 0);
                        bool[][] grouped = bools.GroupConsecutive().ToArray();
                        if (grouped.Length != line.lengths.Length)
                            continue;
                        bool countUp = true;
                        for (int j = 0; j < line.lengths.Length; j++)
                        {
                            if (grouped[j].Length != line.lengths[j])
                            {
                                countUp = false;
                                break;
                            }
                        }
                        if (countUp)
                            count++;
                    }
                }
                totalCount += count;
            }

            return totalCount;
        }

        private static ulong StateMachine(string input, bool extendDebug = false)
        {
            (string sequence, int[] lengths)[] lines = input.Split("\r\n").Select(n => {
                var arr = n.Split(' ');
                return (arr[0], arr[1].Split(',').Select(r => int.Parse(r)).ToArray());
            }).ToArray();

            ulong count = 0;
            uint lineCount = 0;
            foreach (var line in lines)
            {
                lineCount++;
                if (!extendDebug && lineCount % 100 == 0)
                    Console.WriteLine($"Processing line {lineCount}...");
                if (extendDebug)
                    Console.WriteLine($"Processing line {lineCount}...");

                List<DeterministicFiniteAutomata.DFAStep> steps = new();
                for (int i = 0; i < line.lengths.Length; i++)
                {
                    steps.Add(new(['#'], ['.']));
                    for (int j = 1; j < line.lengths[i]; j++)
                    {
                        steps.Add(new(['#'], []));
                    }
                    if (i != line.lengths.Length - 1)
                    {
                        steps.Add(new(['.'], []));
                    }
                    if (i == line.lengths.Length - 1)
                    {
                        steps.Add(new([], ['.']));
                    }
                }
                DeterministicFiniteAutomata dfa = new(steps.ToArray());
                count += StepQueue(new Queue<char>(line.sequence), dfa);
            }

            return count;
        }

        private static ulong StepQueue(Queue<char> queue, DeterministicFiniteAutomata dfa)
        {
            if (queue.Count == 0)
            {
                if (dfa.IsDone)
                    return 1;
                return 0;
            }

            char c = queue.Dequeue();
            
            if (c == '?')
            {
                List<char> l1 = ['#'];
                l1.AddRange(queue);
                List<char> l2 = ['.'];
                l2.AddRange(queue);
                return StepQueue(new(l1), new(dfa.Steps, dfa.Index)) + StepQueue(new(l2), new(dfa.Steps, dfa.Index));
            }

            DeterministicFiniteAutomata.StepResult result = dfa.ValidateNext(c);
            if (result == DeterministicFiniteAutomata.StepResult.Continue)
                return StepQueue(new(queue), new(dfa.Steps, dfa.Index + 1));
            if (result == DeterministicFiniteAutomata.StepResult.Stay)
                return StepQueue(new(queue), new(dfa.Steps, dfa.Index));
            if (result == DeterministicFiniteAutomata.StepResult.Invalid)
                return 0;
            
            throw new InvalidOperationException($"DFA returned bad data: {result}");
        }

        private static void Increment(BitArray bArray)
        {
            for (int i = 0; i < bArray.Count; i++)
            {
                bool previous = bArray[i];
                bArray[i] = !previous;
                if (!previous)
                {
                    // Found a clear bit - now that we've set it, we're done
                    return;
                }
            }
        }

        private static Boolish[] ToBoolish(string sequence)
        {
            Boolish[] boolishes = new Boolish[sequence.Length];
            for (int i = 0; i < boolishes.Length; i++)
            {
                switch (sequence[i])
                {
                    case '#': boolishes[i] = new(Boolish.BoolishValue.True); break;
                    case '.': boolishes[i] = new(Boolish.BoolishValue.False); break;
                    case '?': boolishes[i] = new(Boolish.BoolishValue.True | Boolish.BoolishValue.False); break;
                    default: throw new ArgumentException($"Invalid input char: {sequence[i]}");
                }
            }

            return boolishes;
        }

        private static bool BitwiseANDCheck(BitArray a, Boolish[] b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Arrays are not the same length");

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }

        private static IEnumerable<bool[]> GroupConsecutive(this IEnumerable<bool> list)
        {
            if (list.Any())
            {
                int index = 0;
                bool rootItem = list.First();

                for (int i = 1; i < list.Count(); i++)
                {
                    if (!list.ElementAt(i).Equals(rootItem))
                    {
                        // Only yields the damaged sequences
                        if (rootItem)
                            yield return list.Take(new Range(index, i)).ToArray();
                        index = i;
                        rootItem = list.ElementAt(i);
                    }
                }
                if (rootItem)
                    yield return list.Take(new Range(index, list.Count())).ToArray();
            }
        }
    }

    public class Boolish
    {
        [Flags]
        public enum BoolishValue
        {
            True = 1,
            False = 2
        }

        public BoolishValue Value { get; set; }

        public Boolish(BoolishValue value)
        {
            Value = value;
        }

        public Boolish(bool value)
        {
            Value = value ? BoolishValue.True : BoolishValue.False;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Boolish)
            {
                Boolish other = (Boolish)obj;
                return (this.Value.HasFlag(BoolishValue.True) && other.Value.HasFlag(BoolishValue.True))
                || (this.Value.HasFlag(BoolishValue.False) && other.Value.HasFlag(BoolishValue.False));
            }
            if (obj is bool)
            {
                bool other = (bool)obj;
                return (this.Value.HasFlag(BoolishValue.True) && other) || (this.Value.HasFlag(BoolishValue.False) && !other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            if (Value.HasFlag(BoolishValue.True))
            {
                if (Value.HasFlag(BoolishValue.False))
                    return "Maybe";
                return "True";
            }
            if (Value.HasFlag(BoolishValue.False))
                return "False";
            return "Empty";
        }

        public static bool operator ==(Boolish a, Boolish b) => a.Equals(b);
        public static bool operator !=(Boolish a, Boolish b) => !a.Equals(b);

        public static bool operator ==(Boolish a, bool b) => a.Equals(b);
        public static bool operator !=(Boolish a, bool b) => !a.Equals(b);

        public static bool operator ==(bool a, Boolish b) => b.Equals(a);
        public static bool operator !=(bool a, Boolish b) => !b.Equals(a);
    }

    public class DeterministicFiniteAutomata
    {
        public int Index { get; private set; }

        public DFAStep[] Steps { get; set; }

        public bool IsDone => Index >= Steps.Length || (Index == Steps.Length - 1 && Steps[Index].ForwardInputs.Length == 0);

        public enum StepResult
        {
            Continue,
            Stay,
            Invalid
        }

        public class DFAStep
        {
            public char[] ForwardInputs;
            public char[] LoopedInputs;

            public DFAStep(char[] forward, char[] looped)
            {
                ForwardInputs = forward;
                LoopedInputs = looped;
            }

            public StepResult Validate(char value)
            {
                if (ForwardInputs.Contains(value))
                    return StepResult.Continue;
                if (LoopedInputs.Contains(value))
                    return StepResult.Stay;
                return StepResult.Invalid;
            }
        }

        public DeterministicFiniteAutomata(DFAStep[] steps, int index = 0)
        {
            Steps = steps;
            Index = index;
        }

        public StepResult ValidateNext(char value)
        {
            return Steps[Index].Validate(value);
        }
    }
}
