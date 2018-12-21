using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2018
{
    public class Day19 : IAdventDay
    {
        private class CPU
        {
            private int[] _registers;

            private (string OpCode, int A, int B, int C)[] _program;

            private int _ip;

            private int _ipRegister;

            private long _steps;

            private Dictionary<string, Dictionary<int, long>>[] _states;

            private HashSet<int> _cycleFound = new HashSet<int>();

            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            private (int[] Registers, long Steps)[][] _branches;

            private HashSet<int> _branchInstructions;

            public CPU()
            {
                _registers = new int[] { 0, 0, 0, 0, 0, 0 };
                _states = new Dictionary<string, Dictionary<int, long>>[_registers.Length];

                var input = GetInput();
                _ipRegister = int.Parse(input[0].Split(' ')[1]);

                _program = input.Skip(1).Select(GetInstruction).ToArray();
                _branches = Enumerable.Range(0, _program.Length).Select(_ => new (int[], long)[3]).ToArray();
                _branchInstructions = new HashSet<int>(Enumerable.Range(0, _program.Length).Where(i => _program[i].OpCode == "eqrr" || _program[i].OpCode == "gtrr"));

                (string, int, int, int) GetInstruction(string s)
                {
                    var items = s.Split(' ');

                    return (items[0], int.Parse(items[1]), int.Parse(items[2]), int.Parse(items[3]));
                }

                System.IO.Directory.CreateDirectory(@"2018/Day19");
                System.IO.File.Delete(@"2018/Day19/Steps.txt");
            }

            public bool Step()
            {
                if (_ip < 0 || _ip >= _program.Length)
                {
                    // System.IO.File.WriteAllText(@"2018/Day19/Steps.txt", builder.ToString());
                    return false;
                }

                _registers[_ipRegister] = _ip;
                var instruction = _program[_ip];

                if (_branchInstructions.Contains(_ip))
                {
                    var branchArray = _branches[_ip];
                    branchArray[0] = branchArray[1];
                    branchArray[1] = branchArray[2];
                    branchArray[2] = (_registers.ToArray(), _steps);

                    // check if in the middle of another cycle
                    foreach(int otherIP in _branchInstructions.Where( i => i != _ip ).ToList())
                    {
                        if(_branches[otherIP][1].Registers != null && _branches[otherIP][2].Registers != null
                            && _steps < 2 * _branches[otherIP][2].Steps - _branches[otherIP][1].Steps)
                        {
                            _branchInstructions.Remove(otherIP);
                        }
                    }

                    if (branchArray[0].Registers != null & branchArray[1].Registers != null)
                    {
                        var diff1 = GetDiff(branchArray[0].Registers, branchArray[1].Registers);
                        var diff2 = GetDiff(branchArray[1].Registers, branchArray[2].Registers);

                        if (diff1.Zip(diff2, (i1, i2) => i1 == i2).All(b => b) && branchArray[2].Steps + branchArray[0].Steps == 2 * branchArray[1].Steps)
                        {
                            long cycleLength = _steps - branchArray[1].Steps;
                            int cyclesToJump = _program.Where((o, i) => _branchInstructions.Contains(i) && diff1[o.A] > 0).Min(o => (_registers[o.B] - _registers[o.A]) / diff1[o.A]);

                            if (cyclesToJump > 1)
                            {
                                Console.WriteLine($"IP {_ip}: Cycle of length {cycleLength} for register {instruction.C} found at step {_steps}!");

                                _registers = _registers.Zip(diff1, (r, d) => r + cyclesToJump * d).ToArray();
                                _steps += cyclesToJump * cycleLength;

                                Console.WriteLine($"Jumped {cyclesToJump} cycles ahead to step {_steps}");

                                branchArray[0] = (null, 0);
                                branchArray[1] = (null, 0);
                                branchArray[2] = (null, 0);
                            }
                        }
                    }

                    bool result = instruction.OpCode == "eqrr" ? _registers[instruction.A] == _registers[instruction.B] : _registers[instruction.A] > _registers[instruction.B];
                    // builder.AppendLine($"Step {_steps}: {new string(' ', 3*_ip)} I{_ip:D2} {result}");

                    // if (branchArray[0].Registers != null && branchArray[1].Registers != null)
                    //     System.IO.File.AppendAllText($@"2018/Day19/{_ip}.txt", $"Step {_steps}: {GetDiff(branchArray[0].Registers, branchArray[1].Registers, branchArray[2].Registers)}");
                }

                Perform(instruction.OpCode, instruction.A, instruction.B, instruction.C);

                int[] GetDiff(int[] r0, int[] r1) => r1.Zip(r0, (i1, i0) => i1 - i0).ToArray();

                // builder.AppendLine($"Step {_steps}: {GetRegisters(_registers)}");

                _ip = _registers[_ipRegister] + 1;

                _steps++;

                // if (_steps >= 100000)
                //     return false;

                return true;

                string GetRegisters(int[] registers) => $"[{string.Join(", ", registers)}]";
            }

            public void SetRegister(int register, int value)
            {
                _registers[register] = value;
            }

            public int GetRegister(int index) => _registers[index];

            public void Perform(string operation, int A, int B, int C)
            {
                switch (operation)
                {
                    case "addr": _registers[C] = _registers[A] + _registers[B]; break;
                    case "addi": _registers[C] = _registers[A] + B; break;
                    case "mulr": _registers[C] = _registers[A] * _registers[B]; break;
                    case "muli": _registers[C] = _registers[A] * B; break;
                    case "banr": _registers[C] = _registers[A] & _registers[B]; break;
                    case "bani": _registers[C] = _registers[A] & B; break;
                    case "borr": _registers[C] = _registers[A] | _registers[B]; break;
                    case "bori": _registers[C] = _registers[A] | B; break;
                    case "setr": _registers[C] = _registers[A]; break;
                    case "seti": _registers[C] = A; break;
                    case "gtir": _registers[C] = A > _registers[B] ? 1 : 0; break;
                    case "gtri": _registers[C] = _registers[A] > B ? 1 : 0; break;
                    case "gtrr": _registers[C] = _registers[A] > _registers[B] ? 1 : 0; break;
                    case "eqir": _registers[C] = A == _registers[B] ? 1 : 0; break;
                    case "eqri": _registers[C] = _registers[A] == B ? 1 : 0; break;
                    case "eqrr": _registers[C] = _registers[A] == _registers[B] ? 1 : 0; break;
                }
            }
        }

        public string Name => "19. 12. 2018";

        public static string[] GetInput() => System.IO.File.ReadAllLines(@"2018/Resources/day19.txt");

        public string Solve()
        {
            var cpu = new CPU();

            while (cpu.Step())
                ;

            return cpu.GetRegister(0).ToString();
        }

        public string SolveAdvanced()
        {
            return string.Empty;
            var cpu = new CPU();
            cpu.SetRegister(0, 1);

            while (cpu.Step())
                ;

            return cpu.GetRegister(0).ToString();
        }
    }
}