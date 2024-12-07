using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2024;

public class Day7 : IAdventDay
{
    public string Name => "7. 12. 2024";

    private static string GetInput() => File.ReadAllText("2024/Resources/day7.txt");

    private record Equation(long Result, List<long> Operands)
    {
        public static Equation Parse(string line)
        {
            var parts = line.Split(": ");
            var result = long.Parse(parts[0]);
            var operands = parts[1].Split(" ").Select(long.Parse).ToList();

            return new Equation(result, operands);
        }

        public bool IsFeasible(bool allowConcatenation)
        {
            var stack = new Stack<(long Result, int Index, char Operator)>();

            stack.Push((Operands[0], 1, '+'));
            stack.Push((Operands[0], 1, '*'));

            if (allowConcatenation)
            {
                stack.Push((Operands[0], 1, '|'));
            }

            while (stack.Count > 0)
            {
                (long currentResult, int index, char currentOperator) = stack.Pop();

                if (index == Operands.Count)
                {
                    if (currentResult == Result)
                        return true;
                }
                else
                {
                    var operand = Operands[index];

                    var nextResult = currentOperator switch
                    {
                        '+' => currentResult + operand,
                        '*' => currentResult * operand,
                        '|' => Concatenate(currentResult, operand),
                        _ => throw new InvalidDataException()
                    };

                    stack.Push((nextResult, index + 1, '+'));
                    stack.Push((nextResult, index + 1, '*'));

                    if (allowConcatenation)
                    {
                        stack.Push((nextResult, index + 1, '|'));
                    }
                }
            }

            return false;

            long Concatenate(long a, long b)
            {
                long c = b;

                while (c > 0)
                {
                    a *= 10;
                    c /= 10;
                }

                return a + b;
            }
        }
    }

    public string Solve() => GetInput()
        .SplitToLines()
        .Select(Equation.Parse)
        .Where(equation => equation.IsFeasible(allowConcatenation: false))
        .Sum(e => e.Result)
        .ToString();

    public string SolveAdvanced() => GetInput()
        .SplitToLines()
        .Select(Equation.Parse)
        .Where(equation => equation.IsFeasible(allowConcatenation: true))
        .Sum(e => e.Result)
        .ToString();
}
