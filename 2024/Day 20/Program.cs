using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Day_20
{
    internal class Program
    {
        static string[] raceTrack = File.ReadAllLines("input.txt");

        struct Position
        {
            public int top; public int left;
        }

        struct Node
        {
            public int shortestDistance;
            public bool locked;
        }

        static void Main(string[] args)
        {
            List<Position> squares = Distances();

            List<(Position, Position, int)> partOneSaves = Saves(squares, 2);
            Console.WriteLine(partOneSaves.Where(save => save.Item3 >= 100).Count());

            List<(Position, Position, int)> partTwoSaves = Saves(squares, 20);
            Console.WriteLine(partTwoSaves.Where(save => save.Item3 >= 100).Count());

            Console.ReadKey();
        }

        static List<(Position, Position, int)> Saves(List<Position> squares, int maximumDistance)
        {
            List<(Position, Position, int)> saves = new List<(Position, Position, int)>();

            foreach (Position square in squares)
            {
                Dictionary<Position, Node> shortestDistances = Djikstras(square, maximumDistance)
                    .Where(pnp => squares.Contains(pnp.Key))
                    .Where(pnp => squares.IndexOf(pnp.Key) > squares.IndexOf(square))
                    .ToDictionary(pnp => pnp.Key, pnp => pnp.Value);

                foreach (KeyValuePair<Position, Node> pnp in shortestDistances)
                {
                    int timeSaved = (squares.IndexOf(pnp.Key) - squares.IndexOf(square)) - pnp.Value.shortestDistance;

                    if (timeSaved > 0)
                    {
                        saves.Add((square, pnp.Key, timeSaved));
                    }
                }
            }

            return saves;
        }

        static Dictionary<Position, Node> Djikstras(Position startingPosition, int maximumDistance)
        {
            Dictionary<Position, Node> cache = new Dictionary<Position, Node>();

            Node startingNode = new Node { shortestDistance = 0, locked = true };

            cache.Add(startingPosition, startingNode);

            Position position = startingPosition;

            while (true)
            {
                UpdateNeighbourShortestDistances(cache, position, maximumDistance);

                KeyValuePair<Position, Node>[] unlockedPositionNodePairs = cache
                    .Where(pnp => !pnp.Value.locked)
                    .ToArray();

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

        static void UpdateNeighbourShortestDistances(Dictionary<Position, Node> cache, Position position, int maximumDistance)
        {
            if (cache[position].shortestDistance >= maximumDistance) return;

            Position[] neighbours = ConstructNeighbours(position)
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
            Position up = new Position() { top = position.top - 1, left = position.left };
            Position down = new Position() { top = position.top + 1, left = position.left };
            Position left = new Position() { top = position.top, left = position.left - 1 };
            Position right = new Position() { top = position.top, left = position.left + 1 };

            Position[] neighbours = new Position[] { up, down, left, right };

            return neighbours
                .Where(neighbour => neighbour.top >= 0 && neighbour.left >= 0)
                .Where(neighbour => neighbour.top < raceTrack.Length && neighbour.left < raceTrack[0].Length)
                .ToArray();
        }

        static List<Position> Distances()
        {
            Position start = new Position();
            Position end = new Position();

            for (int top = 0; top < raceTrack.Length; top++)
            {
                for (int left = 0; left < raceTrack[top].Length; left++)
                {
                    if (raceTrack[top][left] == 'S')
                    {
                        start.top = top; start.left = left;
                    }

                    if (raceTrack[top][left] == 'E')
                    {
                        end.top = top; end.left = left;
                    }
                }
            }

            List<Position> distances = new List<Position>();

            distances.Add(start);

            Position nextSquare = NextSquare(start, distances);

            while (raceTrack[nextSquare.top][nextSquare.left] != 'E')
            {
                distances.Add(nextSquare);

                nextSquare = NextSquare(nextSquare, distances);
            }

            distances.Add(end);

            return distances;
        }

        static Position NextSquare(Position position, List<Position> distances)
        {
            Position[] neighbours = ConstructNeighbours(position);

            return neighbours
                .Where(neighbour => raceTrack[neighbour.top][neighbour.left] != '#')
                .Where(neighbour => !distances.Contains(neighbour))
                .First();
        }

        static void DisplayRaceTrack((Position, Position, int) save)
        {
            Console.WriteLine("\n\n");

            for (int top = 0; top < raceTrack.Length; top++)
            {
                Console.WriteLine();

                for (int left = 0; left < raceTrack[top].Length; left++)
                {
                    if (top == save.Item1.top && left == save.Item1.left)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                    }
                    if (top == save.Item2.top && left == save.Item2.left)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                    }
                    if (raceTrack[top][left] == '#')
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    Console.Write(raceTrack[top][left]);

                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            Console.WriteLine(save.Item3);
        }
    }
}
