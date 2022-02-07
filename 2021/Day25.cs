using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2021;

public class Day25 : IAdventDay
{
    public string Name => "25. 12. 2021";

    private static char[][] GetInput() => File.ReadAllLines("2021/Resources/day25.txt").Select(s => s.ToArray()).ToArray();

    public string Solve()
    {
        var array = GetInput();
        var eastward = new HashSet<(int Row, int Col)>();
        var southward = new HashSet<(int Row, int Col)>();

        for (int row = 0; row < array.Length; row++)
        {
            for (int col = 0; col < array[row].Length; col++)
            {
                var set = array[row][col] switch
                {
                    '>' => eastward,
                    'v' => southward,
                    _ => null,
                };

                set?.Add((row, col));
            }
        }

        for (int step = 1;; step++)
        {
            var newEastward = new HashSet<(int Row, int Col)>();

            foreach (var position in eastward.ToList())
            {
                if (East(position, out var east) == '.')
                {
                    newEastward.Add(east);
                    eastward.Remove(position);
                }
            }

            foreach (var position in newEastward)
            {
                array[position.Row][position.Col] = '>';

                West(position) = '.';
            }

            var newSouthward = new HashSet<(int Row, int Col)>();

            foreach (var position in southward.ToList())
            {
                if (South(position, out var south) == '.')
                {
                    newSouthward.Add(south);
                    southward.Remove(position);
                }
            }

            foreach (var position in newSouthward)
            {
                array[position.Row][position.Col] = 'v';

                North(position) = '.';
            }

            if (!newEastward.Any() && !newSouthward.Any())
                return step.ToString();

            eastward.AddRange( newEastward );
            southward.AddRange( newSouthward );

            ref char East((int Row, int Col) position, out (int Row, int Col) east)
            {
                east = position.Col == array[position.Row].Length - 1 ? (position.Row, 0) : (position.Row, position.Col + 1);

                return ref array[east.Row][east.Col];
            }

            ref char West((int Row, int Col) position)
            {
                (int row, int col) = position.Col == 0 ? (position.Row, array[position.Row].Length - 1) : (position.Row, position.Col - 1);

                return ref array[row][col];
            }

            ref char South((int Row, int Col) position, out (int Row, int Col) south)
            {
                south = position.Row == array.Length - 1 ? (0, position.Col) : (position.Row + 1, position.Col);

                return ref array[south.Row][south.Col];
            }

            ref char North((int Row, int Col) position)
            {
                (int row, int col) = position.Row == 0 ? (array.Length - 1, position.Col) : (position.Row - 1, position.Col);

                return ref array[row][col];
            }
        }
    }

    public string SolveAdvanced() => "Merry Christmas!";
}
