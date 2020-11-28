using System.Collections.Generic;

namespace Advent_of_Code.Year2015
{
    public class Day3 : IAdventDay
    {
        public string Name => "3. 12. 2015";

        private static string GetInput() => System.IO.File.ReadAllText("2015/Resources/day3.txt");

        public string Solve()
        {
            (int X, int Y) current = (0, 0);
            var visited = new HashSet<(int X, int Y)> { current };

            foreach (char c in GetInput())
            {
                var next = GetNextPosition(current, c);

                visited.Add(next);

                current = next;
            }

            return visited.Count.ToString();
        }

        public string SolveAdvanced()
        {
            var current = new (int X, int Y)[] { (0, 0), (0, 0) };
            var visited = new HashSet<(int X, int Y)> { current[0] };
            int index = 0;

            foreach (char c in GetInput())
            {
                var next = GetNextPosition(current[index], c);

                visited.Add(next);

                current[index] = next;
                index = 1 - index;
            }

            return visited.Count.ToString();
        }

        private static (int, int) GetNextPosition((int X, int Y) current, char c) => c switch
        {
            '^' => (current.X, current.Y + 1),
            '>' => (current.X + 1, current.Y),
            'v' => (current.X, current.Y - 1),
            '<' => (current.X - 1, current.Y),
            _ => throw new System.InvalidOperationException(),
        };
    }
}
