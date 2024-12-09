﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tools;

public class Grid<T> : IEnumerable<GridPosition>
{
    #region Fields

    private readonly T[][] _grid;

    #endregion

    #region Constructors

    private Grid(T[][] grid)
    {
        _grid = grid;
    }

    #endregion

    #region Properties

    public int Height => _grid.Length;

    public int Width => _grid[0].Length;

    public T this[GridPosition position] => _grid[position.Row][position.Column];

    #endregion

    #region Methods

    public static Grid<int> ParseAsIntegers(string input) => new( input.ParseAsGrid(c => int.Parse(c.ToString())) );

    public static Grid<char> ParseAsCharacters(string input) => new( input.ParseAsGrid(c => c) );

    public bool IsValid(GridPosition position) => position.Row >= 0 && position.Row < Height && position.Column >= 0 && position.Column < Width;

    public IEnumerable<GridPosition> FindAll(T value)
    {
        for (var row = 0; row < Height; row++)
        {
            for (var column = 0; column < Width; column++)
            {
                if (Equals(_grid[row][column], value))
                {
                    yield return new GridPosition(row, column);
                }
            }
        }
    }

    public IEnumerable<GridPosition> GetNeighbours(GridPosition position, bool includeDiagonals)
    {
        return GetOffsets()
            .Select(offset => new GridPosition(position.Row + offset.RowOffset, position.Column + offset.ColOffset))
            .Where(IsValid);

        IEnumerable<(int RowOffset, int ColOffset)> GetOffsets()
        {
            yield return (0, -1);
            yield return (-1, 0);
            yield return (0, +1);
            yield return (+1, 0);

            if (includeDiagonals)
            {
                yield return (-1, -1);
                yield return (-1, +1);
                yield return (+1, -1);
                yield return (+1, +1);
            }
        }
    }

    #endregion

    #region IEnumerable<GridPosition>

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<GridPosition> GetEnumerator()
    {
        for (var row = 0; row < Height; row++)
        {
            for (var column = 0; column < Width; column++)
            {
                yield return new GridPosition(row, column);
            }
        }
    }

    #endregion
}

public record GridPosition(int Row, int Column)
{
    #region Properties

    public GridPosition Above => this with { Row = Row - 1 };

    public GridPosition Below => this with { Row = Row + 1 };

    public GridPosition Left => this with { Column = Column - 1 };

    public GridPosition Right => this with { Column = Column + 1 };

    #endregion
}
