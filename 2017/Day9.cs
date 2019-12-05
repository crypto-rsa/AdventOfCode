namespace Advent_of_Code.Year2017
{
    public class Day9 : IAdventDay
    {
        private enum State
        {
            Start,
            InGroup,
            InGarbage,
        }

        public string Name => "9. 12. 2017";

        private static string GetInput() => System.IO.File.ReadAllText(@"2017/Resources/day9.txt");

        public string Solve()
        {
            return Process(out _).ToString();
        }

        private static int Process( out int garbageCharacters )
        {
            string input = GetInput();
            int totalScore = 0;
            var state = State.Start;
            int depth = 0;

            garbageCharacters = 0;

            for( int i = 0; i < input.Length; i++ )
            {
                char c = input[i];

                if( state == State.InGarbage )
                {
                    switch( c )
                    {
                        case '!':
                            i++;
                            break;

                        case '>':
                            state = depth == 0 ? State.Start : State.InGroup;
                            break;

                        default:
                            garbageCharacters++;
                            break;
                    }
                }
                else
                {
                    switch( c )
                    {
                        case '{':
                            state = State.InGroup;
                            depth++;
                            break;

                        case '}':
                            totalScore += depth;
                            depth--;
                            state = depth == 0 ? State.Start : State.InGroup;
                            break;

                        case '<':
                            state = State.InGarbage;
                            break;
                    }
                }
            }

            return totalScore;
        }

        public string SolveAdvanced()
        {
            Process( out int garbageCharacters );

            return garbageCharacters.ToString();
        }
    }
}
