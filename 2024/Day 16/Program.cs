using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Day_16
{
    internal class Program
    {
        struct Position
        {
            public int top;
            public int left;
        }

        struct Node
        {
            public int workingValue;
            public bool locked;
            public Facing facing;
        }

        enum Facing
        {
            North,
            East,
            South,
            West
        }

        static void Main(string[] args)
        {
            string[] maze = File.ReadAllLines("test1.txt");
            Dictionary<Position, Node> cache = new Dictionary<Position, Node>();

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

            Node startingNode = new Node() { workingValue = 0, locked = true, facing = Facing.East };

            cache.Add(startingPosition, startingNode);

            KeyValuePair<Position, Node> nextKVP = new KeyValuePair<Position, Node>(startingPosition, startingNode);

            while (true)
            {
                Djikstras(maze, cache, nextKVP.Key, nextKVP.Value.facing, nextKVP.Value.workingValue);


                List<KeyValuePair<Position, Node>> unlockedKVPs = cache.Where(kvp => !kvp.Value.locked).ToList();

                if (unlockedKVPs.Count() > 0)
                {
                    KeyValuePair<Position, Node> next = unlockedKVPs.OrderBy(kvp => kvp.Value.workingValue).First();
                    cache[next.Key] = new Node() { workingValue = next.Value.workingValue, locked = true, facing = next.Value.facing };

                    nextKVP = next;
                }
                else
                {
                    break;
                }
            }

            Console.WriteLine(cache[endingPosition].workingValue);

            List<Position> onBestPath = new List<Position>();
            onBestPath.Add(endingPosition);

            List<Position> visited = new List<Position>();
            visited.Add(endingPosition);

            BackTrack(maze, cache, new KeyValuePair<Position, Node>(endingPosition, cache[endingPosition]), onBestPath, visited);

            for (int top = 0; top < maze.Length; top++)
            {
                Console.WriteLine();
                for (int left = 0; left < maze[top].Length; left++)
                {
                    if (onBestPath.Contains(new Position { top = top, left = left }))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    if (maze[top][left] == '#')
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write("#######");
                        Console.BackgroundColor = ConsoleColor.Black;
                        continue;
                    }

                    Node node = cache[new Position { top = top, left = left }];

                    switch (node.facing)
                    {
                        case Facing.North:
                            Console.Write('^');
                            break;
                        case Facing.South:
                            Console.Write('v');
                            break;
                        case Facing.West:
                            Console.Write('<');
                            break;
                        case Facing.East:
                            Console.Write('>');
                            break;
                    }

                    Console.Write(node.workingValue.ToString().PadLeft(5));
                    Console.Write(' ');

                    Console.ForegroundColor = ConsoleColor.Gray;

                }
            }

            Console.WriteLine();

            Console.WriteLine(onBestPath.Distinct().Count());

            Console.ReadKey();
        }

        static void Djikstras(string[] maze, Dictionary<Position, Node> cache, Position position, Facing facing, int points)
        {
            Position positionInFront = PositionInFront(position, facing);

            Position up;
            up.left = position.left;
            up.top = position.top - 1;

            Position down;
            down.left = position.left;
            down.top = position.top + 1;

            Position left;
            left.left = position.left - 1;
            left.top = position.top;

            Position right;
            right.left = position.left + 1;
            right.top = position.top;

            Position[] neighbours = { up, down, left, right };

            foreach (Position neighbour in neighbours)
            {
                if (neighbour.top < 0 || neighbour.left < 0) continue;
                if (neighbour.top >= maze.Length || neighbour.left >= maze[0].Length) continue;
                if (maze[neighbour.top][neighbour.left] == '#') continue;
                if (cache.ContainsKey(neighbour) && cache[neighbour].locked) continue;

                if (neighbour.top == positionInFront.top && neighbour.left == positionInFront.left)
                {
                    int neighbourPoints = points + 1;

                    if (cache.ContainsKey(neighbour) && cache[neighbour].workingValue < neighbourPoints) continue;

                    cache[neighbour] = new Node() { workingValue = points + 1, locked = false, facing = facing };
                }
                else
                {
                    int neighbourPoints = points + 1000 + 1;

                    if (cache.ContainsKey(neighbour) && cache[neighbour].workingValue < neighbourPoints) continue;

                    Facing neighbourFacing = facing;

                    if (neighbour.top < position.top) neighbourFacing = Facing.North;
                    if (neighbour.top > position.top) neighbourFacing = Facing.South;
                    if (neighbour.left < position.left) neighbourFacing = Facing.West;
                    if (neighbour.left > position.left) neighbourFacing = Facing.East;

                    cache[neighbour] = new Node() { workingValue = points + 1000 + 1, locked = false, facing = neighbourFacing };
                }
            }
        }

        static Position PositionInFront(Position position, Facing facing)
        {
            Position positionInFront = new Position() { top = position.top, left = position.left };

            switch (facing)
            {
                case Facing.North:
                    positionInFront.top -= 1;
                    break;
                case Facing.South:
                    positionInFront.top += 1;
                    break;
                case Facing.West:
                    positionInFront.left -= 1;
                    break;
                case Facing.East:
                    positionInFront.left += 1;
                    break;
            }

            return positionInFront;
        }

        static void BackTrack(string[] maze, Dictionary<Position, Node> cache, KeyValuePair<Position, Node> kvp, List<Position> onBestPath, List<Position> visited)
        {
            Position position = kvp.Key;
            Facing facing = kvp.Value.facing;
            int points = kvp.Value.workingValue;

            Position up;
            up.left = position.left;
            up.top = position.top - 1;

            Position down;
            down.left = position.left;
            down.top = position.top + 1;

            Position left;
            left.left = position.left - 1;
            left.top = position.top;

            Position right;
            right.left = position.left + 1;
            right.top = position.top;

            Position[] neighbours = { up, down, left, right };

            foreach (Position neighbour in neighbours)
            {
                if (neighbour.top < 0 || neighbour.left < 0) continue;
                if (neighbour.top >= maze.Length || neighbour.left >= maze[0].Length) continue;
                if (maze[neighbour.top][neighbour.left] == '#') continue;

                Position positionInFront = PositionInFront(neighbour, cache[neighbour].facing);

                int pointsShouldBe;

                if (position.top == positionInFront.top && position.left == positionInFront.left) pointsShouldBe = cache[neighbour].workingValue + 1;
                else pointsShouldBe = cache[neighbour].workingValue + 1000 + 1;

                if (points != pointsShouldBe) continue;
                if (visited.Contains(neighbour) || onBestPath.Contains(neighbour)) continue;

                onBestPath.Add(neighbour);
                visited.Add(neighbour);

                BackTrack(maze, cache, new KeyValuePair<Position, Node>(neighbour, cache[neighbour]), onBestPath, visited);
            }
        }
    }
}
