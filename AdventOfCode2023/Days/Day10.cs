using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Days
{
    internal static class Day10
    {
        public static void Run(string input)
        {
            //Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        static int Part1(string input)
        {
            string[] lines = input.Split("\r\n");
            int[,] rawPipeGraph = new int[lines[0].Length, lines.Length];

            // X+ right, Y+ down
            (int, int) start = (0, 0);
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[0].Length; x++)
                {
                    rawPipeGraph[x, y] = lines[y][x];
                    if (lines[y][x] == (int)PipeType.START)
                        start = (x, y);
                }
            }

            rawPipeGraph[start.Item1, start.Item2] = (int)GetStartType(start.Item1, start.Item2, rawPipeGraph);

            int[,] nodes = new int[lines.Length * lines[0].Length, lines.Length * lines[0].Length];
            for (int i = 0; i < nodes.GetLength(0); i++)
                for (int j = 0; j < nodes.GetLength(1); j++)
                    nodes[i, j] = 0;

            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[0].Length; x++)
                {
                    for (int i = -1; i < 2; i += 2)
                    {
                        if (y + i >= 0 && y + i < lines.Length)
                            if (IsPipeConnected(rawPipeGraph[x, y], rawPipeGraph[x, y + i], i == -1 ? Direction.Up : Direction.Down))
                                nodes[y * lines[0].Length + x, (y + i) * lines[0].Length + x] = 1;
                        if (x + i >= 0 && x + i < lines[0].Length)
                            if (IsPipeConnected(rawPipeGraph[x, y], rawPipeGraph[x + i, y], i == -1 ? Direction.Left : Direction.Right))
                                nodes[y * lines[0].Length + x, y * lines[0].Length + x + i] = 1;
                    }
                }
            }

            int[] distancesFromStartNode = Dijkstra(nodes, start.Item2 * lines[0].Length + start.Item1);
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[0].Length; x++)
                {
                    if (distancesFromStartNode[y * lines[0].Length + x] == int.MaxValue)
                        Console.Write(".".PadLeft(3));
                    else
                        Console.Write(distancesFromStartNode[y * lines[0].Length + x].ToString().PadLeft(3));
                }
                Console.WriteLine();
            }

            return distancesFromStartNode.Where(x => x != int.MaxValue).Max();
        }

        static int Part2(string input)
        {
            string[] lines = input.Split("\r\n");
            int[,] rawPipeGraph = new int[lines[0].Length, lines.Length];

            // X+ right, Y+ down
            (int, int) start = (0, 0);
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[0].Length; x++)
                {
                    rawPipeGraph[x, y] = lines[y][x];
                    if (lines[y][x] == (int)PipeType.START)
                        start = (x, y);
                }
            }

            rawPipeGraph[start.Item1, start.Item2] = (int)GetStartType(start.Item1, start.Item2, rawPipeGraph);

            int[,] nodes = new int[lines[0].Length, lines.Length];
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[0].Length; x++)
                {
                    nodes[x, y] = 0;
                }
            }
            nodes[start.Item1, start.Item2] = 1;

            List<(int, int)> fill_pipes = [start];
            while (fill_pipes.Count > 0)
            {
                List<(int, int)> newPipes = new();
                foreach ((int, int) pipe in fill_pipes)
                {
                    newPipes.AddRange(PipeFill(pipe.Item1, pipe.Item2, rawPipeGraph, ref nodes));
                }
                fill_pipes = newPipes;
            }

            int[,] paintGrid = new int[lines[0].Length, lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[0].Length; j++)
                {
                    paintGrid[j, i] = 0;
                }
            }

            for (int y = 0; y < lines.Length; y++)
            {
                if (nodes[0, y] == 0)
                    BoundaryFill(0, y, rawPipeGraph, nodes, ref paintGrid);
                if (nodes[lines[0].Length - 1, y] == 0)
                    BoundaryFill(lines[0].Length - 1, y, rawPipeGraph, nodes, ref paintGrid);
            }
            for (int x = 0; x < lines[0].Length; x++)
            {
                if (nodes[x, 0] == 0)
                    BoundaryFill(x, 0, rawPipeGraph, nodes, ref paintGrid);
                if (nodes[x, lines.Length - 1] == 0)
                    BoundaryFill(x, lines.Length - 1, rawPipeGraph, nodes, ref paintGrid);
            }

            // Setup node graph offset by 0.5 up and right to represent midpoints
            // Node graph of ints representing a flagged enum for bypassable directions at that midpoint
            // Bypassable nodes likely cover all empty nodes as well, match every node point direction that doesn't hit a wall

            int[,] pathGraph = new int[lines[0].Length - 1, lines.Length - 1];
            for (int y = 0; y < lines.Length - 1; y++)
            {
                for (int x = 0; x < lines[0].Length - 1; x++)
                {
                    pathGraph[x, y] = 0;
                    if (y != lines.Length && !IsPipeConnected(rawPipeGraph[x, y], rawPipeGraph[x, y + 1], Direction.Down))
                        pathGraph[x, y] |= (int)Direction.Left;
                    if (x != lines[0].Length && !IsPipeConnected(rawPipeGraph[x, y], rawPipeGraph[x + 1, y], Direction.Right))
                        pathGraph[x, y] |= (int)Direction.Up;
                    if (y != lines.Length && x != lines[0].Length 
                        && !IsPipeConnected(rawPipeGraph[x + 1, y], rawPipeGraph[x + 1, y + 1], Direction.Down))
                        pathGraph[x, y] |= (int)Direction.Right;
                    if (y != lines.Length && x != lines[0].Length
                        && !IsPipeConnected(rawPipeGraph[x, y + 1], rawPipeGraph[x + 1, y + 1], Direction.Right))
                        pathGraph[x, y] |= (int)Direction.Down;
                }
            }

            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[0].Length; x++)
                {
                    if (paintGrid[x, y] == 1)
                        Console.Write("#");
                    else
                    {
                        if (nodes[x, y] == 1)
                            Console.Write((char)rawPipeGraph[x, y]);
                        else
                            Console.Write(".");
                    }
                }
                Console.WriteLine();
            }
            for (int i = 0; i < lines[0].Length; i++)
                Console.Write("=");
            Console.WriteLine();

            int[,] offsetGraph = new int[pathGraph.GetLength(0), pathGraph.GetLength(1)];
            for (int y = 0; y < pathGraph.GetLength(1); y++)
            {
                if (offsetGraph[0, y] == 0)
                    FillOffset(pathGraph, ref offsetGraph, (0, y));
                if (offsetGraph[pathGraph.GetLength(0) - 1, y] == 0)
                    FillOffset(pathGraph, ref offsetGraph, (pathGraph.GetLength(0) - 1, y));
            }
            for (int x = 0; x < pathGraph.GetLength(0); x++)
            {
                if (offsetGraph[x, 0] == 0)
                    FillOffset(pathGraph, ref offsetGraph, (x, 0));
                if (offsetGraph[x, pathGraph.GetLength(1) - 1] == 0)
                    FillOffset(pathGraph, ref offsetGraph, (x, pathGraph.GetLength(1) - 1));
            }

            for (int y = 1; y < lines.Length - 1; y++)
            {
                for (int x = 1; x < lines[0].Length - 1; x++)
                {
                    if (nodes[x, y] == 0)
                    {
                        if (offsetGraph[x - 1, y - 1] == 1
                            && offsetGraph[x, y - 1] == 1
                            && offsetGraph[x - 1, y] == 1
                            && offsetGraph[x, y] == 1)
                        {
                            paintGrid[x, y] = 1;
                        }
                    }
                }
            }

            int insideNodes = 0;
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[0].Length; x++)
                {
                    if (paintGrid[x, y] == 0 && nodes[x, y] == 0)
                        insideNodes++;
                }
            }

            for (int y = 0; y < offsetGraph.GetLength(1); y++)
            {
                for (int x = 0; x < offsetGraph.GetLength(0); x++)
                {
                    Console.Write(pathGraph[x, y].ToString().PadLeft(3));
                }
                Console.WriteLine();
            }
            for (int i = 0; i < lines[0].Length; i++)
                Console.Write("=");
            Console.WriteLine();

            for (int y = 0; y < offsetGraph.GetLength(1); y++)
            {
                for (int x = 0; x < offsetGraph.GetLength(0); x++)
                {
                    Console.Write(offsetGraph[x, y]);
                }
                Console.WriteLine();
            }
            for (int i = 0; i < lines[0].Length; i++)
                Console.Write("=");
            Console.WriteLine();

            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[0].Length; x++)
                {
                    if (paintGrid[x, y] == 1)
                        Console.Write("#");
                    else
                    {
                        if (nodes[x, y] == 1)
                            Console.Write((char)rawPipeGraph[x, y]);
                        else
                            Console.Write(".");
                    }
                }
                Console.WriteLine();
            }

            return insideNodes;
        }

        enum PipeType
        {
            Empty = 46,
            NS = 124,
            EW = 45,
            NE = 76,
            NW = 74,
            SW = 55,
            SE = 70,
            START = 83
        }

        [Flags]
        enum Direction
        {
            Down = 1,
            Right = 2,
            Up = 4,
            Left = 8
        }

        static int MinDistance(int[] dist, bool[] sptSet)
        {
            int min = int.MaxValue, min_index = -1;

            for (int v = 0; v < dist.Length; v++)
                if (sptSet[v] == false && dist[v] <= min)
                {
                    min = dist[v];
                    min_index = v;
                }

            return min_index;
        }

        static int[] Dijkstra(int[,] graph, int src)
        {
            int V = graph.GetLength(0);
            int[] dist = new int[V];

            bool[] sptSet = new bool[V];
            for (int i = 0; i < V; i++)
            {
                dist[i] = int.MaxValue;
                sptSet[i] = false;
            }
            dist[src] = 0;

            for (int count = 0; count < V - 1; count++)
            {
                int u = MinDistance(dist, sptSet);

                sptSet[u] = true;
                for (int v = 0; v < V; v++)
                    if (!sptSet[v] && graph[u, v] != 0
                        && dist[u] != int.MaxValue
                        && dist[u] + graph[u, v] < dist[v])
                        dist[v] = dist[u] + graph[u, v];
            }

            return dist;
        }

        static bool IsPipeConnected(int pipe1, int pipe2, Direction direction)
        {
            if (pipe1 == (int)PipeType.Empty || pipe2 == (int)PipeType.Empty) return false;
            if (direction == Direction.Down)
            {
                if (pipe1 == (int)PipeType.NS || pipe1 == (int)PipeType.SW || pipe1 == (int)PipeType.SE)
                    return pipe2 == (int)PipeType.NS || pipe2 == (int)PipeType.NE || pipe2 == (int)PipeType.NW;
                return false;
            }
            if (direction == Direction.Right)
            {
                if (pipe1 == (int)PipeType.EW || pipe1 == (int)PipeType.NE || pipe1 == (int)PipeType.SE)
                    return pipe2 == (int)PipeType.EW || pipe2 == (int)PipeType.NW || pipe2 == (int)PipeType.SW;
                return false;
            }
            if (direction == Direction.Up)
            {
                if (pipe1 == (int)PipeType.NS || pipe1 == (int)PipeType.NE || pipe1 == (int)PipeType.NW)
                    return pipe2 == (int)PipeType.NS || pipe2 == (int)PipeType.SW || pipe2 == (int)PipeType.SE;
                return false;
            }
            if (direction == Direction.Left)
            {
                if (pipe1 == (int)PipeType.EW || pipe1 == (int)PipeType.NW || pipe1 == (int)PipeType.SW)
                    return pipe2 == (int)PipeType.EW || pipe2 == (int)PipeType.NE || pipe2 == (int)PipeType.SE;
                return false;
            }
            return false;
        }

        static PipeType GetStartType(int x, int y, int[,] grid)
        {
            List<PipeType> guesses = new();
            foreach (PipeType guess in Enum.GetValues(typeof(PipeType)))
            {
                if (guess == PipeType.Empty || guess == PipeType.START)
                    continue;
                if (y + 1 < grid.GetLength(1) && IsPipeConnected((int)guess, grid[x, y+1], Direction.Down))
                {
                    if (!guesses.Contains(guess))
                        guesses.Add(guess);
                    else
                        return guess;
                }
                if (x + 1 < grid.GetLength(0) && IsPipeConnected((int)guess, grid[x+1, y], Direction.Right))
                {
                    if (!guesses.Contains(guess))
                        guesses.Add(guess);
                    else
                        return guess;
                }
                if (y - 1 >= 0 && IsPipeConnected((int)guess, grid[x, y-1], Direction.Up))
                {
                    if (!guesses.Contains(guess))
                        guesses.Add(guess);
                    else
                        return guess;
                }
                if (x - 1 >= 0 && IsPipeConnected((int)guess, grid[x-1, y], Direction.Left))
                {
                    if (!guesses.Contains(guess))
                        guesses.Add(guess);
                    else
                        return guess;
                }
            }
            return PipeType.Empty;
        }

        static void BoundaryFill(int x, int y, int[,] grid, int[,] pipeGrid, ref int[,]? paintGrid)
        {
            if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1))
                return;
            if (pipeGrid[x, y] == 0 && paintGrid[x, y] != 1)
            {
                paintGrid[x, y] = 1;
                if (y + 1 < grid.GetLength(1))
                    BoundaryFill(x, y + 1, grid, pipeGrid, ref paintGrid);
                if (x + 1 < grid.GetLength(0))
                    BoundaryFill(x + 1, y, grid, pipeGrid, ref paintGrid);
                if (y - 1 >= 0)
                    BoundaryFill(x, y - 1, grid, pipeGrid, ref paintGrid);
                if (x - 1 >= 0)
                    BoundaryFill(x - 1, y, grid, pipeGrid, ref paintGrid);
            }
        }

        static List<(int, int)> PipeFill(int x, int y, int[,] grid, ref int[,] paintGrid)
        {
            List<(int, int)> newPipes = new();
            if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1))
                return newPipes;

            if (y + 1 < grid.GetLength(1) && paintGrid[x, y + 1] == 0 && IsPipeConnected(grid[x, y], grid[x, y + 1], Direction.Down))
            {
                paintGrid[x, y+1] = 1;
                newPipes.Add((x, y+1));
            }
            if (x + 1 < grid.GetLength(0) && paintGrid[x + 1, y] == 0 && IsPipeConnected(grid[x, y], grid[x + 1, y], Direction.Right))
            {
                paintGrid[x + 1, y] = 1;
                newPipes.Add((x + 1, y));
            }
            if (y - 1 >= 0 && paintGrid[x, y - 1] == 0 && IsPipeConnected(grid[x, y], grid[x, y - 1], Direction.Up))
            {
                paintGrid[x, y - 1] = 1;
                newPipes.Add((x, y - 1));
            }
            if (x - 1 >= 0 && paintGrid[x - 1, y] == 0 && IsPipeConnected(grid[x, y], grid[x - 1, y], Direction.Left))
            {
                paintGrid[x - 1, y] = 1;
                newPipes.Add((x - 1, y));
            }
            return newPipes;
        }

        static void FillOffset(int[,] graph, ref int[,] offsetGraph, (int x, int y) pos)
        {
            if (pos.x < 0 || pos.x >= graph.GetLength(0) || pos.y < 0 || pos.y >= graph.GetLength(1))
                return;
            if (offsetGraph[pos.x, pos.y] == 1)
                return;
            offsetGraph[pos.x, pos.y] = 1;

            int x = pos.x;
            int y = pos.y;

            if (x != 0 && ((Direction)graph[x, y]).HasFlag(Direction.Left) && offsetGraph[x - 1, y] == 0)
                FillOffset(graph, ref offsetGraph, (x - 1, y));
            if (x != graph.GetLength(0) - 1 && ((Direction)graph[x, y]).HasFlag(Direction.Right) && offsetGraph[x + 1, y] == 0)
                FillOffset(graph, ref offsetGraph, (x + 1, y));
            if (y != 0 && ((Direction)graph[x, y]).HasFlag(Direction.Up) && offsetGraph[x, y - 1] == 0)
                FillOffset(graph, ref offsetGraph, (x, y - 1));
            if (y != graph.GetLength(1) - 1 && ((Direction)graph[x, y]).HasFlag(Direction.Down) && offsetGraph[x, y + 1] == 0)
                FillOffset(graph, ref offsetGraph, (x, y + 1));
        }
    }
}
