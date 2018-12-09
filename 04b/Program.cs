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
            foreach(var item in inputDataLines){
                if (item.StatusCode == 1) 
                    Console.WriteLine($"[{item.Date.ToShortDateString()} {item.Date.ToShortTimeString()}] Guard #{item.GuardID} begins shift");
                else if (item.StatusCode == 2)
                    Console.WriteLine($"[{item.Date.ToShortDateString()} {item.Date.ToShortTimeString()}] falls asleep");
                else if (item.StatusCode == 4)
                    Console.WriteLine($"[{item.Date.ToShortDateString()} {item.Date.ToShortTimeString()}] wakes up");
            }
            var result = GetTimeShiftDetails(inputDataLines);
            
            var minuteMostAsleep = result.Key;
            var guardId = result.Value;

            Console.WriteLine($"The minute when guards are most aleep is: {minuteMostAsleep}");
            Console.WriteLine($"The most sleeping guard sleeping in that minute: {guardId}");
            Console.WriteLine($"What is the ID of the guard you chose multiplied by the minute you chose?: {guardId * minuteMostAsleep}");

            sw.Stop();
            Console.WriteLine($"Stopwatch stops: {sw.Elapsed.TotalSeconds}");
        }

        private static KeyValuePair<int, int> GetTimeShiftDetails(List<Shift> shifts) {
            var guardDict = new Dictionary<int, List<int>>();
            for(int i = 0; i < 60; i++)
                guardDict.Add(i, new List<int>());

            DateTime sleepStart = DateTime.MinValue;
            int guardId = 0;

            foreach(var shift in shifts) {
                switch (shift.StatusCode) {
                    case 1 :
                        guardId = shift.GuardID;
                        break;
                    case 2 : // falls asleep 
                        sleepStart = shift.Date;
                        break;
                    case 4 : // wakes up 

                        if (sleepStart.DayOfYear < shift.Date.DayOfYear) {
                            Console.WriteLine($"{sleepStart} > {shift.Date.TimeOfDay}");
                        }

                        for(var ts= sleepStart.TimeOfDay; ts < shift.Date.TimeOfDay; ts=ts.Add(new TimeSpan(0, 1, 0))) {
                            guardDict[ts.Minutes].Add(guardId);
                        }

                        sleepStart = DateTime.MinValue;
                        break;
                }
            }

            Tuple<int, int> temp = new Tuple<int, int>(0,0);

            foreach(var item in guardDict) {
                if (item.Value.Count > temp.Item2) {
                    temp = new Tuple<int, int>(item.Key, item.Value.Count);
                }
            }
            var mostAsleepMinute = temp.Item1;

            var mostSleepingGuard = from x in guardDict[mostAsleepMinute]
                                    group x by x into g
                                    let count = g.Count()
                                    orderby count descending
                                    select new {Value = g.Key, Count = count};

            return new KeyValuePair<int, int> (mostAsleepMinute, mostSleepingGuard.ToList()[0].Value);
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
