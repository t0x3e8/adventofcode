using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _04b
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine($"StopWatch started.");

            var inputDataLines = new List<Shift>();

            // read file
            using (var stream = File.OpenRead("Input.txt"))
            {
                var rdr = new StreamReader(stream);

                while (!rdr.EndOfStream)
                {
                    string line = rdr.ReadLine();
                    Shift shift = LineToShift(line);
                    inputDataLines.Add(shift);
                }
            }

            inputDataLines.Sort((sh1, sh2) => sh1.Date.CompareTo(sh2.Date));
            var result = GetTimeShiftDetails(inputDataLines);

            var minuteMostAsleep = result.Key;
            var guardId = result.Value;

            Console.WriteLine($"The minute when guards are most asleep is: {minuteMostAsleep}");
            Console.WriteLine($"The most sleeping guard sleeping in that minute: {guardId}");
            Console.WriteLine($"What is the ID of the guard you chose multiplied by the minute you chose?: {guardId * minuteMostAsleep}");

            sw.Stop();
            Console.WriteLine($"Stopwatch stops: {sw.Elapsed.TotalSeconds}");
        }

        private static KeyValuePair<int, int> GetTimeShiftDetails(List<Shift> shifts)
        {
            var guardDict = new Dictionary<int, List<int>>();
            for (int i = 0; i < 60; i++)
                guardDict.Add(i, new List<int>());

            DateTime sleepStart = DateTime.MinValue;
            int guardId = 0;

            foreach (var shift in shifts)
            {
                switch (shift.StatusCode)
                {
                    case 1:
                        guardId = shift.GuardID;
                        break;
                    case 2: // falls asleep 
                        sleepStart = shift.Date;
                        break;
                    case 4: // wakes up 

                        for (var ts = sleepStart.TimeOfDay; ts < shift.Date.TimeOfDay; ts = ts.Add(new TimeSpan(0, 1, 0)))
                        {
                            guardDict[ts.Minutes].Add(guardId);
                        }

                        sleepStart = DateTime.MinValue;
                        break;
                }
            }

            //item1 - minute, item2 - guardId, item3 - guard on minute count
            Tuple<int, int, int> temp = new Tuple<int, int, int>(0, 0, 0);

            foreach (var item in guardDict)
            {
                if (item.Value.Count > 0)
                {
                    var groupQuery = from g in item.Value
                                     group g by g;

                    var groupCountedQuery = from g in groupQuery
                                            select new { GuardId = g.Key, Count = g.Count() };

                    var mostCountedGuard = groupCountedQuery.OrderByDescending(el => el.Count).First();

                    if (mostCountedGuard.Count > temp.Item3)
                    {
                        temp = new Tuple<int, int, int>(item.Key, mostCountedGuard.GuardId, mostCountedGuard.Count);
                    }
                }
            }

            return new KeyValuePair<int, int>(temp.Item1, temp.Item2);
        }
        private static Shift LineToShift(string line)
        {
            Shift shift = new Shift();
            Regex regex = new Regex(@"[#](\d+).");
            var matchId = regex.Match(line);

            if (matchId.Success)
            {
                shift.GuardID = int.Parse(matchId.Groups[1].Value);
                shift.StatusCode = 1;
            }
            regex = new Regex(@"[[?](\d*-\d*-\d*)[\s]?(\d*:\d*)[\]?]([\s\w(#\d+)]*)");

            var match = regex.Match(line);
            var date = DateTime.Parse(match.Groups[1].Value);
            var time = TimeSpan.Parse(match.Groups[2].Value);
            shift.Date = date + time;
            shift.Status = match.Groups[3].Value.Trim();
            if (shift.StatusCode != 1)
                shift.StatusCode = (shift.Status.StartsWith("falls") ? 2 : 4);

            return shift;
        }
    }
}

public class Shift
{
    public DateTime Date { get; set; }
    public string Status { get; set; }
    ///
    /// 1 - Guard starts shift; 2 - falls asleep; 4 - wakes up
    ///
    public int StatusCode { get; set; }
    public int GuardID { get; set; }
}
