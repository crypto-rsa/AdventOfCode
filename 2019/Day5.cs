using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2019
{
    public class Day5 : IAdventDay
    {
        public string Name => "5. 12. 2019";

        public string Solve() => Run(GetInput(), 1)?.ToString() ?? "No output";

        public string SolveAdvanced() => Run(GetInput(), 5)?.ToString() ?? "No output";

        private static int[] GetInput() => File.ReadAllText("2019/Resources/day5.txt").Split(',').Select(int.Parse).ToArray();

        private int? Run(int[] input, int systemID)
        {
            int position = 0;
            int? lastOutput = null;

            while (true)
            {
                switch (input[position] % 100)
                {
                    case 1:
                        input[input[position + 3]] = GetParameterValue(0) + GetParameterValue(1);
                        position += 4;
                        break;

                    case 2:
                        input[input[position + 3]] = GetParameterValue(0) * GetParameterValue(1);
                        position += 4;
                        break;

                    case 3:
                        input[input[position + 1]] = systemID;
                        position += 2;
                        break;

                    case 4:
                        lastOutput = GetParameterValue(0);
                        position += 2;
                        break;

                    case 5:
                        if (GetParameterValue(0) != 0)
                        {
                            position = GetParameterValue(1);
                        }
                        else
                        {
                            position += 3;
                        }
                        break;

                    case 6:
                        if (GetParameterValue(0) == 0)
                        {
                            position = GetParameterValue(1);
                        }
                        else
                        {
                            position += 3;
                        }
                        break;

                    case 7:
                        input[input[position + 3]] = GetParameterValue(0) < GetParameterValue(1) ? 1 : 0;
                        position += 4;
                        break;

                    case 8:
                        input[input[position + 3]] = GetParameterValue(0) == GetParameterValue(1) ? 1 : 0;
                        position += 4;
                        break;

                    case 99:
                        return lastOutput;

                    default:
                        return null;
                }
            }

            int GetParameterValue(int index)
            {
                int opCode = input[position] / 100;

                for (int i = 0; i < index; i++)
                {
                    opCode /= 10;
                }

                return opCode % 10 == 0 ? input[input[position + index + 1]] : input[position + index + 1];
            }
        }
    }
}