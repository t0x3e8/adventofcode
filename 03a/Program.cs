using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _03a
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine($"StopWatch started.");

            var inputDataLines = new List<FabricClaim>();
            int maxWidth = 0;
            int maxHeight = 0;

            using (var stream = File.OpenRead("Input.txt"))
            {
                var rdr = new StreamReader(stream);

                while (!rdr.EndOfStream)
                {
                    string line = rdr.ReadLine();
                    FabricClaim claim = LineToFabricClaim(line);
                    inputDataLines.Add(claim);

                    maxWidth = Math.Max(maxWidth, claim.Left + claim.Width);
                    maxHeight = Math.Max(maxHeight, claim.Top + claim.Height);
                }
            }

            Dictionary<int, int> board = new Dictionary<int, int>();
            for (int i = 1; i <= maxHeight * maxWidth; i++) board.Add(i, 0);

            foreach(var claim in inputDataLines) {
                for (int h = claim.Top; h < claim.Top + claim.Height; h++) {
                    for (int w = claim.Left; w < claim.Left + claim.Width; w++) {
                        int cellNumber = (h * maxWidth) + w + 1;
                        board[cellNumber]++;
                    }
                }
            }

            int overlaps = board.Values.Where(v => v >= 2).Count();
            Console.WriteLine($"Overlaping fabric claims is: { overlaps }");

            sw.Stop();
            Console.WriteLine($"Stopwatch stops: {sw.Elapsed.TotalSeconds}");
        }

        private static FabricClaim LineToFabricClaim(string line)
        {
            Regex regex = new Regex(@"^#(\d+) @ (\d+),(\d+): (\d+)x(\d+)$");
            Match match = regex.Match(line);

            FabricClaim claim = new FabricClaim();
            claim.Id = int.Parse(match.Groups[1].Value);
            claim.Left = int.Parse(match.Groups[2].Value);
            claim.Top = int.Parse(match.Groups[3].Value);
            claim.Width = int.Parse(match.Groups[4].Value);
            claim.Height = int.Parse(match.Groups[5].Value);

            return claim;
        }
    }
}
    public struct FabricClaim {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Id { get; set; }
    }
