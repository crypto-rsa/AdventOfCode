using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day19 : IAdventDay
{
    public string Name => "19. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day19.txt");

    private class Blueprint
    {
        public static Blueprint FromString(string input)
        {
            var match = System.Text.RegularExpressions.Regex.Match(input,
                @"Blueprint (\d+): Each ore robot costs (\d+) ore. Each clay robot costs (\d+) ore. Each obsidian robot costs (\d+) ore and (\d+) clay. Each geode robot costs (\d+) ore and (\d+) obsidian.");

            int oreRobotCost = Parse(2);
            int clayRobotCost = Parse(3);
            int obsidianRobotOreCost = Parse(4);
            int obsidianRobotClayCost = Parse(5);
            int geodeRobotOreCost = Parse(6);
            int geodeRobotObsidianCost = Parse(7);

            return new Blueprint
            {
                Id = Parse(1),
                OreRobotCost = oreRobotCost,
                ClayRobotCost = clayRobotCost,
                ObsidianRobotOreCost = obsidianRobotOreCost,
                ObsidianRobotClayCost = obsidianRobotClayCost,
                GeodeRobotOreCost = geodeRobotOreCost,
                GeodeRobotObsidianCost = geodeRobotObsidianCost,
                MaxOreNeeded = System.Math.Max(oreRobotCost, System.Math.Max(clayRobotCost, System.Math.Max(obsidianRobotOreCost, geodeRobotOreCost))),
            };

            int Parse(int group) => int.Parse(match.Groups[group].Value);
        }

        public int Id { get; private init; }

        private int OreRobotCost { get; init; }

        private int ClayRobotCost { get; init; }

        private int ObsidianRobotOreCost { get; init; }

        private int ObsidianRobotClayCost { get; init; }

        private int GeodeRobotOreCost { get; init; }

        private int GeodeRobotObsidianCost { get; init; }

        private int MaxOreNeeded { get; init; }

        public IEnumerable<State> GetNeighbourStates(State state)
        {
            foreach ((bool buildOreRobot, bool buildClayRobot, bool buildObsidianRobot, bool buildGeodeRobot) in GetBuildFlags())
            {
                var nextState = state.GetNextState();

                if (buildOreRobot)
                {
                    yield return nextState with { Ore = nextState.Ore - OreRobotCost, OreRobots = nextState.OreRobots + 1 };
                }
                else if (buildClayRobot)
                {
                    yield return nextState with { Ore = nextState.Ore - ClayRobotCost, ClayRobots = nextState.ClayRobots + 1 };
                }
                else if (buildObsidianRobot)
                {
                    yield return nextState with { Ore = nextState.Ore - ObsidianRobotOreCost, Clay = nextState.Clay - ObsidianRobotClayCost, ObsidianRobots = nextState.ObsidianRobots + 1 };
                }
                else if (buildGeodeRobot)
                {
                    yield return nextState with { Ore = nextState.Ore - GeodeRobotOreCost, Obsidian = nextState.Obsidian - GeodeRobotObsidianCost, GeodeRobots = nextState.GeodeRobots + 1 };
                }
                else
                {
                    yield return nextState;
                }
            }

            IEnumerable<(bool, bool, bool, bool)> GetBuildFlags()
            {
                yield return (false, false, false, false);

                if (state.Ore >= OreRobotCost && state.OreRobots < MaxOreNeeded)
                    yield return (true, false, false, false);

                if (state.Ore >= ClayRobotCost)
                    yield return (false, true, false, false);

                if (state.Ore >= ObsidianRobotOreCost && state.Clay >= ObsidianRobotClayCost)
                    yield return (false, false, true, false);

                if (state.Ore >= GeodeRobotOreCost && state.Obsidian >= GeodeRobotObsidianCost)
                    yield return (false, false, false, true);
            }
        }
    }

    private readonly record struct State(int Minute, int OreRobots, int Ore, int ClayRobots, int Clay, int ObsidianRobots, int Obsidian, int GeodeRobots, int GeodesOpen)
    {
        public State GetNextState() => this with
        {
            Minute = Minute + 1,
            Ore = Ore + OreRobots,
            Clay = Clay + ClayRobots,
            Obsidian = Obsidian + ObsidianRobots,
            GeodesOpen = GeodesOpen + GeodeRobots,
        };

        public int GetEstimatedGeodesOpen(int totalMinutes)
        {
            int n = totalMinutes - Minute;

            return GeodesOpen + n * GeodeRobots + n * (n + 1) / 2;
        }
    }

    private int GetMaximumGeodes(Blueprint blueprint, int totalMinutes)
    {
        var startState = new State(0, 1, 0, 0, 0, 0, 0, 0, 0);

        var foundStates = new HashSet<State>();
        var openStates = new Heap<State>();

        openStates.Push(startState, 0);

        while (openStates.Count > 0)
        {
            var state = openStates.Pop();

            if (state.Minute == totalMinutes)
                return state.GeodesOpen;

            foreach (var nextState in blueprint.GetNeighbourStates(state))
            {
                if (foundStates.Add(nextState))
                {
                    openStates.Push(nextState, -nextState.GetEstimatedGeodesOpen(totalMinutes));
                }
            }
        }

        return 0;
    }

    public string Solve() => GetInput()
        .SplitToLines()
        .Select(Blueprint.FromString)
        .Select(b => (ID: b.Id, Max: GetMaximumGeodes(b, 24)))
        .Sum(i => i.ID * i.Max)
        .ToString();

    public string SolveAdvanced() => GetInput()
        .SplitToLines()
        .Take(3)
        .Select(Blueprint.FromString)
        .Select(b => GetMaximumGeodes(b, 32))
        .Aggregate(1L, (cur, next) => cur * next)
        .ToString();
}
