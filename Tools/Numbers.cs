using System;
using System.Linq;

namespace Tools
{
    /// <summary>
    /// Contains methods for number manipulation
    /// </summary>
    public static class Numbers
    {
        #region Methods

        /// <summary>
        /// Performs fast exponentiation
        /// </summary>
        /// <param name="base">The base</param>
        /// <param name="exponent">The exponent</param>
        /// <returns><paramref name="base"/>^<paramref name="exponent"/></returns>
        public static long Pow( long @base, long exponent )
        {
            long result = 1;
            long temp = @base;
            long tempExponent = exponent;

            while( tempExponent > 0 )
            {
                if( (tempExponent & 1) == 1 )
                {
                    result *= temp;
                }

                temp *= temp;
                tempExponent >>= 1;
            }

            return result;
        }

        /// <summary>
        /// Returns a Fibonacci number
        /// </summary>
        /// <param name="n">The index of the Fibonacci number to return</param>
        /// <returns>F_n (1 for n = 1 and 2, etc.)</returns>
        public static long Fib(long n)
        {
            if (n < 1)
                throw new System.ArgumentOutOfRangeException(nameof(n), "The value must be greater or equal to 1!");

            if (n == 1 || n == 2)
                return 1;

            var array = new long[n + 1];
            array[1] = array[2] = 1;

            for (int i = 3; i <= n; i++)
            {
                array[i] = array[i - 1] + array[i - 2];
            }

            return array[n];
        }

        /// <summary>
        /// Solves a quadratic equation
        /// </summary>
        /// <param name="a">The quadratic factor</param>
        /// <param name="b">The linear factor</param>
        /// <param name="c">The constant factor</param>
        /// <returns>An array of real solutions of <paramref name="a"/>x^2 + <paramref name="b"/>x + <paramref name="c"/></returns>
        public static double[] SolveQuadratic(double a, double b, double c) => (b * b - 4 * a * c) switch
        {
            double d when d > 0.0 => new double[] { (-b + Math.Sqrt(d)) / (2 * a), (-b - Math.Sqrt(d)) / (2 * a) },
            double d when d == 0.0 => new double[] { (-b + Math.Sqrt(d)) / (2 * a) },
            _ => Array.Empty<double>(),
        };

        public static long GreatestCommonDivisor(long a, long b) => Combinatorics.GCD(Math.Abs(a), Math.Abs(b)).ToNumber();

        public static long LeastCommonMultiple(long a, long b) => Combinatorics.LCM(Math.Abs(a), Math.Abs(b)).ToNumber();

        public static long SolveCongruences((long Mod, long Rem)[] congruences)
        {
            long gcd = Combinatorics.GCD(congruences.Select(c => c.Mod).ToArray()).ToNumber();

            if (gcd == 1)
                return SolveCoprime(congruences);

            if (congruences.Select(c => c.Rem % gcd).Distinct().Count() > 1)
                throw new InvalidOperationException($"The remainders are not congruent modulo {gcd} (= the GCD of the moduli)!");

            var factorizations = congruences.Select(c => Factorization.Of(c.Mod)).ToArray();
            var primes = factorizations.SelectMany(f => f.Factors.Select(i => i.Prime)).Distinct().ToArray();

            var subResults = primes.Select(GetPrimePowerSolution).ToArray();

            if (subResults.Any(i => i.Mod == 0))
                throw new InvalidOperationException("No solution exists!");

            return SolveCoprime(subResults);

            (long Mod, long Rem) GetPrimePowerSolution(long prime)
            {
                var candidates = factorizations
                    .Select((f, i) => (Mod: f.GetPrimeFactor(prime).ToNumber(), congruences[i].Rem))
                    .Where(i => i.Mod > 1)
                    .ToArray();

                long minPower = candidates.Min(i => i.Mod);

                if (candidates.Select(c => c.Rem % minPower).Distinct().Count() > 1)
                    return (0, 0);

                return candidates.MaxBy(i => i.Mod);
            }

            static long SolveCoprime((long Mod, long Rem)[] congruences)
            {
                long lcm = congruences.Aggregate(1L, (cur, next) => cur * next.Mod);
                var m = congruences.Select(i => lcm / i.Mod).ToArray();
                var inverse = new long[m.Length];

                for (int i = 0; i < m.Length; i++)
                {
                    for (int j = 1; j < congruences[i].Mod; j++)
                    {
                        if (m[i] * j % congruences[i].Mod == 1)
                        {
                            inverse[i] = j;

                            break;
                        }
                    }
                }

                return congruences.Select((c, i) => inverse[i] * c.Rem * m[i]).Sum() % lcm;
            }
        }

        #endregion
    }
}
