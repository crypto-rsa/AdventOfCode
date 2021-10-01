using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2016
{
    public class Day12 : IAdventDay
    {
        public string Name => "12. 12. 2016";

        private static string[][] GetInput() => File.ReadAllLines("2016/Resources/day12.txt").Select(s => s.Split(' ')).ToArray();

        private string Process(int c)
        {
            var instructions = GetInput();
            var registers = new Dictionary<string, int>()
            {
                ["a"] = 0,
                ["b"] = 0,
                ["c"] = c,
                ["d"] = 0,
            };
            int ip = 0;

            while (ip < instructions.Length)
            {
                var instruction = instructions[ip];

                switch (instruction[0])
                {
                    case "cpy":
                        registers[instruction[2]] = GetValue(instruction[1]);
                        break;

                    case "inc":
                        registers[instruction[1]]++;
                        break;

                    case "dec":
                        registers[instruction[1]]--;
                        break;

                    case "jnz":
                        if (GetValue(instruction[1]) != 0)
                        {
                            ip += int.Parse(instruction[2]);
                            continue;
                        }
                        break;
                }

                ip++;
            }

            return registers["a"].ToString();

            int GetValue(string s) => registers.TryGetValue(s, out int value) ? value : int.Parse(s);
        }

        public string Solve() => Process(0);

        public string SolveAdvanced() => Process(1);
    }
}
