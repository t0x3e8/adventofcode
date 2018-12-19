using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _06b
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
            CalculateDistanceForCell(grid);

            PrintGrid(grid);
            Console.WriteLine($"the size of the region containing all locations which have a total distance to all given coordinates of less than 10000 is: {grid.GetSizeOfRegion()}");
            sw.Stop();
            Console.WriteLine($"Stopwatch stops: {sw.Elapsed.TotalSeconds}");
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
                    pointsSet.Add(point);
                    point.IsMasterPoint = true;
                    point.Symbol = symbol;

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
                else if (grid.X == -1 || point.X < grid.X)
                    grid.X = point.X;
                if (point.Y > grid.Height)
                    grid.Height = point.Y;
                else if (grid.Y == -1 || point.Y < grid.Y)
                    grid.Y = point.Y;
            }

            return grid;
        }
        
        private static void ArrangeGrid(Grid grid)
        {
            for (int y = 1; y <= grid.Height; y++)
                for (int x = 1; x <= grid.Width; x++)
                    grid.AddCell(new MyPoint(x, y));
        }

        private static void PopulateGrid(Grid grid, HashSet<MyPoint> pointsSet)
        {
            foreach (var point in pointsSet) 
            {
                grid.AddCell(point);
            }
        }

        private static void CalculateDistanceForCell(Grid grid)
        {
            HashSet<MyPoint> masterCells = grid.GetMasterCells();

            for(int y = 1; y <= grid.Height; y++)
            {
				for(int x = 1; x <= grid.Width; x++) {
					MyPoint cell = grid.GetCell(x, y);

                    int totalDistance = 0;

                    foreach(var masterCell in masterCells) {
                        int dx = Math.Abs(x - masterCell.X);
 						int dy = Math.Abs(y - masterCell.Y);
    					int distance = dx + dy;
                        totalDistance += distance;
                    }
                    cell.TotalDistance = totalDistance;
				}
            }
        }

        private static void PrintGrid(Grid grid)
        {
            for (int y = 1; y <= grid.Height; y++)
            {
                for (int x = 1; x <= grid.Width; x++)
                {
                    var cell = grid.GetCell(x, y);
                    string cellText = cell.Symbol;

                    if (!cell.IsMasterPoint)
                        cellText = cell.TotalDistance < grid.TotalDistanceLimit ? "X" : "-";

                    Console.Write(cellText);
                }
                Console.WriteLine();
            }
        }

    }
}

public class Grid
{
    public int TotalDistanceLimit {get; private set; }
    private Dictionary<string, MyPoint> CellsSet { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Grid()
    {
        this.X = this.Y = -1;
        this.CellsSet = new Dictionary<string, MyPoint>();
        this.TotalDistanceLimit = 10000;
    }

    public MyPoint GetCell(int x, int y)
    {       
        string keyText = $"{x}:{y}";
        return this.CellsSet[keyText];
    }

    public void AddCell(MyPoint point)
    {
        string keyText = $"{point.X}:{point.Y}";
        if (this.CellsSet.ContainsKey(keyText))
            this.CellsSet[keyText] = point;
        else
            this.CellsSet.Add(keyText, point);
    }

    public HashSet<MyPoint> GetMasterCells() {
        var query = from c in this.CellsSet
                    where c.Value.IsMasterPoint
                    select c.Value;

        return query.ToHashSet();
    }

    public int GetSizeOfRegion() {
        var query = from c in this.CellsSet
                    where c.Value.TotalDistance < this.TotalDistanceLimit
                    select c;

        return query.Count();
    }
}

public class MyPoint
{
    public bool IsMasterPoint { get; set; }
    public int TotalDistance {get; set; }
    public string  Symbol {get; set; }
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