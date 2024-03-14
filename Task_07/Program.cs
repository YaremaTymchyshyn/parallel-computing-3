using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Linq;

namespace Task7
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

                RunPrimSequential(graph, nodeA, out int resultSeq, out TimeSpan timeSeq);

                foreach (int threads in threadsNum)
                {
                    RunPrimParallel(graph, nodeA, threads, out int resultPar, out TimeSpan timePar);
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

        public static void RunPrimSequential(Graph graph, int nodeA, out int result, out TimeSpan time)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            graph.Initialize();
            graph.PrimSequentialMethod(nodeA);

            stopwatch.Stop();
            time = stopwatch.Elapsed;

            Console.WriteLine($"Sequential Prim algorithm: {time}");

            result = graph.CalculateMinimumSpanningTreeWeight();
        }

        public static void RunPrimParallel(Graph graph, int nodeA, int threadNum, out int result, out TimeSpan time)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            graph.Initialize();
            graph.PrimParallelMethod(nodeA, threadNum);

            stopwatch.Stop();
            time = stopwatch.Elapsed;

            Console.WriteLine($"Parallel Prim algorithm with {threadNum} threads: {time}");

            result = graph.CalculateMinimumSpanningTreeWeight();
        }
    }

    public class GraphNode  
    {
        public string Name { get; set; }
        public bool IncludedInMST { get; set; }
        public List<GraphConnection> Connections { get; set; }
        public int Weight { get; set; }

        public GraphNode(string name)
        {
            Name = name;
            IncludedInMST = false;
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
                node.IncludedInMST = false;
            }
        }

        public int CalculateMinimumSpanningTreeWeight()
        {
            int weight = 0;
            foreach (var node in Nodes)
            {
                if (node.IncludedInMST)
                {
                    foreach (var connection in node.Connections)
                    {
                        if (connection.SecondNode.IncludedInMST)
                        {
                            weight += connection.Weight;
                        }
                    }
                }
            }
            return weight / 2;
        }

        public void PrimSequentialMethod(int nodeA)
        {
            Nodes[nodeA].Weight = 0;
            GraphNode current = Nodes[nodeA];

            while (current != null)
            {
                current.IncludedInMST = true;
                foreach (var connection in current.Connections)
                {
                    if (!connection.SecondNode.IncludedInMST && connection.Weight < connection.SecondNode.Weight)
                    {
                        connection.SecondNode.Weight = connection.Weight;
                    }
                }

                current = Nodes
                    .Where(node => !node.IncludedInMST)
                    .OrderBy(node => node.Weight)
                    .FirstOrDefault();
            }
        }

        public void PrimParallelMethod(int nodeA, int threadNum)
        {
            CountdownEvent countdownEvent = new CountdownEvent(threadNum);

            Nodes[nodeA].Weight = 0;
            GraphNode current = Nodes[nodeA];

            object lockObject = new object();

            for (int i = 0; i < threadNum; i++)
            {
                int threadId = i;

                ThreadPool.QueueUserWorkItem(state =>
                {
                    while (true)
                    {
                        GraphNode nextNode = null;

                        lock (lockObject)
                        {
                            nextNode = Nodes
                                .Where(node => node.Weight != int.MaxValue)
                                .Where(node => !node.IncludedInMST)
                                .OrderBy(node => node.Weight)
                                .FirstOrDefault();

                            if (nextNode != null)
                            {
                                nextNode.IncludedInMST = true;
                            }
                        }

                        if (nextNode == null)
                        {
                            break;
                        }

                        foreach (var connection in nextNode.Connections)
                        {
                            if (connection.SecondNode.Weight > connection.Weight)
                            {
                                connection.SecondNode.Weight = connection.Weight;
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