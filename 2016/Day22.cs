using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2016
{
    public class Day22 : IAdventDay
    {
        public string Name => "22. 12. 2016";

        private static IEnumerable<Node> GetInput() => File.ReadAllLines("2016/Resources/day22.txt").Skip(2).Select(Node.Create);

        private record Node(string Name, int Col, int Row, int Size, short Used)
        {
            public static Node Create(string input)
            {
                var match = System.Text.RegularExpressions.Regex.Match(input, @"/dev/grid/node-x(\d+)-y(\d+) +(\d+)T + (\d+)T.*");

                return new Node(input, Parse(1), Parse(2), Parse(3), (short)Parse(4));

                int Parse(int index) => int.Parse(match.Groups[index].Value);
            }

            public int Available => Size - Used;
        }

        private class Grid
        {
            public const int TargetNode = 0;

            public Grid()
            {
                var rawNodes = GetInput().ToList();
                var nodesByRow = rawNodes.GroupBy(n => n.Row).OrderBy(g => g.Key).ToList();
                Rows = nodesByRow.Count;
                Columns = nodesByRow.Select(g => g.Count()).Distinct().Single();
                Nodes = nodesByRow.SelectMany(g => g.OrderBy(n => n.Col)).ToList();
                Neighbours = new List<int>[Nodes.Count];

                CalculateNeighbours();
                CalculateComponents();
                CalculateAllDistances(TargetNode);
            }

            private void CalculateNeighbours()
            {
                foreach (var node in Nodes)
                {
                    var list = new List<int>();

                    Neighbours[LocationToIndex(node.Row, node.Col)] = list;

                    for (int rowOffset = -1; rowOffset <= +1; rowOffset++)
                    {
                        for (int colOffset = -1; colOffset <= +1; colOffset++)
                        {
                            if (rowOffset + colOffset is not -1 and not +1)
                                continue;

                            (int r, int c) = (node.Row + rowOffset, node.Col + colOffset);

                            if (!IsValid(r, c))
                                continue;

                            list.Add(LocationToIndex(r, c));
                        }
                    }
                }

                bool IsValid(int row, int col) => row >= 0 && row < Rows && col >= 0 && col < Columns;
            }

            private void CalculateComponents()
            {
                var components = Nodes.Select((n, i) => (Items: new HashSet<int> { i }, n.Used, n.Size)).ToList();

                bool anyChange;

                do
                {
                    anyChange = false;

                    for (int i = 0; i < components.Count; i++)
                    {
                        var neighbours = components[i].Items.SelectMany(k => Neighbours[k]).ToHashSet();

                        for (int j = components.Count - 1; j > i; j--)
                        {
                            var c1 = components[i];
                            var c2 = components[j];

                            if (c2.Items.Any(neighbours.Contains) && c1.Used < c2.Size && c2.Used < c1.Size)
                            {
                                components[i] = (c1.Items.Concat(c2.Items).ToHashSet(), Math.Max(c1.Used, c2.Used), Math.Min(c1.Size, c2.Size));
                                components.RemoveAt(j);
                                anyChange = true;
                            }
                        }
                    }
                } while (anyChange);

                for (int i = 0; i < components.Count; i++)
                {
                    foreach (int k in components[i].Items)
                    {
                        Components[k] = i;
                    }
                }
            }

            public void CalculateAllDistances(int from)
            {
                var fromLocation = IndexToLocation(from);

                var visited = new HashSet<int>();

                Distances[(from, from)] = 0;

                var heap = new Heap<int>();
                heap.Push(from, 0);

                while (heap.Count > 0)
                {
                    var state = heap.Pop();

                    visited.Add(state);

                    int steps = Distances[(from, state)];

                    foreach (var nextState in Neighbours[state])
                    {
                        if (visited.Contains(nextState))
                            continue;

                        if (Components[nextState] != Components[state])
                            continue;

                        if (!Distances.TryGetValue((from, nextState), out int curDistance) || steps + 1 < curDistance)
                        {
                            Distances[(from, nextState)] = steps + 1;
                            Distances[(nextState, from)] = steps + 1;

                            var location = IndexToLocation(nextState);

                            heap.Push(nextState, steps + Math.Abs(fromLocation.Row - location.Row) + Math.Abs(fromLocation.Col - location.Col));
                        }
                    }
                }
            }

            public int LocationToIndex(int row, int col) => row * Columns + col;

            public (int Row, int Col) IndexToLocation(int index) => (index / Columns, index % Columns);

            private int Rows { get; }

            public int Columns { get; }

            public List<Node> Nodes { get; }

            public List<int>[] Neighbours { get; }

            private Dictionary<int, int> Components { get; } = new();

            public Dictionary<(int From, int To), int> Distances { get; } = new();
        }

        private class State
        {
            private readonly Grid _grid;
            private readonly int _dataLocation;
            private readonly Dictionary<int, short> _used = new();

            public State(Grid grid)
            {
                _grid = grid;
                _dataLocation = _grid.LocationToIndex(0, _grid.Columns - 1);
                DistanceEstimate = (_grid.Columns - 1) * _grid.Nodes.Count;
            }

            private State(State from, int sourceLocation, int targetLocation)
            {
                _grid = from._grid;
                _used = new Dictionary<int, short>(from._used);

                short transferred = GetUsed(sourceLocation);

                SetUsed(sourceLocation, 0);
                SetUsed(targetLocation, (short)(GetUsed(targetLocation) + transferred));

                _dataLocation = from._dataLocation == sourceLocation ? targetLocation : from._dataLocation;
            }

            private short GetUsed(int location) => _used.TryGetValue(location, out short used) ? used : _grid.Nodes[location].Used;

            private void SetUsed(int location, short used)
            {
                if (_grid.Nodes[location].Used == used)
                {
                    _used.Remove(location);
                }
                else
                {
                    _used[location] = used;
                }
            }

            public bool IsEndState => _dataLocation == Grid.TargetNode;

            public int DistanceEstimate { get; private init; }

            public IEnumerable<State> GetNeighbourStates()
            {
                var dataLocation = _grid.IndexToLocation(_dataLocation);

                _grid.CalculateAllDistances(_dataLocation);

                for (int sourceIndex = 0; sourceIndex < _grid.Nodes.Count; sourceIndex++)
                {
                    if (!_grid.Distances.TryGetValue((sourceIndex, _dataLocation), out int distanceEstimate))
                        continue;

                    distanceEstimate += (dataLocation.Row + dataLocation.Col) * _grid.Nodes.Count;

                    foreach (var targetIndex in _grid.Neighbours[sourceIndex])
                    {
                        short used = GetUsed(sourceIndex);

                        if (used == 0)
                            continue;

                        if (_grid.Nodes[targetIndex].Size < GetUsed(targetIndex) + used)
                            continue;

                        yield return new State(this, sourceIndex, targetIndex)
                        {
                            DistanceEstimate = distanceEstimate,
                        };
                    }
                }
            }

            public override int GetHashCode() => _used.Aggregate(HashCode.Combine(_used.Count), HashCode.Combine);

            public override bool Equals(object obj)
            {
                if (obj is not State other)
                    return false;

                if (_used.Count != other._used.Count)
                    return false;

                return HasSameValues(_used, other._used) && HasSameValues(other._used, _used);

                bool HasSameValues(Dictionary<int, short> dict1, Dictionary<int, short> dict2) => dict1.All(p => dict2.TryGetValue(p.Key, out var value) && value == p.Value);
            }
        }

        public string Solve()
        {
            var nodes = GetInput().ToList();
            int pairs = 0;

            foreach (var node1 in nodes)
            {
                foreach (var node2 in nodes)
                {
                    if (node1.Name != node2.Name && node1.Used > 0 && node1.Used <= node2.Available)
                    {
                        pairs++;
                    }
                }
            }

            return pairs.ToString();
        }

        public string SolveAdvanced()
        {
            var grid = new Grid();
            var initialState = new State(grid);
            var visited = new HashSet<State>();

            var distances = new Dictionary<State, int>
            {
                [initialState] = 0,
            };

            var heap = new Heap<State>();
            heap.Push(initialState, 0);

            while (heap.Count > 0)
            {
                var state = heap.Pop();

                if (state.IsEndState)
                    return distances[state].ToString();

                visited.Add(state);

                int steps = distances[state];

                foreach (var nextState in state.GetNeighbourStates())
                {
                    if (visited.Contains(nextState))
                        continue;

                    if (!distances.TryGetValue(nextState, out int curDistance) || steps + 1 < curDistance)
                    {
                        distances[nextState] = steps + 1;
                        heap.Push(nextState, steps + nextState.DistanceEstimate);
                    }
                }
            }

            return "No path found";
        }
    }
}
