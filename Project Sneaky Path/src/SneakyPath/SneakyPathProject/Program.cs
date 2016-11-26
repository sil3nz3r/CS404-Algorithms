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
        #region global variables

        /// <summary>
        /// number of vertices/cities in the graph
        /// </summary>
        static int size;

        /// <summary>
        /// Starting point of the path in question
        /// </summary>
        static int source;

        /// <summary>
        /// Destination point of the path in question
        /// </summary>
        static int destination;

        /// <summary>
        /// Represent infinity
        /// </summary>
        const int big = 10000;

        /// <summary>
        /// Distance Calculation matrix
        /// </summary>
        static int[][,] MyDist;

        /// <summary>
        /// The original Edge weight matrix
        /// </summary>
        static int?[,] EdgeWeight;

        /// <summary>
        /// The Flow matrix F representing the traffic on the path
        /// </summary>
        static int[,] FlowMatrix;

        /// <summary>
        /// The Load matrix L representing the traffic on each edge
        /// </summary>
        static int?[,] LoadMatrix;

        /// <summary>
        /// First place i stops
        /// </summary>
        static int[][,] FirstStop;

        /// <summary>
        /// Matrix of shortest paths
        /// </summary>
        static Queue<int>[,] ShortestPaths;

        /// <summary>
        /// All the paths from the source to the destination
        /// </summary>
        static List<Queue<int>> allPaths = new List<Queue<int>>();

        /// <summary>
        /// Sneaky Path
        /// </summary>
        static Queue<int> SneakyPath = new Queue<int>();

        static int?[,] allPathsWithTraffic;
        /// <summary>
        /// multipurpose counting variable
        /// </summary>
        static int k;

        #endregion global variables


        static void Main(string[] args)
        {
            // Read the file
            List<string> inputFile = File.ReadAllLines("N6.txt").ToList();
            // Remove empty entries
            inputFile = inputFile.Where(s => !String.IsNullOrWhiteSpace(s)).Distinct().ToList();

            // Extract the following info from first line
            // + Number of cities
            // + Starting point
            // + Destination
            string[] firstLine = Helper.ExtractValue(inputFile[0]);
            size = Int32.Parse(firstLine[0]);
            source = Int32.Parse(firstLine[1]);
            destination = Int32.Parse(firstLine[2]);
            inputFile.RemoveAt(0);

            #region Iteration 1: Initialization, k = 0
            k = 0;

            Console.WriteLine("\n\n*** Iteration 1: Initialization");

            // Initialize matrices
            MyDist = new int[size + 1][,];
            MyDist[k] = new int[size, size];
            Helper.InitMatrix(MyDist[k], big);

            EdgeWeight = new int?[size, size];

            FlowMatrix = new int[size, size];

            LoadMatrix = new int?[size, size];

            FirstStop = new int[size + 1][,];
            FirstStop[k] = new int[size, size];

            // Extract the rest of the input file
            foreach (string line in inputFile)
            {
                if (String.IsNullOrEmpty(line))
                {
                    // empty entry, continue
                    continue;
                }

                string[] values = Helper.ExtractValue(line);
                int i = Int32.Parse(values[1]) - 1;
                int j = Int32.Parse(values[2]) - 1;

                if (values[0] == "E") // Weight matrix E
                {
                    MyDist[k][i, j] = Int32.Parse(values[3]);
                    EdgeWeight[i, j] = Int32.Parse(values[3]);
                }
                else if (values[0] == "F") // Flow matrix F
                {
                    FlowMatrix[i, j] = Int32.Parse(values[3]);
                }
                else // This line has an incorrect format
                {
                    Console.WriteLine("There is something wrong with the line: \n" + line);
                }

                // EdgeTraffic does not have to be initialized right now
                LoadMatrix[i, j] = 0;
                // FirstStop is assumed to be the direct destination
                FirstStop[k][i, j] = j + 1;
            }

            // Prevent circular link
            for (int i = 0; i < size; i++)
            {
                MyDist[k][i, i] = 0;
                FirstStop[k][i, i] = 0;
                LoadMatrix[i, i] = 0;
                FlowMatrix[i, i] = 0;
                EdgeWeight[i, i] = 0;
            }

            Console.WriteLine(String.Format("MyDist: "));
            Helper.PrintIntMatrix(MyDist[k], 6);

            Console.WriteLine(String.Format("EdgeWeight[{0}]: ", k));
            Helper.PrintIntMatrix(EdgeWeight, 2);

            Console.WriteLine(String.Format("PathTraffic[{0}]: ", k));
            Helper.PrintIntMatrix(FlowMatrix, 2);

            Console.WriteLine(String.Format("EdgeTraffic[{0}]: ", k));
            Helper.PrintIntMatrix(LoadMatrix, 4);

            Console.WriteLine("Press any key to end iteration 1");
            Console.ReadKey();

            #endregion

            #region Iteration 2: Finding the all-pairs shortest paths
            Console.WriteLine("\n\n*** Iteration 2: Finding the all-pairs shortest paths", size);

            // Using Floyd-Warshall, find the all-pairs shortest paths
            for (k = 1; k <= size; k++)
            {
                MyDist[k] = new int[size, size];
                FirstStop[k] = new int[size, size];

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        // First route: go from one node to the other
                        int directPath = MyDist[k - 1][i, j];
                        // Second route: go to a intermediate node k
                        int altPath = MyDist[k - 1][i, k - 1] + MyDist[k - 1][k - 1, j];

                        // Compare the two routes, take the smaller
                        // or favors the direct option in case of tie
                        // We still use FirstStop and LastStop
                        // FirstStop
                        if (directPath <= altPath)
                        {
                            // Direct route
                            MyDist[k][i, j] = directPath;
                            FirstStop[k][i, j] = FirstStop[k - 1][i, j];
                        }
                        else
                        {
                            // intermediate route
                            MyDist[k][i, j] = altPath;
                            FirstStop[k][i, j] = FirstStop[k - 1][i, k - 1];
                        }
                    } // j
                } // i
            } // k

            k = 6;
            Console.WriteLine("> k = {0}", k);
            Console.WriteLine(String.Format("MyDist[{0}]: ", k));
            Helper.PrintIntMatrix(MyDist[k], 4);

            Console.WriteLine(String.Format("FirstStop[{0}]: ", k));
            Helper.PrintIntMatrix(FirstStop[k], 2);

            Console.WriteLine("Press any key to end iteration 2");
            Console.ReadKey();

            #endregion

            #region Iteration 3: Calculate the traffic on each edge

            Console.WriteLine("\n\n*** Iteration 3: Calculate the traffic on each edge");

            ShortestPaths = new Queue<int>[size, size];

            // The path has to start from the current node
            for (int iii = 0; iii < size; iii++)
            {
                for (int jjj = 0; jjj < size; jjj++)
                {
                    ShortestPaths[iii, jjj] = new Queue<int>();
                    ShortestPaths[iii, jjj].Enqueue(iii + 1);
                }
            }

            // Calculate the load on each edge using the previous calculated app-pairs shortest paths
            for (int ii = 0; ii < size; ii++)
            {
                for (int jj = 0; jj < size; jj++)
                {
                    int i = ii;
                    int j = jj;

                    while (FirstStop[size][i, j] != 0)
                    {
                        int ifrom = i;
                        i = FirstStop[size][i, j] - 1;
                        LoadMatrix[ifrom, i] = LoadMatrix[ifrom, i] + FlowMatrix[ii, jj];

                        ShortestPaths[ii, jj].Enqueue(i + 1);
                    } // while

                    if (EdgeWeight[ii, jj] == null)
                    {
                        LoadMatrix[ii, jj] = null;
                    }

                    //Console.WriteLine("For [{0}, {1}], the shortest path is {2}", ii + 1, jj + 1, Helper.QueueToString(ShortestPaths[ii, jj]));
                } // jj
            } // ii

            Console.WriteLine(String.Format("MyDist[{0}]: ", size));
            Helper.PrintIntMatrix(MyDist[size], 2);

            Console.WriteLine(String.Format("ActualPath: ", k));
            Helper.PrintQueueMatrix(ShortestPaths, 10);

            Console.WriteLine(String.Format("PathTraffic: ", k));
            Helper.PrintIntMatrix(FlowMatrix, 2);

            Console.WriteLine(String.Format("EdgeTraffic: ", k));
            Helper.PrintIntMatrix(LoadMatrix, 4);

            Console.WriteLine("Press any key to end iteration 3");
            Console.ReadKey();

            #endregion

            #region Iteration 5: Find the Sneaky Path (the path with the least amount of traffic)

            Console.WriteLine("\n\n*** Iteration 4: Find the Sneaky Path");

            Queue<int> shortestPath = DijstraSPA(LoadMatrix, source, destination);

            if (shortestPath != null)
            {
                Console.WriteLine("Sneaky Path: {0}", Helper.QueueToString(shortestPath));
            }
            else
            {
                Console.WriteLine("Could not find the Sneaky Path");
            }

            Console.WriteLine("Press any key to end iteration 4");
            Console.ReadKey();

            #endregion

        }

        public static Queue<int> DijstraSPA(int?[,] Graph, int startNode, int EndNode)
        {
            // Variable declaration and initialization
            bool[] visited = new bool[size];
            int[] distance = new int[size];
            int[] parent = new int[size];
            Queue<int> ShortestPath = new Queue<int>();

            // Assign default values to visited and distance array
            for (int v = 0; v < size; v++)
            {
                visited[v] = false;
                distance[v] = big;
            }

            // MinHeap for the algorithm
            // First int is for the distance
            // Second int is for the node number
            var minHeap = new C5.IntervalHeap<Tuple<int, int>>();

            distance[startNode - 1] = 0;
            minHeap.Add(new Tuple<int, int>(distance[startNode - 1], startNode));

            while (!minHeap.IsEmpty)
            {
                int selMin = minHeap.FindMin().Item2;
                minHeap.DeleteMin();

                // This node is being visited for examination
                visited[selMin - 1] = true;

                // Found the destination. Terminate
                if (selMin == EndNode)
                {
                    int current = destination;
                    Stack<int> shortestPathStack = new Stack<int>();

                    while (current != 0)
                    {
                        shortestPathStack.Push(current);
                        current = parent[current - 1];
                    }

                    Queue<int> shortestPath = new Queue<int>();
                    int stackCount = shortestPathStack.Count;
                    for (int i = 0; i < stackCount; i++)
                    {
                        shortestPath.Enqueue(shortestPathStack.Pop());
                    }

                    return shortestPath;
                }

                for (int i = 0; i < size; i++)
                {
                    // This node has been visited and examined
                    if (visited[i] == true)
                    {
                        continue;
                    }

                    // There is no edge between the current node and the potential node
                    // big and null mean the same thing
                    // Prevent overflow If use big as max int value
                    int? potentialDist = Graph[selMin - 1, i];
                    if (potentialDist == big || potentialDist == null)
                    {
                        continue;
                    }

                    int currentNodeDist = distance[selMin - 1];
                    int selectionDist = distance[i];

                    if (currentNodeDist + potentialDist.Value < selectionDist)
                    {
                        distance[i] = currentNodeDist + potentialDist.Value;
                        parent[i] = selMin;
                        minHeap.Add(new Tuple<int, int>(distance[i], i + 1));
                    }
                } // for loop
            } // while loop

            // Cannot find the path, return null
            return null;
        } // DijstraSPA
    }
}
