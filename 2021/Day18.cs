using System;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021;

public class Day18 : IAdventDay
{
    public string Name => "18. 12. 2021";

    private static string[] GetInput() => File.ReadAllLines("2021/Resources/day18.txt");

    private class Node
    {
        private Node(Node parent)
        {
            Parent = parent;
        }

        private Node Parent { get; set; }

        private Node Left { get; set; }

        private Node Right { get; set; }

        private int? Value { get; set; }

        private long Magnitude
        {
            get
            {
                if (Value.HasValue)
                    return Value.Value;

                return 3L * Left.Magnitude + 2L * Right.Magnitude;
            }
        }

        private static Node Create(string input)
        {
            Node current = new Node(null);

            foreach (char c in input)
            {
                switch (c)
                {
                    case '[':
                        current.Left = new Node(current);
                        current = current.Left;

                        break;

                    case ',':
                        current!.Parent.Right = new Node(current.Parent);
                        current = current.Parent.Right;

                        break;

                    case ']':
                        current = current.Parent;

                        break;

                    case >= '0' and <= '9':
                        current!.Value = int.Parse(c.ToString());

                        break;
                }
            }

            return current;
        }

        private Node AddLeftChild(Node child)
        {
            child.Parent = this;
            Left = child;
            Value = null;

            return this;
        }

        private Node AddRightNode(Node child)
        {
            child.Parent = this;
            Right = child;
            Value = null;

            return this;
        }

        private Node Add(Node other) => new Node(null).AddLeftChild(this).AddRightNode(other).Reduce();

        private Node Reduce()
        {
            bool needsReduction = true;

            while (needsReduction)
            {
                var nodeToExplode = GetNodeToExplode(this, 0);

                if (nodeToExplode != null)
                {
                    var leftAdditionNode = nodeToExplode;

                    while (leftAdditionNode.Parent != null && leftAdditionNode == leftAdditionNode.Parent.Left)
                    {
                        leftAdditionNode = leftAdditionNode.Parent;
                    }

                    if (leftAdditionNode.Parent != null)
                    {
                        leftAdditionNode = leftAdditionNode.Parent.Left;

                        while (leftAdditionNode.Right != null)
                        {
                            leftAdditionNode = leftAdditionNode.Right;
                        }

                        leftAdditionNode.Value += nodeToExplode.Left.Value;
                    }

                    var rightAdditionNode = nodeToExplode;

                    while (rightAdditionNode.Parent != null && rightAdditionNode == rightAdditionNode.Parent.Right)
                    {
                        rightAdditionNode = rightAdditionNode.Parent;
                    }

                    if (rightAdditionNode.Parent != null)
                    {
                        rightAdditionNode = rightAdditionNode.Parent.Right;

                        while (rightAdditionNode.Left != null)
                        {
                            rightAdditionNode = rightAdditionNode.Left;
                        }

                        rightAdditionNode.Value += nodeToExplode.Right.Value;
                    }

                    if (nodeToExplode == nodeToExplode.Parent.Left)
                    {
                        nodeToExplode.Parent.Left = new Node(nodeToExplode.Parent) { Value = 0 };
                    }
                    else
                    {
                        nodeToExplode.Parent.Right = new Node(nodeToExplode.Parent) { Value = 0 };
                    }

                    continue;
                }

                var nodeToSplit = GetNodeToSplit(this);

                if (nodeToSplit != null)
                {
                    int value = nodeToSplit.Value!.Value;

                    nodeToSplit
                        .AddLeftChild(new Node(nodeToSplit) { Value = (int)Math.Floor(value / 2.0) })
                        .AddRightNode(new Node(nodeToSplit) { Value = (int)Math.Ceiling(value / 2.0) });

                    continue;
                }

                needsReduction = false;
            }

            return this;
        }

        private static Node GetNodeToExplode(Node parent, int depth)
        {
            if (parent == null)
                return null;

            if (depth == 4)
            {
                if (parent.Value.HasValue)
                    return null;

                if (parent.Left.Value.HasValue && parent.Right.Value.HasValue)
                    return parent;
            }

            return GetNodeToExplode(parent.Left, depth + 1) ?? GetNodeToExplode(parent.Right, depth + 1);
        }

        private static Node GetNodeToSplit(Node parent)
        {
            if (parent == null)
                return null;

            if (parent.Value.GetValueOrDefault() >= 10)
                return parent;

            return GetNodeToSplit(parent.Left) ?? GetNodeToSplit(parent.Right);
        }

        public static long GetMagnitudeOfSum()
        {
            var numbers = GetInput().Select(Create).ToList();

            return numbers.Skip(1).Aggregate(numbers[0], (cur, next) => cur.Add(next), n => n.Magnitude);
        }

        public static long GetLargestSumMagnitude()
        {
            var input = GetInput();
            var indices = Enumerable.Range(0, input.Length).ToArray();

            return indices.Max(i => indices.Max(j => i != j ? Create(input[i]).Add(Create(input[j])).Magnitude : 0));
        }
    }

    public string Solve() => Node.GetMagnitudeOfSum().ToString();

    public string SolveAdvanced() => Node.GetLargestSumMagnitude().ToString();
}
