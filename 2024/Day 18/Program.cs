using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_18
{
    internal class Program
    {
        static int MAX_COORDINATE = 70;

        struct Position
        {
            public int x, y;
        }

        struct Node
        {
            public int shortestDistance;
            public bool locked;
        }

        static void Main(string[] args)
        {
            Position[] bytePositions = File.ReadAllLines("input.txt")
                .Select(line => line.Split(','))
                .Select(tokens => tokens.Select(int.Parse).ToList())
                .Select(numbers => new Position { x = numbers[0], y = numbers[1] })
                .ToArray();

            // For part one
            PartOne(bytePositions);

            // For part two
            PartTwo(bytePositions);

            Console.ReadKey();
        }

        static void PartOne(Position[] bytePositions)
        {
            Position startingPosition = new Position() { x = 0, y = 0 };
            Position endingPosition = new Position() { x = MAX_COORDINATE, y = MAX_COORDINATE };

            Dictionary<Position, Node> cache = Djikstras(bytePositions.Take(1024).ToArray(), startingPosition);

            Node endingNode = cache[endingPosition];

            Console.WriteLine(endingNode.shortestDistance);
        }

        static void PartTwo(Position[] bytePositions)
        {
            Position startingPosition = new Position() { x = 0, y = 0 };
            Position endingPosition = new Position() { x = MAX_COORDINATE, y = MAX_COORDINATE };

            int upperBound = bytePositions.Length - 1;
            int lowerBound = 0;
            int i = (lowerBound + upperBound) / 2;

            while (upperBound - lowerBound > 1)
            {
                Dictionary<Position, Node> cache = Djikstras(bytePositions.Take(i).ToArray(), startingPosition);

                if (cache.ContainsKey(endingPosition)) lowerBound = i;
                else upperBound = i;

                i = (lowerBound + upperBound) / 2;
            }


            Console.WriteLine($"({bytePositions[i].x}, {bytePositions[i].y})");
        }

        static Dictionary<Position, Node> Djikstras(Position[] bytePositions, Position startingPosition)
        {
            Dictionary<Position, Node> cache = new Dictionary<Position, Node>();

            Node startingNode = new Node { shortestDistance = 0, locked = true };

            cache.Add(startingPosition, startingNode);

            Position position = startingPosition;

            while (true)
            {
                UpdateNeighbourShortestDistances(cache, bytePositions, position);

                KeyValuePair<Position, Node>[] unlockedPositionNodePairs = cache.Where(pnp => !pnp.Value.locked).ToArray();

                if (unlockedPositionNodePairs.Count() > 0)
                {
                    position = unlockedPositionNodePairs.OrderBy(pnp => pnp.Value.shortestDistance).First().Key;

                    cache[position] = new Node
                    {
                        shortestDistance = cache[position].shortestDistance,
                        locked = true,
                    };
                }
                else
                {
                    break;
                }
            }

            return cache;
        }

        static void UpdateNeighbourShortestDistances(Dictionary<Position, Node> cache, Position[] bytePositions, Position position)
        {
            Position[] neighbours = ConstructNeighbours(position)
                .Where(neighbour => !bytePositions.Contains(neighbour))
                .Where(neighbour =>
                {
                    if (cache.ContainsKey(neighbour)) return !cache[neighbour].locked;
                    else return true;
                })
                .Where(neighbour =>
                {
                    if (cache.ContainsKey(neighbour)) return cache[position].shortestDistance + 1 < cache[neighbour].shortestDistance;
                    else return true;
                })
                .ToArray();

            foreach (Position neighbour in neighbours)
            {
                cache[neighbour] = new Node
                {
                    shortestDistance = cache[position].shortestDistance + 1,
                    locked = false,
                };
            }
        }

        static Position[] ConstructNeighbours(Position position)
        {
            Position up = new Position() { x = position.x, y = position.y - 1 };
            Position down = new Position() { x = position.x, y = position.y + 1 };
            Position left = new Position() { x = position.x - 1, y = position.y };
            Position right = new Position() { x = position.x + 1, y = position.y };

            Position[] neighbours = new Position[] { up, down, left, right };

            return neighbours
                .Where(neighbour => neighbour.x >= 0 && neighbour.y >= 0)
                .Where(neighbour => neighbour.x <= MAX_COORDINATE && neighbour.y <= MAX_COORDINATE)
                .ToArray();
        }
    }
}
