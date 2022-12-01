using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2018
{
    public class Day9 : IAdventDay
    {
        private class MarbleList : LinkedList<int>
        {
            public LinkedListNode<int> Next(LinkedListNode<int> from, int offset)
            {
                for (int i = 0; i < offset; i++)
                {
                    from = from.Next ?? First;
                }

                return from;
            }

            public LinkedListNode<int> Previous(LinkedListNode<int> from, int offset)
            {
                for (int i = 0; i < offset; i++)
                {
                    from = from.Previous ?? Last;
                }

                return from;
            }
        }

        public string Name => "9. 12. 2018";

        private const int PlayerCount = 431;

        private const int LastMarble = 70950;

        private const int LastMarbleAdvanced = 7095000;

        public string Solve()
        {
            return Solve(LastMarble);
        }

        private string Solve(int lastMarble)
        {
            var marbles = new MarbleList();
            marbles.AddFirst(0);

            var currentNode = marbles.First;
            var playerScore = new long[PlayerCount];

            int currentPlayer = 0;

            for (int marble = 1; marble <= lastMarble; marble++)
            {
                if (marble % 23 == 0)
                {
                    playerScore[currentPlayer] += marble;
                    currentNode = marbles.Previous(currentNode, 7);
                    playerScore[currentPlayer] += currentNode.Value;
                    currentNode = currentNode.Next;
                    marbles.Remove(currentNode.Previous);
                }
                else
                {
                    marbles.AddAfter(marbles.Next(currentNode, 1), marble);
                    currentNode = marbles.Next(currentNode, 2);
                }

                currentPlayer = (currentPlayer + 1) % PlayerCount;
            }

            return playerScore.Max().ToString();
        }

        public string SolveAdvanced()
        {
            return Solve(LastMarbleAdvanced);
        }
    }
}