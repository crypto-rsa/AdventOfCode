using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2017
{
    public class Day6 : IAdventDay
    {
        private class Distribution : List<int>, IEquatable<Distribution>
        {
            public Distribution(List<int> source)
                : base(source)
            {
            }

            public Distribution Redistribute()
            {
                var newDistribution = new Distribution(this);

                int sourceIndex = IndexOf(this.Max());
                int blocks = this[sourceIndex];
                newDistribution[sourceIndex] = 0;

                int nextIndex = (sourceIndex + 1) % Count;
                while (blocks > 0)
                {
                    newDistribution[nextIndex]++;
                    blocks--;
                    nextIndex = (nextIndex + 1) % Count;
                }

                return newDistribution;
            }

            public override int GetHashCode() => Count;

            public override bool Equals(object obj) => Equals(obj as Distribution);

            public bool Equals(Distribution other) => other != null && this.SequenceEqual(other);
        }

        private static List<int> GetInput() => new List<int>() { 5, 1, 10, 0, 1, 7, 13, 14, 3, 12, 8, 10, 7, 12, 0, 6 };

        public string Name => "6. 12. 2017";

        public string Solve()
        {
            var distribution = new Distribution(GetInput());
            var distributions = new HashSet<Distribution>() { distribution };

            while (true)
            {
                distribution = distribution.Redistribute();
                if (!distributions.Add(distribution))
                    break;
            }

            return distributions.Count.ToString();
        }

        public string SolveAdvanced()
        {
            var distribution = new Distribution(GetInput());
            int nextIndex = 0;
            var distributions = new Dictionary<Distribution, int>() { [distribution] = nextIndex++ };

            while (true)
            {
                distribution = distribution.Redistribute();
                if (distributions.TryGetValue(distribution, out int curIndex))
                    return (nextIndex - curIndex).ToString();

                distributions[distribution] = nextIndex++;
            }
        }
    }
}
