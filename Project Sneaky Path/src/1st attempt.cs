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
        const int big = 100000;

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
        static int[,] PathTraffic;

        /// <summary>
        /// Traffic on each edge
        /// </summary>
        static int?[,] EdgeTraffic;
        /// <summary>
        /// First place i stops
        /// </summary>
        static int[][,] FirstStop;
        static int?[] Parent;
        static int[] Distance;

        static int[][,] FlowPerEdge;
        static int[][,] FirstNewStop;

        static Queue<int>[,] ActualPath;

        static List<Queue<int>> allPaths = new List<Queue<int>>();

        static int k;

        #endregion global variables


        static void Main(string[] args)
        {
            // Read the file
            List<string> inputFile = File.ReadAllLines(@"..\..\SneakyPathInputFile.txt").ToList();
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

            Console.WriteLine("\n\n*** Iteration 1: Initialization, k = {0}", k);

            // Initialize matrices
            MyDist = new int[size + 1][,];
            MyDist[k] = new int[size, size];
            Helper.InitMatrix(MyDist[k], big);

            EdgeWeight = new int?[size, size];

            PathTraffic = new int[size, size];

            EdgeTraffic = new int?[size, size];

            FirstStop = new int[size + 1][,];
            FirstStop[k] = new int[size, size];

            // Extract input file
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
                    PathTraffic[i, j] = Int32.Parse(values[3]);
                }
                else // This line has an incorrect format
                {
                    Console.WriteLine("There is something wrong with the line: \n" + line);
                }
                //Console.WriteLine(line);

                // EdgeTraffic does not have to be initialized right now
                EdgeTraffic[i, j] = 0;
                // FirstStop is assumed to be the direct destination
                FirstStop[k][i, j] = j + 1;

                // If the distane is larger than 10, deem it big
                // This is for simplicity sake
                if (MyDist[k][i, j] > 10)
                {
                    MyDist[k][i, j] = big;
                    FirstStop[k][i, j] = -1;
                    EdgeWeight[i, j] = null;
                }
            }

            // Prevent circular link
            for (int i = 0; i < size; i++)
            {
                MyDist[k][i, i] = 0;
                FirstStop[k][i, i] = 0;
                EdgeTraffic[i, i] = 0;
                PathTraffic[i, i] = 0;
                EdgeWeight[i, i] = 0;
            }

            Console.WriteLine(String.Format("MyDist: "));
            Helper.PrintIntMatrix(MyDist[k], 4);

            Console.WriteLine(String.Format("EdgeWeight[{0}]: ", k));
            Helper.PrintIntMatrix(EdgeWeight, 2);

            Console.WriteLine(String.Format("PathTraffic[{0}]: ", k));
            Helper.PrintIntMatrix(PathTraffic, 2);

            Console.WriteLine(String.Format("EdgeTraffic[{0}]: ", k));
            Helper.PrintIntMatrix(EdgeTraffic, 2);

            Console.WriteLine("Press any key to end iteration 1");
            Console.ReadKey();

            #endregion

            #region Iteration 2: Find all the paths

            Console.WriteLine("\n\n*** Iteration 2: Find all the paths");

            FindAllPaths(source, destination);

            Console.WriteLine("Press any key to end iteration 2");
            Console.ReadKey();

            #endregion

            #region Iteration 3: Calculate the flow of each paths

            Console.WriteLine("\n\n*** Iteration 3: Find the actual path");

            //Hops = new int[size, size];
            ActualPath = new Queue<int>[size, size];

            // The path has to start from the current node
            for (int iii = 0; iii < size; iii++)
            {
                for (int jjj = 0; jjj < size; jjj++)
                {
                    ActualPath[iii, jjj] = new Queue<int>();
                    ActualPath[iii, jjj].Enqueue(iii + 1);
                }
            }

            //Queue<int> ShortestPath = new Queue<int>();
            //ShortestPath.Enqueue(startingPoint);

            //int a = startingPoint - 1;
            //int b = destination - 1;

            //while (FirstStop[size][a, b] != 0)
            //{
            //    ShortestPath.Enqueue(FirstStop[size][a, b]);
            //    int ifrom = a;
            //    a = b;
            //    b = FirstStop[size][ifrom, b] - 1;
            //    //Hops[ii, jj] = Hops[ii, jj] + 1;
            //    EdgeTraffic[ifrom, i] = EdgeTraffic[ifrom, i] + PathTraffic[ii, jj];
            //} // while

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
                        //Hops[ii, jj] = Hops[ii, jj] + 1;
                        EdgeTraffic[ifrom, i] = EdgeTraffic[ifrom, i] + PathTraffic[ii, jj];
                        ActualPath[ii, jj].Enqueue(i + 1);
                    } // while

                    Console.WriteLine("For [{0}, {1}], the actual path is {2}", ii + 1, jj + 1, Helper.QueueToString(ActualPath[ii, jj]));
                } // jj
            } // ii

            Console.WriteLine(String.Format("MyDist[{0}]: ", size));
            Helper.PrintIntMatrix(MyDist[size], 2);

            Console.WriteLine(String.Format("ActualPath: ", k));
            Helper.PrintQueueMatrix(ActualPath, 10);

            Console.WriteLine(String.Format("PathTraffic: ", k));
            Helper.PrintIntMatrix(PathTraffic, 2);

            Console.WriteLine(String.Format("EdgeTraffic: ", k));
            Helper.PrintIntMatrix(EdgeTraffic, 2);

            Console.WriteLine("Press any key to end iteration 3");
            Console.ReadKey();

            #endregion

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="d"></param>
        static void FindAllPaths(int s, int d)
        {
            // Temporary array to avoid cycle
            bool[] visitedTemp = new bool[size];
            // Temporary path array
            int[] pathTemp = new int[size];
            // Temporary Index
            int indexTemp = 0;

            for (int i = 0; i < size; i++)
            {
                visitedTemp[i] = false;
            }

            FindAllPathsUtil(s - 1, d - 1, visitedTemp, pathTemp, indexTemp);
        }

        static void FindAllPathsUtil(int current, int destination, bool[] visited, int[] path, int path_index)
        {
            visited[current] = true;
            path[path_index] = current + 1;
            path_index++;

            if (current == destination)
            {
                Queue<int> tempPath = new Queue<int>();
                for (int i = 0; i < path_index; i++)
                {
                    tempPath.Enqueue(path[i]);
                    //Console.Write(path[i]);
                }
                allPaths.Add(tempPath);
                //Console.Write(Environment.NewLine);
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    if (MyDist[0][current, i] != 0
                        && MyDist[0][current, i] != big
                        && !visited[i])
                    {
                        FindAllPathsUtil(i, destination, visited, path, path_index);
                    }
                }
            }

            path_index--;
            visited[current] = false;
        }

        // DijstraSPA attempt
        //DijstraSPA(MyDist, startingPoint, destination);

        //Queue<int>[] ActualPath = new Queue<int>[size];
        //Stack<int>[] ReversePath = new Stack<int>[size];

        //for (int i = 0; i < Parent.Count(); i++)
        //{
        //    ReversePath[i] = new Stack<int>();
        //    ActualPath[i] = new Queue<int>();

        //    ReversePath[i].Push(i + 1);
        //    int? parent = Parent[i];
        //    while (parent != null)
        //    {
        //        ReversePath[i].Push(parent.Value);
        //        parent = Parent[parent.Value - 1];
        //    }

        //    while (ReversePath[i].Count != 0)
        //    {
        //        ActualPath[i].Enqueue(ReversePath[i].Pop());
        //    }
        //}

        static char[] Color;

        public static void FindAllPaths(int?[,] adjMatrix, int start, int end)
        {
            Color = new char[size];
            for (int i = 0; i < size; i++)
            {
                Color[i] = 'w';
            }

            var path = new List<int>[size];
            int path_index = 0;
            path[path_index] = new List<int>();
            path[path_index].Add(start);

            Travese(adjMatrix, start, end, path, ref path_index);
        }

        public static void Travese(int?[,] adjMatrix, int start, int end, List<int>[] pathArray, ref int arrayIndex)
        {
            // Useing modified DFS/BFS
            Color[start - 1] = 'g';
            pathArray[arrayIndex].Add(start);
            arrayIndex++;
            pathArray[arrayIndex] = new List<int>();

            if (start == end)
            {
                return;
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    //if (Color[i] == 'w' && EdgeWeight[start - 1, i] != null)
                    //{
                    //    pathArray[arrayIndex].Add(i + 1);
                    //    Travese(adjMatrix, i + 1, end, pathArray, ref arrayIndex);
                    //}
                    pathArray[arrayIndex].Add(i + 1);
                    Travese(adjMatrix, i + 1, end, pathArray, ref arrayIndex);
                }
            }

            Color[start - 1] = 'b';
        }

        public static void DijstraSPA(int[,] Graph, int startNode, int EndNode)
        {
            char[] Color = new char[size];
            Distance = new int[size];
            Parent = new int?[size];

            for (int v = 0; v < size; v++)
            {
                Color[v] = 'w';
                Distance[v] = big;
            }

            var minHeap = new C5.IntervalHeap<Tuple<int, int>>();
            Distance[startNode - 1] = 0;
            Parent[startNode - 1] = null;
            Color[startNode - 1] = 'g';
            minHeap.Add(new Tuple<int, int>(Distance[startNode - 1], startNode));

            while (!minHeap.IsEmpty)
            {
                int selMin = minHeap.FindMin().Item2;
                minHeap.DeleteMin();

                // Black node = out of the heap for examination
                Color[selMin - 1] = 'b';

                //if (selMin == EndNode)
                //{
                //     return Parent;
                //}

                for (int i = 0; i < size; i++)
                {
                    if (Color[i] == 'b')
                    {
                        continue;
                    }

                    int currentNodeDist = Distance[selMin - 1];
                    int selectionDist = Distance[i];
                    int potentialDist = EdgeWeight[selMin - 1, i].Value;

                    if (potentialDist == big)
                    {
                        continue;
                    }

                    if (currentNodeDist + potentialDist < selectionDist)
                    {
                        Distance[i] = currentNodeDist + potentialDist;
                        Parent[i] = selMin;
                        minHeap.Add(new Tuple<int, int>(Distance[i], i + 1));
                        Color[i] = 'g';
                    }
                } // for
            } // while
        } // DijstraSPA
    }
}
