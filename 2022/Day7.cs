using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day7 : IAdventDay
{
    public string Name => "7. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day7.txt");

    private class Dir
    {
        public Dir(string name, Dir parent)
        {
            Name = name;
            Parent = parent;
        }

        public string Name { get; }

        public Dir Parent { get; }

        private long? TotalSize { get; set; }

        public List<Dir> Dirs { get; } = new();

        public List<(string Name, long Size)> Files { get; } = new();

        public long GetTotalSize()
        {
            TotalSize ??= Dirs.Sum(d => d.GetTotalSize()) + Files.Sum(f => f.Size);

            return TotalSize.Value;
        }

        public IEnumerable<Dir> GetAllDirs()
        {
            foreach (var subDir in Dirs.SelectMany(d => d.GetAllDirs()))
            {
                yield return subDir;
            }

            yield return this;
        }
    }

    private static Dir ReadTree()
    {
        var root = new Dir("/", null);
        Dir currentDir = null;

        foreach (string command in GetInput().SplitToLines())
        {
            var parts = command.Split(' ');

            switch (parts[0])
            {
                case "$" when parts[1] == "cd":
                    currentDir = parts[2] switch
                    {
                        "/" => root,
                        ".." => currentDir!.Parent,
                        _ => currentDir!.Dirs.First(d => d.Name == parts[2]),
                    };

                    break;

                case "dir":
                    currentDir!.Dirs.Add(new Dir(parts[1], currentDir));

                    break;

                default:
                {
                    if (parts[0] != "$")
                    {
                        currentDir!.Files.Add((parts[1], long.Parse(parts[0])));
                    }

                    break;
                }
            }
        }

        root.GetTotalSize();

        return root;
    }

    public string Solve() => ReadTree()
        .GetAllDirs()
        .Where(d => d.GetTotalSize() <= 100_000)
        .Sum(d => d.GetTotalSize())
        .ToString();

    public string SolveAdvanced()
    {
        var root = ReadTree();
        long unused = 70_000_000 - root.GetTotalSize();
        long required = 30_000_000 - unused;

        return root
            .GetAllDirs()
            .Where(d => d.GetTotalSize() >= required)
            .Select(d => d.GetTotalSize())
            .Min()
            .ToString();
    }
}
