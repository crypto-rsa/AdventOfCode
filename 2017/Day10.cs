using System.Linq;

namespace Advent_of_Code.Year2017
{
    public class Day10 : IAdventDay
    {
        private const string Input = "102,255,99,252,200,24,219,57,103,2,226,254,1,0,69,216";

        public string Name => "10. 12. 2017";

        public string Solve()
        {
            const int listLength = 256;
            var lengths = Input.Split( ',' ).Select( s => int.Parse( s ) ).ToList();

            int currentPosition = 0;
            int skipSize = 0;

            var list = Enumerable.Range( 0, listLength ).ToList();

            foreach( int length in lengths )
            {
                for( int i = 0; i < length / 2; i++ )
                {
                    int startIndex = (currentPosition + i) % listLength;
                    int endIndex = (currentPosition + length - i - 1) % listLength;

                    int temp = list[startIndex];
                    list[startIndex] = list[endIndex];
                    list[endIndex] = temp;
                }

                currentPosition = (currentPosition + length + skipSize) % listLength;
                skipSize++;
            }

            return (list[0] * list[1]).ToString();
        }

        public string SolveAdvanced() => GetHash(Input);

        public static string GetHash(string input)
        {
            const int listLength = 256;

            var lengths = input.Trim().Select( c => (int) c ).Concat( new int[] { 17, 31, 73, 47, 23 } ).ToList();
            int currentPosition = 0;
            int skipSize = 0;

            var list = Enumerable.Range( 0, listLength ).ToList();

            for( int round = 0; round < 64; round++ )
            {
                foreach( int length in lengths )
                {
                    for( int i = 0; i < length / 2; i++ )
                    {
                        int startIndex = (currentPosition + i) % listLength;
                        int endIndex = (currentPosition + length - i - 1) % listLength;

                        int temp = list[startIndex];
                        list[startIndex] = list[endIndex];
                        list[endIndex] = temp;
                    }

                    currentPosition = (currentPosition + length + skipSize) % listLength;
                    skipSize++;
                }
            }

            var denseHash = Enumerable.Range( 0, listLength / 16 ).Select( i => list.GetRange( i * 16, 16 ).Aggregate( 0, ( cur, next ) => cur ^ next ) ).ToList();
            var output = string.Join( string.Empty, denseHash.Select( i => i.ToString( "x2" ) ) );

            return output;
        }
    }
}
