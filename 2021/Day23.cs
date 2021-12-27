using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2021;

public class Day23 : IAdventDay
{
    public string Name => "23. 12. 2021";

    private static string[] GetInput() => File.ReadAllLines("2021/Resources/day23.txt");

    private class State
    {
        private readonly string _hallway;

        private readonly string _roomA;
        private readonly string _roomB;
        private readonly string _roomC;
        private readonly string _roomD;

        private static readonly Dictionary<char, int> _targetRoomPosition = new()
        {
            ['A'] = 2,
            ['B'] = 4,
            ['C'] = 6,
            ['D'] = 8,
        };

        private static readonly Dictionary<char, int> _energyMultiplier = new()
        {
            ['A'] = 1,
            ['B'] = 10,
            ['C'] = 100,
            ['D'] = 1000,
        };

        private static readonly char[] _names = { 'A', 'B', 'C', 'D' };

        private static readonly int[] _restHallwayPositions = { 0, 1, 3, 5, 7, 9, 10 };

        public int Energy { get; }

        public int DistanceEstimate { get; }

        public bool IsEndState => _roomA.All(c => c == 'A') && _roomB.All(c => c == 'B') && _roomC.All(c => c == 'C') && _roomD.All(c => c == 'D');

        public State(string[] map)
        {
            var slice = map[2..^1];

            _hallway = map[1][1..^1];
            _roomA = string.Concat(slice.Select(s => s[3]));
            _roomB = string.Concat(slice.Select(s => s[5]));
            _roomC = string.Concat(slice.Select(s => s[7]));
            _roomD = string.Concat(slice.Select(s => s[9]));

            DistanceEstimate = GetDistanceEstimate();
        }

        private State(State other, bool fromHallway, int hallwayPosition, int roomPosition)
        {
            string room = other.GetRoom(roomPosition);
            int topIndex = fromHallway ? room.IndexOf('.') : room.IndexOfAny(_names);
            char movingChar = fromHallway ? other._hallway[hallwayPosition] : room[topIndex];

            _hallway = ReplaceChar(other._hallway, hallwayPosition, fromHallway ? '.' : movingChar);
            _roomA = GetAdjustedRoom(2);
            _roomB = GetAdjustedRoom(4);
            _roomC = GetAdjustedRoom(6);
            _roomD = GetAdjustedRoom(8);
            Energy = other.Energy + GetSteps(roomPosition, topIndex, hallwayPosition) * _energyMultiplier[movingChar];
            DistanceEstimate = GetDistanceEstimate();

            string GetAdjustedRoom(int position)
            {
                var source = other.GetRoom(position);

                return position == roomPosition
                    ? ReplaceChar(source, topIndex, fromHallway ? movingChar : '.')
                    : source;
            }
        }

        private State(State other, int fromPosition, int toPosition)
        {
            (int sourceDepth, char movingChar) = other.GetTopNonEmpty(fromPosition)!.Value;
            var targetRoom = other.GetRoom(toPosition);
            int targetDepth = targetRoom.LastIndexOf('.');

            _hallway = other._hallway;
            _roomA = GetAdjustedRoom(2);
            _roomB = GetAdjustedRoom(4);
            _roomC = GetAdjustedRoom(6);
            _roomD = GetAdjustedRoom(8);
            Energy = other.Energy + (GetSteps(fromPosition, sourceDepth, toPosition) + targetDepth + 1) * _energyMultiplier[movingChar];
            DistanceEstimate = GetDistanceEstimate();

            string GetAdjustedRoom(int position)
            {
                var source = other.GetRoom(position);

                if (position == fromPosition)
                {
                    return ReplaceChar(source, sourceDepth, '.');
                }

                if (position == toPosition)
                {
                    return ReplaceChar(source, targetDepth, movingChar);
                }

                return source;
            }
        }

        public override int GetHashCode() => HashCode.Combine(_hallway, _roomA, _roomB, _roomC, _roomD);

        public override bool Equals(object obj) => obj is State other
            && other._hallway == _hallway
            && other._roomA == _roomA
            && other._roomB == _roomB
            && other._roomC == _roomC
            && other._roomD == _roomD;

        private string GetRoom(int roomPosition) => roomPosition switch
        {
            2 => _roomA,
            4 => _roomB,
            6 => _roomC,
            8 => _roomD,
            _ => throw new ArgumentOutOfRangeException(nameof( roomPosition )),
        };

        public IEnumerable<State> GetNeighbourStates()
        {
            for (int i = 0; i < _hallway.Length; i++)
            {
                char c = _hallway[i];

                if (c == '.')
                    continue;

                if (!IsHallwayFree(i, _targetRoomPosition[c], excludeFrom: true))
                    continue;

                if (GetRoom(_targetRoomPosition[c]).Any(t => t != '.' && t != c))
                    continue;

                yield return new State(this, fromHallway: true, i, _targetRoomPosition[c]);
            }

            foreach (var roomPosition in _targetRoomPosition.Values)
            {
                var top = GetTopNonEmpty(roomPosition);

                if (top == null)
                    continue;

                foreach (int hallwayPosition in _restHallwayPositions)
                {
                    if (!IsHallwayFree(hallwayPosition, roomPosition, excludeFrom: false))
                        continue;

                    yield return new State(this, fromHallway: false, hallwayPosition, roomPosition);
                }
            }

            foreach (var from in _targetRoomPosition)
            {
                var fromTop = GetTopNonEmpty(from.Value);

                if (fromTop == null || fromTop.Value.Char == from.Key)
                    continue;

                var targetPosition = _targetRoomPosition[fromTop.Value.Char];

                if (!IsHallwayFree(from.Value, targetPosition, excludeFrom: false))
                    continue;

                var toTop = GetTopNonEmpty(targetPosition);

                if (toTop?.Index == 0)
                    continue;

                if (GetRoom(targetPosition).Any(t => t != '.' && t != from.Key))
                    continue;

                yield return new State(this, from.Value, targetPosition);
            }
        }

        private (int Index, char Char)? GetTopNonEmpty(int roomPosition)
        {
            var room = GetRoom(roomPosition);
            int index = room.IndexOfAny(_names);

            if (index == -1)
                return null;

            return (index, room[index]);
        }

        private static string ReplaceChar(string input, int index, char c) => $"{input[..index]}{c}{input[(index + 1)..]}";

        private bool IsHallwayFree(int from, int to, bool excludeFrom)
        {
            int offset = excludeFrom ? 1 : 0;
            (int start, int end) = from <= to ? (from + offset, to) : (to, from - offset);

            return _hallway[start..(end + 1)].IndexOfAny(_names) == -1;
        }

        private static int GetSteps(int roomPosition, int depth, int toHallwayPosition) => depth + 1 + Math.Abs(roomPosition - toHallwayPosition);

        private int GetDistanceEstimate()
        {
            return _targetRoomPosition.Values.Sum(GetRoomEstimate) + _hallway.Select((c, i) => GetCharEstimate(c, i, 0, true)).Sum();

            int GetRoomEstimate(int position) => GetRoom(position).Select((c, i) => GetCharEstimate(c, position, i, false)).Sum();

            int GetCharEstimate(char c, int from, int depth, bool fromHallway)
            {
                if (c == '.')
                    return 0;

                int target = _targetRoomPosition[c];

                if (from != target)
                    return _energyMultiplier[c] * (GetSteps(from, depth, target) + (fromHallway ? 0 : 1));

                return 0;
            }
        }
    }

    private static string Solve(string[] map)
    {
        var initialState = new State(map);

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

            foreach (var nextState in state.GetNeighbourStates())
            {
                int newEnergy = nextState.Energy;

                if (!distances.TryGetValue(nextState, out int curEnergy) || newEnergy < curEnergy)
                {
                    distances[nextState] = newEnergy;
                    heap.Push(nextState, newEnergy + nextState.DistanceEstimate);
                }
            }
        }

        return "No path found";
    }

    public string Solve() => Solve(GetInput());

    public string SolveAdvanced()
    {
        var input = GetInput().ToList();

        input.Insert(input.Count - 2, "  #D#C#B#A#");
        input.Insert(input.Count - 2, "  #D#B#A#C#");

        return Solve(input.ToArray());
    }
}
