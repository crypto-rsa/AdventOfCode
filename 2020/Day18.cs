using System;
using System.Linq;

namespace Advent_of_Code.Year2020
{
    public class Day18 : IAdventDay
    {
        private interface IExpression
        {
            long Evaluate();
        }

        private class Number : IExpression
        {
            public Number(string input)
            {
                _value = long.Parse(input);
            }

            private readonly long _value;

            public long Evaluate() => _value;
        }

        private class Operation : IExpression
        {
            private readonly IExpression _left;

            private readonly IExpression _right;

            private Func<long, long, long> _operation;

            public Operation(IExpression left, IExpression right, Func<long, long, long> operation)
            {
                _left = left;
                _right = right;
                _operation = operation;
            }

            public long Evaluate() => _operation(_left.Evaluate(), _right.Evaluate());
        }

        public string Name => "18. 12. 2020";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2020/Resources/day18.txt");

        public string Solve() => GetInput().Sum(s => Parse(s.Replace(" ", string.Empty), false).Evaluate()).ToString();

        public string SolveAdvanced() => GetInput().Sum(s => Parse(s.Replace(" ", string.Empty), true).Evaluate()).ToString();

        private IExpression Parse(string input, bool multiplyLast)
        {
            if (multiplyLast)
            {
                int level = 0;
                int multiplicationPos = 0;

                while (multiplicationPos < input.Length && (input[multiplicationPos] != '*' || level != 0))
                {
                    if (input[multiplicationPos] == '(')
                        level++;

                    if (input[multiplicationPos] == ')')
                        level--;

                    multiplicationPos++;
                }

                if (multiplicationPos < input.Length)
                    return new Operation(Parse(input[0..multiplicationPos], multiplyLast), Parse(input[(multiplicationPos + 1)..], multiplyLast), (l1, l2) => l1 * l2);
            }

            int rightExpressionStart = input.Length - 1;
            IExpression rightExpression = null;

            if( input[^1] == ')')
            {
                int counter = 1;
                
                while(rightExpressionStart > 0 && counter > 0)
                {
                    rightExpressionStart--;

                    if (input[rightExpressionStart] == ')')
                        counter++;

                    if (input[rightExpressionStart] == '(')
                        counter--;
                }

                rightExpression = Parse(input[(rightExpressionStart + 1)..(input.Length - 1)], multiplyLast);
            }
            else
            {
                while (rightExpressionStart > 0 && char.IsDigit(input[rightExpressionStart - 1]))
                    rightExpressionStart--;

                rightExpression = new Number(input[rightExpressionStart..]);
            }

            if (rightExpressionStart == 0)
                return rightExpression;

            Func<long, long, long> operation = input[rightExpressionStart - 1] switch
            {
                '+' => (l1, l2) => l1 + l2,
                '*' => (l1, l2) => l1 * l2,
                _ => throw new InvalidOperationException(),
            };

            return new Operation(Parse(input[0..(rightExpressionStart - 1)], multiplyLast), rightExpression, operation);
        }
    }
}
