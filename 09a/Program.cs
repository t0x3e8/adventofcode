using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _09a
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine($"StopWatch started.");


            Dictionary<int, int> players = InitializePlayers(9);
            Play(players, 25);       

            sw.Stop();
            Console.WriteLine($"Stopwatch stops: {sw.Elapsed.TotalSeconds}");
        }

        private static Dictionary<int, int> InitializePlayers(int numberOfPlayers)
        {
            Dictionary<int, int> players = new Dictionary<int, int>();
            for (int player = 1; player <= numberOfPlayers; player++) {
                players.Add(player, 0);
            }

            return players;
        }

        private static void Play(Dictionary<int, int> players, int lastMarble)
        {
            int currentMarble = 0;
            int nextMarble = 0;
            List<int> circle = new List<int>();
            circle.Add(currentMarble);

            Print(circle, 0, 0);
            
            while(true) {
                Dictionary<int, int> tempPlayers = new Dictionary<int, int>();
                foreach(var player in players){
                    nextMarble++;

                    // add values to temp collection, as the players dict. cannot be modified
                    if ((nextMarble % 23) == 0) {
                        tempPlayers.Add(player.Key, nextMarble);

                        int indexOfCurentMarble = circle.IndexOf(currentMarble);
                        circle.RemoveAt(indexOfCurentMarble - 7);

                        currentMarble = circle.ElementAt(indexOfCurentMarble - 7);
                        Print(circle, currentMarble, player.Key);
                    }
                    else {    
                        int marbleInsertIndex = DetermineInsertionIndex(circle, currentMarble);
                        
                        circle.Insert(marbleInsertIndex, nextMarble);   
                        Print(circle, nextMarble, player.Key);
                        currentMarble = nextMarble;
                    }

                    if (currentMarble < lastMarble)
                        break;
                }

                foreach(var tempPlayer in tempPlayers) {
                    players[tempPlayer.Key] += tempPlayer.Value;
                }
            }
        }

        private static int DetermineInsertionIndex(List<int> circle, int currentMarble)
        {  
            int currentIndex = circle.FindIndex(0, circle.Count, i => i == currentMarble);

            int result = (currentIndex + 1) >= circle.Count ? 1 : currentIndex + 2;

            return result;
        }

        private static void Print(List<int> circle, int currentMarble, int playerId)
        {
            int currentMarbleIndex = circle.IndexOf(currentMarble);

            Console.Write($"[{playerId}]   ");

            // print before current marble
            if (currentMarbleIndex > 0)
                Console.Write($" {string.Join(' ', circle.GetRange(0, currentMarbleIndex))} ");
            
            // print current marble
            Console.Write($"({circle[currentMarbleIndex]})");

            // print after current marble
            // if( currentMarbleIndex + 1 < circle.Count)
                Console.Write($" {string.Join(' ', circle.GetRange(currentMarbleIndex + 1, circle.Count - currentMarbleIndex - 1))} ");
            

            Console.WriteLine();
        }
    }
}