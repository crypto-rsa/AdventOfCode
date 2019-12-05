using System.Collections.Generic;

namespace Advent_of_Code.Year2017
{
    public class Day17 : IAdventDay
    {
        public const string Input = "354";

        public string Name => "17. 12. 2017";

        public string Solve()
        {
            int steps = int.Parse( input );
            var list = new List<int>( steps + 1 ) { 0 };
            int curPosition = 0;

            for( int i = 0; i < 2017; i++ )
            {
                curPosition = (curPosition + steps + 1) % list.Count;
                list.Insert( curPosition, i + 1 );
            }

            return list[(curPosition + 1) % list.Count].ToString();
        }

        public string SolveAdvanced()
        {
            int steps = int.Parse( input );
            int curPosition = 0;
            int zeroPosition = 0;
            int next = 1;

            for( int i = 0; i < 50_000_000; i++ )
            {
                curPosition = (curPosition + steps + 1) % (i + 1);
                if( curPosition == (zeroPosition + 1) % (i + 1) )
                {
                    next = i + 1;
                }

                if( curPosition <= zeroPosition )
                {
                    zeroPosition++;
                }
            }

            return next.ToString();
        }
    }
}
