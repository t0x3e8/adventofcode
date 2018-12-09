using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _02
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime start = DateTime.Now;
            Console.WriteLine($"Start: {start}");
            var frequencyResults = new List<int>();
            var inputDataLines = new List<string>();
            Dictionary<int, int> repititionCount = new Dictionary<int, int>();
            int result = 1;

            using (var stream = File.OpenRead("Input.txt")) {
                var rdr = new StreamReader(stream);

                while(!rdr.EndOfStream) {
                    string line = rdr.ReadLine();
                    inputDataLines.Add(line);
                }
            }

            for(int i = 0; i < inputDataLines.Count; i++) {
                string word = inputDataLines[i];
                Dictionary<int, bool> lettersCountRegister = new Dictionary<int, bool>();                

                foreach(var letter in word) {
                    var count = word.Count(c => c == letter);
                    if (count >= 2) {
                        if (lettersCountRegister.ContainsKey(count))
                            lettersCountRegister[count] = true;
                        else
                            lettersCountRegister.Add(count, true);
                    }
                }

                Console.WriteLine($"Word '{word}' contains: {string.Join(", ", lettersCountRegister.Keys.ToArray())}");

                lettersCountRegister.Keys.ToList().ForEach(k => {
                    if (repititionCount.ContainsKey(k))
                        repititionCount[k]++;
                    else
                        repititionCount.Add(k, 1);
                });
            }

            repititionCount.Values.ToList().ForEach(v => {
                result *= v;
            });
            Console.WriteLine($"Repition count is: {string.Join("; ", repititionCount.ToArray())}, and the result is: {result}");

            Console.WriteLine($"Execution in seconds: {DateTime.Now.Subtract(start).TotalSeconds}");
        }
    }
}
