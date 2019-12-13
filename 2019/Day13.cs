using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2019
{
    public class Day13 : IAdventDay
    {
        public string Name => "13. 12. 2019";

        public string Solve() => GetBlockCount().ToString();

        public string SolveAdvanced() => GetFinalScore().ToString();

        private static List<long> GetInput() => File.ReadAllText("2019/Resources/day13.txt").Split(',').Select(long.Parse).ToList();

        private readonly Dictionary<(int X, int Y), int> _tiles = new Dictionary<(int X, int Y), int>();

        private int _blockCount;

        private int _score;

        private (int X, int Y) _ballPosition;

        private (int X, int Y) _paddlePosition;

        private int GetBlockCount()
        {
            Run(GetInput(), showProgress: false);

            return _blockCount;
        }

        private int GetFinalScore()
        {
            var program = GetInput();
            program[0] = 2;

            Run(program, showProgress: false);

            return _score;
        }

        private void Run(List<long> program, bool showProgress)
        {
            int position = 0;
            int relativeBase = 0;

            var output = new int[3];
            int outputIndex = 0;

            _blockCount = 0;

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
                        int input = (_ballPosition.X - _paddlePosition.X) switch
                        {
                            int i when i < 0 => -1,
                            int i when i > 0 => +1,
                            _ => 0,
                        };

                        if (showProgress)
                        {
                            Console.Clear();
                            Print();

                            System.Threading.Thread.Sleep(100);
                        }
                        else
                        {
                            PrintInfo();
                        }

                        Set(GetWriteIndex(0), input);
                        position += 2;
                        break;

                    case 4:
                        output[outputIndex++] = (int)GetParameterValue(0);
                        position += 2;

                        if (outputIndex == 3)
                        {
                            if (output[0] == -1 && output[1] == 0)
                            {
                                _score = output[2];
                            }
                            else
                            {
                                var key = (output[0], output[1]);
                                switch (output[2])
                                {
                                    case 0:
                                        if (_tiles.TryGetValue(key, out int prevValue) && prevValue == 2)
                                            _blockCount--;
                                        break;

                                    case 2: _blockCount++; break;
                                    case 3: _paddlePosition = key; break;
                                    case 4: _ballPosition = key; break;
                                }

                                _tiles[(output[0], output[1])] = output[2];
                            }

                            outputIndex = 0;
                        }
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
                        if (showProgress)
                        {
                            Print();
                        }
                        else
                        {
                            PrintInfo();
                        }
                        return;

                    default:
                        throw new InvalidOperationException();
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
                    _ => throw new InvalidOperationException(),
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
                    _ => throw new InvalidOperationException(),
                };
            }

            long Get(int index) => index <= program.Count ? program[index] : 0;

            void Set(int index, long value)
            {
                if (index >= program.Count)
                {
                    program.AddRange(Enumerable.Repeat(0L, index - program.Count + 1));
                }

                program[index] = value;
            }
        }

        private void Print()
        {
            var (minX, minY) = _tiles.Keys.Aggregate((X: int.MaxValue, Y: int.MaxValue), (cur, next) => (Math.Min(cur.X, next.X), Math.Min(cur.Y, next.Y)));
            var (maxX, maxY) = _tiles.Keys.Aggregate((X: int.MinValue, Y: int.MinValue), (cur, next) => (Math.Max(cur.X, next.X), Math.Max(cur.Y, next.Y)));

            PrintInfo();

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    char value = GetChar(_tiles[(x, y)]);

                    switch (value)
                    {
                        case '-': Console.ForegroundColor = ConsoleColor.Green; break;
                        case '*': Console.ForegroundColor = ConsoleColor.Red; break;
                        default: Console.ResetColor(); break;
                    }

                    Console.Write(value);
                }

                Console.WriteLine();
            }

            static char GetChar(int tile) => tile switch
            {
                1 => 'X',
                2 => '=',
                3 => '-',
                4 => '*',
                _ => ' ',
            };
        }

        private void PrintInfo()
        {
            Console.WriteLine($"Blocks remaining: {_blockCount}, paddle position: {ToString(_ballPosition)}, ball position: {ToString(_paddlePosition)}");

            static string ToString((int X, int Y) position) => $"[{position.X}, {position.Y}]";
        }
    }
}
