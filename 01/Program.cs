using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace _01
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start(); 
            Console.WriteLine("Stopwatch started");
            int currentFrequency = 0;
            var frequencyResults = new Dictionary<int, int>();
            // var frequencyResults = new HashSet<int>();
            var inputDataInLines = new List<int>();

            using (var stream = File.OpenRead("Input.txt")) {
                var rdr = new StreamReader(stream);

                while(!rdr.EndOfStream) {
                    string line = rdr.ReadLine();
                    inputDataInLines.Add(int.Parse(line));
                }
            }

            for(int i=0; i<inputDataInLines.Count; i++) {
                int newFrequency = currentFrequency + inputDataInLines[i];

                // Console.WriteLine($"{i} Current frequency  {currentFrequency}, change of {inputDataInLines[i]}; resulting frequency  {newFrequency}");
                currentFrequency = newFrequency;

                var isFrequencyExisiting = frequencyResults.ContainsKey(currentFrequency);
                if (isFrequencyExisiting) {
                    Console.WriteLine($"FOUND: {currentFrequency}");
                    break;
                }
                else {
                    frequencyResults.Add(currentFrequency, currentFrequency);
                }

                if (i == (inputDataInLines.Count - 1)) {
                    i = -1;
                }
            }

            Console.WriteLine("Current frequency is: " + currentFrequency);
            sw.Stop();
            Console.WriteLine($"Execution in seconds: {sw.Elapsed.Seconds}");
        }
    }
}
