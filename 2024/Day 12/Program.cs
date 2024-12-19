using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day_12
{
    internal class Program
    {
        static string[] garden = File.ReadAllLines("test1.txt");
        static int GARDEN_HEIGHT = garden.Length;
        static int GARDEN_WIDTH = garden[0].Length;

        struct Position
        {
            public int top, left;
        }

        static List<Position> Neighbours(Position position)
        {
            Position north = new Position() { left = position.left, top = position.top - 1 };
            Position south = new Position() { left = position.left, top = position.top + 1 };
            Position east = new Position() { left = position.left + 1, top = position.top };
            Position west = new Position() { left = position.left - 1, top = position.top };

            List<Position> neighbours = new List<Position>() { north, south, east, west };

            return neighbours
                .Where(neighbour => neighbour.top >= 0 && neighbour.left >= 0)
                .Where(neighbour => neighbour.top < GARDEN_HEIGHT && neighbour.left < GARDEN_WIDTH)
                .ToList();
        }

        static void Main(string[] args)
        {
            List<(char plant, List<Position> plots)> regions = FindRegions();

            Console.WriteLine(regions.Count);

            Console.ReadKey();
        }


        static List<(char plant, List<Position> plots)> FindRegions()
        {
            List<(char plant, List<Position> plots)> regions = new List<(char plant, List<Position> plots)> ();

            Position searchStart = new Position() { top = 0, left = 0 };

            while (true)
            {
                char plant = garden[searchStart.top][searchStart.left];

                List<Position> region = new List<Position>();
                FindRegion(plant, region, searchStart);
                regions.Add((plant, region));

                Position nextSearchStart = new Position() { top = searchStart.top, left = searchStart.left };

                // TODO: this search is not working :(

                for (int top = searchStart.top; top < GARDEN_HEIGHT; top++)
                {
                    int leftBound = 0;

                    if (top == searchStart.top) leftBound = searchStart.left + 1;

                    for (int left = leftBound; left < GARDEN_WIDTH; left++)
                    {
                        if (garden[top][left] == plant) continue;

                        nextSearchStart.top = top;
                        nextSearchStart.left = left;

                        break;
                    }

                    if (nextSearchStart.top != searchStart.top || nextSearchStart.left != searchStart.left) break;
                }

                if (nextSearchStart.top == searchStart.top && nextSearchStart.left == searchStart.left) break;
            }

            return regions;
        }

        static void FindRegion(char plant, List<Position> region, Position startPosition)
        {
            region.Add(startPosition);

            List<Position> neighbours = Neighbours(startPosition);

            foreach (Position neighbour in neighbours)
            {
                if (garden[neighbour.top][neighbour.left] != plant) continue;
                if (region.Contains(neighbour)) continue;

                FindRegion(plant, region, neighbour);
            }
        }
    }
}
