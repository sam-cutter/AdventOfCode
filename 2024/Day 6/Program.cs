using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Day_6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            char[][] grid = File
                .ReadAllLines("input.txt")
                .Select(line => line.ToCharArray())
                .ToArray();

            (int, int) currentGuardCoords = (-1, -1);

            for (int row = 0; row < grid.Length; row++)
            {
                if (!grid[row].Contains('^')) continue;

                int column = grid[row].ToList().IndexOf('^');

                currentGuardCoords = (row, column);

                break;
            }

            // For part one
            PartOne(grid, currentGuardCoords);

            // For part two
            PartTwo(grid, currentGuardCoords);

            Console.ReadKey();
        }

        static void PartOne(char[][] grid, (int, int) currentGuardCoords)
        {
            (bool, int, List<((int, int), int)>) result = Walkpath(grid, currentGuardCoords);

            Console.WriteLine(result.Item2);
        }

        static void PartTwo(char[][] grid, (int, int) currentGuardCoords)
        {
            int count = 0;

            (int, int)[] distinctPositions = Walkpath(grid, currentGuardCoords).Item3.Select(e => e.Item1).Distinct().ToArray();

            for (int i = 0; i < distinctPositions.Length; i++)
            {
                Console.WriteLine($"{i + 1}/{distinctPositions.Length}");

                int row = distinctPositions[i].Item1;
                int column = distinctPositions[i].Item2;

                if (currentGuardCoords == (row, column)) continue;
                if (grid[row][column] == '#') continue;

                char[][] newGrid = grid.Select(r => r.ToArray()).ToArray();

                newGrid[row][column] = '#';

                if (Walkpath(newGrid, currentGuardCoords).Item1)
                {
                    count += 1;
                }
            }


            Console.WriteLine(count);
        }

        static (bool, int, List<((int, int), int)>) Walkpath(char[][] grid, (int, int) currentGuardCoords)
        {
            (int, int) nextGuardCoords = (currentGuardCoords.Item1 - 1, currentGuardCoords.Item2);

            int currentDirection = 0;
            int distinctPositions = 1;

            List<((int, int), int)> previousGuardPositions = new List<((int, int), int)>() { (currentGuardCoords, currentDirection) };

            while (nextGuardCoords.Item1 > -1 && nextGuardCoords.Item1 < grid.Length && nextGuardCoords.Item2 > -1 && nextGuardCoords.Item2 < grid[0].Length)
            {
                if (grid[nextGuardCoords.Item1][nextGuardCoords.Item2] == '#')
                {
                    currentDirection += 90;
                    currentDirection = currentDirection % 360;
                }
                else
                {
                    currentGuardCoords = nextGuardCoords;

                    if (!previousGuardPositions.Select(p => p.Item1).Contains(currentGuardCoords))
                    {
                        distinctPositions += 1;
                        previousGuardPositions.Add((currentGuardCoords, currentDirection));
                    } else if (previousGuardPositions.Contains((currentGuardCoords, currentDirection)))
                    {
                        return (true, -1, previousGuardPositions);
                    }
                }

                switch (currentDirection)
                {
                    case 0:
                        nextGuardCoords = (currentGuardCoords.Item1 - 1, currentGuardCoords.Item2);
                        break;
                    case 90:
                        nextGuardCoords = (currentGuardCoords.Item1, currentGuardCoords.Item2 + 1);
                        break;
                    case 180:
                        nextGuardCoords = (currentGuardCoords.Item1 + 1, currentGuardCoords.Item2);
                        break;
                    case 270:
                        nextGuardCoords = (currentGuardCoords.Item1, currentGuardCoords.Item2 - 1);
                        break;
                    default:
                        throw new Exception();
                }
            }

            return (false, distinctPositions, previousGuardPositions);
        }

        static void DisplayGrid(char[][] grid, (int, int) currentGuardCoords)
        {
            for (int row = 0; row < grid.Length; row++)
            {
                Console.WriteLine();
                for (int column = 0; column < grid[row].Length; column++)
                {
                    if (currentGuardCoords == (row, column))
                    {
                        Console.Write("X");
                    }
                    else
                    {
                        Console.Write(grid[row][column]);
                    }
                }
            }
        }
    }
}
