using System;

namespace Advent_of_Code.Year2017
{
    public class Day11 : IAdventDay
    {
        public string Name => "11. 12. 2017";

        private static string[] GetInput() => System.IO.File.ReadAllText(@"2017/Resources/day11.txt").Split(',');

        public string Solve()
        {
            int x = 0;
            int y = 0;

            foreach( var step in GetInput() )
            {
                switch( step )
                {
                    case "nw": y--; break;
                    case "n": x++; y--; break;
                    case "ne": x++; break;
                    case "se": y++; break;
                    case "s": y++; x--; break;
                    case "sw": x--; break;
                    default: throw new InvalidOperationException($"The input contains unrecognized string '{step}'");
                }
            }

            return GetPathLength( x, y ).ToString();
        }

        private int GetPathLength( int x, int y )
        {
            int pathLength = Math.Abs( x ) + Math.Abs( y );

            if( (x < 0) != (y < 0) )
            {
                // shorten the path via "n" or "s"
                pathLength -= Math.Min( Math.Abs( x ), Math.Abs( y ) );
            }

            return pathLength;
        }

        public string SolveAdvanced()
        {
            int x = 0;
            int y = 0;
            int furthestPath = 0;

            foreach( var step in GetInput() )
            {
                switch( step )
                {
                    case "nw": y--; break;
                    case "n": x++; y--; break;
                    case "ne": x++; break;
                    case "se": y++; break;
                    case "s": y++; x--; break;
                    case "sw": x--; break;
                    default: throw new InvalidOperationException($"The input contains unrecognized string '{step}'");
                }

                furthestPath = Math.Max( furthestPath, GetPathLength( x, y ) );
            }

            return furthestPath.ToString();
        }
    }
}
