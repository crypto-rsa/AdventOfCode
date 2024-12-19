using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tools;

public class Grid<T>(T[][] grid) : IEnumerable<GridPosition>
{
    #region Fields

    private readonly T[][] _grid = grid;

    #endregion

    #region Constructors

    public Grid(Grid<T> source)
        : this(source._grid.Select(row => row.ToArray()).ToArray())
    {
    }

    #endregion

    #region Properties

    public int Height => _grid.Length;

    public int Width => _grid[0].Length;

    public T this[GridPosition position]
    {
        get => _grid[position.Row][position.Column];
        set => _grid[position.Row][position.Column] = value;
    }

    public GridPosition TopLeft => new( 0, 0 );

    public GridPosition BottomRight => new( Height - 1, Width - 1 );

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

    public GridPosition Find(T value) => FindAll(value).First();

    public IEnumerable<GridPosition> GetNeighbours(GridPosition position, bool includeDiagonals = false)
    {
        return GetOffsets(includeDiagonals)
            .Select(offset => new GridPosition(position.Row + offset.RowOffset, position.Column + offset.ColOffset))
            .Where(IsValid);
    }

    private static IEnumerable<(int RowOffset, int ColOffset)> GetOffsets(bool includeDiagonals = false)
    {
        yield return (0, -1);
        yield return (-1, 0);
        yield return (0, +1);
        yield return (+1, 0);

        if (!includeDiagonals)
            yield break;

        yield return (-1, -1);
        yield return (-1, +1);
        yield return (+1, -1);
        yield return (+1, +1);
    }

    public override string ToString() => string.Join(System.Environment.NewLine, _grid.Select(row => string.Join(string.Empty, row)));

    public static GridPosition GetOffset(char instruction) => instruction switch
    {
        '<' => new GridPosition(0, -1),
        '^' => new GridPosition(-1, 0),
        '>' => new GridPosition(0, +1),
        'v' => new GridPosition(+1, 0),
        _ => throw new System.ArgumentException($"Invalid instruction: {instruction}", nameof( instruction ))
    };

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

    #region Overrides

    public override string ToString() => $"({Row}, {Column})";

    #endregion

    #region Methods

    public int DistanceTo(GridPosition other) => System.Math.Abs(Row - other.Row) + System.Math.Abs(Column - other.Column);

    #endregion

    #region Operators

    public static GridPosition operator +(GridPosition a, GridPosition b) => new( a.Row + b.Row, a.Column + b.Column );

    #endregion
}
