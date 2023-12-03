using System;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2023;

public class Day2 : IAdventDay
{
    public string Name => "2. 12. 2023";

    private static string GetInput() => File.ReadAllText("2023/Resources/day2.txt");

    private record Game(int Red, int Green, int Blue)
    {
        public bool IsValidWithRespectTo(Game other)
            => Red <= other.Red && Green <= other.Green && Blue <= other.Blue;

        public static Game FromString(string input)
        {
            int[] numbers = { 0, 0, 0 };

            foreach (string ball in input.Split(", "))
            {
                var parts = ball.Split(' ');

                int index = parts[1] switch
                {
                    "red" => 0,
                    "green" => 1,
                    "blue" => 2,
                    _ => -1,
                };

                numbers[index] = int.Parse(parts[0]);
            }

            return new Game(numbers[0], numbers[1], numbers[2]);
        }

        public int Power => Red * Green * Blue;

        public static Game AggregateByMax(Game prev, Game next)
            => new( Math.Max(prev.Red, next.Red), Math.Max(prev.Green, next.Green), Math.Max(prev.Blue, next.Blue) );
    }

    public string Solve()
    {
        var maxGame = new Game(12, 13, 14);

        return GetInput()
            .SplitToLines()
            .Select(GetValidId)
            .Sum()
            .ToString();

        int GetValidId(string input)
        {
            var match = System.Text.RegularExpressions.Regex.Match(input, @"Game (\d+): (.*)");
            int id = int.Parse(match.Groups[1].Value);
            var subGames = match.Groups[2].Value.Split("; ");

            return subGames.Select(Game.FromString).All(g => g.IsValidWithRespectTo(maxGame))
                ? id
                : 0;
        }
    }

    public string SolveAdvanced()
    {
        return GetInput()
            .SplitToLines()
            .Select(GetMinPower)
            .Sum()
            .ToString();

        int GetMinPower(string input)
        {
            var match = System.Text.RegularExpressions.Regex.Match(input, @"Game (\d+): (.*)");
            var subGames = match.Groups[2].Value.Split("; ");

            var minGame = subGames
                .Select(Game.FromString)
                .Aggregate(new Game(0, 0, 0), Game.AggregateByMax);

            return minGame.Power;
        }
    }
}
