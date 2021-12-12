using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021
{
    public class Day12 : IAdventDay
    {
        public string Name => "12. 12. 2021";

        private static string[] GetInput() => File.ReadAllLines("2021/Resources/day12.txt");

        private class State
        {
            private readonly string _path;
            private readonly HashSet<string> _visitedSmallCaves;
            private readonly string _caveVisitedTwice;

            public State()
            {
                LastCave = "start";
                _path = "start";
                _visitedSmallCaves = new HashSet<string> { "start" };
            }

            public State(State previous, string target, bool isSmall, bool secondVisit)
            {
                _path = $"{previous._path},{target}";
                _visitedSmallCaves = new HashSet<string>(previous._visitedSmallCaves);
                _caveVisitedTwice = previous._caveVisitedTwice;

                if (isSmall)
                {
                    _visitedSmallCaves.Add(target);
                }

                if (secondVisit)
                {
                    _caveVisitedTwice = target;
                }

                LastCave = target;
            }

            public bool? CanVisit(string target, bool isSmall, bool allowDoubleVisit)
            {
                if (!isSmall)
                    return true;

                if (!_visitedSmallCaves.Contains(target))
                    return true;

                if (allowDoubleVisit && _caveVisitedTwice == null)
                    return null;

                return false;
            }

            public bool IsEnd => LastCave == "end";

            public string LastCave { get; }
        }

        private static Dictionary<string, HashSet<string>> CreateMap()
        {
            var map = new Dictionary<string, HashSet<string>>();

            foreach (var line in GetInput())
            {
                var parts = line.Split('-');

                Add(parts[0], parts[1]);
                Add(parts[1], parts[0]);
            }

            return map;

            void Add(string from, string to)
            {
                if (to == "start")
                    return;

                if (!map.TryGetValue(from, out var list))
                {
                    list = new HashSet<string>();
                    map[from] = list;
                }

                list.Add(to);
            }
        }

        private static int GetPathCount(bool allowDoubleVisit)
        {
            var map = CreateMap();
            var smallCaves = map.Keys.Where(s => s.ToLower() == s).ToHashSet();

            var queue = new Queue<State>();
            queue.Enqueue(new State());

            int pathCount = 0;

            while (queue.Any())
            {
                var state = queue.Dequeue();

                if (state.IsEnd)
                {
                    pathCount++;

                    continue;
                }

                foreach (string target in GetTargets(state.LastCave))
                {
                    bool isSmall = smallCaves.Contains(target);
                    var canVisit = state.CanVisit(target, isSmall, allowDoubleVisit);

                    if (canVisit != false)
                    {
                        queue.Enqueue(new State(state, target, isSmall, canVisit == null));
                    }
                }
            }

            return pathCount;

            IEnumerable<string> GetTargets(string source) => map.TryGetValue(source, out var list) ? list : Enumerable.Empty<string>();
        }

        public string Solve() => GetPathCount(allowDoubleVisit: false).ToString();

        public string SolveAdvanced() => GetPathCount(allowDoubleVisit: true).ToString();
    }
}
