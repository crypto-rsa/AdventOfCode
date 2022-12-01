using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2016
{
    public class Day23 : IAdventDay
    {
        public string Name => "23. 12. 2016";

        private static string[][] GetInput() => File.ReadAllLines("2016/Resources/day23.txt").Select(s => s.Split(' ')).ToArray();

        private class LoopInfo
        {
            private string _counter;

            public int StartInstruction { get; }

            public int JumpInstruction { get; }

            public bool CanBeProcessed { get; }

            public LoopInfo(string[][] instructions, int ip)
            {
                JumpInstruction = ip;

                if( int.TryParse(instructions[ip][1], out int jump ))
                {
                    StartInstruction = jump;
                }

                _counter = instructions[ip][0];

                CanBeProcessed = true;
            }

            public bool Process(Dictionary<string, int> registers)
            {
                return false;
            }
        }

        private string Process(int a)
        {
            var instructions = GetInput();
            var registers = new Dictionary<string, int>()
            {
                ["a"] = a,
                ["b"] = 0,
                ["c"] = 0,
                ["d"] = 0,
            };
            int ip = 0;

            List<(string Name, int Index)> log = new();

            var toggled = new Dictionary<int, string>();

            while (ip < instructions.Length)
            {
                var instruction = instructions[ip];

                if (!toggled.TryGetValue(ip, out var instructionName))
                {
                    instructionName = instruction[0];
                }

                // log.Add((instructionName, ip));

                switch (instructionName)
                {
                    case "cpy":
                        if (!registers.ContainsKey(instruction[2]))
                            break;

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
                            ip += GetValue(instruction[2]);
                            continue;
                        }
                        break;

                    case "tgl":
                        int location = ip + GetValue(instruction[1]);

                        if (location < 0 || location >= instructions.Length)
                            break;

                        if (toggled.ContainsKey(location))
                        {
                            toggled.Remove(location);
                        }
                        else
                        {
                            toggled[location] = instructions[location][0] switch
                            {
                                "cpy" => "jnz",
                                "inc" => "dec",
                                "dec" => "inc",
                                "jnz" => "cpy",
                                "tgl" => "inc",
                                _ => throw new InvalidOperationException(),
                            };
                        }
                        break;
                }

                ip++;
            }

            return registers["a"].ToString();

            int GetValue(string s) => registers.TryGetValue(s, out int value) ? value : int.Parse(s);
        }

        public string Solve() => Process(7);

        public string SolveAdvanced() => Process(12);
    }
}
