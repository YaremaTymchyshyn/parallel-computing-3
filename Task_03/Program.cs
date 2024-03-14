using System;
using System.Collections.Generic;
using System.Threading;

namespace Task_03
{
    class Program
    {
        private const int Size = 10000;
        private const double Epsilon = 0.001;
        static void Main()
        {
            var generated = GenerateRandomLinearEquation(Size);
            int[] threadNum = { 2, 4, 8, 16 };

            double[] LinearRes = new double[Size];

            Console.WriteLine("/// Starting linear:");
            var linStart = DateTime.Now;
            JacobiLinear(Size, generated["Matrix"], generated["Free elements"], ref LinearRes, Epsilon);
            var linFinish = DateTime.Now;
            Console.WriteLine($"Time for linear: {linFinish - linStart}\n");

            foreach (var numOfThreads in threadNum)
            {
                double[] ParallelRes = new double[Size];

                Console.WriteLine($"/// Starting parallel with {numOfThreads} threads:");
                var parStart = DateTime.Now;
                JacobiParallel(Size, generated["Matrix"], generated["Free elements"], ref ParallelRes, Epsilon, numOfThreads);
                var parFinish = DateTime.Now;
                Console.WriteLine($"Time for paralell with {numOfThreads} threads: {parFinish - parStart}");
                var speedup = (linFinish - linStart).TotalMilliseconds / (parFinish - parStart).TotalMilliseconds;
                Console.WriteLine($"Speedup: {speedup}");
                Console.WriteLine($"Efficiency: {speedup / numOfThreads}\n");
            }
        }

        public static void JacobiLinear(int size, double[][] coefficients, double[] values, ref double[] X, double eps)
        {
            double[] previousX = new double[size];
            double err;

            do
            {
                err = 0;
                double[] newValues = new double[size];

                for (int i = 0; i < size; i++)
                {
                    newValues[i] = values[i];

                    for (int j = 0; j < size; j++)
                    {
                        if (i != j)
                        {
                            newValues[i] -= coefficients[i][j] * previousX[j];
                        }
                    }

                    newValues[i] = newValues[i] / coefficients[i][i];

                    if (Math.Abs(previousX[i] - newValues[i]) > err)
                    {
                        err = Math.Abs(previousX[i] - newValues[i]);
                    }
                }
                previousX = newValues;
            } while (err > eps);

            X = previousX;
        }

        public static void JacobiParallel(int size, double[][] coefficients, double[] values, ref double[] X, double eps, int threadNum)
        {
            double[] previousX = new double[size];
            double err;

            Thread[] threads = new Thread[threadNum];

            int[,] parameters = new int[threadNum, 2];


            int remainder = size % threadNum;
            int remainderPassed = 0;

            for (int k = 0; k < threadNum; k++)
            {
                int from = size / threadNum * k + remainderPassed;
                int to = size / threadNum * (k + 1) + remainderPassed;

                if (remainder > 0)
                {
                    remainder--;
                    remainderPassed++;
                    to++;
                }
                parameters[k, 0] = from;
                parameters[k, 1] = to;
            }

            do
            {
                err = 0;
                double[] newValues = new double[size];

                for (int k = 0; k < threadNum; k++)
                {
                    int toPass = k;
                    threads[k] = new Thread(v =>
                    {
                        for (int i = parameters[toPass, 0]; i < parameters[toPass, 1]; i++)
                        {
                            newValues[i] = values[i];

                            for (int j = 0; j < size; j++)
                            {
                                if (i != j)
                                {
                                    newValues[i] -= coefficients[i][j] * previousX[j];
                                }
                            }

                            newValues[i] = newValues[i] / coefficients[i][i];

                            if (Math.Abs(previousX[i] - newValues[i]) > err)
                            {
                                err = Math.Abs(previousX[i] - newValues[i]);
                            }
                        }
                    });
                }

                foreach (var item in threads)
                {
                    item.Start();
                }

                foreach (var item in threads)
                {
                    item.Join();
                }

                previousX = newValues;

            } while (err > eps);

            X = previousX;
        }

        public static Dictionary<string, dynamic> GenerateRandomLinearEquation(int vars)
        {
            Dictionary<string, dynamic> toReturn = new Dictionary<string, dynamic>();

            var solutions = new double[vars];
            var freeElements = new double[vars];
            List<double[]> matrix = new List<double[]>();
            Random random = new Random();

            for (int i = 0; i < vars; i++)
                solutions[i] = random.Next(1, 100);

            for (int i = 0; i < vars; i++)
            {
                var toAdd = new double[vars];

                for (int j = 0; j < vars; j++)
                {
                    if (j == i)
                        toAdd[j] = random.Next(100 * vars, 200 * vars);
                    else
                        toAdd[j] = random.Next(1, 100);
                    freeElements[i] += toAdd[j] * solutions[j];
                }

                matrix.Add(toAdd);
            }

            toReturn["Matrix"] = matrix.ToArray();
            toReturn["Free elements"] = freeElements;
            toReturn["Solutions"] = solutions;
            return toReturn;
        }

        public static void PrintLinearEquationSystem(double[][] coefficients, double[] values)
        {
            int size = values.Length;

            Console.WriteLine("Linear Equation System:");
            for (int i = 0; i < size; i++)
            {
                Console.Write("(");
                for (int j = 0; j < size; j++)
                {
                    Console.Write($"{coefficients[i][j]}x{j + 1}");
                    if (j < size - 1)
                        Console.Write(" + ");
                }
                Console.Write($") = {values[i]}\n");
            }
            // Console.WriteLine("Generated Linear Equation System:");
            // PrintLinearEquationSystem(generated["Matrix"], generated["Free elements"]);
        }
        public static bool EqualResult(double[] first, double[] second)
        {
            if (first.Length != second.Length)
            {
                return false;
            }
            for (int i = 0; i < first.Length; i++)
            {
                if (Math.Round(first[i], 2) != Math.Round(second[i], 2))
                {
                    return false;
                }
            }
            return true;
        }
        public static void PrintMatrix(double[][] matrix)
        {
            if (matrix.Length != 0)
            {
                Console.Write("[");
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    Console.Write("[");
                    for (int j = 0; j < matrix[0].GetLength(0); j++)
                    {
                        Console.Write(matrix[i][j]);
                        if (matrix[0].GetLength(0) - 1 != j)
                            Console.Write(", ");
                    }
                    Console.Write("]");
                    if (matrix.GetLength(0) - 1 != i)
                    {
                        Console.WriteLine(", ");
                    }
                }
                Console.WriteLine("]");
            }
        }
        public static void PrintArray<T>(T[] arr)
        {
            Console.Write("[");
            Console.Write(String.Join(", ", arr));
            Console.WriteLine("]");
        }
    }
}