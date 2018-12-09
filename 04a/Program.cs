using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _04a
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
            var guardShiftDetails = GetGuardShiftDetails(inputDataLines);
            var guardId = guardShiftDetails.Key;
            var minuteMostAsleep = guardShiftDetails.Value.Minutes.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

            Console.WriteLine($"The guard most sleeping ID is: {guardId}");
            Console.WriteLine($"The minute of often being asleep is: {minuteMostAsleep}");
            Console.WriteLine($"What is the ID of the guard you chose multiplied by the minute you chose?: {guardId * minuteMostAsleep}");

            sw.Stop();
            Console.WriteLine($"Stopwatch stops: {sw.Elapsed.TotalSeconds}");
        }

        private static KeyValuePair<int, ShiftDetails> GetGuardShiftDetails(List<Shift> shifts) {
            var guardDict = new Dictionary<int, ShiftDetails>();
            TimeSpan sleepStart = TimeSpan.Zero;
            int guardId = 0;

            foreach(var shift in shifts) {
                switch (shift.StatusCode) {
                    case 1 :
                        if (!guardDict.ContainsKey(shift.GuardID))
                            guardDict.Add(shift.GuardID, new ShiftDetails());
                        guardId = shift.GuardID;
                        break;
                    case 2 : // falls asleep 
                        sleepStart = shift.Date.TimeOfDay;
                        break;
                    case 4 : // wakes up 
                        TimeSpan diffTS = shift.Date.TimeOfDay.Subtract(sleepStart);
                        guardDict[guardId].TotalMinutes += diffTS.Minutes;

                        for(var ts= sleepStart; ts< shift.Date.TimeOfDay; ts=ts.Add(new TimeSpan(0, 1, 0))) {
                            guardDict[guardId].Minutes[ts.Minutes]++;
                        }

                        sleepStart = TimeSpan.Zero;
                        break;
                }
            }

            var foundGuardShift = guardDict.Aggregate((x, y) => x.Value.TotalMinutes > y.Value.TotalMinutes ? x : y);

            return new KeyValuePair<int, ShiftDetails> (foundGuardShift.Key, foundGuardShift.Value );

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

public class ShiftDetails {
    public int TotalMinutes { get; set; }
    public Dictionary<int, int> Minutes { get; set; }

    public ShiftDetails()
    {
        this.Minutes = new Dictionary<int, int>();
        for(int i=0; i<60; i++)
            this.Minutes.Add(i, 0);
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
