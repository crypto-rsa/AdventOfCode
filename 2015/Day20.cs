namespace Advent_of_Code.Year2015
{
    public class Day20 : IAdventDay
    {
        public string Name => "20. 12. 2015";

        private const int Input = 36000000;

        public string Solve() => GetNumberWithDivisorSum(10, int.MaxValue);

        public string SolveAdvanced() => GetNumberWithDivisorSum(11, 50);

        private static string GetNumberWithDivisorSum(int presentsPerHouse, int maxHouses)
        {
            long upperBound = Input / presentsPerHouse;

            var divisors = new long[upperBound + 1];
            long minimum = long.MaxValue;

            for (long d = 1; d <= upperBound; d++)
            {
                for (long i = d, k = 1; k <= maxHouses && i <= upperBound && i <= minimum; i += d, k++)
                {
                    divisors[i] += presentsPerHouse * d;

                    if (divisors[i] >= Input)
                    {
                        if (i < minimum)
                        {
                            minimum = i;
                        }

                        if (d >= minimum)
                            break;
                    }
                }

                if (d >= minimum)
                    return minimum.ToString();
            }

            return string.Empty;
        }
    }
}
