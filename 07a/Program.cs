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
        static bool IsDebug = false;

        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine($"StopWatch started.");

            Dictionary<string, Step> stepsSet = ReadInputFile("input.txt");
            if(IsDebug)
                Print(stepsSet);

            AnalyzeSteps(stepsSet);

            sw.Stop();
            Console.WriteLine($"Stopwatch stops: {sw.Elapsed.TotalSeconds}");
        }

        private static Dictionary<string, Step> ReadInputFile(string inputFilePath)
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
                    var rootStepName = match.Groups[1].Value.Trim();
                    var childStepName = match.Groups[2].Value.Trim();
                    Step rootStep;
                    Step childStep = (stepsSet.ContainsKey(childStepName) ? stepsSet[childStepName] : new Step(childStepName));

                    if (stepsSet.ContainsKey(rootStepName))
                    {
                        stepsSet[rootStepName].AddChild(childStep);
                    }
                    else
                    {
                        rootStep = new Step(rootStepName);
                        rootStep.AddChild(childStep);
                        stepsSet.Add(rootStepName, rootStep);
                    }
                }
            }
            return stepsSet;
        }

        private static void AnalyzeSteps(Dictionary<string, Step> inputStepsSet)
        {
            if(IsDebug)
                Console.WriteLine($"All steps are here: {string.Join(", ", inputStepsSet.Keys.OrderBy(x => x))}");

            List<Step> allStepsSet = inputStepsSet.Values.OrderBy(x => x.Name).ToList();
            List<Step> availableSteps = new List<Step>();
            StringBuilder result = new StringBuilder();

            int i = 0;
            while (true)
            {
                availableSteps = FindAvailableSteps(allStepsSet);
                if (availableSteps.Count == 0)
                    break;

                if(IsDebug)
                    Console.WriteLine($"Working set {i++}: {string.Join(", ", availableSteps)}");

                result.Append(availableSteps[0]);

                if(IsDebug)
                    Console.WriteLine($"Pick '{availableSteps[0]}', so the output is '{result}'");

                allStepsSet.Remove(availableSteps[0]);
            }

           Console.WriteLine($"The order of  steps in instructions should be completed as '{result}'");
        }

        private static List<Step> FindAvailableSteps(ICollection<Step> stepsSet)
        {
            List<Step> availableSteps = new List<Step>();

            foreach (var step in stepsSet)
            {
                if (!StepsHelper.IsChild(step, stepsSet))
                {
                    availableSteps.Add(step);
                }

            }

            return availableSteps;
        }

        private static void Print(Dictionary<string, Step> stepsSet)
        {
            foreach (var step in stepsSet)
            {
                Console.WriteLine($"{string.Join(", ", step.Value.ToStringWithChildren())}");
            }
        }
    }

    public static class StepsHelper
    {
        public static bool IsChild(Step searchStep, ICollection<Step> steps)
        {
            bool isChild = false;

            foreach (var childStep in steps)
            {
                isChild = childStep.ContainsChild(searchStep.Name);
                if (isChild)
                    break;
            }

            return isChild;
        }
    }

    public class Step
    {
        public string Name { get; set; }
        private HashSet<Step> ChildSteps { get; set; }
        public Step(string stepName)
        {
            this.ChildSteps = new HashSet<Step>();
            this.Name = stepName;
        }

        public void AddChild(Step child)
        {
            this.ChildSteps.Add(child);
        }

        public bool ContainsChild(string childName)
        {
            if (childName == this.Name)
                return false;

            var query = from c in this.ChildSteps
                        where c.Name.Equals(childName)
                        select c;

            return query.Count() > 0;
        }

        public override string ToString()
        {
            return $"{this.Name}";
        }

        public HashSet<Step> GetChildren() {
            return this.ChildSteps;
        }
        public string ToStringWithChildren()
        {
            return $"{this.Name}: {string.Join(", ", this.ChildSteps.OrderBy(x => x.Name))}";
        }
        
        public override bool Equals(object obj) {
            if (obj == null || !(obj is Step))
                return false;

            Step newStep = obj as Step;
            return newStep.Name == this.Name;
        }
    }
}