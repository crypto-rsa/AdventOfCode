using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2015
{
    public class Day23 : IAdventDay
    {
        private class Instruction
        {
            public Instruction(string input)
            {
                var match = Regex.Match(input, @"(.*?) (.*)");
                Operation = match.Groups[1].Value;
                Arguments = match.Groups[2].Value.Split(',');
            }

            public string Operation { get; }

            public string[] Arguments { get; }
        }

        public string Name => "23. 12. 2015";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2015/Resources/day23.txt");

        public string Solve() => Iterate(0).ToString();

        public string SolveAdvanced() => Iterate(1).ToString();

        private long Iterate(int initialValue)
        {
            var instructions = GetInput().Select(s => new Instruction(s)).ToList();

            var registers = new Dictionary<string, long>()
            {
                ["a"] = initialValue,
                ["b"] = 0,
            };

            int ip = 0;

            while (ip < instructions.Count)
            {
                var instruction = instructions[ip];

                switch (instruction.Operation)
                {
                    case "hlf":
                        registers[instruction.Arguments[0]] /= 2;
                        ip++;
                        break;

                    case "tpl":
                        registers[instruction.Arguments[0]] *= 3;
                        ip++;
                        break;

                    case "inc":
                        registers[instruction.Arguments[0]]++;
                        ip++;
                        break;

                    case "jmp":
                        ip += int.Parse(instruction.Arguments[0]);
                        break;

                    case "jie":
                        if (registers[instruction.Arguments[0]] % 2 == 0)
                        {
                            ip += int.Parse(instruction.Arguments[1]);
                        }
                        else
                        {
                            ip++;
                        }
                        break;

                    case "jio":
                        if (registers[instruction.Arguments[0]] == 1)
                        {
                            ip += int.Parse(instruction.Arguments[1]);
                        }
                        else
                        {
                            ip++;
                        }
                        break;
                }
            }

            return registers["b"];

        }
    }
}
