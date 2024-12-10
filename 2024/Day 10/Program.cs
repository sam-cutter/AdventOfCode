using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Day_10
{
    internal class Program
    {
        static List<((int x, int y), (int x, int y))> foundRoutes = new List<((int x, int y), (int x, int y))>();

        static void Main(string[] args)
        {
            int[][] grid = File
                .ReadAllLines("input.txt")
                .Select(line => line.ToCharArray())
                .Select(characters => characters.Select(c => int.Parse(c.ToString())).ToArray())
                .ToArray();

            for (int y = 0; y < grid.Length; y++)
            {
                for (int x = 0; x < grid[y].Length; x++)
                {
                    if (grid[y][x] != 0) continue;

                    CalculateTrailEnds((x, y), (x, y), grid);
                }
            }

            // For part one
            Console.WriteLine(foundRoutes.Distinct().Count());

            // For part two
            Console.WriteLine(foundRoutes.Count());

            Console.ReadKey();
        }

        static void CalculateTrailEnds((int x, int y) startPoint, (int x, int y) currentPoint, int[][] grid)
        {
            int startPointHeight = grid[currentPoint.y][currentPoint.x];

            (int x, int y) up = (currentPoint.x, currentPoint.y - 1);
            (int x, int y) down = (currentPoint.x, currentPoint.y + 1);
            (int x, int y) left = (currentPoint.x - 1, currentPoint.y);
            (int x, int y) right = (currentPoint.x + 1, currentPoint.y);

            foreach ((int x, int y) nextPoint in new List<(int x, int y)>() { up, down, left, right })
            {
                if (nextPoint.x < 0 || nextPoint.y < 0) continue;
                if (nextPoint.x >= grid[0].Length || nextPoint.y >= grid.Length) continue;

                int nextPointHeight = grid[nextPoint.y][nextPoint.x];

                if (nextPointHeight != startPointHeight + 1) continue;
                
                if (nextPointHeight == 9)
                {
                    foundRoutes.Add((startPoint, nextPoint));
                } else
                {
                    CalculateTrailEnds(startPoint, nextPoint, grid);
                }
            }
        }
    }
}
