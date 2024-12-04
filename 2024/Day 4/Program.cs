using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day_4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            char[][] grid = File
                .ReadAllLines("input.txt")
                .Select(line => line.ToCharArray())
                .ToArray();

            // For part one
            PartOne(grid);

            // For part two
            PartTwo(grid);

            Console.ReadKey();
        }

        static void PartOne(char[][] grid)
        {
            int count = 0;

            for (int row = 0; row < grid.Length; row++)
            {
                for (int column = 0; column < grid[row].Length; column++)
                {
                    if (grid[row][column] != 'X') continue;

                    (int, int)[] template = new (int, int)[4] { (row, column), (row, column), (row, column), (row, column) };

                    (int, int)[] positiveHorizontal = new (int, int)[4] { (row, column), (row, column + 1), (row, column + 2), (row, column + 3) };
                    (int, int)[] negativeHorizontal = new (int, int)[4] { (row, column), (row, column - 1), (row, column - 2), (row, column - 3) };
                    (int, int)[] positiveVertical = new (int, int)[4] { (row, column), (row + 1, column), (row + 2, column), (row + 3, column) };
                    (int, int)[] negativeVertical = new (int, int)[4] { (row, column), (row - 1, column), (row - 2, column), (row - 3, column) };
                    (int, int)[] diagonal045 = new (int, int)[4] { (row, column), (row - 1, column + 1), (row - 2, column + 2), (row - 3, column + 3) };
                    (int, int)[] diagonal135 = new (int, int)[4] { (row, column), (row + 1, column + 1), (row + 2, column + 2), (row + 3, column + 3) };
                    (int, int)[] diagonal225 = new (int, int)[4] { (row, column), (row + 1, column - 1), (row + 2, column - 2), (row + 3, column - 3) };
                    (int, int)[] diagonal315 = new (int, int)[4] { (row, column), (row - 1, column - 1), (row - 2, column - 2), (row - 3, column - 3) };


                    (int, int)[][] searchLines = new (int, int)[][] { positiveHorizontal, negativeHorizontal, positiveVertical, negativeVertical, diagonal045, diagonal135, diagonal225, diagonal315 };

                    foreach ((int, int)[] searchLine in searchLines)
                    {
                        int i = 1;
                        bool valid = true;

                        while (i < 4 && valid)
                        {
                            (int, int) coordinatePair = searchLine[i];

                            if (coordinatePair.Item1 < 0 || coordinatePair.Item2 < 0 || coordinatePair.Item1 > grid[row].Length - 1 || coordinatePair.Item2 > grid[row].Length - 1)
                            {
                                valid = false;
                                continue;
                            }

                            if (GetCharAt(grid, coordinatePair) != "XMAS"[i])
                            {
                                valid = false;
                                continue;
                            }
                            else
                            {
                                valid = true;
                                i += 1;
                            }
                        }

                        if (valid)
                        {
                            count += 1;
                        }
                    }
                }
            }

            Console.WriteLine(count);
        }

        static void PartTwo(char[][] grid)
        {
            int count = 0;

            for (int row = 0; row < grid.Length - 2; row++)
            {
                for (int column = 0; column < grid[row].Length - 2; column++)
                {
                    (int, int) topLeft = (row, column);
                    (int, int) topRight = (row, column + 2);
                    (int, int) middle = (row + 1, column + 1);
                    (int, int) bottomLeft = (row + 2, column);
                    (int, int) bottomRight = (row + 2, column + 2);

                    char topLeftChar = GetCharAt(grid, topLeft);
                    char topRightChar = GetCharAt(grid, topRight);
                    char middleChar = GetCharAt(grid, middle);
                    char bottomLeftChar = GetCharAt(grid, bottomLeft);
                    char bottomRightChar = GetCharAt(grid, bottomRight);

                    if (middleChar != 'A') continue;

                    if (!"MS".Contains(topLeftChar)) continue;
                    if (!"MS".Contains(topRightChar)) continue;
                    if (!"MS".Contains(bottomLeftChar)) continue;
                    if (!"MS".Contains(bottomRightChar)) continue;

                    bool valid = false;

                    if (topLeftChar == 'M' && bottomRightChar == 'S' && topRightChar == 'M' && bottomLeftChar == 'S') valid = true;
                    if (topLeftChar == 'S' && bottomRightChar == 'M' && topRightChar == 'M' && bottomLeftChar == 'S') valid = true;
                    if (topLeftChar == 'S' && bottomRightChar == 'M' && topRightChar == 'S' && bottomLeftChar == 'M') valid = true;
                    if (topLeftChar == 'M' && bottomRightChar == 'S' && topRightChar == 'S' && bottomLeftChar == 'M') valid = true;

                    if (valid) count += 1;
                }
            }

            Console.WriteLine(count);
        }

        static char GetCharAt(char[][] grid, (int, int) coordinatePair)
        {
            return grid[coordinatePair.Item1][coordinatePair.Item2];
        }
    }
}
