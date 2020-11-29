using System.Linq;

namespace Advent_of_Code.Year2015
{
    public class Day6 : IAdventDay
    {
        private class LightAction
        {
            private readonly string _action;

            private readonly (int X, int Y) _start;

            private readonly (int X, int Y) _end;

            public LightAction(string input)
            {
                const string pattern = @"(turn on|turn off|toggle) (\d+),(\d+) through (\d+),(\d+)";
                var match = System.Text.RegularExpressions.Regex.Match(input, pattern);

                _action = match.Groups[1].Value;
                _start = (int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
                _end = (int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value));

                System.Diagnostics.Debug.Assert(_start.X <= _end.X && _start.Y <= _end.Y);
            }

            public void Update(bool[,] lights)
            {
                for (int x = _start.X; x <= _end.X; x++)
                {
                    for (int y = _start.Y; y <= _end.Y; y++)
                    {
                        lights[x, y] = GetValue(lights[x, y]);
                    }
                }

                bool GetValue(bool oldValue) => _action switch
                {
                    "turn on" => true,
                    "turn off" => false,
                    "toggle" => !oldValue,
                    _ => oldValue,
                };
            }

            public void Update(int[,] lights)
            {
                for (int x = _start.X; x <= _end.X; x++)
                {
                    for (int y = _start.Y; y <= _end.Y; y++)
                    {
                        lights[x, y] += GetDifference(lights[x, y]);
                    }
                }

                int GetDifference(int oldValue) => _action switch
                {
                    "turn on" => +1,
                    "turn off" when oldValue > 0 => -1,
                    "toggle" => +2,
                    _ => 0,
                };
            }
        }

        public string Name => "6. 12. 2015";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2015/Resources/day6.txt");

        public string Solve()
        {
            const int size = 1000;
            var lights = new bool[size, size];

            foreach (var line in GetInput())
            {
                new LightAction(line).Update(lights);
            }

            return lights.Cast<bool>().Count(b => b).ToString();
        }

        public string SolveAdvanced()
        {
            const int size = 1000;
            var lights = new int[size, size];

            foreach (var line in GetInput())
            {
                new LightAction(line).Update(lights);
            }

            return lights.Cast<int>().Sum().ToString();
        }
    }
}
