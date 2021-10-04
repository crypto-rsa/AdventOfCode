using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tools;

namespace Advent_of_Code.Year2016
{
    public class Day11 : IAdventDay
    {
        public string Name => "11. 12. 2016";

        private static string[] GetInput() => System.IO.File.ReadAllLines(@"2016/Resources/day11.txt");

        private class Item
        {
            public Item(int index, string name)
            {
                Name = name;
                MaskGenerator = (short)(1 << index);
                MaskChip = (short)(1 << (index + 1));
            }

            private string Name { get; }

            public short MaskGenerator { get; }

            public short MaskChip { get; }

            public short Mask => (short)(MaskGenerator | MaskChip);
        }

        private readonly struct State
        {
            public State(byte elevatorPosition, short[] floors, int[] itemCount)
            {
                ElevatorPosition = elevatorPosition;
                Floor1 = floors[0];
                Floor2 = floors[1];
                Floor3 = floors[2];
                Floor4 = floors[3];

                DistanceEstimate = 3 * itemCount[Floor1] + 2 * itemCount[Floor2] + 1 * itemCount[Floor3];
            }

            public State(State old, int movement, short items, int[] itemCount)
            {
                byte newElevatorPosition = (byte)(old.ElevatorPosition + movement);

                ElevatorPosition = newElevatorPosition;
                Floor1 = GetValue(0);
                Floor2 = GetValue(1);
                Floor3 = GetValue(2);
                Floor4 = GetValue(3);

                DistanceEstimate = 3 * itemCount[Floor1] + 2 * itemCount[Floor2] + 1 * itemCount[Floor3];

                short GetValue(int index)
                {
                    bool isOld = index == old.ElevatorPosition;
                    bool isNew = index == newElevatorPosition;

                    if (isOld)
                        return (short)(old.GetFloor(index) & ~items);

                    if (isNew)
                        return (short)(old.GetFloor(index) | items);

                    return old.GetFloor(index);
                }
            }

            public short GetFloor(int index) => index switch
            {
                0 => Floor1,
                1 => Floor2,
                2 => Floor3,
                3 => Floor4,
                _ => throw new InvalidOperationException(),
            };

            public byte ElevatorPosition { get; }

            private short Floor1 { get; }

            private short Floor2 { get; }

            private short Floor3 { get; }

            private short Floor4 { get; }

            public int DistanceEstimate { get; }

            public override bool Equals(object obj) => obj is State other && Equals(other);

            public bool Equals(State other)
                => ElevatorPosition == other.ElevatorPosition && Floor1 == other.Floor1 && Floor2 == other.Floor2 && Floor3 == other.Floor3 && Floor4 == other.Floor4;

            public override int GetHashCode()
            {
                return HashCode.Combine(ElevatorPosition, (ushort)Floor1 | (Floor2 << 16), (ushort)Floor3 | (Floor4 << 16));
            }
        }

        private class Search
        {
            private Dictionary<string, Item> _items;
            private bool[] _isValidState;
            private int[] _itemCount;
            private State _initialState;
            private State _endState;
            private short _fullState;
            private short[][] _validCargos;

            public Search(string[] input)
            {
                Initialize(input);
            }

            private void Initialize(string[] input)
            {
                _items = new Dictionary<string, Item>();
                var floors = new short[4];

                for (int index = 0; index < input.Length; index++)
                {
                    string line = input[index];

                    foreach (Match match in Regex.Matches(line, @"an? (\w+)( generator|-compatible microchip)"))
                    {
                        var name = match.Groups[1].Value;

                        if (!_items.TryGetValue(name, out var item))
                        {
                            item = new Item(2 * _items.Count, name);
                            _items.Add(name, item);
                        }

                        if (match.Groups[2].Value.Contains("generator"))
                        {
                            floors[index] |= item.MaskGenerator;
                        }
                        else
                        {
                            floors[index] |= item.MaskChip;
                        }
                    }
                }

                _fullState = (short)((1 << (2 * _items.Count)) - 1);
                _isValidState = new bool[_fullState + 1];
                _itemCount = new int[_fullState + 1];

                for (int i = 0; i <= _fullState; i++)
                {
                    (_, bool vulnerable, bool dangerous) = _items.Values.Aggregate((i, false, false), CheckItem);

                    _isValidState[i] = !vulnerable || !dangerous;
                    _itemCount[i] = _items.Values.Sum(item => GetItemCount(i, item));
                }

                _initialState = new State(0, floors, _itemCount);
                _endState = new State(3, new short[] { 0, 0, 0, _fullState }, _itemCount);
                _validCargos = Enumerable.Range(0, _fullState + 1)
                    .Select(i => Enumerable.Range(0, _fullState + 1).Where(j => (i & j) == j && _itemCount[j] is 1 or 2).Select(k => (short)k).ToArray())
                    .ToArray();

                (int, bool, bool) CheckItem((int Index, bool Vulnerable, bool Dangerous) acc, Item cur)
                {
                    bool vulnerable = acc.Vulnerable || (acc.Index & cur.Mask) == cur.MaskChip;
                    bool dangerous = acc.Dangerous || (acc.Index & cur.MaskGenerator) != 0;

                    return (acc.Index, vulnerable, dangerous);
                }

                int GetItemCount(int index, Item item)
                {
                    int masked = index & item.Mask;

                    if (masked == item.Mask)
                        return 2;

                    return masked == 0 ? 0 : 1;
                }
            }

            public int FindShortestPath()
            {
                var movements = new[] { -1, +1 };
                var visited = new HashSet<State>();
                var distances = new Dictionary<State, int>
                {
                    [_initialState] = 0,
                };
                var heap = new Heap<State>();
                heap.Push(_initialState, 0);

                while (heap.Count > 0)
                {
                    var state = heap.Pop();

                    if (state.Equals(_endState))
                        return distances[_endState];

                    visited.Add(state);

                    int steps = distances[state];

                    foreach (var movement in movements)
                    {
                        if (state.ElevatorPosition + movement is > 3 or < 0)
                            continue;

                        short source = state.GetFloor(state.ElevatorPosition);
                        short target = state.GetFloor(state.ElevatorPosition + movement);

                        foreach (short i in _validCargos[source])
                        {
                            short sourceNew = (short)(source & ~i);
                            short targetNew = (short)(target | i);

                            if (!_isValidState[sourceNew] || !_isValidState[targetNew])
                                continue;

                            var nextState = new State(state, movement, i, _itemCount);

                            if (visited.Contains(nextState))
                                continue;

                            if (!distances.TryGetValue(nextState, out int curDistance) || steps + 1 < curDistance)
                            {
                                distances[nextState] = steps + 1;
                                heap.Push(nextState, steps + nextState.DistanceEstimate);
                            }
                        }
                    }
                }

                return -1;
            }
        }

        public string Solve() => new Search(GetInput()).FindShortestPath().ToString();

        public string SolveAdvanced()
        {
            var input = GetInput();

            input[0] = input[0].Replace(".", ", an elerium generator, an elerium-compatible microchip, a dilithium generator, and a dilithium-compatible microchip.");

            return new Search(input).FindShortestPath().ToString();
        }
    }
}
