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

        static int size;
        static int startingPoint;
        static int destination;
        const int big = 1000 ^ 3;

        static int[][,] MyDist;
        static int?[,] EdgeCost;
        static int[,] PathTraffic;
        static int?[,] EdgeTraffic;
        /// <summary>
        /// First place i stops
        /// </summary>
        static int[][,] FirstStop;
        /// <summary>
        /// Last place j stops
        /// </summary>
        static int[][,] LastStop;
        static int[,] Hops;
        static Queue<int>[,] ActualPath;

        static int[][,] FlowPerEdge;
        static int[][,] FirstNewStop;
        static int[][,] LastNewStop;

        static int k;

        #endregion global variables


        static void Main(string[] args)
        {
            // Read the file
            List<string> inputFile = File.ReadAllLines(@"N10b.txt").ToList();
            // Remove empty entries
            inputFile = inputFile.Where(s => !String.IsNullOrWhiteSpace(s)).Distinct().ToList();

            // Extract the following info from first line
            // + Number of cities
            // + Starting point
            // + Destination
            string[] firstLine = Helper.ExtractValue(inputFile[0]);
            size = Int32.Parse(firstLine[0]);
            startingPoint = Int32.Parse(firstLine[1]);
            destination = Int32.Parse(firstLine[2]);
            inputFile.RemoveAt(0);

            #region Iteration 1: Initialization, k = 0
            k = 0;

            Console.WriteLine("\n\n*** Iteration 1: Initialization, k = {0}", k);

            // Initialize matrices
            MyDist = new int[size + 1][,];
            MyDist[k] = new int[size, size];
            Helper.InitMatrix(MyDist[k], big);

            EdgeCost = new int?[size, size];

            PathTraffic = new int[size, size];

            EdgeTraffic = new int?[size, size];

            FirstStop = new int[size + 1][,];
            FirstStop[k] = new int[size, size];

            LastStop = new int[size + 1][,];
            LastStop[k] = new int[size, size];

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
                    EdgeCost[i, j] = MyDist[k][i, j];
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
                FirstStop[k][i, j] = j + 1;
                LastStop[k][i, j] = i + 1;

                //// If the distane is larger than 10, deem it big
                //// This is for simplicity sake
                //if (MyDist[k][i, j] > 10)
                //{
                //    MyDist[k][i, j] = big;
                //    FirstStop[k][i, j] = -1;
                //    LastStop[k][i, j] = -1;
                //    EdgeCost[i, j] = null;
                //}
            }

            // Prevent circular link
            for (int i = 0; i < size; i++)
            {
                MyDist[k][i, i] = 0;
                FirstStop[k][i, i] = 0;
                LastStop[k][i, i] = 0;
                EdgeTraffic[i, i] = 0;
                PathTraffic[i, i] = 0;
                EdgeCost[i, i] = 0;
            }

            Console.WriteLine(String.Format("MyDist[{0}]: ", k));
            Helper.PrintIntMatrix(MyDist[k], 4);

            Console.WriteLine(String.Format("EdgeCost[{0}]: ", k));
            Helper.PrintIntMatrix(EdgeCost, 2);

            Console.WriteLine(String.Format("PathTraffic[{0}]: ", k));
            Helper.PrintIntMatrix(PathTraffic, 2);

            Console.WriteLine(String.Format("EdgeTraffic[{0}]: ", k));
            Helper.PrintIntMatrix(EdgeTraffic, 2);

            Console.WriteLine(String.Format("FirstStop[{0}]: ", k));
            Helper.PrintIntMatrix(FirstStop[k], 2);

            Console.WriteLine(String.Format("LastStop[{0}]: ", k));
            Helper.PrintIntMatrix(LastStop[k], 2);

            Console.WriteLine("Press any key to end iteration 1");
            //Console.ReadKey();

            #endregion

            #region Iteration 2, k = 1 to n (6)
            Console.WriteLine("\n\n*** Iteration 2: k = 1 to {0}", size);

            for (k = 1; k <= size; k++)
            {
                MyDist[k] = new int[size, size];
                FirstStop[k] = new int[size, size];
                LastStop[k] = new int[size, size];

                Console.WriteLine("k = {0}", k);

                for (int i = 0; i < size; i++)
                {
                    //Console.WriteLine("  i = {0}", i + 1);
                    for (int j = 0; j < size; j++)
                    {
                        //Console.WriteLine("    j = {0}", j + 1);
                        // First route: go from one node to the other
                        int optionDirect = MyDist[k - 1][i, j];
                        // Second route: go to a intermediate node k
                        int optionViaK = MyDist[k - 1][i, k - 1] + MyDist[k - 1][k - 1, j];

                        // Compare the two routes, take the smaller
                        // or favors the direct option in case of tie
                        // We still use FirstStop and LastStop
                        // FirstStop
                        if (optionDirect <= optionViaK)
                        {
                            // Direct route
                            MyDist[k][i, j] = optionDirect;
                            FirstStop[k][i, j] = FirstStop[k - 1][i, j];
                            LastStop[k][i, j] = LastStop[k - 1][i, j];
                            //Console.WriteLine("    Direct: then {0}, {1}", optionDirect, optionViaK);
                        }
                        else
                        {
                            // intermediate route
                            MyDist[k][i, j] = optionViaK;
                            FirstStop[k][i, j] = FirstStop[k - 1][i, k - 1];
                            LastStop[k][i, j] = LastStop[k - 1][k - 1, j];
                            //Console.WriteLine("    Via K: else {0}, {1}", optionDirect, optionViaK);
                        }
                    } // j
                } // i
            } // k

            k = 0;
            Console.WriteLine("> k = {0}", k);

            Console.WriteLine(String.Format("MyDist[{0}]: ", k));
            Helper.PrintIntMatrix(MyDist[k], 4);

            Console.WriteLine(String.Format("FirstStop[{0}]: ", k));
            Helper.PrintIntMatrix(FirstStop[k], 2);

            Console.WriteLine(String.Format("LastStop[{0}]: ", k));
            Helper.PrintIntMatrix(LastStop[k], 2);

            k = 6;
            Console.WriteLine("> k = {0}", k);
            Console.WriteLine(String.Format("MyDist[{0}]: ", k));
            Helper.PrintIntMatrix(MyDist[k], 4);

            Console.WriteLine(String.Format("FirstStop[{0}]: ", k));
            Helper.PrintIntMatrix(FirstStop[k], 2);

            Console.WriteLine(String.Format("LastStop[{0}]: ", k));
            Helper.PrintIntMatrix(LastStop[k], 2);

            //Console.WriteLine(String.Format("FirstStop[{0}]: ", 1));
            //Helper.PrintIntMatrix(FirstStop[1], 2);

            Console.WriteLine("Press any key to end iteration 2");
            //Console.ReadKey();

            #endregion

            #region Iteration 3, Find the path and calculate traffic (8-10)

            Console.WriteLine("\n\n*** Iteration 3: Find the actual path");

            Hops = new int[size, size];
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
                        Hops[ii, jj] = Hops[ii, jj] + 1;
                        EdgeTraffic[ifrom, i] = EdgeTraffic[ifrom, i] + PathTraffic[ii, jj];
                        ActualPath[ii, jj].Enqueue(i + 1);
                    } // while

                    //Console.WriteLine("For [{0}, {1}], the actual path is {2}", ii + 1, jj + 1, Helper.QueueToString(ActualPath[ii, jj]));
                } // jj
            } // ii

            Console.WriteLine(String.Format("MyDist[{0}]: ", size));
            Helper.PrintIntMatrix(MyDist[size], 2);

            Console.WriteLine(String.Format("ActualPath: ", k));
            Helper.PrintQueueMatrix(ActualPath, 10);

            Console.WriteLine(String.Format("Hops: ", k));
            Helper.PrintIntMatrix(Hops, 2);

            Console.WriteLine(String.Format("PathTraffic: ", k));
            Helper.PrintIntMatrix(PathTraffic, 2);

            Console.WriteLine(String.Format("EdgeTraffic: ", k));
            Helper.PrintIntMatrix(EdgeTraffic, 2);

            Console.WriteLine("Press any key to end iteration 3");
            //Console.ReadKey();

            #endregion

            #region Ieration 4: Calculate flow

            Console.WriteLine("\n\n*** Iteration 4: Calculate flow", size);

            FlowPerEdge = new int[size + 1][,];
            FlowPerEdge[0] = new int[size, size];

            FirstNewStop = new int[size + 1][,];
            FirstNewStop[0] = new int[size, size];

            LastNewStop = new int[size + 1][,];
            LastNewStop[0] = new int[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (MyDist[0][i, j] == big)
                    {
                        FlowPerEdge[0][i, j] = big;
                        EdgeTraffic[i, j] = null;
                    }
                    else
                    {
                        FlowPerEdge[0][i, j] = Math.Max(0, EdgeTraffic[i, j].Value);
                    }
                }
                FlowPerEdge[0][i, i] = 0;
            }

            Console.WriteLine(String.Format("FlowPerEdge[0]: "));
            Helper.PrintIntMatrix(FlowPerEdge[0], 4);

            Console.WriteLine(String.Format("Hops: ", k));
            Helper.PrintIntMatrix(Hops, 2);

            Console.WriteLine(String.Format("ActualPath: ", k));
            Helper.PrintQueueMatrix(ActualPath, 10);

            Console.WriteLine(String.Format("PathTraffic: ", k));
            Helper.PrintIntMatrix(PathTraffic, 2);

            Console.WriteLine(String.Format("EdgeTraffic: ", k));
            Helper.PrintIntMatrix(EdgeTraffic, 2);

            Console.WriteLine("Press any key to end iteration 4");
            //Console.ReadKey();

            #endregion

            #region Iteration 5: Reset values (11-12)

            Console.WriteLine("\n\n*** Iteration 5: Reset values");

            FirstNewStop[0] = new int[size, size];
            LastNewStop[0] = new int[size, size];
            k = 0;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    FirstNewStop[k][i, j] = j + 1;
                    LastNewStop[k][i, j] = i + 1;
                }
                FirstNewStop[k][i, i] = 0;
                LastNewStop[k][i, i] = 0;
            }

            Console.WriteLine(String.Format("Hops: ", k));
            Helper.PrintIntMatrix(Hops, 2);

            Console.WriteLine(String.Format("ActualPath: ", k));
            Helper.PrintQueueMatrix(ActualPath, 10);

            Console.WriteLine(String.Format("PathTraffic: ", k));
            Helper.PrintIntMatrix(PathTraffic, 2);

            Console.WriteLine(String.Format("EdgeTraffic: ", k));
            Helper.PrintIntMatrix(EdgeTraffic, 2);

            Console.WriteLine("Press any key to end iteration 5");
            //Console.ReadKey();

            #endregion

            #region Iteration 6: (13-14)

            Console.WriteLine("\n\n*** Iteration 6");

            for (k = 1; k <= size; k++)
            {
                FlowPerEdge[k] = new int[size, size];
                FirstNewStop[k] = new int[size, size];
                LastNewStop[k] = new int[size, size];

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        int optionDirect = FlowPerEdge[k - 1][i, j];
                        int optionViaK = FlowPerEdge[k - 1][i, k - 1] + FlowPerEdge[k - 1][k - 1, j];

                        if (optionDirect <= optionViaK)
                        {
                            FlowPerEdge[k][i, j] = optionDirect;
                            FirstNewStop[k][i, j] = FirstNewStop[k - 1][i, j];
                            LastNewStop[k][i, j] = LastNewStop[k - 1][i, j];
                        }
                        else
                        {
                            FlowPerEdge[k][i, j] = optionViaK;
                            FirstNewStop[k][i, j] = FirstNewStop[k - 1][i, k - 1];
                            LastNewStop[k][i, j] = LastNewStop[k - 1][k - 1, j];
                        }
                    } // j
                } // i
            } // k

            k = 0;
            Console.WriteLine("> k = {0}", k);

            Console.WriteLine(String.Format("FlowPerEdge[{0}]: ", k));
            Helper.PrintIntMatrix(FlowPerEdge[k], 4);
            Console.WriteLine(String.Format("FirstNewStop[{0}]: ", k));
            Helper.PrintIntMatrix(FirstNewStop[k], 2);
            Console.WriteLine(String.Format("LastNewStop[{0}]: ", k));
            Helper.PrintIntMatrix(LastNewStop[k], 2);

            k = size;
            Console.WriteLine("> k = {0}", k);

            Console.WriteLine(String.Format("FlowPerEdge[{0}]: ", k));
            Helper.PrintIntMatrix(FlowPerEdge[k], 4);
            Console.WriteLine(String.Format("FirstNewStop[{0}]: ", k));
            Helper.PrintIntMatrix(FirstNewStop[k], 2);
            Console.WriteLine(String.Format("LastNewStop[{0}]: ", k));
            Helper.PrintIntMatrix(LastNewStop[k], 2);

            Console.WriteLine("Press any key to end iteration 6");
            //Console.ReadKey();

            #endregion

            #region Iteration 7: Find Min and Mand and Average paths (15-16)

            //while (FirstNewStop[size][a2, b2] != 0)
            //{
            //    ShortestPath2.Enqueue(FirstNewStop[size][a2, b2]);
            //    int ifrom = a2;
            //    b2 = FirstNewStop[size][a2, b2] - 1;
            //    //Hops[ii, jj] = Hops[ii, jj] + 1;
            //    //EdgeTraffic[ifrom, i] = EdgeTraffic[ifrom, i] + PathTraffic[ii, jj];
            //    a2 = b2;
            //} // while

            int[,] Hops2 = new int[size, size];
            Queue<int>[,] ActualPath2 = new Queue<int>[size, size];

            for (int iii = 0; iii < size; iii++)
            {
                for (int jjj = 0; jjj < size; jjj++)
                {
                    ActualPath2[iii, jjj] = new Queue<int>();
                    ActualPath2[iii, jjj].Enqueue(iii + 1);
                }
            }

            int[,] MinSneakyPath = new int[size, size];
            int[,] MaxSneakyPath = new int[size, size];
            int[,] AvgSneakyPath = new int[size, size];

            for (int ii = 0; ii < size; ii++)
            {
                for (int jj = 0; jj < size; jj++)
                {
                    int i = ii;
                    int j = jj;

                    MinSneakyPath[ii, jj] = big;
                    MaxSneakyPath[ii, jj] = 0;
                    AvgSneakyPath[ii, jj] = 0;

                    while (FirstNewStop[size][i, j] != 0)
                    {
                        int ifrom = i;
                        i = FirstNewStop[size][i, j] - 1;
                        Hops2[ii, jj] = Hops2[ii, jj] + 1;
                        MinSneakyPath[ii, jj] = Math.Min(MinSneakyPath[ii, jj], FlowPerEdge[k][ifrom, i]);
                        MaxSneakyPath[ii, jj] = Math.Max(MaxSneakyPath[ii, jj], FlowPerEdge[k][ifrom, i]);
                        AvgSneakyPath[ii, jj] = AvgSneakyPath[ii, jj] + FlowPerEdge[k][ifrom, i];

                        ActualPath2[ii, jj].Enqueue(i + 1);
                    }

                    if (Hops2[ii, jj] > 0)
                    {
                        AvgSneakyPath[ii, jj] = AvgSneakyPath[ii, jj] / Hops2[ii, jj];
                    }
                }
            }

            for (int iii = 0; iii < size; iii++)
            {
                MinSneakyPath[iii, iii] = 0;
            }

            Console.WriteLine(String.Format("ActualPath: ", k));
            Helper.PrintQueueMatrix(ActualPath, 10);

            Console.WriteLine(String.Format("ActualPath2: ", k));
            Helper.PrintQueueMatrix(ActualPath2, 10);

            Console.WriteLine(String.Format("MinSneakyPath: ", k));
            Helper.PrintIntMatrix(MinSneakyPath, 2);

            Console.WriteLine(String.Format("MaxSneakyPath: ", k));
            Helper.PrintIntMatrix(MaxSneakyPath, 2);

            Console.WriteLine(String.Format("EdgeCost: ", k));
            Helper.PrintIntMatrix(EdgeCost, 2);

            Console.WriteLine(String.Format("PathTraffic: ", k));
            Helper.PrintIntMatrix(PathTraffic, 2);

            Console.WriteLine(String.Format("EdgeTraffic: ", k));
            Helper.PrintIntMatrix(EdgeTraffic, 2);

            Console.WriteLine(String.Format("Sneaky Path: {0}", Helper.QueueToString(ActualPath2[startingPoint - 1, destination - 1])));

            Console.WriteLine("Press any key to end iteration 7");
            Console.ReadKey();

            #endregion

        }
    }
}
