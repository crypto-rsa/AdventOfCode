using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2018
{
    public class Day14 : IAdventDay
    {
        private class Iterator
        {
            private readonly int[] _indices = new int[] { 0, 1 };

            private readonly byte[][] _digits = new byte[][]
            {
                new byte[] { 0 },
                new byte[] { 1 },
                new byte[] { 2 },
                new byte[] { 3 },
                new byte[] { 4 },
                new byte[] { 5 },
                new byte[] { 6 },
                new byte[] { 7 },
                new byte[] { 8 },
                new byte[] { 9 },
                new byte[] { 1, 0 },
                new byte[] { 1, 1 },
                new byte[] { 1, 2 },
                new byte[] { 1, 3 },
                new byte[] { 1, 4 },
                new byte[] { 1, 5 },
                new byte[] { 1, 6 },
                new byte[] { 1, 7 },
                new byte[] { 1, 8 },
                new byte[] { 1, 9 },
            };

            private readonly byte[] _inputDigits = new byte[] { 9, 2, 0, 8, 3, 1 };

            private int _inputDigitsNextCheckIndex;

            private int _inputDigitsNextCheckPosition;

            public List<byte> Recipes { get; } = new List<byte>(Input + 20) { 3, 7 };

            public int InputDigitsFoundAt { get; private set; } = -1;

            public void Step()
            {
                int newNumber = Recipes[_indices[0]] + Recipes[_indices[1]];
                Recipes.AddRange(_digits[newNumber]);

                _indices[0] = (_indices[0] + Recipes[_indices[0]] + 1) % Recipes.Count;
                _indices[1] = (_indices[1] + Recipes[_indices[1]] + 1) % Recipes.Count;

                for (; ; _inputDigitsNextCheckPosition++)
                {
                    if (InputDigitsFoundAt >= 0)
                        break;

                    if (_inputDigitsNextCheckPosition >= Recipes.Count)
                        break;

                    if (Recipes[_inputDigitsNextCheckPosition] == _inputDigits[_inputDigitsNextCheckIndex])
                    {
                        if (++_inputDigitsNextCheckIndex == _inputDigits.Length)
                        {
                            InputDigitsFoundAt = _inputDigitsNextCheckPosition - _inputDigits.Length + 1;
                        }
                    }
                    else
                    {
                        _inputDigitsNextCheckIndex = 0;
                    }
                }
            }
        }

        public string Name => "14. 12. 2018";

        private const int Input = 920831;

        public string Solve()
        {
            var iterator = new Iterator();

            while (iterator.Recipes.Count < Input + 10)
            {
                iterator.Step();
            }

            return string.Join(string.Empty, iterator.Recipes.Skip(Input).Take(10).Select(b => b.ToString()));
        }

        public string SolveAdvanced()
        {
            var iterator = new Iterator();

            while (iterator.InputDigitsFoundAt < 0)
            {
                iterator.Step();
            }

            return iterator.InputDigitsFoundAt.ToString();
        }
    }
}