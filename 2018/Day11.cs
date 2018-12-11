using System.Linq;

namespace Advent_of_Code.Year2018
{
    public class Day11 : IAdventDay
    {
        public string Name => "11. 12. 2018";

        private const int FieldSize = 300;

        private const int Input = 9810;

        private int[,] ConstructArray()
        {
            var array = new int[FieldSize + 1, FieldSize + 1];

            for (int i = 1; i <= FieldSize; i++)
            {
                for (int j = 1; j <= FieldSize; j++)
                {
                    int rackID = i + 10;
                    int fuel = (rackID * j + Input) * rackID;
                    int hundreds = (fuel % 1000) / 100;
                    array[i, j] = hundreds - 5;
                }
            }

            return array;
        }

        private int[,] SumArray()
        {
            var array = ConstructArray();
            var sums = new int[FieldSize + 1, FieldSize + 1];
            sums[1, 1] = array[1, 1] + array[1, 2] + array[1, 3] + array[2, 1] + array[2, 2] + array[2, 3] + array[3, 1] + array[3, 2] + array[3, 3];

            for (int i = 2; i <= FieldSize - 3; i++)
            {
                sums[1, i] = sums[1, i - 1] + Enumerable.Range(0, 3).Sum(k => array[1 + k, i + 2] - array[1 + k, i - 1]);
            }

            for (int i = 2; i <= FieldSize - 3; i++)
            {
                for (int j = 1; j <= FieldSize - 3; j++)
                {
                    sums[i, j] = sums[i - 1, j] + Enumerable.Range(0, 3).Sum(k => array[i + 2, j + k] - array[i - 1, j + k]);
                }
            }

            return sums;
        }

        public string Solve()
        {
            var sums = SumArray();
            var maxCoordinates = (X: 0, Y: 0);
            int max = int.MinValue;

            for (int i = 1; i <= FieldSize - 3; i++)
            {
                for (int j = 1; j <= FieldSize; j++)
                {
                    if (sums[i, j] >= max)
                    {
                        max = sums[i, j];
                        maxCoordinates = (i, j);
                    }
                }
            }

            return $"{maxCoordinates.X},{maxCoordinates.Y}";
        }

        public string SolveAdvanced()
        {
            var array = ConstructArray();
            var sums = new int[FieldSize + 1, FieldSize + 1];
            var maxCoordinates = (X: 0, Y: 0, Size: 1);
            int max = int.MinValue;

            for (int squareSize = 1; squareSize <= 300;squareSize++)
            {
                for (int i = 1; i <= FieldSize - squareSize;i++)
                {
                    for (int j = 1; j <= FieldSize - squareSize;j++)
                    {
                        sums[i, j] += Enumerable.Range(0, squareSize).Sum(k => array[i + k, j + squareSize - 1] + array[i + squareSize - 1, j + k]) - array[i + squareSize - 1, j + squareSize - 1];

                        if(sums[i,j]>max)
                        {
                            max = sums[i, j];
                            maxCoordinates = (i, j, squareSize);
                        }
                    }
                }
            }

                return $"{maxCoordinates.X},{maxCoordinates.Y},{maxCoordinates.Size}";
        }
    }
}