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

            Dictionary<string, Step> stepsSet = ReadInputFile("input.txt");
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

                    if (stepsSet.ContainsKey(rootStepName)) {
                        stepsSet[rootStepName].AddChild(childStep);
                    } else {
                        rootStep = new Step(rootStepName);
                        rootStep.AddChild(childStep);
                        stepsSet.Add(rootStepName, rootStep);
                    }   
                }
            }
            return stepsSet;        
        }

        private static void AnalyzeSteps(Dictionary<string, Step> stepsSet)
        {
            List<Step> orderedList = new List<Step>();

            var firstStep = StepsHelper.FindFirstStep(stepsSet);
            var lastStep = StepsHelper.FindLastStep(stepsSet);

            orderedList.Add(firstStep);
            AnalyzeSingleStep(firstStep, ref stepsSet, ref orderedList);
            orderedList.Add(lastStep);
                        
            Console.WriteLine($"the first step is: {firstStep}");
            Console.WriteLine($"Ordered steps: {string.Join("", orderedList)}");
        }

        private static void AnalyzeSingleStep(Step rootStep, ref Dictionary<string, Step> notOrderedSteps, ref List<Step> orderedSteps)
        {
            foreach(var childStep in rootStep.GetChildrenOrdered()) {
                if (notOrderedSteps.ContainsKey(childStep.Name)) {
                    orderedSteps.Add(childStep);
                    AnalyzeSingleStep(notOrderedSteps[childStep.Name], ref notOrderedSteps, ref orderedSteps);
                }
                notOrderedSteps.Remove(rootStep.Name);
                    // var stepsCollection = StepsHelper.FindStepsContainingChild(childStep, notOrderedSteps.Values);
                    // foreach(var backwardStep in stepsCollection) {
                    //     AnalyzeSingleStep(backwardStep, notOrderedSteps, orderedSteps);
                    // }
                    
            }
        }

        private static void Print(Dictionary<string, Step> stepsSet)
        {
            foreach (var step in stepsSet)
            {
                Console.WriteLine($"{string.Join(", ", step.Value.ToStringWithChildren())}");
            }
        }
    }

    public static class StepsHelper {
        public static Step FindFirstStep(Dictionary<string, Step> stepsSet) {
            bool isRootAsChild = false;

            foreach (var rootStep in stepsSet.Values) {
                isRootAsChild = IsChild(rootStep.Name, stepsSet.Values);
                if (isRootAsChild == false)
                    return rootStep;
            }

            return null;
        }
         private static bool IsChild(string searchStepName, ICollection<Step> steps)
         {
            bool isChild = false;

            foreach(var childStep in steps) {
                isChild = childStep.ContainsChild(searchStepName);
                if (isChild) 
                    break;
            }

            return isChild;
         }

        public static Step FindLastStep(Dictionary<string, Step> stepsSet) {
            foreach (var rootStep in stepsSet.Values) {
                foreach (var childRootStep in rootStep.GetChildrenOrdered()) {
                    bool isChildAsRoot = stepsSet.ContainsKey(childRootStep.Name);
                    if (!isChildAsRoot)
                        return childRootStep;
                }
            }

            return null;
        }


         public static ICollection<Step> FindStepsContainingChild(Step childStep, ICollection<Step> steps) {
             var query = from s in steps
                         where s.ContainsChild(childStep.Name)
                         orderby s.Name
                         select s;

            return query.ToHashSet();
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
        
        public IOrderedEnumerable<Step> GetChildrenOrdered() {
            return this.ChildSteps.OrderBy(s => s.Name);
        }

        public bool ContainsChild(string childName) {
            if (childName == this.Name)
                return false;

            var query = from c in this.ChildSteps
                        where c.Name.Equals(childName)
                        select c;
            
            return query.Count() > 0;
        }

        public override string ToString() {
            return $"{this.Name}";
        }

        public string ToStringWithChildren() {
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