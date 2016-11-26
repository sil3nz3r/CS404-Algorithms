using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakyPathProject
{
    static class Helper
    {
        /// <summary>
        /// Extract a string with comma delimiter
        /// </summary>
        /// <param name="lineOfText"></param>
        /// <returns></returns>
        public static string[] ExtractValue(string lineOfText)
        {
            return lineOfText.Split(',').Select(value => value.Trim()).ToArray();
        }

        public static void InitMatrix(int[,] inputMatrix, int valueToInitialize)
        {
            for (int i = 0; i < inputMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < inputMatrix.GetLength(1); j++)
                {
                    inputMatrix[i, j] = valueToInitialize;
                }
            }
        }

        /// <summary>
        /// Print a 2 dimentional array in matrix format
        /// non nullable type
        /// </summary>
        /// <param name="inputMatrix"></param>
        public static void PrintIntMatrix(int[,] inputMatrix, int numOfDigits)
        {
            int rowLength = inputMatrix.GetLength(0);
            int colLength = inputMatrix.GetLength(1);
            for (int i = 0; i < rowLength; i++)
            {
                Console.Write('|');
                for (int j = 0; j < colLength; j++)
                {
                    Console.Write(String.Format("{" + String.Format("0, {0}", numOfDigits + 1) + "}", inputMatrix[i, j]));
                }
                Console.Write('|');
                Console.Write(Environment.NewLine);
            }
        }

        /// <summary>
        /// Print a 2 dimentional array in matrix format
        /// nullable type
        /// </summary>
        /// <param name="inputMatrix"></param>
        public static void PrintIntMatrix(int?[,] inputMatrix, int numOfDigits)
        {
            int rowLength = inputMatrix.GetLength(0);
            int colLength = inputMatrix.GetLength(1);
            for (int i = 0; i < rowLength; i++)
            {
                Console.Write('|');
                for (int j = 0; j < colLength; j++)
                {
                    if (inputMatrix[i, j] == null)
                    {
                        Console.Write(String.Format("{" + String.Format("0, {0}", numOfDigits + 1) + "}", "na"));
                    }
                    else
                    {
                        Console.Write(String.Format("{" + String.Format("0, {0}", numOfDigits + 1) + "}", inputMatrix[i, j]));
                    }
                }
                Console.Write('|');
                Console.Write(Environment.NewLine);
            }
        }

        public static void PrintQueueMatrix(Queue<int>[,] inputMatrix, int numOfDigits)
        {
            int rowLength = inputMatrix.GetLength(0);
            int colLength = inputMatrix.GetLength(1);

            for (int i = 0; i < rowLength; i++)
            {
                Console.Write('|');
                for (int j = 0; j < colLength; j++)
                {
                    if (inputMatrix[i, j] == null)
                    {
                        Console.Write(String.Format("{" + String.Format("0, {0}", numOfDigits + 1) + "}", "na"));
                    }
                    else
                    {
                        string Path = String.Empty;
                        foreach (var element in inputMatrix[i, j])
                        {
                            Path += element.ToString();
                        }
                        Path = String.Join<char>(",", Path);
                        Path = "(" + Path + ")";
                        Console.Write(String.Format("{" + String.Format("0, {0}", numOfDigits + 1) + "}", Path));
                    }
                }
                Console.Write('|');
                Console.Write(Environment.NewLine);
            }
        }

        public static void PrintQueueArray(Queue<int>[] inputQueueArray, int numOfDigits)
        {
            int arrayLength = inputQueueArray.Count();

            Console.Write('|');
            
            foreach (var element in inputQueueArray)
            {
                string Path = QueueToString(element);
                //Console.Write(String.Format("{" + String.Format("0, {0}", numOfDigits + 1) + "}", Path));
                Console.Write(" " + Path + " ");
            }

            Console.Write('|');
            Console.Write(Environment.NewLine);
        }

        public static string QueueToString(Queue<int> queue)
        {
            string Path = String.Empty;
            foreach (var element in queue)
            {
                Path += element.ToString();
            }
            Path = String.Join<char>(",", Path);
            Path = "(" + Path + ")";

            return Path;
        }

        public static int GetMinDistance(int[] distanceSet, bool[] vertexSet)
        {
            // Initialize min value
            int min = Int32.MaxValue;
            int min_index = Int32.MinValue;

            for (int i = 0; i < distanceSet.GetLength(0); i++)
            {
                if (vertexSet[i] == false && distanceSet[i] <= min)
                {
                    min = distanceSet[i];
                    min_index = i;
                }
            }

            return min_index;
        }
    }
}
