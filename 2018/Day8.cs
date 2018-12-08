using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2018
{
    public class Day8 : IAdventDay
    {
        private class Node
        {
            public char Name { get; set; }

            public int[] MetaData { get; set; }

            public Node[] ChildNodes { get; set; }

            public int Value { get; private set; }

            public void UpdateValue()
            {
                if (!ChildNodes.Any())
                    Value = MetaData.Sum();
                else
                    Value = MetaData.Where(i => i >= 1 && i <= ChildNodes.Length).Sum(i => ChildNodes[i - 1].Value);
            }

        }
        public string Name => "8. 12. 2018";

        public int[] GetInput() => System.IO.File.ReadAllText(@"2018/Resources/day8.txt").Split(' ').Select(s => int.Parse(s)).ToArray();
        // public int[] GetInput() => "2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2".Split(' ').Select(s => int.Parse(s)).ToArray();

        public string Solve()
        {
            int metaDataSum = 0;
            var nodes = new Stack<Node>();
            nodes.Push(GetRoot());

            while (nodes.Count > 0)
            {
                var node = nodes.Pop();
                metaDataSum += node.MetaData.Sum();

                foreach (var childNode in node.ChildNodes)
                {
                    nodes.Push(childNode);
                }
            }

            return metaDataSum.ToString();
        }

        private Node GetRoot()
        {
            char nextName = 'A';
            var nodes = new Stack<Node>();
            var rootNode = new Node() { Name = nextName++ };
            nodes.Push(rootNode);

            var inputStack = new Stack<int>(GetInput().Reverse());
            while (nodes.Count > 0)
            {
                var node = nodes.Peek();
                if (node.ChildNodes == null)
                {
                    node.ChildNodes = new Node[(inputStack.Pop())];
                    node.MetaData = new int[inputStack.Pop()];
                    for (int i = 0; i < node.ChildNodes.Length; i++)
                    {
                        node.ChildNodes[i] = new Node() { Name = nextName++ };
                    }

                    foreach (var childNode in node.ChildNodes.Reverse<Node>())
                    {
                        nodes.Push(childNode);
                    }

                    if (node.ChildNodes.Length > 0)
                        continue;
                }

                for (int i = 0; i < node.MetaData.Length; i++)
                {
                    node.MetaData[i] = inputStack.Pop();
                }

                node.UpdateValue();

                nodes.Pop();
            }

            return rootNode;
        }

        public string SolveAdvanced()
        {
            return GetRoot().Value.ToString();
        }
    }
}