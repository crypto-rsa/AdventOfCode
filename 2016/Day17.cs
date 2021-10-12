using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Tools;

namespace Advent_of_Code.Year2016
{
    public class Day17 : IAdventDay
    {
        public string Name => "17. 12. 2016";

        private const string Input = "pxxbnzuo";

        private static (string Path, int Row, int Col) GetRoom((string Path, int Row, int Col) state, char direction)
        {
            return direction switch
            {
                'U' => (state.Path + direction, state.Row - 1, state.Col),
                'D' => (state.Path + direction, state.Row + 1, state.Col),
                'L' => (state.Path + direction, state.Row, state.Col - 1),
                'R' => (state.Path + direction, state.Row, state.Col + 1),
                _ => state,
            };
        }

        private static bool IsValid(int row, int col) => row is >= 0 and < 4 && col is >= 0 and < 4;

        private static string Search(bool findLongest)
        {
            var md5 = MD5.Create();
            var visited = new HashSet<(string, int, int)>();
            var endRoom = (Row: 3, Col: 3);

            var heap = new Heap<(string Path, int Row, int Col)>();
            heap.Push((string.Empty, 0, 0), 0);

            string longest = string.Empty;

            while (heap.Count > 0)
            {
                var state = heap.Pop();

                if (state.Row == endRoom.Row && state.Col == endRoom.Col)
                {
                    if( !findLongest )
                        return state.Path;

                    if (state.Path.Length > longest.Length)
                    {
                        longest = state.Path;
                    }

                    continue;
                }

                visited.Add(state);

                foreach (var nextState in GetNeighbours(state).Where(i => IsValid(i.Row, i.Col)))
                {
                    if (visited.Contains(nextState))
                        continue;

                    heap.Push(nextState, nextState.Path.Length + GetDistanceEstimate(nextState.Row, nextState.Col));
                }
            }

            return findLongest ? longest.Length.ToString() : "<no path exists>";

            int GetDistanceEstimate(int row, int col) => Math.Abs(row - endRoom.Row) + Math.Abs(col - endRoom.Col);

            IEnumerable<(string Path, int Row, int Col)> GetNeighbours((string Path, int Row, int Col) state)
            {
                var hash = Convert.ToHexString(md5.ComputeHash(Encoding.ASCII.GetBytes(Input + state.Path))).ToLower();

                if (IsOpen(hash[0]))
                    yield return GetRoom(state, 'U');

                if (IsOpen(hash[1]))
                    yield return GetRoom(state, 'D');

                if (IsOpen(hash[2]))
                    yield return GetRoom(state, 'L');

                if (IsOpen(hash[3]))
                    yield return GetRoom(state, 'R');
            }

            static bool IsOpen(char c) => c is 'b' or 'c' or 'd' or 'e' or 'f';
        }

        public string Solve() => Search(findLongest: false);

        public string SolveAdvanced() => Search(findLongest: true);
    }
}
