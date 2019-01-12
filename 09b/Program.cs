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
        static readonly int lastMarble = 7165700;
        static readonly bool canPrint2Console = false;

        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine($"StopWatch started.");

            Dictionary<int, int> players = InitializePlayers(numberOfPlayers);
            players = Play(players, lastMarble);       

            var winningPlayer = players.OrderByDescending(kv => kv.Value).First();

            Console.WriteLine($"The winning Elf is #{winningPlayer.Key} with the score of {winningPlayer.Value}.");

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

        private static Dictionary<int, int> Play(Dictionary<int, int> players, int lastMarble)
        {
            int currentMarble = 0;
            int nextMarble = 0;
            List<int> circle = new List<int>();
            circle.Add(currentMarble);

            if (canPrint2Console)
                Print(circle, 0, 0);
            
            while(currentMarble < lastMarble) {
                Dictionary<int, int> playersScoresInTurn = new Dictionary<int, int>();
                foreach(var player in players){
                    nextMarble++;

                    // add values to temp collection, as the players dict. cannot be modified
                    if ((nextMarble % 23) == 0) {

                        int indexOfCurentMarble = circle.IndexOf(currentMarble);
                        // if the index of current marble is less then the value of shift
                        if (indexOfCurentMarble < 7)
                            indexOfCurentMarble = circle.Count - Math.Abs(indexOfCurentMarble - 7);
                        else 
                            indexOfCurentMarble = indexOfCurentMarble - 7;
                        
                        playersScoresInTurn.Add(player.Key, nextMarble + circle.ElementAt(indexOfCurentMarble));

                        circle.RemoveAt(indexOfCurentMarble);
                        currentMarble = circle.ElementAt(indexOfCurentMarble);
                        
                        if (canPrint2Console) Print(circle, currentMarble, player.Key);
                    }
                    else {    
                        int marbleInsertIndex = DetermineInsertionIndex(circle, currentMarble);
                        
                        circle.Insert(marbleInsertIndex, nextMarble);   
                        
                        if (canPrint2Console) Print(circle, nextMarble, player.Key);
                        
                        currentMarble = nextMarble;
                    }

                    if (nextMarble == lastMarble)
                        break;
                }

                foreach(var playerScore in playersScoresInTurn) {
                    players[playerScore.Key] += playerScore.Value;
                }
            }

            return players;
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