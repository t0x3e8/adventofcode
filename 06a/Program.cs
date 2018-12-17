using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _06a
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine($"StopWatch started.");

            HashSet<MyPoint> pointsSet = ReadInputFile("input.txt");
            Grid grid = MeasureArea(pointsSet);
            ArrangeGrid(grid);
            PopulateGrid(grid, pointsSet);

            // PrintGrid(grid);
            Console.WriteLine($"The size of the largest area is {grid.GetLargestArea()}");
            sw.Stop();
            Console.WriteLine($"Stopwatch stops: {sw.Elapsed.TotalSeconds}");
        }

        private static void PopulateGrid(Grid grid, HashSet<MyPoint> pointsSet)
        {
            foreach (var point in pointsSet) 
            {
                grid.AddCell(point);
                CalculateDistanceForCell(point.X, point.Y, point.GetSymbol().ToLower(), grid);
                Console.WriteLine($"The point {point.GetSymbol()} is done");
            }
        }

        private static void CalculateDistanceForCell(int originX, int originY, string symbol, Grid grid)
        {
            Parallel.For(1, grid.Height, (y) =>
            {
                Parallel.For(1, grid.Width, (x) =>
                {
                    MyPoint cell = grid.GetCell(x, y);
                    if (!cell.IsMasterPoint) {
                        int dx = Math.Abs(x - originX);
                        int dy = Math.Abs(y - originY);
                        int distance = dx + dy;
                        
                        cell.AddSymbolDistance(symbol, distance);

                        // uncomment code below for small animation
                        // PrintGrid(grid);
                        // System.Threading.Thread.Sleep(100);
                    }
                });
            });
        }

        private static void PrintGrid(Grid grid)
        {
            try
            {
                Console.Clear();
            }
            catch { }

            for (int y = 1; y <= grid.Height; y++)
            {
                for (int x = 1; x <= grid.Width; x++)
                {
                    string cellText = grid.GetCell(x, y).GetSymbol();
                    Console.Write(cellText);
                }
                Console.WriteLine();
            }

            // diagnostic details
            // foreach(var cell in grid.Cells) {
            //     Console.WriteLine($"X: {cell.X} Y: {cell.Y} Distances: {string.Join(", ", cell.distances)}");
            // }
        }

        private static void ArrangeGrid(Grid grid)
        {
            for (int y = 1; y <= grid.Height; y++)
                for (int x = 1; x <= grid.Width; x++)
                    grid.AddCell(new MyPoint(x, y));
        }

        private static HashSet<MyPoint> ReadInputFile(string inputFilePath)
        {
            HashSet<MyPoint> pointsSet = new HashSet<MyPoint>();

            using (var stream = File.OpenRead(inputFilePath))
            {
                var rdr = new StreamReader(stream);
                string symbol = "A";

                while (!rdr.EndOfStream)
                {
                    MyPoint point = MyPoint.CreatePoint(rdr.ReadLine());
                    point.AddSymbolDistance(symbol, 0);
                    pointsSet.Add(point);
                    point.IsMasterPoint = true;

                    symbol = symbol.NextSymbol();
                }
            }

            return pointsSet;
        }

        private static Grid MeasureArea(HashSet<MyPoint> points)
        {
            Grid grid = new Grid();

            foreach (var point in points)
            {
                if (point.X > grid.Width)
                    grid.Width = point.X;
                if (point.Y > grid.Height)
                    grid.Height = point.Y;
            }

            return grid;
        }
    }
}

public class Grid
{
    private Dictionary<string, MyPoint> CellsSet { get; set; }
    public int X { get; }
    public int Y { get; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Grid()
    {
        this.X = this.Y = 0;
        this.CellsSet = new Dictionary<string, MyPoint>();
    }

    public MyPoint GetCell(int x, int y)
    {       
        string keyText = $"{x}:{y}";
        return this.CellsSet[keyText];
    }

    
    public Tuple<string, int> GetLargestArea() {
        var query = this.CellsSet.GroupBy(c => c.Value.GetSymbol().ToLower())
                  .Select(group => new {
                      Symbol = group.Key,
                      Count = group.Count()
                  })
                  .OrderByDescending(x => x.Count);
        
        return new Tuple<string, int>(query.First().Symbol, query.First().Count);
    }

    public void AddCell(MyPoint point)
    {
        string keyText = $"{point.X}:{point.Y}";
        if (this.CellsSet.ContainsKey(keyText))
            this.CellsSet[keyText] = point;
        else
            this.CellsSet.Add(keyText, point);
    }
}

public class MyPoint
{
    public bool IsMasterPoint { get; set; }
    private Dictionary<string, int> distances;
    public int X { get; }
    public int Y { get; }
    public static MyPoint CreatePoint(string data)
    {
        var pair = data.Split(",");
        int x = int.Parse(pair[0]);
        int y = int.Parse(pair[1]);
        return new MyPoint(x, y);
    }

    public MyPoint(int x, int y)
    {
        this.X = x;
        this.Y = y;
        this.distances = new Dictionary<string, int>();
    }

    public void AddSymbolDistance(string name, int value)
    {
        if (this.distances.ContainsKey(name) && this.distances[name] > value)
            this.distances[name] = value;
        else if (!this.distances.ContainsKey(name))
            this.distances.Add(name, value);
    }

    public string GetSymbol()
    {
        if (this.distances.Count > 0) {
            var query = from d in this.distances
                orderby d.Value ascending
                select d;
        
            var top = query.First();
            var countValue = from v in this.distances.Values
                         where v.Equals(top.Value)
                         select v;
            
            if (countValue.Count() > 1)
                return "*";
            else
                return top.Key;
        }
        else
            return "-";
    }
}

public static class ExtentionStrings
{
    public static string NextSymbol(this string data)
    {
        if (string.IsNullOrEmpty(data))
            return "A";

        char lastLetter = data[data.Length - 1];
        if (lastLetter >= 'Z')
            return data + "A";
        else
            data = data.Remove(data.Length - 1);
        return data + ++lastLetter;
    }
}