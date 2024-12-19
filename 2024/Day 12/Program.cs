using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Day_12
{
    internal class Program
    {
        static string[] garden = File.ReadAllLines("input.txt");
        static int GARDEN_HEIGHT = garden.Length;
        static int GARDEN_WIDTH = garden[0].Length;

        struct Position
        {
            public int top, left;
        }

        struct Edge
        {
            public Position position;
            public Orientation orientation;
        }

        struct Side
        {
            public Position position;
            public Orientation orientation;
            public int length;
        }

        enum Orientation
        {
            Horizontal,
            Vertical
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

        static List<Position> ForeignNeighbours(Position position, char plant)
        {
            List<Position> neighbours = Neighbours(position)
                .Where(neighbour => garden[neighbour.top][neighbour.left] != plant)
                .ToList();

            if (position.top == 0) neighbours.Add(new Position { top = -1, left = position.left });
            if (position.left == 0) neighbours.Add(new Position { top = position.top, left = -1 });
            if (position.top == GARDEN_HEIGHT - 1) neighbours.Add(new Position { top = GARDEN_HEIGHT, left = position.left });
            if (position.left == GARDEN_WIDTH - 1) neighbours.Add(new Position { top = position.top, left = GARDEN_WIDTH });

            return neighbours;
        }

        static Edge NeighbourToEdge(Position position, Position neighbour)
        {
            Edge edge = new Edge();

            if (neighbour.top != position.top)
            {
                edge.orientation = Orientation.Horizontal;

                if (neighbour.top < position.top) edge.position = position;
                else edge.position = neighbour;
            }
            else if (neighbour.left != position.left)
            {
                edge.orientation = Orientation.Vertical;

                if (neighbour.left < position.left) edge.position = position;
                else edge.position = neighbour;
            }

            return edge;
        }

        static Edge BeforeSide(Side side)
        {
            Edge edge = new Edge();

            edge.orientation = side.orientation;

            switch (side.orientation)
            {
                case Orientation.Horizontal:
                    edge.position.top = side.position.top;
                    edge.position.left = side.position.left - 1;
                    break;
                case Orientation.Vertical:
                    edge.position.top = side.position.top - 1;
                    edge.position.left = side.position.left;
                    break;
            }

            return edge;
        }

        static Edge AfterSide(Side side)
        {
            Edge edge = new Edge();

            edge.orientation = side.orientation;

            switch (side.orientation)
            {
                case Orientation.Horizontal:
                    edge.position.top = side.position.top;
                    edge.position.left = side.position.left + side.length;
                    break;
                case Orientation.Vertical:
                    edge.position.top = side.position.top + side.length;
                    edge.position.left = side.position.left;
                    break;
            }

            return edge;
        }

        static bool ValidEdgeAddition(Side side, Edge edge, char plant)
        {
            if (side.orientation == Orientation.Horizontal)
            {
                if (side.position.top == 0 || side.position.top == GARDEN_HEIGHT) return true;
                if (edge.position.left == -1 || edge.position.left == GARDEN_WIDTH) return false;

                bool regionAboveSide = garden[side.position.top][side.position.left] == plant;

                return regionAboveSide == (garden[edge.position.top][edge.position.left] == plant);
            }
            else if (side.orientation == Orientation.Vertical)
            {
                if (side.position.left == 0 || side.position.left == GARDEN_WIDTH) return true;
                if (edge.position.top == -1 || edge.position.top == GARDEN_HEIGHT) return false;

                bool regionRightOfSide = garden[side.position.top][side.position.left] == plant;

                return regionRightOfSide == (garden[edge.position.top][edge.position.left] == plant);
            }

            return false;
        }

        static List<Side> EdgesToSides(List<Edge> edges, char plant)
        {
            List<Side> sides = new List<Side>();
            List<Edge> remainingEdges = edges.ToList();

            Side currentSide = new Side();

            while (remainingEdges.Count > 0)
            {
                currentSide.position = remainingEdges[0].position;
                currentSide.orientation = remainingEdges[0].orientation;
                currentSide.length = 1;
                remainingEdges.RemoveAt(0);

                while (true)
                {
                    Edge before = BeforeSide(currentSide);
                    Edge after = AfterSide(currentSide);

                    bool additionsMade = false;

                    if (remainingEdges.Contains(before) && ValidEdgeAddition(currentSide, before, plant))
                    {
                        currentSide.length += 1;
                        currentSide.position = before.position;

                        remainingEdges.Remove(before);

                        additionsMade = true;
                    }

                    if (remainingEdges.Contains(after) && ValidEdgeAddition(currentSide, after, plant))
                    {
                        currentSide.length += 1;
                        remainingEdges.Remove(after);

                        additionsMade = true;
                    }

                    if (!additionsMade) break;
                }

                sides.Add(currentSide);
            }

            return sides;
        }

        static void Main(string[] args)
        {
            List<(char plant, List<Position> plots)> regions = FindRegions();

            int partOne = 0;
            int partTwo = 0;

            foreach ((char plant, List<Position> plots) in regions)
            {
                partOne += RegionArea(plots) * RegionPerimeter(plant, plots);
                partTwo += RegionArea(plots) * RegionSides(plant, plots);
            }

            Console.WriteLine(partOne);
            Console.WriteLine(partTwo);

            Console.ReadKey();
        }

        static int RegionArea(List<Position> plots)
        {
            return plots.Count;
        }

        static int RegionPerimeter(char plant, List<Position> plots)
        {
            int perimeter = 0;

            foreach (Position plot in plots)
            {
                perimeter += ForeignNeighbours(plot, garden[plot.top][plot.left]).Count;
            }

            return perimeter;
        }

        static int RegionSides(char plant, List<Position> plots)
        {
            List<Position> edgePlots = plots
                .Where(plot => ForeignNeighbours(plot, plant).Count > 0)
                .ToList();

            List<Edge> edges = new List<Edge>();

            foreach (Position edgePlot in edgePlots)
            {
                List<Position> foreignNeighbours = ForeignNeighbours(edgePlot, plant);
                edges.AddRange(foreignNeighbours.Select(neigbour => NeighbourToEdge(edgePlot, neigbour)));
            }

            List<Side> sides = EdgesToSides(edges, plant);

            return sides.Count;
        }

        static List<(char plant, List<Position> plots)> FindRegions()
        {
            List<(char plant, List<Position> plots)> regions = new List<(char plant, List<Position> plots)>();

            Position startingPlot = new Position() { top = 0, left = 0 };

            while (true)
            {
                char plant = garden[startingPlot.top][startingPlot.left];

                List<Position> region = new List<Position>();
                FindRegion(plant, region, startingPlot);
                regions.Add((plant, region));

                Position nextStartingPlot = new Position() { top = startingPlot.top, left = startingPlot.left };

                for (int top = startingPlot.top; top < GARDEN_HEIGHT; top++)
                {
                    int leftBound = 0;

                    if (top == startingPlot.top) leftBound = startingPlot.left + 1;

                    for (int left = leftBound; left < GARDEN_WIDTH; left++)
                    {
                        if (regions.Any(r => r.plots.Contains(new Position { left = left, top = top }))) continue;

                        nextStartingPlot.top = top;
                        nextStartingPlot.left = left;

                        break;
                    }

                    if (nextStartingPlot.top != startingPlot.top || nextStartingPlot.left != startingPlot.left) break;
                }

                if (nextStartingPlot.top == startingPlot.top && nextStartingPlot.left == startingPlot.left) break;

                startingPlot = nextStartingPlot;
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
