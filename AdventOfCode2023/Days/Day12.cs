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
            Console.WriteLine(StateMachine(sb.ToString()));
        }

        private static ulong StateMachine(string input)
        {
            (string sequence, int[] lengths)[] lines = input.Split("\r\n").Select(n => {
                var arr = n.Split(' ');
                return (arr[0], arr[1].Split(',').Select(r => int.Parse(r)).ToArray());
            }).ToArray();

            ulong count = 0;
            foreach (var line in lines)
            {
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
                
                // X => Sequence index; Y => State
                ulong[,] stateTable = new ulong[line.sequence.Length + 1, dfa.Steps.Length];
                stateTable[0, 0] = 1;
                for (int x = 0; x < line.sequence.Length; x++)
                {
                    for (int y = 0; y < dfa.Steps.Length; y++)
                    {
                        DeterministicFiniteAutomata.StepResult result = dfa.Steps[y].Validate(line.sequence[x]);
                        if (result.HasFlag(DeterministicFiniteAutomata.StepResult.Continue))
                            stateTable[x+1, y+1] += stateTable[x, y];
                        if (result.HasFlag(DeterministicFiniteAutomata.StepResult.Stay))
                            stateTable[x+1, y] += stateTable[x, y];
                    }
                }

                count += (ulong)stateTable[line.sequence.Length, dfa.Steps.Length - 1];
            }

            return count;
        }
    }

    public class DeterministicFiniteAutomata
    {
        public DFAStep[] Steps { get; set; }

        [Flags]
        public enum StepResult
        {
            Invalid = 0,
            Continue = 1,
            Stay = 2,
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
                if (value == '?')
                    return Validate('.') | Validate('#');
                return StepResult.Invalid;
            }
        }

        public DeterministicFiniteAutomata(DFAStep[] steps)
        {
            Steps = steps;
        }
    }
}
