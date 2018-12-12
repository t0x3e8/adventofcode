using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _05a
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine($"StopWatch started.");

            StringBuilder inputData = new StringBuilder();
            int delta = 'a' - 'A';

            // read file
            using (var stream = File.OpenRead("Input.txt"))
            {
                var rdr = new StreamReader(stream);
                inputData.Append(rdr.ReadToEnd());
            }

            int curr = 0;
            while (true)
            {
                if (inputData.Length > curr + 1)
                {
                    int nextLetter = inputData[curr + 1];
                    int currLetterLeft = (inputData[curr] - delta >= 0) ? (inputData[curr] - delta) : 0;
                    int currLetterRight = (inputData[curr] + delta <= 255) ? (inputData[curr] + delta) : 255;

                    if (currLetterLeft == nextLetter || currLetterRight == nextLetter)
                    {
                        inputData.Remove(curr, 2);
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

            Console.WriteLine($"The resulting polymer '{inputData.ToString()}' contains: {inputData.Length} units");

            sw.Stop();
            Console.WriteLine($"Stopwatch stops: {sw.Elapsed.TotalSeconds}");
        }
    }
}