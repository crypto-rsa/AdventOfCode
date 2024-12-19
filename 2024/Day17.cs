using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Tools;

namespace Advent_of_Code.Year2024;

public class Day17 : IAdventDay
{
    public string Name => "17. 12. 2024";

    private static string GetInput() => File.ReadAllText("2024/Resources/day17.txt");

    private class Computer
    {
        public Computer(string[] input)
        {
            var matchA = Regex.Match(input[0], @"Register A: (\d+)");
            A = int.Parse(matchA.Groups[1].Value);

            var matchB = Regex.Match(input[1], @"Register B: (\d+)");
            B = int.Parse(matchB.Groups[1].Value);

            var matchC = Regex.Match(input[2], @"Register C: (\d+)");
            C = int.Parse(matchC.Groups[1].Value);

            var matchProgram = Regex.Match(input[3], "Program: (.*)");
            Program = matchProgram.Groups[1].Value.Split(',').Select(int.Parse).ToList();
        }

        #region Properties

        private List<int> Program { get; }

        private int InstructionPointer { get; set; }

        private long A { get; set; }

        private long B { get; set; }

        private long C { get; set; }

        private List<int> Output { get; } = new();

        private int OutputIndex { get; set; }

        #endregion

        #region Methods

        private void DoStep()
        {
            if (InstructionPointer >= Program.Count)
                return;

            int instruction = Program[InstructionPointer];

            switch (instruction)
            {
                case 0:
                    A >>= (int)GetOperand();

                    break;

                case 1:
                    B ^= GetOperand(asLiteral: true);

                    break;

                case 2:
                    B = GetOperand() % 8;

                    break;

                case 3:
                    if (A != 0)
                    {
                        InstructionPointer = (int)GetOperand(asLiteral: true);

                        return;
                    }

                    break;

                case 4:
                    B ^= C;

                    break;

                case 5:
                    int value = (int)(GetOperand() % 8);

                    if (OutputIndex >= Output.Count)
                    {
                        Output.Add(0);
                    }

                    Output[OutputIndex++] = value;

                    break;

                case 6:
                    B = A >> (int)GetOperand();

                    break;

                case 7:
                    C = A >> (int)GetOperand();

                    break;
            }

            InstructionPointer += 2;

            long GetOperand(bool asLiteral = false)
            {
                if (InstructionPointer >= Program.Count - 1)
                    return -1;

                if (asLiteral)
                    return Program[InstructionPointer + 1];

                return Program[InstructionPointer + 1] switch
                {
                    >= 0 and <= 3 => Program[InstructionPointer + 1],
                    4 => A,
                    5 => B,
                    6 => C,
                };
            }
        }

        public string Run() => Run(A);

        private string Run(long a)
        {
            A = a;
            B = 0;
            C = 0;
            InstructionPointer = 0;
            OutputIndex = 0;

            while (InstructionPointer < Program.Count)
            {
                DoStep();
            }

            return string.Join(",", Output);
        }

        public long FindAForSameOutput()
        {
            var stack = new Stack<(long Prefix, int RemainingOutput)>();

            stack.Push((0, Program.Count));

            while (stack.Count > 0)
            {
                (long prefix, int remainingOutput) = stack.Pop();

                if (remainingOutput == 0)
                    return prefix;

                for (short i = 7; i >= 0; i--)
                {
                    long newPrefix = (prefix << 3) | (byte)i;

                    Run(newPrefix);

                    if (Output[0] == Program[remainingOutput - 1])
                    {
                        stack.Push((newPrefix, remainingOutput - 1));
                    }
                }
            }

            Console.WriteLine("No solution found");

            return -1;
        }

        #endregion
    }

    public string Solve() => new Computer(GetInput().SplitToLines().ToArray()).Run();

    public string SolveAdvanced() => new Computer(GetInput().SplitToLines().ToArray()).FindAForSameOutput().ToString();
}
