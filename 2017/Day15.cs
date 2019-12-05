using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2017
{
    public class Day15 : IAdventDay
    {
        private static int[] GetInput() => new int[] {679, 771};

        public string Name => "15. 12. 2017";

        public string Solve()
        {
            int[] startValues = GetInput();

            var generatorA = Generate( startValues[0], 16807, 1 );
            var generatorB = Generate( startValues[1], 48271, 1 );

            const int loopCount = 40_000_000;
            const long mask = 0xFFFF;
            var sum = Enumerable.Zip( generatorA, generatorB, ( l1, l2 ) => (l1 & mask) == (l2 & mask) ? 1 : 0 ).Take( loopCount ).Sum();

            return sum.ToString();
        }

        private IEnumerable<long> Generate( long startValue, long factor, int multiple )
        {
            long lastValue = startValue;

            while( true )
            {
                long curValue = (lastValue * factor) % 2147483647;

                if( curValue % multiple == 0 )
                    yield return curValue;

                lastValue = curValue;
            }
        }

        public string SolveAdvanced()
        {
            int[] startValues = GetInput();

            var generatorA = Generate( startValues[0], 16807, 4 );
            var generatorB = Generate( startValues[1], 48271, 8 );

            const int loopCount = 5_000_000;
            const long mask = 0xFFFF;
            var sum = Enumerable.Zip( generatorA, generatorB, ( l1, l2 ) => (l1 & mask) == (l2 & mask) ? 1 : 0 ).Take( loopCount ).Sum();

            return sum.ToString();
        }
    }
}
