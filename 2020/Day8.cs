using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2020
{
    public class Day8 : IAdventDay
    {
        private class Instruction
        {
            public Instruction(string input)
            {
                var parts = input.Split(' ');

                Operation = parts[0];

                if (parts.Length > 1)
                {
                    Argument = int.Parse(parts[1]);
                }
            }

            public Instruction(Instruction source, bool swapOperation)
            {
                Operation = (source.Operation, swapOperation) switch
                {
                    ("nop", true) => "jmp",
                    ("jmp", true) => "nop",
                    _ => source.Operation,
                };
                Argument = source.Argument;
            }

            public string Operation { get; }

            public int Argument { get; }
        }

        public string Name => "8. 12. 2020";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2020/Resources/day8.txt");

        public string Solve()
        {
            var instructions = GetInput().Select(s => new Instruction(s)).ToList();

            return RunProgram(instructions).Accumulator.ToString();
        }

        public string SolveAdvanced()
        {
            var instructions = GetInput().Select(s => new Instruction(s)).ToList();

            for (int i = 0; i < instructions.Count; i++)
            {
                if (instructions[i].Operation == "acc")
                    continue;

                var (terminates, accumulator) = RunProgram(FixInstructions(i));

                if (terminates)
                    return accumulator.ToString();
            }

            return string.Empty;

            List<Instruction> FixInstructions(int index) => instructions.Select((instruction, i) => new Instruction(instruction, i == index)).ToList();
        }

        private (bool Terminates, int Accumulator) RunProgram(List<Instruction> instructions)
        {
            var visited = new HashSet<int>();

            int ip = 0;
            int acc = 0;

            while (true)
            {
                if (ip >= instructions.Count)
                    return (true, acc);
                
                if( visited.Contains(ip) )
                    return (false, acc);

                visited.Add(ip);

                switch (instructions[ip].Operation)
                {
                    case "nop":
                        ip++;
                        break;

                    case "acc":
                        acc += instructions[ip].Argument;
                        ip++;
                        break;

                    case "jmp":
                        ip += instructions[ip].Argument;
                        break;
                }
            }
        }
    }
}
