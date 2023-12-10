using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Days
{
    internal static class Day10
    {
        const int INF = 99999;
        static int[,]? connectedPipesMarker;

        public static void Run(string input)
        {
            string[] lines = input.Split("\r\n");
            if (connectedPipesMarker == null)
            {
                connectedPipesMarker = new int[lines.Length, lines.Length];
                for (int i = 0; i < connectedPipesMarker.GetLength(0); i++)
                {
                    for (int j = 0; j < connectedPipesMarker.GetLength(1); j++)
                    {
                        connectedPipesMarker[i, j] = 0;
                    }
                }
            }

            Console.WriteLine(Part1(lines));
        }

        static int Part1(string[] lines)
        {
            int[,] rawPipeGraph = new int[lines.Length, lines.Length];
            (int, int) start = (0, 0);
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines.Length; x++)
                {
                    rawPipeGraph[x, y] = lines[y][x];
                    if (lines[y][x] == (int)PipeType.START)
                        start = (x, y);
                }
            }

            rawPipeGraph[start.Item1, start.Item2] = (int)GetStartType(start.Item1, start.Item2, rawPipeGraph);

            // Y+ down, X+ right
            BoundaryFill(start.Item1, start.Item2, rawPipeGraph);
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines.Length; x++)
                {
                    if (connectedPipesMarker[x, y] != 1)
                        connectedPipesMarker[x, y] = INF;
                }
            }

            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines.Length; x++)
                {
                    Console.Write(connectedPipesMarker[x, y].ToString().PadLeft(7));
                }
                Console.WriteLine();
            }

            int connectedPipes = 0;
            int[,] pipeGraph = new int[connectedPipes, connectedPipes];
            return 0;
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

        enum Direction
        {
            Down,
            Right,
            Up,
            Left
        }

        static int[,] FloydWarshall(int[,] graph, int verticesCount, out int[,] prev)
        {
            int[,] distance = new int[verticesCount, verticesCount];
            prev = new int[verticesCount, verticesCount];

            for (int i = 0; i < verticesCount; ++i)
            {
                for (int j = 0; j < verticesCount; ++j)
                {
                    distance[i, j] = graph[i, j];
                    prev[i, j] = i;
                }
            }

            for (int k = 0; k < verticesCount; ++k)
            {
                for (int i = 0; i < verticesCount; ++i)
                {
                    for (int j = 0; j < verticesCount; ++j)
                    {
                        if (distance[i, k] + distance[k, j] < distance[i, j])
                        {
                            distance[i, j] = distance[i, k] + distance[k, j];
                            prev[i, j] = prev[k, j];
                        }
                    }
                }
            }
            return distance;
        }

        static int[] Path(int[,] prev, int a, int b)
        {
            List<int> path = new List<int>() { b };
            while (a != b)
            {
                b = prev[a, b];
                path = path.Prepend(b).ToList();
            }
            return path.ToArray();
        }

        static void Print(int[,] distance, int verticesCount)
        {
            for (int i = 0; i < verticesCount; ++i)
            {
                for (int j = 0; j < verticesCount; ++j)
                {
                    if (distance[i, j] == INF)
                        Console.Write("INF".PadLeft(7));
                    else
                        Console.Write(distance[i, j].ToString().PadLeft(7));
                }

                Console.WriteLine();
            }
        }

        static void BoundaryFill(int x, int y, int[,] grid)
        {
            if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1))
                return;
            if (grid[x, y] != (int)PipeType.Empty && connectedPipesMarker[x, y] != 1)
            {
                connectedPipesMarker[x, y] = 1;
                if (y + 1 < grid.GetLength(1) && IsPipeConnected(grid[x, y], grid[x, y + 1], 0))
                    BoundaryFill(x, y + 1, grid);
                if (x + 1 < grid.GetLength(0) && IsPipeConnected(grid[x, y], grid[x + 1, y], 1))
                    BoundaryFill(x + 1, y, grid);
                if (y - 1 >= 0 && IsPipeConnected(grid[x, y], grid[x, y - 1], 2))
                    BoundaryFill(x, y - 1, grid);
                if (x - 1 >= 0 && IsPipeConnected(grid[x, y], grid[x - 1, y], 3))
                    BoundaryFill(x - 1, y, grid);
            }
        }

        static bool IsPipeConnected(int pipe1, int pipe2, int direction)
        {
            if (direction == (int)Direction.Down)
            {
                if (pipe1 == (int)PipeType.NS || pipe1 == (int)PipeType.SW || pipe1 == (int)PipeType.SE)
                    return pipe2 == (int)PipeType.NS || pipe2 == (int)PipeType.NE || pipe2 == (int)PipeType.NW;
                return false;
            }
            if (direction == (int)Direction.Right)
            {
                if (pipe1 == (int)PipeType.EW || pipe1 == (int)PipeType.NE || pipe1 == (int)PipeType.SE)
                    return pipe2 == (int)PipeType.EW || pipe2 == (int)PipeType.NW || pipe2 == (int)PipeType.SW;
                return false;
            }
            if (direction == (int)Direction.Up)
            {
                if (pipe1 == (int)PipeType.NS || pipe1 == (int)PipeType.NE || pipe1 == (int)PipeType.NW)
                    return pipe2 == (int)PipeType.NS || pipe2 == (int)PipeType.SW || pipe2 == (int)PipeType.SE;
                return false;
            }
            if (direction == (int)Direction.Left)
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
                if (y + 1 < grid.GetLength(1) && IsPipeConnected((int)guess, grid[x, y+1], 0))
                {
                    if (!guesses.Contains(guess))
                        guesses.Add(guess);
                    else
                        return guess;
                }
                if (x + 1 < grid.GetLength(0) && IsPipeConnected((int)guess, grid[x+1, y], 1))
                {
                    if (!guesses.Contains(guess))
                        guesses.Add(guess);
                    else
                        return guess;
                }
                if (y - 1 >= 0 && IsPipeConnected((int)guess, grid[x, y-1], 2))
                {
                    if (!guesses.Contains(guess))
                        guesses.Add(guess);
                    else
                        return guess;
                }
                if (x - 1 >= 0 && IsPipeConnected((int)guess, grid[x-1, y], 3))
                {
                    if (!guesses.Contains(guess))
                        guesses.Add(guess);
                    else
                        return guess;
                }
            }
            return PipeType.Empty;
        }
    }
}
