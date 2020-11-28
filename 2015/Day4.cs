using System.Security.Cryptography;

namespace Advent_of_Code.Year2015
{
    public class Day4 : IAdventDay
    {
        public string Name => "4. 12. 2015";

        private const string Input = "iwrupvqb";

        public string Solve()
        {
            var md5 = MD5.Create();

            for( int i = 1; ; i++ )
            {
                var bytes = md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(Input + i.ToString()));

                if (bytes[0] == 0 && bytes[1] == 0 && bytes[2] < 16)
                    return i.ToString();
            }
        }

        public string SolveAdvanced()
        {
            var md5 = MD5.Create();

            for (int i = 1; ; i++)
            {
                var bytes = md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(Input + i.ToString()));

                if (bytes[0] == 0 && bytes[1] == 0 && bytes[2] == 0)
                    return i.ToString();
            }
        }
    }
}
