using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day21 : IAdventDay
{
    public string Name => "21. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day21.txt");

    private class TreeNode
    {
        public TreeNode(long value)
        {
            Value = new Fraction(value);
        }

        public TreeNode(string source)
        {
            Source = source;
        }

        public TreeNode(string @operator, TreeNode left, TreeNode right)
        {
            Operator = @operator;
            Left = left;
            Right = right;
        }

        private TreeNode Left { get; }

        private TreeNode Right { get; }

        private string Operator { get; }

        private Fraction Value { get; }

        private string Source { get; }

        public Fraction Evaluate()
        {
            if (Left == null || Right == null)
                return Value;

            Fraction left = Left.Evaluate();
            Fraction right = Right.Evaluate();

            return Operator switch
            {
                "+" => left + right,
                "-" => left - right,
                "*" => left * right,
                "/" => left / right,
                _ => throw new NotSupportedException(),
            };
        }

        (Fraction Linear, Fraction Constant) GetLinearCombination(string source)
        {
            if (Source == source)
                return (new Fraction(1), new Fraction(0));

            if (Left == null || Right == null)
                return (new Fraction(0), Value);

            var left = Left.GetLinearCombination(source);
            var right = Right.GetLinearCombination(source);

            return Operator switch
            {
                "+" => (left.Linear + right.Linear, left.Constant + right.Constant),
                "-" => (left.Linear - right.Linear, left.Constant - right.Constant),
                "*" => Multiply(),
                "/" => Divide(),
                _ => throw new NotSupportedException(),
            };

            (Fraction, Fraction) Multiply()
            {
                if (left.Linear.IsNonZero && right.Linear.IsNonZero)
                    throw new InvalidOperationException("Cannot solve other equations than linear!");

                return (left.Linear.IsZero ? right.Linear * left.Constant : left.Linear * right.Constant, left.Constant * right.Constant);
            }

            (Fraction, Fraction) Divide()
            {
                if (right.Linear.IsNonZero)
                    throw new InvalidOperationException("Cannot solve other equations than linear!");

                return (left.Linear / right.Constant, left.Constant / right.Constant);
            }
        }

        public Fraction FindSolution(string source)
        {
            var left = Left.GetLinearCombination(source);
            var right = Right.GetLinearCombination(source);

            return (right.Constant - left.Constant) / (left.Linear - right.Linear);
        }
    }

    private static TreeNode GetNode(string name, Dictionary<string, string> calculations, string you)
    {
        string formula = calculations[name];

        if (name == you)
            return new TreeNode(name);

        if (long.TryParse(formula, out long value))
            return new TreeNode(value);

        var parts = formula!.Split(' ');

        var left = GetNode(parts[0], calculations, you);
        var right = GetNode(parts[2], calculations, you);

        return new TreeNode(parts[1], left, right);
    }

    private static (string Key, string Value) Parse(string input)
    {
        var parts = input.Split(": ");

        return (parts[0], parts[1]);
    }

    public string Solve()
    {
        var calculations = GetInput()
            .SplitToLines()
            .Select(Parse)
            .ToDictionary(i => i.Key, i => i.Value);

        return GetNode("root", calculations, null).Evaluate().ToString();
    }

    public string SolveAdvanced()
    {
        string input = GetInput();

        var calculations = input
            .SplitToLines()
            .Select(Parse)
            .ToDictionary(i => i.Key, i => i.Value);

        calculations["root"] = calculations["root"].Replace("+", "=");

        return GetNode("root", calculations, "humn").FindSolution("humn").ToString();
    }
}
