using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2019
{
    public class Day17 : IAdventDay
    {
        public string Name => "17. 12. 2019";

        public string Solve() =>  GetAlignment().ToString();

        public string SolveAdvanced() => GetDustCount().ToString();

        private static List<long> GetInput() => File.ReadAllText("2019/Resources/day17.txt").Split(',').Select(long.Parse).ToList();

        private readonly List<List<char>> _lines = new List<List<char>>();

        private int GetAlignment()
        {
            Run(GetInput(), null);

            var intersectionChars = new char[] { '#', '^', '>', 'v', '<' };
            var offsets = new (int X, int Y)[]
            {
                (0, 0),
                (0, -1),
                (+1, 0),
                (0, +1),
                (-1, 0),
            };

            return GetAllPositions().Where(IsIntersection).Sum(item => item.X * item.Y);

            bool IsIntersection((int X, int Y) position)
            {
                return offsets.All(IsValid);

                bool IsValid((int X, int Y) offset)
                {
                    var fullPosition = (X: position.X + offset.X, Y: position.Y + offset.Y);

                    return IsValidPosition(fullPosition) && intersectionChars.Contains(_lines[fullPosition.Y][fullPosition.X]);
                }
            }
        }

        private bool IsValidPosition((int X, int Y) position)
            => position.Y >= 0 && position.Y < _lines.Count && position.X >= 0 && position.X < _lines[position.Y].Count;

        private IEnumerable<(int X, int Y)> GetAllPositions()
        {
            for (int y = 0; y < _lines.Count; y++)
            {
                for (int x = 0; x < _lines[y].Count; x++)
                {
                    yield return (x, y);
                }
            }
        }

        private int GetDustCount()
        {
            var program = GetInput();
            program[0] = 2;

            var instructions = Split(GetFullPath());

            return Run(program, instructions);
        }

        private string GetFullPath()
        {
            var robotChars = new char[] { '^', '>', 'v', '<' };

            var robotPos = GetAllPositions().FirstOrDefault(p => robotChars.Contains(_lines[p.Y][p.X]));
            char dir = _lines[robotPos.Y][robotPos.X];

            var stringBuilder = new System.Text.StringBuilder();

            while (true)
            {
                var nextOffset = GetOffsets().FirstOrDefault(o => IsScaffolding(o.Value) && NextDir(o.Value).Code != 'D');

                if (!nextOffset.HasValue)
                    break;

                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(',');
                }

                var nextDir = NextDir(nextOffset.Value);
                dir = nextDir.Robot;
                stringBuilder.Append(nextDir.Code).Append('-');

                int steps = 0;
                while (IsScaffolding(nextOffset.Value))
                {
                    Step(nextOffset.Value);
                    steps++;
                }

                stringBuilder.Append(steps);
            }

            return stringBuilder.ToString();

            bool IsScaffolding((int X, int Y) offset)
            {
                var fullPosition = (X: robotPos.X + offset.X, Y: robotPos.Y + offset.Y);

                return IsValidPosition(fullPosition) && _lines[fullPosition.Y][fullPosition.X] == '#';
            }

            static IEnumerable<(int X, int Y)?> GetOffsets()
            {
                yield return (0, -1);
                yield return (+1, 0);
                yield return (0, +1);
                yield return (-1, 0);
            }

            (char Code, char Robot) NextDir((int X, int Y) offset) => (dir, offset.X, offset.Y) switch
            {
                ('^', +1, 0) => ('R', '>'),
                ('^', -1, 0) => ('L', '<'),
                ('>', 0, +1) => ('R', 'v'),
                ('>', 0, -1) => ('L', '^'),
                ('v', +1, 0) => ('L', '>'),
                ('v', -1, 0) => ('R', '<'),
                ('<', 0, +1) => ('L', 'v'),
                ('<', 0, -1) => ('R', '^'),
                _ => ('D', 'D'),
            };

            void Step((int X, int Y) offset) => robotPos = (robotPos.X + offset.X, robotPos.Y + offset.Y);
        }

        private List<int> Split(string input)
        {
            Console.WriteLine(input);
            Console.WriteLine();

            var items = Regex.Split(input, ",");

            var allCandidates = new HashSet<string>();

            for (int start = 0; start < items.Length - 1; start++)
            {
                for (int length = 1; start + length < items.Length; length++)
                {
                    string candidate = string.Join(',', items.Skip(start).Take(length));

                    if (candidate.Length <= 20)
                        allCandidates.Add(candidate);
                }
            }

            var candidates = new string[3];

            foreach (var A in allCandidates.Where(input.StartsWith))
            {
                candidates[0] = A;
                foreach (var B in allCandidates)
                {
                    candidates[1] = B;
                    foreach (var C in allCandidates)
                    {
                        candidates[2] = C;

                        var list = Match(input, candidates, 0);

                        if (list != null)
                        {
                            var allInputs = new List<int>();

                            allInputs.AddRange(list.Select(c => (int) c).Reverse());
                            allInputs.Add(10);
                            allInputs.AddRange(GetCode(A));
                            allInputs.Add(10);
                            allInputs.AddRange(GetCode(B));
                            allInputs.Add(10);
                            allInputs.AddRange(GetCode(C));
                            allInputs.Add(10);
                            allInputs.Add('n');
                            allInputs.Add(10);

                            Console.WriteLine($"A: {A}");
                            Console.WriteLine($"B: {B}");
                            Console.WriteLine($"C: {C}");
                            Console.WriteLine();

                            return allInputs;
                        }
                    }
                }
            }

            return null;

            static IEnumerable<int> GetCode(string s) => s.Replace('-', ',').Select(c => (int) c);
        }

        private List<char> Match(string input, string[] candidates, int start)
        {
            var span = input[start] == ',' ? input.AsSpan(start + 1) : input.AsSpan(start);

            for (int i = 0; i < candidates.Length; i++)
            {
                if (span.SequenceEqual(candidates[i].AsSpan()))
                {
                    return new List<char>() { (char) ('A' + i) };
                }

                if( span.StartsWith(candidates[i].AsSpan()))
                {
                    var list = Match(input, candidates, start + candidates[i].Length + 1);

                    if (list != null)
                    {
                        list.Add(',');
                        list.Add((char) ('A' + i));
                        return list;
                    }
                }
            }

            return null;
        }

        private int Run(List<long> program, List<int> instructions)
        {
            int position = 0;
            int relativeBase = 0;
            int ip = 0;
            int lastOutput = -1;

            _lines.Add(new List<char>());

            while (true)
            {
                switch (Get(position) % 100)
                {
                    case 1:
                        Set(GetWriteIndex(2), GetParameterValue(0) + GetParameterValue(1));
                        position += 4;
                        break;

                    case 2:
                        Set(GetWriteIndex(2), GetParameterValue(0) * GetParameterValue(1));
                        position += 4;
                        break;

                    case 3:
                        if (ip < instructions.Count)
                        {
                            if (instructions[ip] == 10)
                            {
                                Console.WriteLine();
                            }
                            else
                            {
                                Console.Write(instructions[ip]);
                            }
                        }

                        Set(GetWriteIndex(0), instructions[ip++]);
                        position += 2;
                        break;

                    case 4:
                        var output = GetParameterValue(0);
                        lastOutput = (int) output;
                        switch (output)
                        {
                            case 10:
                                _lines.Add(new List<char>());
                                Console.WriteLine();
                                break;

                            default:
                                _lines.Last().Add((char)output);
                                Console.Write((char)output);
                                break;
                        }

                        position += 2;
                        break;

                    case 5:
                        if (GetParameterValue(0) != 0)
                        {
                            position = (int)GetParameterValue(1);
                        }
                        else
                        {
                            position += 3;
                        }
                        break;

                    case 6:
                        if (GetParameterValue(0) == 0)
                        {
                            position = (int)GetParameterValue(1);
                        }
                        else
                        {
                            position += 3;
                        }
                        break;

                    case 7:
                        Set(GetWriteIndex(2), GetParameterValue(0) < GetParameterValue(1) ? 1 : 0);
                        position += 4;
                        break;

                    case 8:
                        Set(GetWriteIndex(2), GetParameterValue(0) == GetParameterValue(1) ? 1 : 0);
                        position += 4;
                        break;

                    case 9:
                        relativeBase += (int)GetParameterValue(0);
                        position += 2;
                        break;

                    case 99:
                        return lastOutput;

                    default:
                        return -1;
                }
            }

            long GetParameterValue(int index)
            {
                long opCode = Get(position) / 100;

                for (long i = 0; i < index; i++)
                {
                    opCode /= 10;
                }

                return (opCode % 10) switch
                {
                    0 => Get((int)Get(position + index + 1)),
                    1 => Get(position + index + 1),
                    2 => Get(relativeBase + (int)Get(position + index + 1)),
                    _ => throw new System.InvalidOperationException(),
                };
            }

            int GetWriteIndex(int index)
            {
                long opCode = Get(position) / 100;

                for (long i = 0; i < index; i++)
                {
                    opCode /= 10;
                }

                return (opCode % 10) switch
                {
                    0 => (int)Get(position + index + 1),
                    2 => relativeBase + (int)Get(position + index + 1),
                    _ => throw new System.InvalidOperationException(),
                };
            }

            long Get(int index) => index < program.Count ? program[index] : 0;

            void Set(int index, long value)
            {
                if (index >= program.Count)
                {
                    program.AddRange(Enumerable.Repeat(0L, index - program.Count + 1));
                }

                program[index] = value;
            }
        }
    }
}
