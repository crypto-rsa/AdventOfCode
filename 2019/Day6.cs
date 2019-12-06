using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2019
{
    public class Day6 : IAdventDay
    {
        public string Name => "6. 12. 2019";

        public string Solve() => CountOrbits().ToString();

        public string SolveAdvanced() => StepsToSanta().ToString();

        private static Dictionary<string, string> GetInput()
        {
            return File.ReadAllLines("2019/Resources/day6.txt").Select(Parse).ToDictionary( k => k.Orbit, k => k.Center );

            (string Center, string Orbit) Parse( string s )
            {
                var a = s.Split(')');

                return (a[0], a[1]);
            }
        }

        private int CountOrbits()
        {
            var orbits = GetInput();
            var orbitCounts = new SortedDictionary<string, int>
            {
                ["COM"] = 0,
            };

            int totalOrbits = 0;

            while (orbitCounts.Any())
            {
                var center = orbitCounts.First().Key;

                foreach (var item in orbits.Where( k => k.Value == center ))
                {
                    orbitCounts[item.Key] = orbitCounts[center] + 1;
                    totalOrbits += orbitCounts[item.Key];
                }

                orbitCounts.Remove(center);
            }

            return totalOrbits;
        }

        private int StepsToSanta()
        {
            var orbits = GetInput();
            var fromMe = GetPathToRoot("YOU");
            var fromSanta = GetPathToRoot("SAN");

            while (fromMe.TryPeek( out string me ) && fromSanta.TryPeek( out string santa ) && me == santa )
            {
                fromMe.Pop();
                fromSanta.Pop();
            }

            return fromMe.Count + fromSanta.Count;

            Stack<string> GetPathToRoot( string from )
            {
                var stack = new Stack<string>();

                while (orbits.TryGetValue( from, out string center ))
                {
                    stack.Push(center);
                    from = center;
                }

                return stack;
            }
        }
    }
}