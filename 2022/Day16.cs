using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day16 : IAdventDay
{
    public string Name => "16. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day16.txt");

    private class Valve
    {
        public static Valve FromString(string input)
        {
            var match = System.Text.RegularExpressions.Regex.Match(input, @"Valve (.*) has flow rate=(\d+); tunnels? leads? to valves? (.*)");

            return new Valve
            {
                Name = match.Groups[1].Value,
                Rate = int.Parse(match.Groups[2].Value),
                Neighbours = match.Groups[3].Value.Split(", ").ToHashSet(),
            };
        }

        public string Name { get; private init; }

        public int Rate { get; private init; }

        public ISet<string> Neighbours { get; private init; }
    }

    private readonly struct State
    {
        public State(string position, long open)
            : this()
        {
            Position = position;
            Open = open;
        }

        public string Position { get; }

        public string ElephantPosition { get; init; }

        public long Open { get; }

        public int Time { get; init; }

        public int TotalFlow { get; init; }

        public int RateSum { get; init; }

        public override bool Equals(object obj) => obj is State other && other.Position == Position && other.ElephantPosition == ElephantPosition && other.Open == Open;

        public override int GetHashCode() => 17 * Position.GetHashCode() + 31 * Open.GetHashCode() + 41 * ElephantPosition.GetHashCode();
    }

    private int Simulate(int totalTime, bool usesElephant)
    {
        var valves = GetInput().SplitToLines().Select(Valve.FromString).ToDictionary(v => v.Name);
        int totalRate = valves.Values.Sum(v => v.Rate);
        var masks = valves.Keys.Select((s, i) => (Name: s, Mask: 1L << i)).ToDictionary(i => i.Name, i => i.Mask);
        var startState = new State("AA", 0L) { ElephantPosition = "AA" };
        long fullMask = masks.Values.Aggregate(0L, (cur, next) => cur | next);

        var flows = new Dictionary<State, int>
        {
            [startState] = 0,
        };

        var states = new Heap<State>();

        states.Push(startState, 0);

        while (states.Count > 0)
        {
            var state = states.Pop();

            if (state.Time == totalTime)
                return state.TotalFlow;

            int curFlow = state.TotalFlow + state.RateSum;

            foreach (var agentTransition in GetTransitions(state, state.Position, isElephant: false))
            {
                foreach (var elephantTransition in GetTransitions(state, state.ElephantPosition, isElephant: true))
                {
                    if (agentTransition.Rate > 0 && elephantTransition.Rate > 0 && agentTransition.Destination == elephantTransition.Destination)
                        continue;

                    var nextState = new State(agentTransition.Destination ?? state.Position, state.Open | agentTransition.Mask | elephantTransition.Mask)
                    {
                        ElephantPosition = elephantTransition.Destination,
                        Time = state.Time + 1,
                        TotalFlow = curFlow,
                        RateSum = state.RateSum + agentTransition.Rate + elephantTransition.Rate,
                    };

                    if (!flows.TryGetValue(nextState, out int nextFlow) || nextFlow < curFlow)
                    {
                        int estimate = GetEstimate(nextState.Open, nextState.Time) + nextState.RateSum * (totalTime - nextState.Time);

                        flows[nextState] = curFlow;
                        states.Push(nextState, -(curFlow + estimate));
                    }
                }
            }
        }

        return 0;

        IEnumerable<(string Destination, int Rate, long Mask)> GetTransitions(State state, string position, bool isElephant)
        {
            if (isElephant && !usesElephant)
            {
                yield return (position, 0, 0);

                yield break;
            }

            var valve = valves[position];

            if ((state.Open & masks[position]) == 0 && valve.Rate > 0)
                yield return (position, valve.Rate, masks[valve.Name]);

            foreach (string neighbour in valves[position].Neighbours)
                yield return (neighbour, 0, 0);
        }

        int GetEstimate(long open, int time)
        {
            long closed = fullMask & ~open;
            int remainingTime = totalTime - time;

            return valves.Values
                .Where(v => (masks[v.Name] & closed) != 0)
                .OrderByDescending(v => v.Rate)
                .Chunk(usesElephant ? 2 : 1)
                .Take(remainingTime / 2)
                .Select(a => a.Sum(v => v.Rate))
                .Aggregate((Rate: 0, Time: remainingTime), (sum, rate) => (sum.Rate + rate * sum.Time, sum.Time - 2), i => i.Rate);
        }
    }

    public string Solve() => Simulate(totalTime: 30, usesElephant: false).ToString();

    public string SolveAdvanced() => Simulate(totalTime: 26, usesElephant: true).ToString();
}
