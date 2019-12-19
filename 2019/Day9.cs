using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2019
{
    public class Day9 : IAdventDay
    {
        public string Name => "9. 12. 2019";

        public string Solve() => Run(GetInput(), 1).ToString();

        public string SolveAdvanced() => Run(GetInput(), 2).ToString();

        private static List<long> GetInput() => File.ReadAllText("2019/Resources/day9.txt").Split(',').Select(long.Parse).ToList();

        private long? Run(List<long> program, long systemID)
        {
            int position = 0;
            long? lastOutput = null;
            int relativeBase = 0;

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
                        Set(GetWriteIndex(0), systemID);
                        position += 2;
                        break;

                    case 4:
                        lastOutput = GetParameterValue(0);
                        System.Console.WriteLine(lastOutput);
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
                        relativeBase += (int) GetParameterValue(0);
                        position += 2;
                        break;

                    case 99:
                        return lastOutput;

                    default:
                        return null;
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
