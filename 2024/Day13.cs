using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Tools;

namespace Advent_of_Code.Year2024;

public class Day13 : IAdventDay
{
    public string Name => "13. 12. 2024";

    private static string GetInput() => File.ReadAllText("2024/Resources/day13.txt");

    internal record Position(long X, long Y)
    {
        public static Position operator +(Position a, Position b) => new( a.X + b.X, a.Y + b.Y );
    }

    private class Game
    {
        #region Constants

        public const int ACost = 3;

        public static readonly int BCost = 1;

        #endregion

        #region Constructors

        public Game(string[] lines, long prizeOffset)
        {
            var aMatch = Regex.Match(lines[0], @"Button A: X\+(\d+), Y\+(\d+)");
            var bMatch = Regex.Match(lines[1], @"Button B: X\+(\d+), Y\+(\d+)");
            var prizeMatch = Regex.Match(lines[2], @"Prize: X=(\d+), Y=(\d+)");

            A = new Position(long.Parse(aMatch.Groups[1].Value), long.Parse(aMatch.Groups[2].Value));
            B = new Position(long.Parse(bMatch.Groups[1].Value), long.Parse(bMatch.Groups[2].Value));
            PrizePosition = new Position(long.Parse(prizeMatch.Groups[1].Value) + prizeOffset, long.Parse(prizeMatch.Groups[2].Value) + prizeOffset);
        }

        #endregion

        #region Properties

        public Position A { get; }

        public Position B { get; }

        public Position PrizePosition { get; }

        #endregion
    }

    private static long GetTotalTokensCount(long prizeOffset)
    {
        var games = GetInput().ParseByBlankLines().Select(i => new Game(i.ToArray(), prizeOffset)).ToArray();
        long tokens = 0;

        foreach (var game in games)
        {
            if (Numbers.SolvePairOfLinearEquations(game.A.X, game.B.X, game.PrizePosition.X, game.A.Y, game.B.Y, game.PrizePosition.Y) is not var (aCount, bCount))
                continue;

            tokens += Game.ACost * aCount + Game.BCost * bCount;
        }

        return tokens;
    }

    public string Solve() => GetTotalTokensCount(prizeOffset: 0).ToString();

    public string SolveAdvanced() => GetTotalTokensCount(prizeOffset: 10000000000000).ToString();
}
