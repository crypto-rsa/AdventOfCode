using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day20 : IAdventDay
{
    public string Name => "20. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day20.txt");

    private record LinkedListNode(long Value)
    {
        public LinkedListNode Prev { get; set; }

        public LinkedListNode Next { get; set; }
    }

    private class LinkedList
    {
        private LinkedListNode Head { get; }

        private int Count { get; }

        public LinkedList(IEnumerable<long> values)
        {
            LinkedListNode prev = null;

            foreach (long value in values)
            {
                if (Head == null)
                {
                    Head = new LinkedListNode(value);
                    Head.Next = Head;
                    Head.Prev = Head;
                    Count = 1;
                    prev = Head;

                    continue;
                }

                var next = new LinkedListNode(value);
                Insert(prev, next);
                prev = next;
                Count++;
            }
        }

        public IEnumerable<LinkedListNode> GetNodes()
        {
            var node = Head;

            do
            {
                yield return node;

                node = node.Next;
            } while (node != Head);
        }

        private static void Insert(LinkedListNode after, LinkedListNode node)
        {
            node.Next = after.Next;
            node.Next.Prev = node;

            after.Next = node;
            after.Next.Prev = after;
        }

        private static void Remove(LinkedListNode node)
        {
            node.Prev.Next = node.Next;
            node.Next.Prev = node.Prev;
        }

        private static void Move(LinkedListNode after, LinkedListNode node)
        {
            Remove(node);
            Insert(after, node);
        }

        public LinkedListNode Find(int value)
        {
            var node = Head;

            while (node != null)
            {
                if (node.Value == value)
                    return node;

                node = node.Next;

                if (node == Head)
                    return null;
            }

            return null;
        }

        public void Mix(LinkedListNode node)
        {
            long value = node.Value;

            long absValue = Math.Abs(value) % (Count - 1);

            (long steps, int dir) = absValue > Count / 2
                ? (Count - 1 - absValue, -Math.Sign(value))
                : (absValue, Math.Sign(value));

            if (dir == 0)
                return;

            if (dir < 0)
            {
                var at = node.Prev;

                for (int i = 0; i < steps; i++)
                {
                    at = at.Prev;
                }

                Move(at, node);
            }
            else
            {
                var at = node;

                for (int i = 0; i < steps; i++)
                {
                    at = at.Next;
                }

                Move(at, node);
            }
        }
    }

    private static long Mix(int multiplier, int iterations)
    {
        var original = GetInput()
            .SplitToLines()
            .Select(s => long.Parse(s) * multiplier)
            .ToList();

        var linkedList = new LinkedList(original);
        var nodes = linkedList.GetNodes().ToList();

        for (int i = 0; i < iterations; i++)
        {
            foreach (var node in nodes)
            {
                linkedList.Mix(node);
            }
        }

        long total = 0;
        var at = linkedList.Find(0);

        for (int i = 1; i <= 3000; i++)
        {
            at = at.Next;

            if (i % 1000 == 0)
            {
                total += at.Value;
            }
        }

        return total;
    }

    public string Solve() => Mix(1, 1).ToString();

    public string SolveAdvanced() => Mix(811589153, 10).ToString();
}
