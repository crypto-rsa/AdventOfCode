namespace Advent_of_Code
{
    public interface IAdventDay
    {
        string Name { get; }

        string Solve();

        string SolveAdvanced();
    }
}