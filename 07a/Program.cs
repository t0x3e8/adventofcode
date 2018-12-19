using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _07a
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine($"StopWatch started.");

            Dictionary<string, Step>  stepsSet = ReadInputFile("input.txt");
            Print(stepsSet);

            sw.Stop();
            Console.WriteLine($"Stopwatch stops: {sw.Elapsed.TotalSeconds}");
        }

         private static Dictionary<string, Step>  ReadInputFile(string inputFilePath)
        {
            Dictionary<string, Step> stepsSet = new Dictionary<string, Step>();
            Regex regex = new Regex(@"Step (\w)+ must be finished before step (\w)+ can begin.");

            using (var stream = File.OpenRead(inputFilePath))
            {
                var rdr = new StreamReader(stream);

                while (!rdr.EndOfStream)
                {
                    string line = rdr.ReadLine();
                    Match match = regex.Match(line);
                    var stepName = match.Groups[1].Value.Trim();
                    var nextStepName = match.Groups[2].Value.Trim();

                    if (stepsSet.ContainsKey(stepName))
                        stepsSet[stepName].AddNextStep(nextStepName);
                    else {
                        var step = new Step();
                        step.Name = stepName;
                        step.AddNextStep(nextStepName);
                        stepsSet.Add(stepName, step);
                    }
                }
            }

            return stepsSet;
        }

        private static void Print(Dictionary<string, Step> stepsSet) {
            foreach (var step in stepsSet) {
                Console.WriteLine($"{step.Key} has dependencies {string.Join(", ", step.Value.GetNextStepASC())}");
            }
        }
    }

    public class Step {
        public string Name { get; set; }
        private List<string> NextSteps { get; set;}
        public Step(){
            this.NextSteps= new List<string>();
        }

        public void AddNextStep (string nextStep) {
            this.NextSteps.Add(nextStep);
        }

        public List<String> GetNextStepASC() {
            this.NextSteps.Sort();
            return this.NextSteps;
        }

    }
}