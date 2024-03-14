using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Linq;

namespace Task6
{
    class Program
    {
        static void Main()
        {
            const int nodeA = 0;
            int[] nodesNum = { 50, 64, 100, 256, 500, 1000, 2048, 5000, 10000 };
            int[] threadsNum = { 2, 4, 6, 8, 10, 16, 32 };

            foreach (int nodes in nodesNum)
            {
                Console.WriteLine($"/// Graph with {nodes} nodes");
                Graph graph = GenerateRandomGraph(nodes);

                RunDijkstraSequential(graph, nodeA, out int[] resultSeq, out TimeSpan timeSeq);

                foreach (int threads in threadsNum)
                {
                    RunDijkstraParallel(graph, nodeA, threads, out int[] resultPar, out TimeSpan timePar);
                }

                Console.WriteLine();
            }
            Console.ReadLine();
        }

        public static Graph GenerateRandomGraph(int size)
        {
            Graph graph = new Graph(size);
            return graph;
        }

        public static void RunDijkstraSequential(Graph graph, int nodeA, out int[] result, out TimeSpan time)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            graph.Initialize();
            graph.DijkstraSequentialMethod(nodeA);

            stopwatch.Stop();
            time = stopwatch.Elapsed;

            Console.WriteLine($"Sequential Dijkstra algorithm: {time}");

            result = graph.GetShortestDistances();
        }

        public static void RunDijkstraParallel(Graph graph, int nodeA, int threadNum, out int[] result, out TimeSpan time)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            graph.Initialize();
            graph.DijkstraParallelMethod(nodeA, threadNum);

            stopwatch.Stop();
            time = stopwatch.Elapsed;

            Console.WriteLine($"Parallel Dijkstra algorithm with {threadNum} threads: {time}");

            result = graph.GetShortestDistances();
        }
    }

    public class GraphNode
    {
        public string Name { get; set; }
        public bool Passed { get; set; }
        public List<GraphConnection> Connections { get; set; }
        public int Weight { get; set; }

        public GraphNode(string name)
        {
            Name = name;
            Passed = false;
            Connections = new List<GraphConnection>();
        }

        public override string ToString()
        {
            return $"{Name}\n\t{string.Join("\n\t", Connections)}";
        }
    }

    public class GraphConnection
    {
        public GraphNode FirstNode { get; set; }
        public GraphNode SecondNode { get; set; }
        public int Weight { get; set; }

        public override string ToString()
        {
            return $"{FirstNode.Name} - {SecondNode.Name}: {Weight}";
        }
    }

    public class Graph
    {
        public List<GraphNode> Nodes { get; set; }
        public List<GraphConnection> Connections { get; set; }

        public Graph(int nodeNum)
        {
            Nodes = new List<GraphNode>();
            Random random = new Random();

            for (int i = 0; i < nodeNum; i++)
            {
                GraphNode newNode = new GraphNode("Node" + i);
                int connection = 0;

                if (Nodes.Count != 0)
                {
                    connection = random.Next(0, Nodes.Count);
                    int weight = random.Next(1, 1000);
                    GraphConnection connectionFrom = new GraphConnection() 
                        { FirstNode = Nodes[connection], SecondNode = newNode, Weight = weight };
                    GraphConnection connectionTo = new GraphConnection() 
                        { FirstNode = newNode, SecondNode = Nodes[connection], Weight = weight };
                    newNode.Connections.Add(connectionTo);
                    Nodes[connection].Connections.Add(connectionFrom);
                }

                foreach (var node in Nodes)
                {
                    if (random.NextDouble() < 0.5 && Nodes[connection].Name != node.Name)
                    {
                        int weight = random.Next(1, 1000);
                        GraphConnection connectionFrom = new GraphConnection() 
                            { FirstNode = node, SecondNode = newNode, Weight = weight };
                        GraphConnection connectionTo = new GraphConnection() 
                            { FirstNode = newNode, SecondNode = node, Weight = weight };
                        newNode.Connections.Add(connectionTo);
                        node.Connections.Add(connectionFrom);
                    }
                }

                Nodes.Add(newNode);
            }
        }

        public void Initialize()
        {
            foreach (var node in Nodes)
            {
                node.Weight = int.MaxValue;
            }
        }

        public int[] GetShortestDistances()
        {
            return Nodes.Select(n => n.Weight).ToArray();
        }

        public void DijkstraSequentialMethod(int nodeA)
        {
            Nodes[nodeA].Weight = 0;
            GraphNode current = Nodes[nodeA];

            while (current != null)
            {
                foreach (var connection in current.Connections)
                {
                    if (connection.SecondNode.Weight > current.Weight + connection.Weight)
                    {
                        connection.SecondNode.Weight = current.Weight + connection.Weight;
                    }
                }

                current = Nodes
                    .Where(n => n.Weight != int.MaxValue)
                    .Where(n => !n.Passed)
                    .OrderBy(n => n.Weight)
                    .FirstOrDefault();

                if (current != null)
                {
                    current.Passed = true;
                }
            }
        }

        public void DijkstraParallelMethod(int nodeA, int threadNum)
        {
            CountdownEvent countdownEvent = new CountdownEvent(threadNum);

            Nodes[nodeA].Weight = 0;
            GraphNode current = Nodes[nodeA];

            object lockObject = new object();

            for (int i = 0; i < threadNum; i++)
            {
                int threadId = i;

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    while (true)
                    {
                        GraphNode nextNode = null;

                        lock (lockObject)
                        {
                            nextNode = Nodes
                                .Where(n => n.Weight != int.MaxValue)
                                .Where(n => !n.Passed)
                                .OrderBy(n => n.Weight)
                                .FirstOrDefault();

                            if (nextNode != null)
                            {
                                nextNode.Passed = true;
                            }
                        }

                        if (nextNode == null)
                        {
                            break;
                        }

                        foreach (var connection in nextNode.Connections)
                        {
                            if (connection.SecondNode.Weight > nextNode.Weight + connection.Weight)
                            {
                                connection.SecondNode.Weight = nextNode.Weight + connection.Weight;
                            }
                        }
                    }
                    countdownEvent.Signal();
                });
            }
            countdownEvent.Wait();
        }
    }
}