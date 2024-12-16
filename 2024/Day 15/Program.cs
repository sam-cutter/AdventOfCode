using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Day_15
{
    internal class Program
    {
        static List<((int top, int left) position, int width)> boxes = new List<((int top, int left) position, int width)>();
        static List<(int top, int left)> walls = new List<(int top, int left)>();
        static (int height, int width) warehouseDimensions;

        static void Main(string[] args)
        {
            // For part one
            PartOne();

            boxes.Clear();
            walls.Clear();

            // For part two
            PartTwo();

            Console.ReadKey();
        }

        static ((int top, int left) robot, List<(char, int)> moves) ParseInput(bool scaleByTwo)
        {
            string[] lines = File.ReadAllLines("input.txt");

            int breakIndex = Array.FindIndex(lines, line => line == "");

            string[] warehouse = lines.Take(breakIndex).ToArray();

            (int top, int left) robot = (0, 0);

            int horizontalScale = (scaleByTwo ? 2 : 1);

            warehouseDimensions.height = warehouse.Length;
            warehouseDimensions.width = warehouse[0].Length * horizontalScale;

            for (int top = 0; top < warehouse.Length; top++)
            {
                for (int left = 0; left < warehouse[top].Length; left++)
                {
                    if (warehouse[top][left] == 'O') boxes.Add(((top, left * horizontalScale), horizontalScale));
                    else if (warehouse[top][left] == '#')
                    {
                        walls.Add((top, left * horizontalScale));

                        if (scaleByTwo)
                        {
                            walls.Add((top, left * horizontalScale + 1));
                        }
                    }
                    else if (warehouse[top][left] == '@') robot = (top, left * horizontalScale);
                }
            }

            string inputMoves = string.Concat(lines.Skip(breakIndex + 1).ToArray());

            List<(char, int)> moves = new List<(char, int)>();

            char currentMove = inputMoves[0];
            int currentCount = 1;

            for (int i = 1; i < inputMoves.Length; i++)
            {
                if (inputMoves[i] == currentMove) currentCount++;
                else
                {
                    moves.Add((currentMove, currentCount));

                    currentMove = inputMoves[i];
                    currentCount = 1;
                }
            }

            moves.Add((currentMove, currentCount));

            return (robot, moves);
        }

        static void PartOne()
        {
            ((int top, int left) robot, List<(char, int)> moves) = ParseInput(false);

            foreach ((char direction, int distance) move in moves)
            {
                int moveableDistance = EmptySpaceBeforeWall(robot, move.direction);
                int distanceToMove = Math.Min(move.distance, moveableDistance);

                if (distanceToMove == 0) continue;

                Move(ref robot, distanceToMove, move.direction);
            }

            int gpsSum = 0;

            foreach (((int top, int left) position, int width) box in boxes)
            {
                gpsSum += 100 * box.position.top + box.position.left;
            }

            Console.WriteLine(gpsSum);
        }

        static void PartTwo()
        {
            ((int top, int left) robot, List<(char, int)> moves) = ParseInput(true);

            foreach ((char direction, int distance) move in moves)
            {
                int moveableDistance = EmptySpaceBeforeWall(robot, move.direction);
                int distanceToMove = Math.Min(move.distance, moveableDistance);

                if (distanceToMove == 0) continue;

                Move(ref robot, distanceToMove, move.direction);
            }

            int gpsSum = 0;

            foreach (((int top, int left) position, int width) box in boxes)
            {
                gpsSum += 100 * box.position.top + box.position.left;
            }

            Console.WriteLine(gpsSum);
        }

        static void Move(ref (int top, int left) obj, int distance, char direction)
        {
            int gapLength = ImmediateEmptySpace(obj, direction).distance;

            if (gapLength < distance)
            {
                (int top, int left) boxPosition = obj;

                switch (direction)
                {
                    case '^':
                        boxPosition.top -= (gapLength + 1);
                        break;
                    case 'v':
                        boxPosition.top += (gapLength + 1);
                        break;
                    case '<':
                        boxPosition.left -= (gapLength + 1);
                        break;
                    case '>':
                        boxPosition.left += (gapLength + 1);
                        break;
                }

                int boxIndex = boxes.FindIndex(box => box.position == boxPosition);

                Move(ref boxPosition, distance - gapLength, direction);

                boxes[boxIndex] = (boxPosition, boxes[boxIndex].width);
            }

            switch (direction)
            {
                case '^':
                    obj.top -= distance;
                    break;
                case 'v':
                    obj.top += distance;
                    break;
                case '<':
                    obj.left -= distance;
                    break;
                case '>':
                    obj.left += distance;
                    break;
            }
        }

        static (int distance, bool wall) ImmediateEmptySpace((int top, int left) obj, char direction)
        {
            int distance = 0;

            (int top, int left) searchSquare = obj;

            while (true)
            {
                switch (direction)
                {
                    case '^':
                        searchSquare.top -= 1;
                        break;
                    case 'v':
                        searchSquare.top += 1;
                        break;
                    case '<':
                        searchSquare.left -= 1;
                        break;
                    case '>':
                        searchSquare.left += 1;
                        break;
                }

                if (searchSquare.top < 0 || searchSquare.left < 0) break;
                if (searchSquare.top >= warehouseDimensions.height || searchSquare.left >= warehouseDimensions.width) break;
                if (boxes.Any(box => box.position == searchSquare) || walls.Contains(searchSquare)) break;

                distance++;
            }

            return (distance, walls.Contains(searchSquare));
        }

        static int EmptySpaceBeforeWall((int top, int left) obj, char direction)
        {
            int distance = 0;

            (int top, int left) searchSquare = obj;

            while (true)
            {
                (int gapLength, bool wall) = ImmediateEmptySpace(searchSquare, direction);

                distance += gapLength;

                if (wall) break;

                switch (direction)
                {
                    case '^':
                        searchSquare.top -= (gapLength + 1);
                        break;
                    case 'v':
                        searchSquare.top += (gapLength + 1);
                        break;
                    case '<':
                        searchSquare.left -= (gapLength + 1);
                        break;
                    case '>':
                        searchSquare.left += (gapLength + 1);
                        break;
                }
            }

            return distance;
        }
    }
}
