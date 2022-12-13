using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day13 : IAdventDay
{
    public string Name => "13. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day13.txt");

    private class Node : IComparable<Node>
    {
        private static Node FromValue(int value) => new() { Value = value };

        private static Node FromList(params int[] values) => new() { Nodes = values.Select(FromValue).ToList() };

        private List<Node> Nodes { get; init; }

        private int? Value { get; init; }

        public int CompareTo(Node other)
        {
            if (Value != null && other.Value != null)
                return Value.Value.CompareTo(other.Value.Value);

            if (Value != null && other.Nodes != null)
                return FromList(Value.Value).CompareTo(other);

            if (Nodes != null && other.Value != null)
                return CompareTo(FromList(other.Value.Value));

            for (int i = 0; i < Math.Max(Nodes!.Count, other.Nodes!.Count); i++)
            {
                if (i >= Nodes.Count)
                    return -1;

                if (i >= other.Nodes.Count)
                    return +1;

                var comparison = Nodes[i].CompareTo(other.Nodes[i]);

                if (comparison != 0)
                    return comparison;
            }

            return 0;
        }

        public static Node FromString(ReadOnlySpan<char> input, out int length)
        {
            int pos = 0;
            var node = new Node { Nodes = new List<Node>() };

            while (pos < input.Length)
            {
                switch (input[pos])
                {
                    case ',':
                        pos++;

                        continue;

                    case '[':
                        var subNode = FromString(input[(pos + 1)..], out int subLength);

                        node.Nodes.Add(subNode);

                        pos += subLength + 1;

                        break;

                    case ']':
                        length = pos + 1;

                        return node;

                    default:
                        int end = pos;

                        while (char.IsDigit(input[end]))
                            end++;

                        node.Nodes.Add(FromValue(int.Parse(input[pos..end])));

                        pos = end;

                        break;
                }
            }

            length = input.Length;

            return node;
        }
    }

    public string Solve()
    {
        return GetInput()
            .ParseByBlankLines()
            .Select((lines, i) => (Number: i + 1, InOrder: AreInRightOrder(lines.ToArray())))
            .Where(i => i.InOrder)
            .Sum(i => i.Number)
            .ToString();

        bool AreInRightOrder(string[] lines)
        {
            var node1 = Node.FromString(lines[0], out _);
            var node2 = Node.FromString(lines[1], out _);

            return node1.CompareTo(node2) < 0;
        }
    }

    public string SolveAdvanced()
    {
        var divider1 = Node.FromString("[[2]]", out _);
        var divider2 = Node.FromString("[[6]]", out _);

        var nodes = GetInput()
            .SplitToLines()
            .Where(s => s != string.Empty)
            .Select(s => Node.FromString(s, out _))
            .Append(divider1)
            .Append(divider2)
            .OrderBy(n => n)
            .ToList();

        return ((nodes.IndexOf(divider1) + 1) * (nodes.IndexOf(divider2) + 1)).ToString();
    }
}
