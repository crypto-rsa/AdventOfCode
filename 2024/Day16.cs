using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;
using State = (Tools.GridPosition Position, char Orientation);

namespace Advent_of_Code.Year2024;

public class Day16 : IAdventDay
{
    public string Name => "16. 12. 2024";

    private static string GetInput() => File.ReadAllText("2024/Resources/day16.txt");

    private static int FindPath(out Dictionary<State, List<State>> ancestors, out HashSet<State> endStates)
    {
        const string orientations = "<^>v";

        var grid = Grid<char>.ParseAsCharacters(GetInput());
        var heap = new Heap<State>();
        var initialState = (grid.Find('S'), '>');
        var endPosition = grid.Find('E');
        ancestors = new Dictionary<State, List<State>>();

        int bestScore = -1;

        var scores = new Dictionary<State, int>
        {
            [initialState] = 0,
        };

        heap.Push(initialState, 0);

        while (heap.Count > 0)
        {
            var state = heap.Pop();
            var offset = Grid<char>.GetOffset(state.Orientation);

            if (grid[state.Position] == 'E' && (scores[state] < bestScore || bestScore < 0))
            {
                bestScore = scores[state];

                continue;
            }

            var score = scores[state];
            var nextPosition = state.Position + offset;

            if (grid[nextPosition] != '#')
            {
                var nextState = new State(nextPosition, state.Orientation);

                AddState(state, nextState, score + 1, ancestors);
            }

            int orientationIndex = orientations.IndexOf(state.Orientation);

            foreach (int orientationOffset in new[] { -1, +1 })
            {
                var nextOrientation = orientations[(orientationIndex + orientationOffset + orientations.Length) % orientations.Length];
                var nextState = new State(state.Position, nextOrientation);

                AddState(state, nextState, score + 1000, ancestors);
            }
        }

        endStates = scores.Where(kvp => kvp.Key.Position == endPosition && kvp.Value == bestScore).Select(kvp => kvp.Key).ToHashSet();

        return bestScore;

        void AddState(State curState, State nextState, int nextScore, Dictionary<State, List<State>> ancestors)
        {
            var alreadyFound = scores.TryGetValue(nextState, out int curScore);

            if (!alreadyFound || nextScore <= curScore)
            {
                if (!alreadyFound || nextScore < curScore)
                {
                    scores[nextState] = nextScore;
                    heap.Push(nextState, nextScore + nextState.Position.DistanceTo(endPosition));
                }

                if (!ancestors.TryGetValue(nextState, out var ancestorList) || nextScore < curScore)
                {
                    ancestorList = [];
                    ancestors[nextState] = ancestorList;
                }

                ancestorList.Add(curState);
            }
        }
    }

    public string Solve()
    {
        var score = FindPath(out _, out _);

        return score >= 0 ? score.ToString() : "No solution found!";
    }

    public string SolveAdvanced()
    {
        FindPath(out var ancestors, out var endStates);

        var bestTiles = new HashSet<GridPosition>();

        var queue = new Queue<State>();

        foreach (var state in endStates)
        {
            queue.Enqueue(state);
        }

        while (queue.Count > 0)
        {
            var state = queue.Dequeue();

            bestTiles.Add(state.Position);

            if (ancestors.TryGetValue(state, out var ancestorList))
            {
                foreach (var ancestor in ancestorList)
                {
                    queue.Enqueue(ancestor);
                }
            }
        }

        return bestTiles.Count.ToString();
    }
}
