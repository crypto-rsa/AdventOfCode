using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2019
{
    public class Day7 : IAdventDay
    {
        public string Name => "7. 12. 2019";

        public string Solve() => GetMaxSignal(0).ToString();

        public string SolveAdvanced() => GetMaxSignal(5).ToString();

        private static int[] GetInput() => File.ReadAllText("2019/Resources/day7.txt").Split(',').Select(int.Parse).ToArray();

        private int GetMaxSignal(int minId)
        {
            var program = GetInput();
            const int factorial = 5 * 4 * 3 * 2 * 1;
            var numbers = Enumerable.Range(minId, 5).ToArray();

            return Enumerable.Range(0, factorial).Max(i => TryPermutation(program, i, numbers));
        }

        private int TryPermutation(int[] program, int permutation, int[] numbers)
        {
            const int amplifierCount = 5;
            var inputs = Enumerable.Range(0, amplifierCount).Select(_ => new Queue<int>()).ToArray();

            ParsePermutation();

            inputs[0].Enqueue(0);

            var output = Run(program, inputs);

            if (!output.HasValue)
                throw new InvalidOperationException();

            return output.Value;

            void ParsePermutation()
            {
                var factorials = new int[] { 24, 6, 2, 1, 1 };
                var numberList = numbers.ToList();

                int n = permutation;
                for (int i = 0; i < amplifierCount; i++)
                {
                    int k = n / factorials[i];
                    inputs[i].Enqueue(numberList[k]);
                    numberList.RemoveAt(k);
                    n %= factorials[i];
                }
            }
        }

        private int? Run(int[] programTemplate, Queue<int>[] inputs)
        {
            var programs = Enumerable.Range(0, inputs.Length).Select(_ => programTemplate.ToArray()).ToArray();
            var positions = new int[inputs.Length];
            int? lastOutput = null;

            while (true)
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    int position = positions[i];
                    var program = programs[i];

                    if (program[position] == 3 && inputs[i].Count == 0)
                        continue;

                    switch (program[position] % 100)
                    {
                        case 1:
                            program[program[position + 3]] = GetParameterValue(program, 0, position) + GetParameterValue(program, 1, position);
                            position += 4;
                            break;

                        case 2:
                            program[program[position + 3]] = GetParameterValue(program, 0, position) * GetParameterValue(program, 1, position);
                            position += 4;
                            break;

                        case 3:
                            program[program[position + 1]] = inputs[i].Dequeue();
                            position += 2;
                            break;

                        case 4:
                            lastOutput = GetParameterValue(program, 0, position);
                            inputs[(i + 1) % inputs.Length].Enqueue(lastOutput.Value);
                            position += 2;
                            break;

                        case 5:
                            if (GetParameterValue(program, 0, position) != 0)
                            {
                                position = GetParameterValue(program, 1, position);
                            }
                            else
                            {
                                position += 3;
                            }
                            break;

                        case 6:
                            if (GetParameterValue(program, 0, position) == 0)
                            {
                                position = GetParameterValue(program, 1, position);
                            }
                            else
                            {
                                position += 3;
                            }
                            break;

                        case 7:
                            program[program[position + 3]] = GetParameterValue(program, 0, position) < GetParameterValue(program, 1, position) ? 1 : 0;
                            position += 4;
                            break;

                        case 8:
                            program[program[position + 3]] = GetParameterValue(program, 0, position) == GetParameterValue(program, 1, position) ? 1 : 0;
                            position += 4;
                            break;

                        case 99:
                            if (i == inputs.Length - 1)
                                return lastOutput;
                            break;

                        default:
                            return null;
                    }

                    positions[i] = position;
                }
            }

            static int GetParameterValue(int[] program, int index, int position)
            {
                int opCode = program[position] / 100;

                for (int i = 0; i < index; i++)
                {
                    opCode /= 10;
                }

                return opCode % 10 == 0 ? program[program[position + index + 1]] : program[position + index + 1];
            }
        }
    }
}
