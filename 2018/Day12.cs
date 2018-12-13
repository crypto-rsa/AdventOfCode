using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advent_of_Code.Year2018
{
    public class Day12 : IAdventDay
    {
        public string Name => "12. 12. 2018";

        private static (string Initial, Dictionary<string, char> Patterns) GetInput()
        {
            var lines = System.IO.File.ReadAllLines(@"2018/Resources/day12.txt");

            string initial = lines[0].Split(':')[1].Trim();

            return (initial, lines.Skip(2).Select(GetItem).ToDictionary(i => i.Key, i => i.Value));

            (string Key, char Value) GetItem(string s)
            {
                var patternItems = s.Split(" => ");

                return (patternItems[0], patternItems[1][0]);
            }
        }

        public string Solve()
        {
            var (current, patterns) = GetInput();

            const int patternLength = 5;
            const int extension = patternLength - 1;
            int startPosition = 0;

            Console.WriteLine($"0, start @ {startPosition}, sum is {current.Select((c, i) => c == '#' ? i - startPosition : 0).Sum()}: {current}");
            for (int iteration = 0; iteration < 20; iteration++)
            {
                var stringBuilder = new StringBuilder(current);
                stringBuilder.Insert(0, ".", extension);
                stringBuilder.Append(new string('.', extension));
                startPosition += extension;

                var searchString = stringBuilder.ToString();
                var queryBuilder = new StringBuilder(new string(searchString.Take(extension + 1).ToArray()));
                for (int position = 0; position < searchString.Length - patternLength; position++)
                {
                    if (!patterns.TryGetValue(queryBuilder.ToString(), out char pattern))
                    {
                        pattern = '.';
                    }

                    stringBuilder[position + patternLength / 2] = pattern;

                    queryBuilder.Remove(0, 1);
                    queryBuilder.Append(searchString[position + patternLength]);
                }

                Console.WriteLine($"First {stringBuilder.ToString().IndexOf('#') - startPosition}, last {stringBuilder.ToString().LastIndexOf('#')}");

                current = stringBuilder.ToString();
                // Console.WriteLine($"{iteration + 1}, start @ {startPosition}, sum is {current.Select((c, i) => c == '#' ? i - startPosition : 0).Sum()}: {current}");
            }

            return current.Select((c, i) => c == '#' ? i - startPosition : 0).Sum().ToString();
        }

        public string SolveAdvanced()
        {
            return string.Empty;
        }
    }
}