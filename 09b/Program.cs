using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _09b
{
    class Program
    {

        static readonly int numberOfPlayers = 476;
        // for lastMarble = 116570, for numberOfPlayers = 476
        // #121 with the score of 953674.
        // Stopwatch stops: 11.173167

        static readonly int lastMarble = 7165700;

        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine($"StopWatch started.");

            Dictionary<int, long> players = InitializePlayers(numberOfPlayers);
            players = Play2(players, lastMarble);

            var winningPlayer = players.OrderByDescending(kv => kv.Value).First();

            Console.WriteLine($"The winning Elf is #{winningPlayer.Key} with the score of: {winningPlayer.Value}.");

            sw.Stop();
            Console.WriteLine($"Stopwatch stops: {sw.Elapsed.TotalSeconds}");
        }

        private static Dictionary<int, long> InitializePlayers(int numberOfPlayers)
        {
            Dictionary<int, long> players = new Dictionary<int, long>();
            for (int player = 1; player <= numberOfPlayers; player++)
            {
                players.Add(player, 0);
            }

            return players;
        }

        private static Dictionary<int, long> Play2(Dictionary<int, long> players, int lastMarble)
        {
            int currentMarbleValue = 0;
            LinkedList<int> circle = new LinkedList<int>();
            LinkedListNode<int> currentMarbleNode = circle.AddFirst(currentMarbleValue);
            Dictionary<int, int> playersScoresInTurn = new Dictionary<int, int>();

            while (currentMarbleValue < lastMarble)
            {
                playersScoresInTurn.Clear();
                foreach (var player in players)
                {
                    currentMarbleValue++;
                    //Console.WriteLine($" {string.Join(' ', circle.ToArray())}, Cur: {currentMarbleNode.Value}");

                    if (currentMarbleValue > 0 && (currentMarbleValue % 23) == 0)
                    {
                        currentMarbleNode = FindPrevSeventhNode(currentMarbleNode);
                        int removedValue = RemovePrevNode(currentMarbleNode);
                        playersScoresInTurn.Add(player.Key, removedValue + currentMarbleValue);
                    }
                    else
                    {
                        if (currentMarbleNode.Next == null && currentMarbleNode.Previous == null)
                            currentMarbleNode = circle.AddAfter(currentMarbleNode, currentMarbleValue);
                        else if (currentMarbleNode.Next != null)
                            currentMarbleNode = circle.AddAfter(currentMarbleNode.Next, currentMarbleValue);
                        else
                            currentMarbleNode = circle.AddAfter(circle.First, currentMarbleValue);
                    }

                    if (currentMarbleValue == lastMarble)
                        break;
                }

                foreach (var playerScore in playersScoresInTurn)
                {
                    players[playerScore.Key] += playerScore.Value;
                }
            }

            return players;
        }

        private static int RemovePrevNode(LinkedListNode<int> currentMarbleNode)
        {            
            int result = 0;
            var list = currentMarbleNode.List;
            if (currentMarbleNode.Previous == null) {
                result = list.Last.Value;
                list.Remove(result);
            }
            else {
                result = currentMarbleNode.Previous.Value;
                list.Remove(result);
            }

            return result;
        }

        private static LinkedListNode<int> FindPrevSeventhNode(LinkedListNode<int> currentMarbleNode)
        {
            for (int i = 0; i < 6; i++)
            {
                if (currentMarbleNode.Previous != null)
                    currentMarbleNode = currentMarbleNode.Previous;
                else
                    currentMarbleNode = currentMarbleNode.List.Last;
            }

            return currentMarbleNode;
        }
    }
}