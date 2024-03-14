using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace Task_02
{
    class Program
    {
        static void Main()
        {
            const int height = 10000;
            const int heightwidth = 5000;
            const int width = 10000;
            int[] threadNums = new int[] { 2, 4, 8, 16, 32 };

            int[,] matrix1 = GenerateRandomMatrix(height, heightwidth);
            int[,] matrix2 = GenerateRandomMatrix(heightwidth, width);

            // Sequential multiplication
            int[,] result = new int[height, width];

            var stopwatchSequential = new Stopwatch();
            stopwatchSequential.Start();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int sum = 0;
                    for (int k = 0; k < heightwidth; k++)
                    {
                        sum += matrix1[i, k] * matrix2[k, j];
                    }
                    result[i, j] = sum;
                }
            }

            stopwatchSequential.Stop();
            Console.WriteLine($"Time spent on sequential multiplication: {stopwatchSequential.ElapsedMilliseconds} ms\n");

            // Parallel multiplication
            foreach (var threadNum in threadNums)
            {
                var temp = MultiplyMatricesAdaptive(matrix1, matrix2, threadNum);
                if (!EqualMatrices(result, temp))
                    Console.WriteLine("An unexpected error occured!");
            }
            Console.ReadLine();
        }
        public static int[,] MultiplyMatricesAdaptive(int[,] matrix1, int[,] matrix2, int threadNum)
        {
            int[,] result = new int[matrix1.GetLength(0), matrix2.GetLength(1)];
            int same = matrix1.GetLength(1);

            var stopwatchParallel = new Stopwatch();
            stopwatchParallel.Start();
            List<Thread> threads = new List<Thread>();

            if (matrix1.GetLength(0) > matrix2.GetLength(1))
            {
                int remainder = matrix1.GetLength(0) % threadNum;
                int remainderTrack = 0;

                for (int i = 0; i < threadNum; i++)
                {
                    int from = i * (matrix1.GetLength(0) / threadNum) + remainderTrack;
                    int to = (i + 1) * (matrix1.GetLength(0) / threadNum) + remainderTrack;

                    if (remainder > 0)
                    {
                        to++;
                        remainder--;
                        remainderTrack++;
                    }
                    Thread thread = new Thread(() => MutliplyMatricesInArea(matrix1, matrix2, result, from, to, 0, matrix2.GetLength(1)));
                    threads.Add(thread);
                }
            }
            else
            {
                int remainder = matrix2.GetLength(1) % threadNum;
                int remainderTrack = 0;

                for (int i = 0; i < threadNum; i++)
                {
                    int from = i * (matrix2.GetLength(1) / threadNum) + remainderTrack;
                    int to = (i + 1) * (matrix2.GetLength(1) / threadNum) + remainderTrack;

                    if (remainder > 0)
                    {
                        to++;
                        remainder--;
                        remainderTrack++;
                    }
                    Thread thread = new Thread(() => MutliplyMatricesInArea(matrix1, matrix2, result, 0, matrix1.GetLength(0), from, to));
                    threads.Add(thread);
                }
            }

            foreach (var x in threads)
                x.Start();
            foreach (var x in threads)
                x.Join();

            stopwatchParallel.Stop();
            Console.WriteLine($"Time spent on parallel multiplication using {threadNum} threads: {stopwatchParallel.ElapsedMilliseconds} ms");
            return result;
        }
        public static int[,] GenerateRandomMatrix(int height, int width)
        {
            Random random = new Random();
            int[,] result = new int[height, width];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    result[i, j] = random.Next(0, 10000);

            return result;
        }
        public static void MutliplyMatricesInArea(int[,] matrix1, int[,] matrix2, int[,] changed, int fromFirst, int toFirst, int fromSecond, int toSecond)
        {
            for (int i = fromFirst; i < toFirst; i++)
            {
                for (int j = fromSecond; j < toSecond; j++)
                {
                    int sum = 0;
                    for (int q = 0; q < matrix1.GetLength(1); q++)
                    {
                        sum += matrix1[i, q] * matrix2[q, j];
                    }
                    changed[i, j] = sum;
                }
            }
        }
        public static bool EqualMatrices(int[,] matrix1, int[,] matrix2)
        {
            if (matrix1.GetLength(0) != matrix2.GetLength(0) || matrix1.GetLength(1) != matrix2.GetLength(1))
                return false;
            for (int i = 0; i < matrix1.GetLength(0); i++)
                for (int j = 0; j < matrix1.GetLength(1); j++)
                    if (matrix1[i, j] != matrix2[i, j])
                        return false;
            return true;
        }
    }
}