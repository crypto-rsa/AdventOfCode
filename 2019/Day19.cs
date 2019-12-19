using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2019
{
    public class Day19 : IAdventDay
    {
        public string Name => "19. 12. 2019";

        public string Solve() => GetBeamLocationCount().ToString();

        public string SolveAdvanced() => GetShipPosition().ToString();

        private static List<long> GetInput() => File.ReadAllText("2019/Resources/day19.txt").Split(',').Select(long.Parse).ToList();

        private int GetBeamLocationCount()
        {
            const int size = 50;
            var input = GetInput();

            return Enumerable.Range( 0, size * size ).Count( i => Run( input.ToList(), i % size, i / size ) == 1 );
        }

        private int GetShipPosition()
        {
            const int shipSize = 100;

            var input = GetInput();
            var bounds = new Queue<(int Y, int Start, int End)>();
            int y = 0;

            (int Y, int Start, int End) lastNonEmpty = (0, 0, 0);

            while (true)
            {
                int nextRow = FindNextNonEmptyRow(y);

                if (nextRow > y)
                {
                    bounds.Clear();
                }

                y = nextRow;

                bounds.Enqueue(GetBounds(y++));

                lastNonEmpty = bounds.Peek();

                while(bounds.Count > shipSize)
                {
                    bounds.Dequeue();
                }

                int position = GetPosition();

                if (position >= 0)
                    return position;
            }

            int FindNextNonEmptyRow( int nextRow )
            {
                if (nextRow == 0)
                    return 0;

                int y = nextRow;
                int x = lastNonEmpty.Start;

                while (true)
                {
                    if (Run(input.ToList(), x, y) == 1)
                        return y;

                    if (y == 0)
                    {
                        y = ++nextRow;
                        x = lastNonEmpty.Start;
                    }
                    else if (x <= lastNonEmpty.End)
                    {
                        x++;
                    }
                    else
                    {
                        y--;
                    }
                }
            }

            (int Y, int Start, int End) GetBounds(int y)
            {
                int start = 0;

                if (bounds.Count > 0)
                {
                    start = bounds.Last().Start;
                }

                while (Run(input.ToList(), start, y) == 0)
                {
                    start++;
                }

                int end = start;

                while (Run(input.ToList(), end, y) == 1)
                {
                    end++;
                }

                return (y, start, end - 1);
            }

            int GetPosition()
            {
                if (bounds.Peek().Start < 0)
                    return -1;

                if (bounds.Count == shipSize && bounds.Peek().End >= bounds.Last().Start + shipSize - 1)
                    return 10000 * bounds.Last().Start + bounds.Peek().Y;

                return -1;
            }
        }

        private int Run(List<long> program, int x, int y)
        {
            int position = 0;
            int relativeBase = 0;
            int step = 0;
            int lastOutput = -1;

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
                        Set(GetWriteIndex(0), step % 2 == 0 ? x : y);
                        position += 2;
                        step++;
                        break;

                    case 4:
                        lastOutput = (int)GetParameterValue(0);
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
                        return -2;
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
