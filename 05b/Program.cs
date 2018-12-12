using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _05b
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine($"StopWatch started.");

            string  inputData = "";
            Tuple<char, int> letterCountResult = new Tuple<char, int>('A', int.MaxValue);

            // read file
            using (var stream = File.OpenRead("Input.txt"))
            {
                var rdr = new StreamReader(stream);
                inputData = rdr.ReadToEnd();
            }

            // find and remove letters
            Parallel.For('A', 'Z', letter =>
            {
                string outputPolymer = RemoveUnitLetter((char)letter, inputData.ToString());
                outputPolymer = ReactPolymer(outputPolymer);

                Console.WriteLine($"The length of the polymer: {(char)letter}:{outputPolymer.Length}");
                if (outputPolymer.Length < letterCountResult.Item2) {
                    letterCountResult = new Tuple<char, int>((char)letter, outputPolymer.Length);
                }
            });

            Console.WriteLine($"Removing all {letterCountResult.Item1}/{(char)(letterCountResult.Item1 + 32)} units was best, producing the answer {letterCountResult.Item2}");

            sw.Stop();
            Console.WriteLine($"Stopwatch stops: {sw.Elapsed.TotalSeconds}");
        }

        private static string RemoveUnitLetter(char unitLetter, string input)
        {
            bool runWhile = true;
            StringBuilder inputSB = new StringBuilder(input);
            int curr = 0;
            int delta = 'a' - 'A';

            while (runWhile)
            {
                if (inputSB.Length > curr)
                {
                    int currLetter = inputSB[curr];

                    if (currLetter == unitLetter || currLetter == unitLetter + delta)
                    {
                        inputSB.Remove(curr, 1);
                        curr = 0;
                    }
                    else
                    {
                        curr++;
                    }
                }
                else
                {
                    runWhile = false;
                }
            }

            return inputSB.ToString();
        }
        private static string ReactPolymer(string input)
        {
            StringBuilder inputSB = new StringBuilder(input);
            int delta = 'a' - 'A';
            int curr = 0;

            while (true)
            {
                if (inputSB.Length > curr + 1)
                {
                    int nextLetter = inputSB[curr + 1];
                    int currLetterLeft = (inputSB[curr] - delta >= 0) ? (inputSB[curr] - delta) : 0;
                    int currLetterRight = (inputSB[curr] + delta <= 255) ? (inputSB[curr] + delta) : 255;

                    if (currLetterLeft == nextLetter || currLetterRight == nextLetter)
                    {
                        inputSB.Remove(curr, 2);
                        curr = 0;
                    }
                    else
                    {
                        curr++;
                    }
                }
                else
                {
                    break;
                }
            }

            return inputSB.ToString();
        }
    }
}