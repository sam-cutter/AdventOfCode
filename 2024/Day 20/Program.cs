using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day_20
{
    internal class Program
    {
        static string[] raceTrack = File.ReadAllLines("input.txt");

        struct Position
        {
            public int top; public int left;
        }

        static Position NextSquare(Position position, List<Position> distances)
        {
            Position north = new Position() { top = position.top - 1, left = position.left };
            Position south = new Position() { top = position.top + 1, left = position.left };
            Position east = new Position() { top = position.top, left = position.left + 1 };
            Position west = new Position() { top = position.top, left = position.left - 1 };

            Position[] neighbours = { north, south, east, west };

            return neighbours
                .Where(neighbour => raceTrack[neighbour.top][neighbour.left] != '#')
                .Where(neighbour => !distances.Contains(neighbour))
                .First();
        }

        static void Main(string[] args)
        {
            List<Position> squares = Distances();

            List<(Position, Position, int)> saves = Saves(squares);
            saves = saves.OrderBy(save => save.Item3).ToList();

            Console.WriteLine(saves.Where(save => save.Item3 >= 100).Count());

            Console.ReadKey();
            // go through each square again and calculate how much time each cheat would save
        }

        static void DisplayMaze((Position, Position, int) save)
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

        static List<(Position, Position, int)> Saves(List<Position> squares)
        {
            List<(Position, Position, int)> saves = new List<(Position, Position, int)>();

            Position northModifier = new Position() { top = -1, left = 0 };
            Position southModifier = new Position() { top = 1, left = 0 };
            Position eastModifier = new Position() { top = 0, left = 1 };
            Position westModifier = new Position() { top = 0, left = -1 };

            Position[] modifiers = { northModifier, southModifier, eastModifier, westModifier };

            foreach (Position position in squares)
            {
                int timeToCurrentSquare = squares.IndexOf(position);

                foreach (Position modifier in modifiers)
                {
                    Position disableForOne = new Position() { top = position.top + 2 * modifier.top, left = position.left + 2 * modifier.left };
                    Position disableForTwo = new Position() { top = position.top + 3 * modifier.top, left = position.left + 3 * modifier.left };
                    Position squareInFront = new Position() { top = position.top + modifier.top, left = position.left + modifier.left };

                    Dictionary<int, Position> potentialSquares = new Dictionary<int, Position>();

                    potentialSquares.Add(1, disableForOne);
                    potentialSquares.Add(2, disableForTwo);

                    foreach (KeyValuePair<int, Position> square in potentialSquares)
                    {
                        if (square.Value.left < 0 || square.Value.top < 0) continue;
                        if (square.Value.left >= raceTrack[0].Length || square.Value.top >= raceTrack.Length) continue;
                        if (raceTrack[square.Value.top][square.Value.left] == '#') continue;

                        int timeToSquare = squares.IndexOf(square.Value);

                        if (timeToSquare < timeToCurrentSquare) continue;

                        int timeSaved = timeToSquare - (timeToCurrentSquare + square.Key + 1);

                        if (timeSaved > 0)
                        {
                            if (square.Key == 2 && raceTrack[squareInFront.top][squareInFront.left] != '#') continue;

                            saves.Add((position, square.Value, timeSaved));

                            if (square.Key == 1) break;
                        }
                    }
                }
            }

            return saves.Distinct().ToList();
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
    }
}
