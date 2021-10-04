using System;
using System.Collections.Generic;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2016
{
    public class Day13 : IAdventDay
    {
        public string Name => "13. 12. 2016";

        private const int Input = 1358;

        private static readonly (int X, int Y) InitialState = (1, 1);

        private class Search
        {
            private readonly Dictionary<(int, int), byte> _maze = new();

            public int FindShortestPath((int X, int Y) endState)
            {
                var visited = new HashSet<(int, int)>();
                var distances = new Dictionary<(int, int), int>
                {
                    [InitialState] = 0,
                };
                var heap = new Heap<(int, int)>();
                heap.Push(InitialState, 0);

                while (heap.Count > 0)
                {
                    var state = heap.Pop();

                    if (state.Equals(endState))
                        return distances[endState];

                    visited.Add(state);

                    int steps = distances[state];

                    foreach (var nextState in GetNeighbours(state).Where(IsValid))
                    {
                        if (visited.Contains(nextState))
                            continue;

                        if (!distances.TryGetValue(nextState, out int curDistance) || steps + 1 < curDistance)
                        {
                            distances[nextState] = steps + 1;
                            heap.Push(nextState, steps + GetDistanceEstimate(nextState));
                        }
                    }
                }

                return -1;

                int GetDistanceEstimate((int X, int Y) state) => Math.Abs(state.X - endState.X) + Math.Abs(state.Y - endState.Y);
            }

            public int GetVisitedStates(int maxDistance)
            {
                var visited = new HashSet<(int, int)>();
                var distances = new Dictionary<(int, int), int>
                {
                    [InitialState] = 0,
                };
                var queue = new Queue<(int, int)>();
                queue.Enqueue(InitialState);

                while (queue.Count > 0)
                {
                    var state = queue.Dequeue();
                    int steps = distances[state];

                    if (steps > maxDistance)
                        return visited.Count;

                    visited.Add(state);

                    foreach (var nextState in GetNeighbours(state).Where(IsValid))
                    {
                        if (visited.Contains(nextState))
                            continue;

                        distances[nextState] = steps + 1;
                        queue.Enqueue(nextState);
                    }
                }

                return visited.Count;
            }

            private bool IsValid((int X, int Y) position) => position.X >= 0 && position.Y >= 0 && GetMazeItem(position) == 0;

            private static IEnumerable<(int X, int Y)> GetNeighbours((int X, int Y) position)
            {
                yield return (position.X, position.Y - 1);
                yield return (position.X + 1, position.Y);
                yield return (position.X, position.Y + 1);
                yield return (position.X - 1, position.Y);
            }

            private byte GetMazeItem((int X, int Y) position)
            {
                if (_maze.TryGetValue(position, out byte value)) return value;

                long t = position.X * position.X + 3 * position.X + 2 * position.X * position.Y + position.Y + position.Y * position.Y + Input;
                byte ones = 0;

                while (t > 0)
                {
                    ones += (byte)(t & 1);
                    t >>= 1;
                }

                value = (byte)(ones & 1);

                _maze[position] = value;

                return value;
            }
        }

        public string Solve() => new Search().FindShortestPath((31, 39)).ToString();

        public string SolveAdvanced() => new Search().GetVisitedStates(50).ToString();
    }
}
