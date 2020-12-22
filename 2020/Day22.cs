using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2020
{
    public class Day22 : IAdventDay
    {
        private class GameState : IEquatable<GameState>
        {
            private readonly List<int>[] _values;

            private readonly int _hashCode;

            public GameState(Queue<int>[] players)
            {
                _values = players.Select(q => q.ToList()).ToArray();
                _hashCode = _values.SelectMany(l => l).Aggregate(19, (acc, next) => 31 * acc + next);
            }

            public bool Equals(GameState other)
                => _values.Zip(other._values, (l1, l2) => (Old: l1, New: l2)).All(i => i.Old.SequenceEqual(i.New));

            public override bool Equals(object obj) => Equals(obj as GameState);

            public override int GetHashCode() => _hashCode;
        }

        public string Name => "22. 12. 2020";

        private static string GetInput() => System.IO.File.ReadAllText("2020/Resources/day22.txt");

        public string Solve()
        {
            var players = GetPlayers();

            PlayGame(players, null);

            return GetScore(players).ToString();
        }

        public string SolveAdvanced()
        {
            var players = GetPlayers();

            PlayGame(players, new Dictionary<GameState, int>());

            return GetScore(players).ToString();
        }

        private static Queue<int>[] GetPlayers()
        {
            return GetInput().Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries).Select(ToQueue).ToArray();

            static Queue<int> ToQueue(string text)
            {
                var queue = new Queue<int>();

                foreach (var line in text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Skip(1))
                {
                    queue.Enqueue(int.Parse(line));
                }

                return queue;
            }
        }

        private static int PlayGame(Queue<int>[] players, Dictionary<GameState, int> cache)
        {
            bool advancedRules = cache != null;
            var states = new HashSet<GameState>();
            var initialState = new GameState(players);

            if (advancedRules && cache.TryGetValue(initialState, out var winner))
                return winner;

            while (players[0].Count > 0 && players[1].Count > 0)
            {
                var state = new GameState(players);

                if (advancedRules && states.Contains(state))
                {
                    cache[initialState] = 0;

                    return 0;
                }

                states.Add(state);

                var card1 = players[0].Dequeue();
                var card2 = players[1].Dequeue();

                if (advancedRules && players[0].Count >= card1 && players[1].Count >= card2)
                {
                    winner = PlayGame(new Queue<int>[] { Copy(players[0], card1), Copy(players[1], card2) }, cache);
                }
                else
                {
                    winner = card1 > card2 ? 0 : 1;
                }

                if (winner == 0)
                {
                    players[0].Enqueue(card1);
                    players[0].Enqueue(card2);
                }
                else
                {
                    players[1].Enqueue(card2);
                    players[1].Enqueue(card1);
                }
            }

            winner = players[0].Count > players[1].Count ? 0 : 1;

            if (advancedRules)
            {
                cache[initialState] = winner;
            }

            return winner;

            static Queue<int> Copy(Queue<int> player, int count)
            {
                var queue = new Queue<int>();

                foreach (var value in player.Take(count))
                {
                    queue.Enqueue(value);
                }

                return queue;
            }
        }

        private static long GetScore(Queue<int>[] players)
        {
            var queue = players.Single(q => q.Count > 0);
            var count = queue.Count;

            return queue.Select((value, index) => (count - index) * (long) value).Sum();
        }
    }
}