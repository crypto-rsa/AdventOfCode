using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2023;

public class Day8 : IAdventDay
{
    public string Name => "8. 12. 2023";

    private static string GetInput() => File.ReadAllText("2023/Resources/day8.txt");

    private class Map
    {
        private string Instructions { get; }

        private Dictionary<string, (string Left, string Right)> Network { get; }

        private record State(string Node, int Instruction);

        private class Path
        {
            public string StartNode { get; }

            public int PrePeriod { get; }

            public int Period { get; }

            private Dictionary<State, int> Steps { get; } = new();

            public Path(string startNode, Map map)
            {
                StartNode = startNode;
                Steps[new State(StartNode, 0)] = 0;

                var current = StartNode;
                int steps = 0;

                while (true)
                {
                    current = map.GetNextNode(current, steps);
                    steps++;

                    var nextState = new State(current, steps % map.Instructions.Length);

                    if (Steps.TryGetValue(nextState, out int prevSteps))
                    {
                        PrePeriod = prevSteps;
                        Period = steps - prevSteps;

                        break;
                    }

                    Steps[nextState] = steps;
                }
            }
        }

        public Map(string input)
        {
            var parts = input.ParseByBlankLines().ToArray();
            Instructions = parts[0].Single();
            Network = new Dictionary<string, (string Left, string Right)>(parts[1].Select(ParseNode));

            KeyValuePair<string, (string, string)> ParseNode(string line)
            {
                var match = System.Text.RegularExpressions.Regex.Match(line, @"(\w+) = \((\w+), (\w+)\)");

                return new KeyValuePair<string, (string, string)>(match.Groups[1].Value, (match.Groups[2].Value, match.Groups[3].Value));
            }
        }

        private string GetNextNode(string currentNode, int steps)
            => Instructions[steps % Instructions.Length] == 'L' ? Network[currentNode].Left : Network[currentNode].Right;

        public int GetStepsToEnd()
        {
            int steps = 0;
            string current = "AAA";

            while (current != "ZZZ")
            {
                current = GetNextNode(current, steps);
                steps++;
            }

            return steps;
        }

        public long GetStepsToAllEnds()
        {
            var paths = Network
                .Keys
                .Where(s => s.EndsWith('A'))
                .Select(s => new Path(s, this))
                .ToArray();

            var maxPrePeriod = paths.Max(p => p.PrePeriod);
            var nodes = paths.Select(p => p.StartNode).ToArray();

            for (int i = 0; i < maxPrePeriod; i++)
            {
                if (nodes.All(s => s.EndsWith('Z')))
                    return i;

                for (int j = 0; j < paths.Length; j++)
                {
                    nodes[j] = GetNextNode(nodes[j], i);
                }
            }

            var rem = new int[nodes.Length];

            for (int j = 0; j < nodes.Length; j++)
            {
                var node = nodes[j];
                int step = maxPrePeriod;

                while (!node.EndsWith('Z'))
                {
                    node = GetNextNode(node, step);
                    step++;
                }

                rem[j] = step - maxPrePeriod;
            }

            return maxPrePeriod + Numbers.SolveCongruences(paths.Zip(rem, (p, i) => ((long) p.Period, (long) i)).ToArray());
        }
    }

    public string Solve() => new Map(GetInput()).GetStepsToEnd().ToString();

    public string SolveAdvanced() => new Map(GetInput()).GetStepsToAllEnds().ToString();
}
