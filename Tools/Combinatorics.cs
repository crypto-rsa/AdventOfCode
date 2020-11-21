using System;
using System.Collections.Generic;
using System.Linq;

namespace Tools
{
    /// <summary>
    /// Contains combinatorics methods
    /// </summary>
    public static class Combinatorics
    {
        #region Methods

        /// <summary>
        /// Calculates the factorial of a number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static long Factorial( long number )
        {
            if( number < 0 || number > 20 )
                throw new ArgumentOutOfRangeException( nameof( number ) );

            if( number <= 1 )
                return 1;

            return number * Factorial( number - 1 );
        }

        /// <summary>
        /// Calculates the greatest common divisor of a collection of numbers
        /// </summary>
        /// <param name="numbers">The numbers to find the GCD of</param>
        /// <returns>The factorization of the GCD of <paramref name="numbers"/></returns>
        public static Factorization GCD( params long[] numbers )
        {
            if( numbers == null )
                throw new ArgumentNullException( nameof( numbers ) );

            if( numbers.Length < 1 )
                throw new ArgumentException( "At least one number is required for finding the greatest common divisor!", nameof( numbers ) );

            var factorizations = numbers.Select( n => Factorization.Of( n ) ).ToList();
            var allPrimes = new HashSet<long>( factorizations.SelectMany( d => d.Keys ) );

            return new Factorization( allPrimes.Select( p => (p, factorizations.Min( f => f.GetExponent( p ) )) ) );
        }

        /// <summary>
        /// Calculates the least common multiple of a collection of numbers
        /// </summary>
        /// <param name="numbers">The numbers to find the LCM of</param>
        /// <returns>The factorization of the LCM of <paramref name="numbers"/></returns>
        public static Factorization LCM(params long[] numbers)
        {
            if (numbers == null)
                throw new ArgumentNullException(nameof(numbers));

            if (numbers.Length < 1)
                throw new ArgumentException("At least one number is required for finding the least common multiple!", nameof(numbers));

            var factorizations = numbers.Select(n => Factorization.Of(n)).ToList();
            var allPrimes = new HashSet<long>(factorizations.SelectMany(d => d.Keys));

            return new Factorization(allPrimes.Select(p => (p, factorizations.Max(f => f.GetExponent(p)))));
        }

        /// <summary>
        /// Calculates a binomial coefficient C(n, k)
        /// </summary>
        /// <param name="n">The number of items to select from</param>
        /// <param name="k">The number of items to select</param>
        /// <returns>A binomial coefficient C(n, k), ie. the number of ways to choose <paramref name="k"/> items from <paramref name="n"/> distinct ones</returns>
        public static Factorization Binomial( int n, int k )
        {
            return Factorization.OfFactorial( n ) / (Factorization.OfFactorial( k ) * Factorization.OfFactorial( n - k ));
        }

        #endregion
    }
}
