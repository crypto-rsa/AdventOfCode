using System.Collections.Generic;
using System.IO;

namespace Advent_of_Code.Year2024;

public class Day4 : IAdventDay
{
    public string Name => "4. 12. 2024";

    private static string[] GetInput() => File.ReadAllLines("2024/Resources/day4.txt");

    private int GetValidXmasCount(string[] lines, int row, int col)
    {
        const string word = "XMAS";
        int count = 0;

        foreach ((int dRow, int dCol) in GetOffsets())
        {
            bool valid = true;
            int currentRow = row;
            int currentCol = col;

            foreach (var c in word)
            {
                if (currentRow < 0 || currentRow >= lines.Length || currentCol < 0 || currentCol >= lines[0].Length || lines[currentRow][currentCol] != c)
                {
                    valid = false;

                    break;
                }

                currentRow += dRow;
                currentCol += dCol;
            }

            if (valid)
            {
                count++;
            }
        }

        return count;

        IEnumerable<(int Row, int Column)> GetOffsets()
        {
            for (int row = -1; row <= +1; row++)
            {
                for (int col = -1; col <= +1; col++)
                {
                    if (row == 0 && col == 0)
                        continue;

                    yield return (row, col);
                }
            }
        }
    }

    private bool IsValidCrossMas(string[] lines, int row, int col)
    {
        if (row == 0 || row == lines.Length - 1 || col == 0 || col == lines[0].Length - 1)
            return false;

        bool mainDiagonal = (Matches(-1, -1, 'M') && Matches(+1, +1, 'S')) || (Matches(-1, -1, 'S') && Matches(+1, +1, 'M'));
        bool secondaryDiagonal = (Matches(-1, +1, 'M') && Matches(+1, -1, 'S')) || (Matches(-1, +1, 'S') && Matches(+1, -1, 'M'));

        return mainDiagonal && secondaryDiagonal;

        bool Matches(int rowOffset, int colOffset, char c) => lines[row + rowOffset][col + colOffset] == c;
    }

    public string Solve()
    {
        int total = 0;
        var lines = GetInput();

        for (int row = 0; row < lines.Length; row++)
        {
            for (int col = 0; col < lines[row].Length; col++)
            {
                if (lines[row][col] == 'X')
                {
                    total += GetValidXmasCount(lines, row, col);
                }
            }
        }

        return total.ToString();
    }

    public string SolveAdvanced()
    {
        int total = 0;
        var lines = GetInput();

        for (int row = 0; row < lines.Length; row++)
        {
            for (int col = 0; col < lines[row].Length; col++)
            {
                if (lines[row][col] == 'A' && IsValidCrossMas(lines, row, col))
                {
                    total++;
                }
            }
        }

        return total.ToString();
    }
}
