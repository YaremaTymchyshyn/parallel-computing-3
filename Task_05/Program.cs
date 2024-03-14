using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Task_05
{
    internal class Program
    {
        public static void Main()
        {
            int nodeA = 4;
            int nodeB = 16;
            int[] nodesNum = { 32, 50, 64, 100, 256, 500, 1024 };
            int[] threadsNum = { 2, 4, 8, 16, 32 };
            var graphs = GenerateRandomGraph(nodesNum);

            RunFloydSequential(graphs, nodeA, nodeB);
            RunFloydParallel(graphs, threadsNum, nodeA, nodeB);
        }
        public static List<int[,]> GenerateRandomGraph(int[] nodesNum)
        {
            var graphs = new List<int[,]>();
            Random random = new Random();
            foreach (var nodes in nodesNum)
            {
                int[,] graph = new int[nodes, nodes];
                for (int i = 0; i < nodes; i++)
                {
                    for (int j = 0; j < nodes; j++)
                    {
                        graph[i, j] = random.Next(1, 100);
                    }
                }
                graphs.Add(graph);
            }
            return graphs;
        }
        public static void RunFloydSequential(List<int[,]> graphs, int nodeA, int nodeB)
        {
            Console.WriteLine("/// Sequential Floyd algorithm:");
            foreach (var graph in graphs)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                int[,] shortestPathsSequential = FloydSequentialMethod(graph);
                stopwatch.Stop();
                Console.WriteLine($"Nodes: {graph.GetLength(0)} " +
                    $"| Time: {stopwatch.ElapsedMilliseconds} ms. " +
                    $"| Result: {shortestPathsSequential[nodeA, nodeB]}");
            }
            Console.WriteLine();
        }
        public static void RunFloydParallel(List<int[,]> graphs, int[] threadsNum, int nodeA, int nodeB)
        {
            foreach (var threads in threadsNum)
            {
                Console.WriteLine($"/// Parallel Floyd algorithm with {threads} threads:");
                foreach (var graph in graphs)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    int[,] shortestPathsParallel = FloydParallelMethod(graph, threads);
                    stopwatch.Stop();
                    Console.WriteLine($"Nodes: {graph.GetLength(0)} " +
                        $"| Time: {stopwatch.ElapsedMilliseconds} ms. " +
                        $"| Result: {shortestPathsParallel[nodeA, nodeB]}");
                }
                Console.WriteLine();
            }
        }
        public static int[,] FloydSequentialMethod(int[,] graph)
        {
            int nodes = graph.GetLength(0);
            int[,] shortestPath = new int[nodes, nodes];

            for (int i = 0; i < nodes; i++)
            {
                for (int j = 0; j < nodes; j++)
                {
                    shortestPath[i, j] = graph[i, j];
                }
            }

            for (int k = 0; k < nodes; k++)
            {
                for (int i = 0; i < nodes; i++)
                {
                    for (int j = 0; j < nodes; j++)
                    {
                        if (shortestPath[i, k] != int.MaxValue && shortestPath[k, j] != int.MaxValue
                            && shortestPath[i, k] + shortestPath[k, j] < shortestPath[i, j])
                        {
                            shortestPath[i, j] = shortestPath[i, k] + shortestPath[k, j];
                        }
                    }
                }
            }
            return shortestPath;
        }
        public static int[,] FloydParallelMethod(int[,] graph, int threadsNum)
        {
            int nodes = graph.GetLength(0);
            int[,] shortestPath = new int[nodes, nodes];

            for (int i = 0; i < nodes; i++)
            {
                for (int j = 0; j < nodes; j++)
                {
                    shortestPath[i, j] = graph[i, j];
                }
            }

            for (int k = 0; k < nodes; k++)
            {
                Parallel.For(0, nodes, new ParallelOptions { MaxDegreeOfParallelism = threadsNum }, i =>
                {
                    for (int j = 0; j < nodes; j++)
                    {
                        if (shortestPath[i, k] != int.MaxValue && shortestPath[k, j] != int.MaxValue
                            && shortestPath[i, k] + shortestPath[k, j] < shortestPath[i, j])
                        {
                            shortestPath[i, j] = shortestPath[i, k] + shortestPath[k, j];
                        }
                    }
                });
            }
            return shortestPath;
        }
    }
}