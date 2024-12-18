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
            public Dictionary<Direction, (int points, bool locked)> lowestPoints;
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
            Dictionary<Position, Node> cache = new Dictionary<Position, Node>();

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
        }

        static void UpdateNeighbours(Dictionary<Position, Node> cache, Position position)
        {
            Dictionary<Direction, Position> neighbours = ConstructNeighbours(position);
        }

        static Dictionary<Direction, Position> ConstructNeighbours(Position position)
        {
            Dictionary<Direction, Position> neighbours = new Dictionary<Direction, Position>
            {
                [Direction.North] = new Position() { top = position.top - 1, left = position.left },
                [Direction.East] = new Position() { top = position.top, left = position.left + 1 },
                [Direction.South] = new Position() { top = position.top + 1, left = position.left },
                [Direction.West] = new Position() { top = position.top, left = position.left - 1 }
            };

            return (Dictionary<Direction, Position>)neighbours
                .Where(neighbour => neighbour.Value.top >= 0 && neighbour.Value.left >= 0)
                .Where(neighbour => neighbour.Value.top < MAZE_HEIGHT && neighbour.Value.left < MAZE_WIDTH);
        }
    }
}
