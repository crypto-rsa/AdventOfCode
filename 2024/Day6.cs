using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2024;

public class Day6 : IAdventDay
{
    public string Name => "6. 12. 2024";

    private static string GetInput() => File.ReadAllText("2024/Resources/day6.txt");

    public (HashSet<(int Row, int Col)> Visited, bool IsLoop) Simulate(char[][] map, int row, int col)
    {
        var direction = '^';
        var visited = new HashSet<(int, int)>();
        var loopCheck = new HashSet<(int, int, char)>();

        while (true)
        {
            if (!loopCheck.Add((row, col, direction)))
                return (visited, true);

            visited.Add((row, col));

            var offset = GetOffset(direction);
            (int nextRow, int nextCol) = (row + offset.RowOffset, col + offset.ColOffset);

            if (!IsValid(nextRow, nextCol))
                break;

            if (map[nextRow][nextCol] == '#')
            {
                direction = GetNextDirection(direction);

                continue;
            }

            (row, col) = (nextRow, nextCol);
        }

        return (visited, false);

        char GetNextDirection(char current) => current switch
        {
            '^' => '>',
            '>' => 'v',
            'v' => '<',
            '<' => '^',
            _ => throw new System.InvalidOperationException()
        };

        (int RowOffset, int ColOffset) GetOffset(char d) => d switch
        {
            '^' => (-1, 0),
            '>' => (0, 1),
            'v' => (1, 0),
            '<' => (0, -1),
            _ => throw new System.InvalidOperationException()
        };

        bool IsValid(int r, int c) => r >= 0 && r < map.Length && c >= 0 && c < map[0].Length;
    }

    public string Solve()
    {
        var map = GetInput().ParseAsGrid(c => c);

        (int row, int col, _) = map.SelectMany((a, i) => a.Select((c, j) => (Row: i, Col: j, Char: c))).First(i => i.Char == '^');

        return Simulate(map, row, col).Visited.Count.ToString();
    }

    public string SolveAdvanced()
    {
        var map = GetInput().ParseAsGrid(c => c);

        (int startRow, int startCol, _) = map.SelectMany((a, i) => a.Select((c, j) => (Row: i, Col: j, Char: c))).First(i => i.Char == '^');
        var (originalVisited, _) = Simulate(map, startRow, startCol);

        int loops = 0;

        foreach ((int row, int col) in originalVisited)
        {
            if (map[row][col] == '^')
                continue;

            map[row][col] = '#';

            if (Simulate(map, startRow, startCol).IsLoop)
            {
                loops++;
            }

            map[row][col] = '.';
        }

        return loops.ToString();
    }
}
