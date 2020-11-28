using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2015
{
    public class Day5 : IAdventDay
    {
        public string Name => "5. 12. 2015";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2015/Resources/day5.txt");

        public string Solve()
        {
            var vowels = new HashSet<char>("aeiou");

            return GetInput().Count(IsNice).ToString();

            bool IsNice(string s)
            {
                int vowelCount = 0;
                bool hasDoubles = false;

                for(int i = 0; i < s.Length; i++)
                {
                    if(vowels.Contains(s[i]))
                    {
                        vowelCount++;
                    }

                    if(i > 0 && s[i - 1] == s[i])
                    {
                        hasDoubles = true;
                    }

                    if(i > 0)
                    {
                        string d = s[(i - 1)..(i + 1)];

                        if (d == "ab" || d == "cd" || d == "pq" || d == "xy")
                            return false;
                    }
                }

                return vowelCount >= 3 && hasDoubles;
            }
        }

        public string SolveAdvanced()
        {
            return GetInput().Count(IsNice).ToString();

            bool IsNice(string s)
            {
                var pairs = Enumerable.Range(0, s.Length - 1).Select(i => (Start: i, Text: s[i..(i + 2)])).ToList();

                if (pairs.GroupBy(i => i.Text).All(g => g.Count() < 2 || (g.Count() == 2 && g.Max(i => i.Start) - g.Min(i => i.Start) == 1)))
                    return false;

                return Enumerable.Range(0, s.Length - 2).Any(i => s[i] == s[i + 2] && s[i] != s[i + 1]);
            }
        }
    }
}
