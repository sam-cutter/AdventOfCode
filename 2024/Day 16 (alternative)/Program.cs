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
            string[] maze = File.ReadAllLines("test1.txt");

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

            Console.WriteLine(cache[endingPosition].entries.Min(entry => entry.Value));

            List<Position> onBestPath = new List<Position>() { endingPosition };
            List<Position> visited = new List<Position>();

            BackTrack(cache, maze, onBestPath, endingPosition);

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

                //DisplayMaze(maze, cache, squaresWithInexhaustedEntries);
                //Console.WriteLine(squaresWithInexhaustedEntries.Count());
                //Console.WriteLine($"({position.top}, {position.left})");
                //Console.ReadKey();
            }

            return cache;
        }

        static void BackTrack(Dictionary<Position, Node> cache, string[] maze, List<Position> onBestPath, Position currentPosition)
        {
            Node currentNode = cache[currentPosition];

            KeyValuePair<Direction, int> lowestEntry = currentNode.entries.OrderBy(entry => entry.Value).First();

            int points = lowestEntry.Value;
            int otherPoints = points + 1000;


            Console.WriteLine(points);

            Dictionary<Direction, Position> neighbours = ConstructNeighbours(currentPosition, maze);

            foreach (KeyValuePair<Direction, Position> neighbour in neighbours)
            {
                Direction neighbourDirection = neighbour.Key;
                Position neighbourPosition = neighbour.Value;
                Node neighbourNode = cache[neighbourPosition];

                // if the 

                List<int> potentialPointsForCurrentNode = new List<int>();

                if (neighbourNode.entries.ContainsKey(neighbourDirection))
                {
                    potentialPointsForCurrentNode.Add(neighbourNode.entries[neighbourDirection] + 1);
                }
                if (neighbourNode.entries.Any(entry => entry.Key != neighbourDirection))
                {
                    potentialPointsForCurrentNode.Add(neighbourNode.entries.Values.Min() + 1000 + 1);
                }

                if (onBestPath.Contains(neighbourPosition)) continue;
                if (potentialPointsForCurrentNode.Contains(points))
                {
                    DisplayMazeBackTrack(maze, cache, onBestPath);

                    onBestPath.Add(neighbourPosition);

                    switch (Console.ReadLine().ToLower())
                    {
                        case "d":
                            Console.WriteLine(points);
                            Console.ReadKey();
                            break;
                    }

                    BackTrack(cache, maze, onBestPath, neighbourPosition);
                }
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
    }
}
