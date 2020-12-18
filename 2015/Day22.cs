using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2015
{
    public class Day22 : IAdventDay
    {
        private class SharedState
        {
            public int BestScore { get; set; } = int.MaxValue;

            public bool ApplyPenalty { get; init; }
        }

        private class PlayerState
        {
            private const int WeaponAttack = -1;
            private const int MagicMissile = 0;
            private const int Drain = 1;
            private const int Shield = 2;
            private const int Poison = 3;
            private const int Recharge = 4;

            private static readonly Dictionary<int, (int Cost, int Length)> _spells = new()
            {
                [MagicMissile] = (53, 0),
                [Drain] = (73, 0),
                [Shield] = (113, 6),
                [Poison] = (173, 6),
                [Recharge] = (229, 5),
            };

            private readonly int[] _spellTimer = new int[5];

            public PlayerState()
            {
            }

            public PlayerState(PlayerState other)
            {
                _spellTimer = other._spellTimer.ToArray();
                HitPoints = other.HitPoints;
                Mana = other.Mana;
                Damage = other.Damage;
                Armor = other.Armor;
                ManaSpent = other.ManaSpent;
                AttacksWithWeapon = other.AttacksWithWeapon;
            }

            public int HitPoints { get; set; }

            public int Mana { get; set; }

            public int Damage { get; set; }

            public int Armor { get; private set; }

            public int ManaSpent { get; private set; }

            public bool AttacksWithWeapon { get; init; }

            public bool IsAlive => HitPoints > 0 && GetAvailableActions().Any();

            public void ApplyImmediateAction(int action, PlayerState opponent)
            {
                if (action == WeaponAttack)
                {
                    opponent.HitPoints -= System.Math.Max(1, Damage - opponent.Armor);
                }
                else if (action == MagicMissile)
                {
                    opponent.HitPoints -= 4;
                }
                else if (action == Drain)
                {
                    HitPoints += 2;
                    opponent.HitPoints -= 2;
                }
            }

            public void ApplyLongRunningActions(PlayerState opponent)
            {
                if (_spellTimer[Shield] > 0)
                {
                    Armor = 7;
                    _spellTimer[Shield]--;
                }
                else
                {
                    Armor = 0;
                }

                if (_spellTimer[Poison] > 0)
                {
                    opponent.HitPoints -= 3;
                    _spellTimer[Poison]--;
                }

                if (_spellTimer[Recharge] > 0)
                {
                    Mana += 101;
                    _spellTimer[Recharge]--;
                }
            }

            public void InitializeAction(int action)
            {
                if (!_spells.TryGetValue(action, out var spell))
                    return;

                Mana -= spell.Cost;
                ManaSpent += spell.Cost;
                _spellTimer[action] = spell.Length;
            }

            public IEnumerable<int> GetAvailableActions()
            {
                if (AttacksWithWeapon)
                {
                    yield return WeaponAttack;
                    yield break;
                }

                foreach (var spell in _spells.Where(p => p.Value.Cost <= Mana && _spellTimer[p.Key] == 0))
                {
                    yield return spell.Key;
                }
            }
        }

        private class GameState
        {
            private readonly PlayerState[] _players;

            private readonly int _nextPlayer;

            public int Score => _players[1].IsAlive ? int.MaxValue : _players[0].ManaSpent;

            public SharedState SharedState { get; }

            public GameState(int nextPlayer, SharedState sharedState, params PlayerState[] players)
            {
                _nextPlayer = nextPlayer;
                _players = players;
                SharedState = sharedState;
            }

            public void DoTurn()
            {
                if (_nextPlayer == 0 && SharedState.ApplyPenalty)
                {
                    _players[0].HitPoints--;

                    if (_players[0].HitPoints <= 0)
                        return;
                }

                _players[1 - _nextPlayer].ApplyLongRunningActions(_players[_nextPlayer]);
                _players[_nextPlayer].ApplyLongRunningActions(_players[1 - _nextPlayer]);

                if (_players[0].ManaSpent >= SharedState.BestScore)
                    return;

                if (_players.Any(p => !p.IsAlive))
                {
                    if(Score < SharedState.BestScore)
                    {
                        SharedState.BestScore = Score;
                    }
                    
                    return;
                }

                foreach (int action in _players[_nextPlayer].GetAvailableActions())
                {
                    var newPlayers = _players.Select(p => new PlayerState(p)).ToArray();

                    newPlayers[_nextPlayer].InitializeAction(action);
                    newPlayers[_nextPlayer].ApplyImmediateAction(action, newPlayers[1 - _nextPlayer]);

                    new GameState(1 - _nextPlayer, SharedState, newPlayers).DoTurn();
                }
            }
        }

        public string Name => "22. 12. 2015";

        private static string GetInput() => System.IO.File.ReadAllText("2015/Resources/day22.txt");

        public string Solve()
        {
            var sharedState = new SharedState();
            Initialize(sharedState).DoTurn();

            return sharedState.BestScore.ToString();
        }

        public string SolveAdvanced()
        {
            var sharedState = new SharedState() { ApplyPenalty = true };
            Initialize(sharedState).DoTurn();

            return sharedState.BestScore.ToString();
        }

        private static GameState Initialize(SharedState sharedState)
        {
            var me = new PlayerState()
            {
                Mana = 500,
                HitPoints = 50
            };

            var match = System.Text.RegularExpressions.Regex.Match(GetInput(), "Hit Points: (\\d+)\r\nDamage: (\\d+)");

            var boss = new PlayerState()
            {
                HitPoints = int.Parse(match.Groups[1].Value),
                Damage = int.Parse(match.Groups[2].Value),
                AttacksWithWeapon = true,
            };

            return new GameState(0, sharedState, me, boss);
        }
    }
}
