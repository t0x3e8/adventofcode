using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace _02b
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine($"StopWatch started.");

            var inputDataLines = new List<string>();

            using (var stream = File.OpenRead("Input.txt"))
            {
                var rdr = new StreamReader(stream);

                while (!rdr.EndOfStream)
                {
                    string line = rdr.ReadLine();
                    inputDataLines.Add(line);
                }
            }

            for (int i = 0; i < inputDataLines.Count; i++)
            {
                int result = 0;

                for (int j = i + 1; j < inputDataLines.Count; j++)
                {
                    result = HowBigDiffrence(inputDataLines[i], inputDataLines[j]);

                    if (result == 1) {
                        string superset = SupersetOfString(inputDataLines[i], inputDataLines[j]);
                        Console.WriteLine($"compare '{inputDataLines[i]}' with '{inputDataLines[j]}', result: {result}, superset is: {superset}");

                        break;
                    }
                }
            }

            sw.Stop();
            Console.WriteLine($"Stopwatch stops: {sw.Elapsed.TotalSeconds}");
        }
        
        public static int HowBigDiffrence(string s1, string s2) {
            int minLetters = Math.Min(s1.Length, s2.Length);
            int maxLetters = Math.Max(s1.Length, s2.Length);
            int diffrenceSize = maxLetters - minLetters;

            for(int i = 0; i < minLetters; i++) {
                if (s1[i] != s2[i]) {
                    diffrenceSize++;
                }
            }

            return diffrenceSize;
        }

        public static string SupersetOfString(string s1, string s2) {
            int minLetters = Math.Min(s1.Length, s2.Length);
            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < minLetters; i++) {
                if (s1[i] == s2[i]) {
                    sb.Append(s1[i]);
                }
            }

            return sb.ToString();
        }
    }
}
