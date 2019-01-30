using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

namespace _10a
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine($"StopWatch started.");
            
            var points = ReadInputFile("input.txt");
            int oldGridWidth = 0, oldGridHeight = 0, sec = 0;
            bool isSmallestArea = false;
            Grid grid = MeasureArea(new Grid(), points);
            
            while (true)
            {
                oldGridHeight = grid.Height;
                oldGridWidth = grid.Width;
                
                TransformPoints(ref points);
                grid = MeasureArea(new Grid(), points);
                PopulateGrid(grid, points);
                
                isSmallestArea = (grid.Width > oldGridWidth && grid.Height > oldGridHeight) && (oldGridHeight != 0 && oldGridWidth != 0);
                if (isSmallestArea) {
                    TransformPoints(ref points, true);
                    break;                        
                }

                sec++;
            }

            // Print(grid, sec);
            string fileName = Save2PNG(grid, sec);
            Console.WriteLine($"Result can be found in the file '{fileName}'");
            Console.WriteLine($"It was all calculated in {sec} seconds");

            sw.Stop();
            Console.WriteLine($"Stopwatch stops: {sw.Elapsed.TotalSeconds}");
        }

        private static void TransformPoints(ref HashSet<Point> points, bool reverse = false)
        {
            int bias = reverse ? -1 : 1;
            foreach (var point in points)
            {
                point.X = point.X + point.VelocityX * bias;
                point.Y = point.Y + point.VelocityY * bias;
            }
        }

        private static string Save2PNG(Grid grid, int sec)
        {
            string fileName= null;
            using (var image = new Bitmap(grid.Width, grid.Height))
            {
                using (var graphics = Graphics.FromImage(image))
                {
                    for (int y = grid.Top; y <= grid.Bottom; y++)
                    {
                        for (int x = grid.Left; x <= grid.Right; x++)
                        {
                            Point point = grid.GetCell(x, y);
                            if (point != null)
                                graphics.FillRectangle(Brushes.Red, x, y, 1, 1);
                        }
                    }
                    string directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    fileName = $"{directory}\\sec{sec}.png";
                    image.Save(fileName, ImageFormat.Png);
                }
            }

            return fileName;
        }
        private static void Print(Grid grid, int sec)
        {
            for (int y = grid.Top; y <= grid.Bottom; y++)
            {
                for (int x = grid.Left; x <= grid.Right; x++)
                {
                    Point point = grid.GetCell(x, y);
                    if (point == null)
                        Console.Write('.');
                    else
                        Console.Write(point.Symbol);
                }
                Console.WriteLine();
            }
        }
        private static void PopulateGrid(Grid grid, HashSet<Point> points)
        {
            foreach (var point in points)
            {
                grid.AddCell(point);
            }
        }
        private static Grid MeasureArea(Grid grid, HashSet<Point> points)
        {
            foreach (var point in points)
            {
                if (point.X < grid.Left)
                    grid.Left = point.X;
                else if (point.X > grid.Right)
                    grid.Right = point.X;
                if (point.Y < grid.Top)
                    grid.Top = point.Y;
                else if (point.Y > grid.Bottom)
                    grid.Bottom = point.Y;
            }

            return grid;
        }
        private static HashSet<Point> ReadInputFile(string inputFilePath)
        {
            HashSet<Point> nodes = new HashSet<Point>();

            using (var stream = File.OpenRead(inputFilePath))
            {
                var rdr = new StreamReader(stream);
                while (!rdr.EndOfStream)
                {
                    string line = rdr.ReadLine();
                    Point point = ParseStringToPoint(line);
                    nodes.Add(point);
                }
            }

            return nodes;
        }
        private static Point ParseStringToPoint(string line)
        {
            Regex rgx = new Regex(@"position=<\s*([-+]?[0-9]*),\s*([-+]?[0-9]*)> velocity=<\s*([-+]?[0-9]*),\s*([-+]?[0-9]*)>");
            var match = rgx.Match(line);

            Point point = new Point();
            point.Symbol = '#';
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
            public char Symbol { get; set; }
            public Point(char symbol = '.')
            {
                this.Symbol = symbol;
            }

            public override int GetHashCode() {
                int hash = 13;
                hash = (hash * 7) + this.X.GetHashCode();
                hash = (hash * 7) + this.Y.GetHashCode();
                hash = (hash * 7) + this.VelocityX.GetHashCode();
                hash = (hash * 7) + this.VelocityY.GetHashCode();
                hash = (hash * 7) + this.Symbol.GetHashCode();

                return hash;
            }
        }

        public class Grid
        {
            public int Width { get { return Math.Abs(this.Left) + Math.Abs(this.Right); } }
             public int Height { get { return Math.Abs(this.Top) + Math.Abs(this.Bottom); } }  
            private HashSet<Point> CellsSet { get; set; }
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
            public Grid()
            {
                this.CellsSet = new HashSet<Point>();
            }

            public void AddCell(Point point)
            {
                int removed = this.CellsSet.RemoveWhere(p => p == point);
                this.CellsSet.Add(point);
            }
            public Point GetCell(int x, int y)
            {
                return (from c in this.CellsSet
                        where c.X == x && c.Y == y
                        select c).FirstOrDefault();
            }
        }
    }
}