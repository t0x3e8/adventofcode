using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _08a
{
    class Program
    {
        static bool IsDebug = true;

        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine($"StopWatch started.");

            var nodes = ReadInputFile("input.txt");
            var sumOfNodesMetadatas = nodes.Sum(n => n.Metadata.Sum());
            Console.WriteLine($"The sum of all metadata entries is {sumOfNodesMetadatas}");

            sw.Stop();
            Console.WriteLine($"Stopwatch stops: {sw.Elapsed.TotalSeconds}");
        }

        private static List<Node> ReadInputFile(string inputFilePath)
        {
            List<Node> nodes = new List<Node>();
            
            using (var stream = File.OpenRead(inputFilePath))
            {
                var rdr = new StreamReader(stream);
                string input = rdr.ReadToEnd();

                ReadNodes(input.Split(' '), 0, ref nodes);
            }

            return nodes;
        }

        private static int ReadNodes(string [] numbers, int currentPos, ref List<Node> nodes) {
            int childQuantity = -1;
            int metadataQuantity = -1;
            Node tempNode = null;
            
            for(; currentPos < numbers.Length; currentPos++) {
                int number = int.Parse(numbers[currentPos]);

                // read child quantity
                if (childQuantity == -1) {
                    childQuantity = number;
                }
                // read metadata quantity
                else if (metadataQuantity == -1) {
                    metadataQuantity = number;
                }
                // read metadata numbers or create a new node
                else if (childQuantity != -1 && metadataQuantity != -1) {
                    for(int childNumber = 1; childNumber <= childQuantity; childNumber++) {
                        currentPos = ReadNodes(numbers, currentPos, ref nodes);
                    }                     
                    
                    tempNode = new Node() {ChildQuantity = childQuantity, MetadataQuantity = metadataQuantity};

                    int metadataQuantityMaxPosition = currentPos + metadataQuantity;
                    for(; currentPos < metadataQuantityMaxPosition; currentPos++) {
                        tempNode.Metadata.Add(int.Parse(numbers[currentPos]));
                    }

                    nodes.Add(tempNode);
                    childQuantity = metadataQuantity = -1; // reset
                    return currentPos;
                }
            }

            return currentPos;
        }
    }

    public class Node {
        public int ChildQuantity { get; set; }
        public int MetadataQuantity { get; set; }
        public List<int> Metadata { get; set; }
        public Node()
        {
             this.Metadata = new List<int>();
        }
    }
}