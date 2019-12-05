using System;
using System.Collections.Generic;

namespace Advent_of_Code.Year2017
{
    public class Day14 : IAdventDay
    {
        public const string Input = "jzgqcdpd";

        public string Name => "14. 12. 2017";

        public string Solve()
        {
            int usedSquares = 0;
            for( int i = 0; i < 128; i++ )
            {
                var rowHash = Day10.GetHash( $"{Input}-{i}" );
                foreach( var c in rowHash )
                {
                    int number = int.Parse( c.ToString(), System.Globalization.NumberStyles.HexNumber );

                    int n = number;
                    while( n > 0 )
                    {
                        usedSquares += n % 2;
                        n /= 2;
                    }
                }
            }

            return usedSquares.ToString();
        }

        public string SolveAdvanced()
        {
            var grid = new bool[128, 128];

            for( int rowIndex = 0; rowIndex < 128; rowIndex++ )
            {
                var rowHash = Day10.GetHash( $"{Input}-{rowIndex}" );
                for( int charIndex = 0; charIndex < rowHash.Length; charIndex++ )
                {
                    char c = rowHash[charIndex];
                    int number = int.Parse( c.ToString(), System.Globalization.NumberStyles.HexNumber );

                    int n = number;
                    for( int j = 0; j < 4; j++ )
                    {
                        grid[rowIndex, charIndex * 4 + 3 - j] = (n % 2) == 1;
                        n /= 2;
                    }
                }
            }

            var neighbourOffsets = new int[][]
            {
                new[] { +1, 0 },
                new[] { 0, +1 },
                new[] { -1, 0 },
                new[] { 0, -1 },
            };

            int regions = 0;
            for( int row = 0; row < 128; row++ )
            {
                for( int col = 0; col < 128; col++ )
                {
                    if( !grid[row, col])
                        continue;

                    grid[row, col] = false;

                    var stack = new Stack<Tuple<int, int>>();
                    stack.Push( Tuple.Create( row, col ) );

                    while( stack.Count > 0 )
                    {
                        var t = stack.Pop();

                        foreach( var offset in neighbourOffsets )
                        {
                            int r = t.Item1 + offset[0];
                            int c = t.Item2 + offset[1];

                            if( r < 0 || r >= 128 || c < 0 || c >= 128 || !grid[r, c] )
                                continue;

                            grid[r, c] = false;
                            stack.Push( Tuple.Create( r, c ) );
                        }
                    }

                    regions++;
                }
            }

            return regions.ToString();
        }
    }
}
