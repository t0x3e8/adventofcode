using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _07b
{
    class Program
    {
        static bool IsDebug = true;

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
                        if (!stepsSet.ContainsKey(childStepName))
                            stepsSet.Add(childStepName, childStep);
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

            if (IsDebug)
                Console.WriteLine($"{"Second", -10}{"Worker 1", -10}{"Worker 2", -10}{"Worker 3", -10}{"Worker 4", -10}{"Worker 5", -10}{"Done", -10}");

            int second = 0;
            while (true)
            {
                Console.Write($"{second++, -10}");

                for (int workerNumber = 1; workerNumber <= 5; workerNumber++) { 
                    availableSteps = FindAvailableSteps(allStepsSet, workerNumber);
                    
                    if (availableSteps.Count > 0) {
                        var workingStep = availableSteps.Where(s => s.WorkerNumber == workerNumber).FirstOrDefault();
                        if (workingStep == null)
                            workingStep = availableSteps[0];
                        
                        if (!workingStep.IsWorkStarted) {
                            workingStep.StartWork(second, workerNumber);
                        }
                        
                        if (workingStep.IsWorkDone(second, workerNumber)) {
                            result.Append(workingStep);
                            allStepsSet.Remove(workingStep);              
                        } 
                        Console.Write($"{workingStep, -10}");
                    }
                    else                        
                        Console.Write($"{".", -10}");
                }
                Console.Write($"{result, -10}");
                Console.WriteLine();

                if (allStepsSet.Count == 0)
                    break;
            }

           Console.WriteLine($"The order of  steps in instructions should be completed as '{result}'");
        }

        private static List<Step> FindAvailableSteps(ICollection<Step> stepsSet, int workerNumber)
        {
            List<Step> availableSteps = new List<Step>();

            foreach (var step in stepsSet)
            {
                if (!StepsHelper.IsChild(step, stepsSet) && (workerNumber == -1 || step.WorkerNumber == workerNumber || step.WorkerNumber == 0))
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
        private int startWorkSecond = -1;
        public  int WorkerNumber {get;set;}
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

        public void StartWork(int currentSecond, int workerNumber) {
            this.startWorkSecond = currentSecond;
            this.WorkerNumber = workerNumber;
        }

        public bool IsWorkStarted {
            get {
                return this.startWorkSecond >= 0;
            }
        }

        public bool IsWorkDone(int currentSecond, int workerNumber) {
            if(workerNumber != this.WorkerNumber)
                return false;

            int offset = 60;
            int length = this.Name[0] - 'A';

            return (offset + length) <= (currentSecond - this.startWorkSecond);
        }
    }
}