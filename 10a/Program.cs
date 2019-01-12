using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _10a
{
    class Program
    {
        static readonly bool canPrint2Console = true;

        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine($"StopWatch started.");

            var points = ReadInputFile("input.txt");


            Console.WriteLine($"The value of the root node is: ");

            sw.Stop();
            Console.WriteLine($"Stopwatch stops: {sw.Elapsed.TotalSeconds}");
        }

        private static HashSet<Point> ReadInputFile(string inputFilePath)
        {
            HashSet<Point> nodes = new HashSet<Point>();

            using (var stream = File.OpenRead(inputFilePath))
            {
                var rdr = new StreamReader(stream);
                while (!rdr.EndOfStream) {
                    string line = rdr.ReadLine();
                    nodes.Add(ParseStringToPoint(line));
                }
            }

            return nodes;
        }

        private static Point ParseStringToPoint(string line)
        {
            Regex rgx = new Regex(@"position=<\s*([-+]?[0-9]*),\s*([-+]?[0-9]*)> velocity=<\s*([-+]?[0-9]*),\s*([-+]?[0-9]*)>");
            var match = rgx.Match(line);  
            
            Point point = new Point();

            point.X = int.Parse(match.Groups[1].Value);
            point.Y = int.Parse(match.Groups[2].Value);
            point.VelocityX = int.Parse(match.Groups[3].Value);
            point.VelocityY = int.Parse(match.Groups[4].Value);

            return point;
        }

        public class Point
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int VelocityX { get; set; }
            public int VelocityY { get; set; }
        }
    }
}