using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2015
{
    public class Day12 : IAdventDay
    {
        private class JsonObject
        {
            public List<JsonObject> Children { get; } = new List<JsonObject>();

            public int Total { get; set; }

            public JsonObject Parent { get; init; }

            public bool IsObject { get; init; }

            public bool IsRed { get; set; }

            public JsonObject AddChild(char startChar)
            {
                var child = new JsonObject
                {
                    Parent = this,
                    IsObject = startChar == '{',
                };

                Children.Add(child);

                return child;
            }

            public int GetTotal(bool nonRedOnly)
            {
                if (nonRedOnly && IsObject && IsRed)
                    return 0;

                return Total + Children.Sum(c => c.GetTotal(nonRedOnly));
            }
        }

        public string Name => "12. 12. 2015";

        private static string GetInput() => System.IO.File.ReadAllText("2015/Resources/day12.txt");

        public string Solve() => GetSum(nonRedOnly: false).ToString();

        public string SolveAdvanced() => GetSum(nonRedOnly: true).ToString();

        private static int GetSum(bool nonRedOnly)
        {
            var input = GetInput();
            var current = new JsonObject();
            int? number = null;
            bool minus = false;

            for (int pos = 0; pos < input.Length; pos++)
            {
                switch( input[pos])
                {
                    case '{':
                        StoreAndReset();
                        current = current.AddChild(input[pos]);
                        break;

                    case '-':
                        minus = true;
                        break;

                    case >= '0' and <= '9':
                        int digit = int.Parse(input[pos..(pos + 1)]);
                        number ??= 0;
                        number = number * 10 + digit;
                        break;

                    case '}':
                        StoreAndReset();
                        current = current.Parent;
                        break;

                    case ':':
                        if (input[(pos + 1)..(pos +6)] == "\"red\"")
                        {
                            current.IsRed = true;
                        }
                        StoreAndReset();
                        break;

                    default:
                        StoreAndReset();
                        break;
                }
            }

            return current.GetTotal(nonRedOnly);

            void StoreAndReset()
            {
                if (number.HasValue)
                {
                    current.Total += (minus ? -1 : +1) * number.Value;
                }

                number = null;
                minus = false;
            }
        }
    }
}
