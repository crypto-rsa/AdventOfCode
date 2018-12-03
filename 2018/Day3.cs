using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Advent_of_Code;

namespace Advent_of_Code_2018
{
    public class Day3 : IAdventDay
    {
        private struct Claim : IEquatable<Claim>
        {
            public Claim(string input)
            {
                var items = input.Split(new char[] { '#', ' ', '@', ',', ':', 'x' }, StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToArray();
                ID = items[0];
                Left = items[1];
                Top = items[2];
                Width = items[3];
                Height = items[4];
            }

            public int ID { get; }

            public int Left { get; }

            public int Top { get; }

            public int Width { get; }

            public int Height { get; }

            public override bool Equals(object obj) => obj is Claim claim && Equals(claim);

            public override int GetHashCode() => ID;

            public bool Equals(Claim other) => ID == other.ID;

            public IEnumerable<(int Left, int Top)> GetIndices()
            {
                for (int i = Left; i < Left + Width; i++)
                {
                    for (int j = Top; j < Top + Height; j++)
                    {
                        yield return (i, j);
                    }
                }
            }
        }

        public string Name => "3. 12. 2018";

        public string Solve()
        {
            return GetArray().Cast<List<Claim>>().Count(l => l != null && l.Count >= 2).ToString();
        }

        private static List<Claim>[,] GetArray()
        {
            var claims = File.ReadAllLines(@"2018/Resources/day3.txt").Select(s => new Claim(s)).ToList();

            var (totalWidth, totalHeight) = claims.Aggregate((Width: 0, Height: 0),
                (current, next) => (Math.Max(current.Width, next.Left + next.Width), Math.Max(current.Height, next.Top + next.Height)));

            var array = new List<Claim>[totalWidth, totalHeight];

            foreach (var claim in claims)
            {
                foreach ((int i, int j) in claim.GetIndices())
                {
                    if (array[i, j] == null)
                    {
                        array[i, j] = new List<Claim>();
                    }

                    array[i, j].Add(claim);
                }
            }

            return array;
        }

        public string SolveAdvanced()
        {
            var nonEmptyLists = GetArray().Cast<List<Claim>>().Where(l => l != null).ToList();
            var allClaims = new HashSet<Claim>(nonEmptyLists.SelectMany(l => l));

            foreach( var list in nonEmptyLists.Where( l => l.Count >= 2 ))
            {
                foreach( var claim in list )
                {
                    allClaims.Remove(claim);
                }
            }

            return allClaims.Single().ID.ToString();
        }
    }
}