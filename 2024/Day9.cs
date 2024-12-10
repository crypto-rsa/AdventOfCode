using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2024;

public class Day9 : IAdventDay
{
    public string Name => "9. 12. 2024";

    private static string GetInput() => File.ReadAllText("2024/Resources/day9.txt");

    private static (long[] DiskMap, int[] BlockLengths) CreateDiskMap()
    {
        var blockLengths = GetInput().Select(c => int.Parse(c.ToString())).ToArray();
        var diskMap = new long[blockLengths.Sum()];

        bool isFile = true;
        long fileId = 0;
        int position = 0;

        foreach (var blockLength in blockLengths)
        {
            for (int i = 0; i < blockLength; i++)
            {
                diskMap[position++] = isFile ? fileId : -1;
            }

            isFile = !isFile;

            if (isFile)
            {
                fileId++;
            }
        }

        return (diskMap, blockLengths);
    }

    public string Solve()
    {
        (long[] diskMap, _) = CreateDiskMap();

        int writePosition = 0;
        int readPosition = diskMap.Length - 1;

        while (true)
        {
            while (writePosition < diskMap.Length && diskMap[writePosition] != -1)
            {
                writePosition++;
            }

            while (readPosition >= 0 && diskMap[readPosition] == -1)
            {
                readPosition--;
            }

            if (writePosition >= readPosition)
                break;

            (diskMap[writePosition], diskMap[readPosition]) = (diskMap[readPosition], diskMap[writePosition]);
        }

        return diskMap
            .TakeWhile(i => i != -1)
            .Select((id, i) => id * i)
            .Sum()
            .ToString();
    }

    public string SolveAdvanced()
    {
        (long[] diskMap, int[] blockLengths) = CreateDiskMap();

        int sourceBlockIndex = blockLengths.Length - 1;

        while (true)
        {
            while (sourceBlockIndex >= 0 && !IsFileBlock(sourceBlockIndex))
            {
                sourceBlockIndex--;
            }

            if (sourceBlockIndex < 0)
                break;

            int sourceIndex = GetBlockOffset(sourceBlockIndex);
            int targetIndex = 0;

            while (targetIndex < sourceIndex && !IsFree(sourceBlockIndex, targetIndex))
            {
                targetIndex++;
            }

            if (targetIndex >= sourceIndex)
            {
                sourceBlockIndex--;

                continue;
            }

            System.Array.Fill(diskMap, sourceBlockIndex / 2, targetIndex, blockLengths[sourceBlockIndex]);
            System.Array.Fill(diskMap, -1, sourceIndex, blockLengths[sourceBlockIndex]);

            sourceBlockIndex--;
        }

        return diskMap
            .Select((id, i) => id * i)
            .Where(i => i > 0)
            .Sum()
            .ToString();

        bool IsFileBlock(int blockIndex) => blockIndex % 2 == 0;

        int GetBlockOffset(int blockIndex) => blockLengths.Take(blockIndex).Sum();

        bool IsFree(int blockIndex, int targetIndex) => targetIndex <= diskMap.Length - blockLengths[blockIndex]
            && diskMap.Skip(targetIndex).Take(blockLengths[blockIndex]).All(i => i == -1);
    }
}
