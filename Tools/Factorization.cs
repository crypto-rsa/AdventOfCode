using System;
using System.Collections.Generic;
using System.Linq;

namespace Tools
{
    /// <summary>
    /// Represents a factorization of a number
    /// </summary>
    public class Factorization : SortedDictionary<long, int>
    {
        #region Fields

        /// <summary>
        /// The empty factorization (ie. factorization of 1)
        /// </summary>
        private static Factorization _empty = new Factorization(Enumerable.Empty<(long, int)>());

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs the factorization from a sequence of primes and their exponents
        /// </summary>
        /// <param name="factors">The factors represented as a tuples of (prime, exponent)</param>
        public Factorization(IEnumerable<(long Prime, int Exponent)> factors)
        {
            foreach (var item in factors.Where(t => t.Exponent > 0))
            {
                Add(item.Prime, item.Exponent);
            }
        }

        #endregion

        #region Overrides

        public override int GetHashCode()
        {
            return Count;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Factorization;
            if (other == null)
                return false;

            if (!this.OrderBy(i => i.Key).SequenceEqual(other.OrderBy(i => i.Key)))
                return false;

            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the factorization of a number
        /// </summary>
        /// <param name="number">The number to get the factorization of</param>
        /// <returns>The factorization of <paramref name="number"/></returns>
        public static Factorization Of(long number)
        {
            if (number < 1)
                throw new ArgumentOutOfRangeException(nameof(number));

            if (number == 1)
                return Empty;

            return new Factorization(Factor(number));
        }

        public static Factorization[] AllUpTo(long number)
        {
            if (number < 1)
                throw new ArgumentOutOfRangeException(nameof(number));

            var primes = Enumerable.Range(1, (int)number + 1).Select(_ => new List<long>()).ToArray();

            long max = (long)Math.Ceiling(Math.Sqrt(number));

            foreach (var prime in Primes.GetPrimes(max))
            {
                for (int p = (int)prime; p <= number; p *= (int)prime)
                {
                    for (int j = p; j <= number; j += p)
                    {
                        primes[j].Add(prime);
                    }
                }
            }

            return primes.Select(list => list.GroupBy(l => l).Select(g => (g.Key, g.Count()))).Select(f => new Factorization(f)).ToArray();
        }

        /// <summary>
        /// Factors a number
        /// </summary>
        /// <param name="number">The number to factor</param>
        /// <returns>A sequence of (prime, exponent) tuples representing the prime factorization</returns>
        private static IEnumerable<(long prime, int exponent)> Factor(long number)
        {
            long remaining = number;
            long max = (long)Math.Ceiling(Math.Sqrt(number));

            foreach (var prime in Primes.GetPrimes(max))
            {
                if (remaining < 2)
                    yield break;

                int exponent = 0;
                while (remaining % prime == 0)
                {
                    remaining /= prime;
                    exponent++;
                }

                if (exponent > 0)
                {
                    yield return (prime, exponent);
                }
            }

            if (remaining > 1)
            {
                yield return (remaining, 1);
            }
        }

        /// <summary>
        /// Returns the factorization of a factorial
        /// </summary>
        /// <param name="number">The number for which the factorization of a factorial will be calculated</param>
        /// <returns>The factorization of <paramref name="number"/>!</returns>
        public static Factorization OfFactorial(int number)
        {
            return Enumerable.Range(1, number).Select(n => Of(n)).Aggregate(Empty, (prev, cur) => prev * cur);
        }

        /// <summary>
        /// Converts the factorization to a <see cref="long"/> number
        /// </summary>
        /// <returns>The numeric representation of this factorization</returns>
        public long ToNumber()
        {
            return Factors.Aggregate(1L, (prev, cur) => prev * Numbers.Pow(cur.Prime, cur.Exponent));
        }

        /// <summary>
        /// Returns the exponent for a prime in this factorization
        /// </summary>
        /// <param name="prime">The prime to get the exponent for</param>
        /// <returns>The exponent for <paramref name="prime"/> (0 if not present in this factorization)</returns>
        public int GetExponent(long prime)
        {
            if (!TryGetValue(prime, out int exponent))
                return 0;

            return exponent;
        }

        public Factorization GetPrimeFactor(long prime) => new( Enumerable.Repeat((prime, GetExponent(prime)), 1) );

        /// <summary>
        /// Returns a collection of factorizations of the divisors of the current number
        /// </summary>
        public IEnumerable<Factorization> GetDivisors()
        {
            yield return Empty;

            if (Count == 0)
                yield break;

            var full = this.Select(kvp => (Prime: kvp.Key, Exponent: kvp.Value)).ToList();
            var current = new (long Prime, int Exponent)[full.Count];

            while (true)
            {
                int index = 0;
                for( ; index < current.Length; index++ )
                {
                    if (current[index].Exponent < full[index].Exponent)
                        break;
                }

                if (index >= current.Length)
                    yield break;

                current[index] = (full[index].Prime, current[index].Exponent + 1);

                for( int i = 0; i < index; i++ )
                {
                    current[i] = (full[i].Prime, 0);
                }

                yield return new Factorization(current);
            }
        }

        /// <summary>
        /// Returns a factorization of a power of the current number
        /// </summary>
        /// <param name="power">The exponent to apply</param>
        public Factorization ToPower(int power) => new Factorization(this.Select(kvp => (kvp.Key, power * kvp.Value)));

        private IEnumerable<(long Prime, int Exponent, int Remainder)> GetRootFactorization(int root) => this.Select(kvp => (kvp.Key, kvp.Value / root, kvp.Value % root));

        /// <summary>
        /// Checks whether the current number is a power of an integer
        /// </summary>
        /// <param name="power">The power to check</param>
        public bool IsPower(int power)
        {
            if (power == 0 && !Equals(Empty))
                return false;

            return GetRootFactorization(power).All(i => i.Remainder == 0);
        }

        public Factorization Root(int root)
        {
            var factorization = GetRootFactorization(root).ToList();

            if (factorization.Any(i => i.Remainder > 0))
                throw new InvalidOperationException($"The number is not a {root}. power of an integer!");

            return new Factorization(factorization.Select(i => (i.Prime, i.Exponent)));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the empty factorization (ie. the factorization of 1)
        /// </summary>
        public static Factorization Empty => _empty;

        /// <summary>
        /// Gets the collection of factors as a named tuple
        /// </summary>
        public IEnumerable<(long Prime, int Exponent)> Factors => this.Select(i => (i.Key, i.Value));

        #endregion

        #region Operators

        public static Factorization operator *(Factorization factors1, Factorization factors2)
        {
            return new Factorization(Enumerable.Concat(factors1, factors2).GroupBy(p => p.Key).Select(g => (g.Key, g.Sum(p => p.Value))));
        }

        public static Factorization operator /(Factorization dividend, Factorization divisor)
        {
            if (divisor.Factors.Any(f => f.Exponent > dividend.GetExponent(f.Prime)))
                throw new InvalidOperationException($"{nameof(dividend) } is not a multiple of {nameof(divisor) }!");

            return new Factorization(dividend.Factors.Select(f => (f.Prime, f.Exponent - divisor.GetExponent(f.Prime))));
        }

        #endregion
    }
}
