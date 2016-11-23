using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SneakyPathProject
{
    class Program
    {
        static int size;
        static int startingPoint;
        static int destination;
        static int[] EdgeCost;
        static int big = 1000 ^ 3;

        static int[][,] MyDist;
        static int[][] FirstStop;
        static int[][] LastStop;
        static int[][,] PathTraffic;
        static int[] EdgeTraffic;

        static int k;

        static void Main(string[] args)
        {
            // Read the file
            List<string> inputFile = File.ReadAllLines(@"..\..\SneakyPathInputFile.txt").ToList();
            // Remove empty entries
            inputFile = inputFile.Where(s => !String.IsNullOrWhiteSpace(s)).Distinct().ToList();

            // Extract the first line
            string[] firstLine = ExtractValue(inputFile[0]);
            size = Int32.Parse(firstLine[0]);
            startingPoint = Int32.Parse(firstLine[1]);
            destination = Int32.Parse(firstLine[2]);
            inputFile.RemoveAt(0);

            int k = 0;

            // Initialize E and F matrices
            MyDist = new int[k + 1][,];
            MyDist[k] = new int[size, size];

            PathTraffic = new int[k + 1][,];
            PathTraffic[k] = new int[size, size];

            foreach (string line in inputFile)
            {
                if (String.IsNullOrEmpty(line))
                {
                    // empty entry, continue
                    continue;
                }

                string[] values = ExtractValue(line);
                int i = Int32.Parse(values[1]) - 1;
                int j = Int32.Parse(values[2]) - 1;
                if (values[0] == "E")
                {
                    MyDist[k][i, j] = Int32.Parse(values[3]);
                }
                else if (values[0] == "F")
                {
                    PathTraffic[k][i, j] = Int32.Parse(values[3]);
                }
                else
                {
                    Console.WriteLine("There is something wrong with the line: \n" + line);
                }
                //Console.WriteLine(line);
            }

            Console.WriteLine(String.Format("MyDist[{0}]: ", k));
            PrintMatrix(MyDist[k]);

            Console.WriteLine(String.Format("PathTraffic[{0}]: ", k));
            PrintMatrix(PathTraffic[k]);

            Console.ReadKey();
        }

        static string[] ExtractValue(string lineOfText)
        {
            return lineOfText.Split(',').Select(value => value.Trim()).ToArray();
        }

        static void PrintMatrix(int[,] inputMatrix)
        {
            int rowLength = inputMatrix.GetLength(0);
            int colLength = inputMatrix.GetLength(1);
            for (int i = 0; i < rowLength; i++)
            {
                Console.Write('|');
                for (int j = 0; j < colLength; j++)
                {
                    Console.Write(String.Format("{0, 2} ", inputMatrix[i, j]));
                }
                Console.Write('|');
                Console.Write(Environment.NewLine);
            }
        }
    }
}
