using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2018
{
    public class Day16 : IAdventDay
    {
        private class CPU
        {
            private int[] _registers;

            public string[] Operations { get; } = new string[]
            {
                "addr",
                "addi",
                "mulr",
                "muli",
                "banr",
                "bani",
                "borr",
                "bori",
                "setr",
                "seti",
                "gtir",
                "gtri",
                "gtrr",
                "eqir",
                "eqri",
                "eqrr",
            };

            public void SetRegisters(int[] registers)
            {
                _registers = registers.ToArray();
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

            public bool MatchesRegisters(int[] registers) => _registers.SequenceEqual(registers);

            public HashSet<string> GetMatches(string code, int[] before, int[] after)
            {
                var items = code.Split(' ');
                int A = int.Parse(items[1]);
                int B = int.Parse(items[2]);
                int C = int.Parse(items[3]);

                return new HashSet<string>(Operations.Where(Matches));

                bool Matches(string operation)
                {
                    SetRegisters(before);
                    Perform(operation, A, B, C);

                    return MatchesRegisters(after);
                }
            }
        }
        public string Name => "16. 12. 2018";

        public static string[] GetInput() => System.IO.File.ReadAllLines(@"2018/Resources/day16.txt");

        private IEnumerable<string[]> GetChunks()
        {
            var input = GetInput();

            for (int i = 0; 4 * i + 2 < input.Length; i++)
            {
                var lines = new string[]
                {
                        input[4*i],
                        input[4*i+1],
                        input[4*i+2],
                };

                if (!lines[0].StartsWith("Before"))
                    yield break;

                yield return lines;
            }
        }

        private IEnumerable<(string Operation, int[] Before, int[] After)> GetSamples()
        {
            return GetChunks().Select(a => (a[1], Extract(a[0]), Extract(a[2])));

            int[] Extract(string line)
                => line.Split(new char[] { '[', ']', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(s => int.Parse(s)).ToArray();

        }

        public string Solve()
        {
            var cpu = new CPU();

            return GetSamples().Count(s => cpu.GetMatches(s.Operation, s.Before, s.After).Count >= 3).ToString();
        }

        public string SolveAdvanced()
        {
            var cpu = new CPU();
            var mapping = cpu.Operations.Select(_ => new HashSet<string>(cpu.Operations)).ToArray();

            foreach (var sample in GetSamples())
            {
                int index = int.Parse(sample.Operation.Split(' ')[0]);

                mapping[index].IntersectWith(cpu.GetMatches(sample.Operation, sample.Before, sample.After));
            }

            while (mapping.Any(s => s.Count > 1))
            {
                var singles = new HashSet<string>(mapping.Where(a => a.Count == 1).SelectMany(a => a));
                foreach (var set in mapping)
                {
                    if (set.Count > 1)
                    {
                        set.RemoveWhere(o => singles.Contains(o));
                    }
                }
            }

            cpu.SetRegisters(new int[] { 0, 0, 0, 0, });

            foreach (var instruction in GetProgram())
            {
                var items = instruction.Split(' ');
                var opCode = int.Parse(items[0]);

                int A = int.Parse(items[1]);
                int B = int.Parse(items[2]);
                int C = int.Parse(items[3]);

                cpu.Perform(mapping[opCode].Single(), A, B, C);
            }

            return cpu.GetRegister(0).ToString();

            IEnumerable<string> GetProgram() => GetInput().Skip(GetChunks().Count() * 4 + 2);
        }
    }
}