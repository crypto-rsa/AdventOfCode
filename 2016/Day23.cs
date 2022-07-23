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
        
        private class LoopDetector
        {
            private readonly string[][] _instructions;

            private readonly Dictionary<string, int> _registers;

            public LoopDetector(string[][] instructions, Dictionary<string, int> registers)
            {
                _instructions = instructions;
                _registers = registers;
            }

            private bool CanBeRewritten(int ip, string register) => _instructions[ip][0] switch
            {
                "inc" or "dec" => _instructions[ip][1] != register,
                _ => false,
            };

            private bool IsLoop(int ip)
            {
                if (_instructions[ip][0] != "jnz")
                    return false;

                string counter = _instructions[ip][1];

                if (!_registers.ContainsKey(counter))
                    return false;

                int counterValue = _registers[counter];

                if (counterValue == 0)
                    return false;

                int jump = GetValue(_registers, _instructions[ip][2]);
                if ( jump >= 0)
                    return false;

                var loopBody = _instructions[(ip - jump)..ip];

                int counterChange = loopBody.Sum(i => i[0] switch
                {
                    "inc" when i[1] == counter => +1,
                    "dec" when i[1] == counter => -1,
                    _ => 0,
                });
                
                if (counterValue * counterChange >= 0)
                    return false;

                if (counterValue % counterChange != 0)
                    return false;

                if (loopBody.Any(i => i[0] is not ("inc" or "dec" or "add" or "mul")))
                    return false;

                return Enumerable.Range(ip - jump, jump).All(i => CanBeRewritten(i, counter));
            }
        }

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

            while (ip < instructions.Length)
            {
                var instruction = instructions[ip];

                // log.Add((instructionName, ip));

                switch (instruction[0])
                {
                    case "cpy":
                        if (!registers.ContainsKey(instruction[2]))
                            break;

                        registers[instruction[2]] = GetValue(registers, instruction[1]);
                        break;

                    case "inc":
                        registers[instruction[1]]++;
                        break;

                    case "dec":
                        registers[instruction[1]]--;
                        break;

                    case "jnz":
                        if (GetValue(registers, instruction[1]) != 0)
                        {
                            ip += GetValue(registers, instruction[2]);
                            continue;
                        }
                        break;

                    case "tgl":
                        int location = ip + GetValue(registers, instruction[1]);

                        if (location < 0 || location >= instructions.Length)
                            break;

                        instructions[location][0] = instructions[location][0] switch
                        {
                            "cpy" => "jnz",
                            "inc" => "dec",
                            "dec" => "inc",
                            "jnz" => "cpy",
                            "tgl" => "inc",
                            _ => throw new InvalidOperationException(),
                        };
                        break;
                }

                ip++;
            }

            return registers["a"].ToString();
        }
        
        private static int GetValue(Dictionary<string, int> registers, string s) => registers.TryGetValue(s, out int value) ? value : int.Parse(s);

        public string Solve() => Process(7);

        public string SolveAdvanced() => Process(12);
    }
}
