using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_16__alternative_
{
    internal class Program
    {
        static int MAZE_HEIGHT = 0;
        static int MAZE_WIDTH = 0;

        struct Position
        {
            public int top, left;
        }

        struct Node
        {
            public Dictionary<Direction, int> entries;
            public bool entriesExhausted;
        }

        enum Direction
        {
            North,
            East,
            South,
            West
        }

        static void Main(string[] args)
        {
            string[] maze = File.ReadAllLines("input.txt");

            MAZE_HEIGHT = maze.Length;
            MAZE_WIDTH = maze[0].Length;

            Position startingPosition = new Position();
            Position endingPosition = new Position();

            for (int top = 0; top < maze.Length; top++)
            {
                for (int left = 0; left < maze[top].Length; left++)
                {
                    if (maze[top][left] == 'S')
                    {
                        startingPosition.top = top;
                        startingPosition.left = left;
                    }
                    else if (maze[top][left] == 'E')
                    {
                        endingPosition.top = top;
                        endingPosition.left = left;
                    }
                }
            }

            Dictionary<Position, Node> cache = Djikstras(maze, startingPosition);

            int points = cache[endingPosition].entries.Min(entry => entry.Value);

            Console.WriteLine(points);

            List<Position> onBestPath = new List<Position>() { endingPosition };

            BackTrack(cache, maze, onBestPath, endingPosition, points, Direction.North, true);

            Console.WriteLine(onBestPath.Count());

            Console.ReadKey();
        }

        static Dictionary<Position, Node> Djikstras(string[] maze, Position startingPosition)
        {
            Dictionary<Position, Node> cache = new Dictionary<Position, Node>();

            Node startingNode = new Node
            {
                entries = new Dictionary<Direction, int>(),
                entriesExhausted = true
            };

            startingNode.entries.Add(Direction.West, 0);

            cache.Add(startingPosition, startingNode);

            Position position = startingPosition;

            while (true)
            {
                UpdateNeighbours(cache, position, maze);

                cache[position] = new Node
                {
                    entries = cache[position].entries,
                    entriesExhausted = true
                };

                KeyValuePair<Position, Node>[] squaresWithInexhaustedEntries = cache
                    .Where(square => !square.Value.entriesExhausted)
                    .ToArray();

                if (squaresWithInexhaustedEntries.Count() > 0)
                {
                    position = squaresWithInexhaustedEntries
                        .OrderBy(square => square.Value.entries.Values.Min())
                        .First()
                        .Key;
                }
                else
                {
                    break;
                }
            }

            return cache;
        }

        static void BackTrack(Dictionary<Position, Node> cache, string[] maze, List<Position> onBestPath, Position currentPosition, int exitPoints, Direction exitDirection, bool endNode)
        {
            Node currentNode = cache[currentPosition];

            KeyValuePair<Direction, int> lowestEntry = currentNode.entries.OrderBy(entry => entry.Value).First();

            Dictionary<Direction, Position> neighbours = ConstructNeighbours(currentPosition, maze);

            foreach (KeyValuePair<Direction, Position> neighbour in neighbours)
            {
                Direction neighbourDirection = neighbour.Key;
                Position neighbourPosition = neighbour.Value;
                Node neighbourNode = cache[neighbourPosition];

                List<int> neighbourToCurrentPoints = new List<int>();

                if (neighbourNode.entries.ContainsKey(neighbourDirection))
                {
                    neighbourToCurrentPoints.Add(neighbourNode.entries[neighbourDirection] + 1);
                }
                if (neighbourNode.entries.Any(entry => entry.Key != neighbourDirection && entry.Key != OppositeDirection(neighbourDirection)))
                {
                    neighbourToCurrentPoints.Add(
                        neighbourNode.entries.Where(entry => entry.Key != neighbourDirection && entry.Key != OppositeDirection(neighbourDirection))
                        .Min(entry => entry.Value) + 1000 + 1);
                }

                List<int> currentToExitPoints = new List<int>();

                int nextExitPoints = 0;

                if (!endNode)
                {
                    if (OppositeDirection(neighbourDirection) != exitDirection)
                    {
                        currentToExitPoints = neighbourToCurrentPoints.Select(p => p + 1000 + 1).ToList();
                        nextExitPoints = exitPoints - 1000 - 1;
                    }
                    else
                    {
                        currentToExitPoints = neighbourToCurrentPoints.Select(p => p + 1).ToList();

                        nextExitPoints = exitPoints - 1;
                    }
                }
                else
                {
                    currentToExitPoints = neighbourToCurrentPoints;

                    nextExitPoints = lowestEntry.Value;
                }

                if (onBestPath.Contains(neighbourPosition)) continue;

                if (!currentToExitPoints.Contains(exitPoints)) continue;

                onBestPath.Add(neighbourPosition);

                BackTrack(cache, maze, onBestPath, neighbourPosition, nextExitPoints, OppositeDirection(neighbourDirection), false);

            }
        }

        static void UpdateNeighbours(Dictionary<Position, Node> cache, Position currentPosition, string[] maze)
        {
            Dictionary<Direction, Position> neighbours = ConstructNeighbours(currentPosition, maze);
            Node currentNode = cache[currentPosition];

            foreach (KeyValuePair<Direction, Position> neighbour in neighbours)
            {
                Direction neighbourDirection = neighbour.Key;
                Direction incomingDirection = OppositeDirection(neighbourDirection);
                Position neighbourPosition = neighbour.Value;

                List<int> potentialPointsForNeighbour = new List<int>();

                if (currentNode.entries.ContainsKey(incomingDirection))
                {
                    potentialPointsForNeighbour.Add(currentNode.entries[incomingDirection] + 1);
                }
                else
                {
                    potentialPointsForNeighbour.Add(currentNode.entries.Values.Min() + 1000 + 1);
                }

                int lowestPointsForNeighbour = potentialPointsForNeighbour.Min();

                if (cache.ContainsKey(neighbourPosition))
                {
                    Node neighbourNode = cache[neighbourPosition];

                    if (neighbourNode.entries.ContainsKey(incomingDirection)) continue;

                    neighbourNode.entries.Add(incomingDirection, lowestPointsForNeighbour);
                }
                else
                {
                    Node neighbourNode = new Node
                    {
                        entries = new Dictionary<Direction, int>(),
                        entriesExhausted = false,
                    };

                    neighbourNode.entries.Add(incomingDirection, lowestPointsForNeighbour);

                    cache.Add(neighbourPosition, neighbourNode);
                }
            }
        }

        static Dictionary<Direction, Position> ConstructNeighbours(Position position, string[] maze)
        {
            Dictionary<Direction, Position> neighbours = new Dictionary<Direction, Position>
            {
                [Direction.North] = new Position() { top = position.top - 1, left = position.left },
                [Direction.East] = new Position() { top = position.top, left = position.left + 1 },
                [Direction.South] = new Position() { top = position.top + 1, left = position.left },
                [Direction.West] = new Position() { top = position.top, left = position.left - 1 }
            };

            return neighbours
                .Where(neighbour => neighbour.Value.top >= 0 && neighbour.Value.left >= 0)
                .Where(neighbour => neighbour.Value.top < MAZE_HEIGHT && neighbour.Value.left < MAZE_WIDTH)
                .Where(neighbour => maze[neighbour.Value.top][neighbour.Value.left] != '#')
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        static Direction OppositeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.North: return Direction.South;
                case Direction.East: return Direction.West;
                case Direction.South: return Direction.North;
                default: return Direction.East;
            }
        }

        static void DisplayMazeBackTrack(string[] maze, Dictionary<Position, Node> cache, List<Position> onBestPath)
        {
            Console.Clear();

            for (int top = 0; top < maze.Length; top++)
            {
                Console.Write($"\n{top.ToString().PadLeft(4)} ");
                for (int left = 0; left < maze[top].Length; left++)
                {
                    if (maze[top][left] == '#')
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Position position = new Position() { left = left, top = top };

                        if (onBestPath.Contains(position))
                        {
                            Console.BackgroundColor = ConsoleColor.Yellow;
                        }
                    }

                    Console.Write(maze[top][left]);
                    Console.BackgroundColor = ConsoleColor.Black;
                }
            }

            Console.WriteLine();
        }

        static void DisplayMaze(string[] maze, Dictionary<Position, Node> cache, KeyValuePair<Position, Node>[] squaresWithInexhaustedEntries)
        {
            Console.Clear();

            for (int top = 0; top < maze.Length; top++)
            {
                Console.Write($"\n{top.ToString().PadLeft(4)} ");
                for (int left = 0; left < maze[top].Length; left++)
                {
                    if (maze[top][left] == '#')
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Position position = new Position() { left = left, top = top };

                        if (cache.ContainsKey(position) && !squaresWithInexhaustedEntries.Any(square => square.Key.top == position.top && square.Key.left == position.left))
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                        }
                        else if (cache.ContainsKey(position))
                        {
                            Console.BackgroundColor = ConsoleColor.Green;
                        }
                    }

                    Console.Write(maze[top][left]);
                    Console.BackgroundColor = ConsoleColor.Black;
                }
            }

            Console.WriteLine();
        }
    }
}
