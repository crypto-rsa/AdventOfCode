using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021;

public class Day24 : IAdventDay
{
    public string Name => "24. 12. 2021";

    private static string[] GetInput() => File.ReadAllLines("2021/Resources/day24.txt");

    private abstract class Operator
    {
        public Operator Left { get; set; }

        public Operator Right { get; set; }

        public abstract Operator Simplify();

        public abstract Range GetRange(Range left, Range right);

        public Range GetRange() => GetRange(Left?.GetRange() ?? default, Right?.GetRange() ?? default);
    }

    private readonly struct Range
    {
        public Range(long minimum, long maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public long Minimum { get; }

        public long Maximum { get; }

        public long? FixedValue => Minimum == Maximum ? Minimum : null;

        public static Range FromValue(long value) => new(value, value);

        public bool Contains(long value) => value >= Minimum && value <= Maximum;
    }

    private class Literal : Operator
    {
        private static readonly Dictionary<long, Literal> _literals = new();

        private Literal(long value)
        {
            Value = value;
        }

        public long Value { get; }

        public override Range GetRange(Range left, Range right) => Range.FromValue(Value);

        public override Operator Simplify() => this;

        public override string ToString() => Value.ToString();

        public static Literal FromValue(long value)
        {
            if (_literals.TryGetValue(value, out var literal))
                return literal;

            literal = new Literal(value);
            _literals[value] = literal;

            return literal;
        }
    }

    private class Input : Operator
    {
        public Input(int inputIndex)
        {
            InputIndex = inputIndex;
        }

        private int InputIndex { get; }

        public int? Value { get; set; }

        private static Range FullRange { get; } = new(1, 9);

        public override Range GetRange(Range left, Range right) => Value.HasValue ? Range.FromValue(Value.Value) : FullRange;

        public override Operator Simplify() => this;

        public override string ToString() => $"i{InputIndex + 1}";
    }

    private class Eql : Operator
    {
        private static Range FullRange { get; } = new(0, 1);

        public override Range GetRange(Range left, Range right)
        {
            if (left.FixedValue.HasValue && right.FixedValue.HasValue && left.FixedValue == right.FixedValue)
                return Range.FromValue(1);

            if (left.Maximum < right.Minimum || right.Maximum < left.Minimum)
                return Range.FromValue(0);

            return FullRange;
        }

        public override Operator Simplify() => GetRange() is { FixedValue: { } value } ? Literal.FromValue(value) : this;

        public override string ToString() => $"({Left} == {Right})";
    }

    private class Mod : Operator
    {
        public override Range GetRange(Range left, Range right)
        {
            if (!right.FixedValue.HasValue)
                return new Range(0, right.Maximum - 1);

            long mod = right.FixedValue.Value;

            if (left.FixedValue.HasValue)
                return Range.FromValue(left.FixedValue.Value % mod);

            long start = left.Minimum % mod;
            long end = start + (left.Maximum - left.Minimum - 1);

            return end >= mod ? new Range(0, mod - 1) : new Range(start, end % mod);
        }

        public override Operator Simplify()
        {
            if (Right is Literal mod && Left.GetRange() is { Minimum: >= 0 } r && r.Maximum <= mod.Value)
                return Left;

            if (Left is Mul { Right: Literal l1 } && Right is Literal l2 && l1.Value == l2.Value)
                return Literal.FromValue(0);

            if (GetRange() is { FixedValue: { } value })
                return Literal.FromValue(value);

            return this;
        }

        public override string ToString() => $"({Left} % {Right})";
    }

    private class Div : Operator
    {
        public override Range GetRange(Range left, Range right) => new(left.Minimum / Math.Max(right.Maximum, 1), left.Maximum / Math.Max(right.Minimum, 1));

        public override Operator Simplify()
        {
            if (Right is Literal div2 && Left is Div { Right: Literal div1 })
                return new Div
                {
                    Left = Left.Left,
                    Right = Literal.FromValue(div1.Value * div2.Value),
                };

            if (Left is Mul { Right: Literal l1 } && Right is Literal l2 && l1.Value == l2.Value)
                return Left.Left;

            var left = Left.GetRange();
            var right = Right.GetRange();

            if (left.FixedValue is { } a && right.FixedValue is { } b)
                return Literal.FromValue(a / b);

            if (left.FixedValue is 0)
                return Literal.FromValue(0);

            if (right.FixedValue is 1)
                return Left;

            return this;
        }

        public override string ToString() => $"({Left} / {Right})";
    }

    private class Mul : Operator
    {
        public override Range GetRange(Range left, Range right) => new(left.Minimum * right.Minimum, left.Maximum * right.Maximum);

        public override Operator Simplify()
        {
            if (Right is Literal div2 && Left is Mul { Right: Literal div1 })
                return new Div
                {
                    Left = Left.Left,
                    Right = Literal.FromValue(div1.Value * div2.Value),
                };

            var left = Left.GetRange();
            var right = Right.GetRange();

            if (left.FixedValue is { } a && right.FixedValue is { } b)
                return Literal.FromValue(a * b);

            if (left.FixedValue is 0 || right.FixedValue is 0)
                return Literal.FromValue(0);

            if (left.FixedValue is 1)
                return Right;

            if (right.FixedValue is 1)
                return Left;

            return this;
        }

        public override string ToString() => $"({Left} * {Right})";
    }

    private class Add : Operator
    {
        public override Range GetRange(Range left, Range right) => new(left.Minimum + right.Minimum, left.Maximum + right.Maximum);

        public override Operator Simplify()
        {
            if (Right is Literal div2 && Left is Add { Right: Literal div1 })
                return new Div
                {
                    Left = Left.Left,
                    Right = Literal.FromValue(div1.Value + div2.Value),
                };

            var left = Left.GetRange();
            var right = Right.GetRange();

            if (left.FixedValue is { } a && right.FixedValue is { } b)
                return Literal.FromValue(a + b);

            if (left.FixedValue is 0)
                return Right;

            if (right.FixedValue is 0)
                return Left;

            return this;
        }

        public override string ToString() => $"({Left} + {Right})";
    }

    private class Program
    {
        private readonly Dictionary<string, Operator> _operators = new()
        {
            ["w"] = Literal.FromValue(0),
            ["x"] = Literal.FromValue(0),
            ["y"] = Literal.FromValue(0),
            ["z"] = Literal.FromValue(0),
        };

        private List<Operator>[] _dependentOperators;

        private readonly Dictionary<Operator, Range> _rangeCache = new();

        private readonly List<Input> _inputOperators = new();

        private int _inputIndex;

        private Operator GetTree(string input) => _operators.TryGetValue(input, out var tree) ? tree : Literal.FromValue(long.Parse(input));

        public string GetResult(int[] valuesToTry)
        {
            foreach (string instruction in GetInput())
            {
                var parts = instruction.Split(' ');

                var op = Create(parts).Simplify();

                _operators[parts[1]] = op;
            }

            var result = _operators["z"];

            FindDependentOperators(result);

            return TryAssignValue(result, 0, valuesToTry) ? string.Concat(_inputOperators.Select(o => o.Value)) : "No solution found";

            Operator Create(string[] input)
            {
                if (input[0] == "inp")
                {
                    var inputOperator = new Input(_inputIndex++);

                    _inputOperators.Add(inputOperator);

                    return inputOperator;
                }

                Operator op = input[0] switch
                {
                    "add" => new Add(),
                    "mul" => new Mul(),
                    "div" => new Div(),
                    "mod" => new Mod(),
                    "eql" => new Eql(),
                    _ => throw new ArgumentException(nameof( input )),
                };

                op.Left = GetTree(input[1]);
                op.Right = GetTree(input[2]);

                return op;
            }
        }

        private bool TryAssignValue(Operator result, int inputIndex, int[] valuesToTry)
        {
            if (inputIndex >= _inputOperators.Count)
                return result.GetRange().FixedValue is 0;

            foreach (var value in valuesToTry)
            {
                _inputOperators[inputIndex].Value = value;

                var range = GetRange(result);

                foreach (var op in _dependentOperators[inputIndex])
                {
                    _rangeCache.Remove(op);
                }

                if (!range.Contains(0))
                    continue;

                if (TryAssignValue(result, inputIndex + 1, valuesToTry))
                    return true;
            }

            _inputOperators[inputIndex].Value = null;

            return false;
        }

        private void FindDependentOperators(Operator result)
        {
            _dependentOperators = Enumerable.Range(0, _inputOperators.Count).Select(_ => new List<Operator>()).ToArray();
            var inputStrings = _inputOperators.Select(o => o.ToString()).ToArray();

            var queue = new Queue<Operator>();
            queue.Enqueue(result);

            while (queue.Any())
            {
                var op = queue.Dequeue();

                if (op == null)
                    continue;

                var expression = op.ToString();

                for (var index = 0; index < inputStrings.Length; index++)
                {
                    if (expression!.Contains(inputStrings[index]))
                    {
                        _dependentOperators[index].Add(op);
                    }
                }

                queue.Enqueue(op.Left);
                queue.Enqueue(op.Right);
            }
        }

        private Range GetRange(Operator op)
        {
            if (op == null)
                return default;

            if (_rangeCache.TryGetValue(op, out var range))
                return range;

            range = op.GetRange(GetRange(op.Left), GetRange(op.Right));
            _rangeCache[op] = range;

            return range;
        }
    }

    public string Solve() => new Program().GetResult(new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 });

    public string SolveAdvanced() => new Program().GetResult(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
}
