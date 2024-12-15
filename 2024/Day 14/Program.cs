using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_14
{
    internal class Program
    {
        const int WIDTH = 101;
        const int HEIGHT = 103;

        static void Main(string[] args)
        {
            List<((int x, int y) p, (int x, int y) v)> robots = new List<((int x, int y) p, (int x, int y) v)>();

            using (StreamReader reader = new StreamReader(File.OpenRead("input.txt")))
            {
                while (!reader.EndOfStream)
                {
                    string[] halves = reader.ReadLine().Split(' ');

                    string[] positionTokens = halves[0].Split(',');
                    int px = int.Parse(positionTokens[0].Substring(positionTokens[0].IndexOf('=') + 1));
                    int py = int.Parse(positionTokens[1].Trim());

                    string[] velocityTokens = halves[1].Split(',');
                    int vx = int.Parse(velocityTokens[0].Substring(velocityTokens[0].IndexOf('=') + 1));
                    int vy = int.Parse(velocityTokens[1].Trim());

                    ((int x, int y) p, (int x, int y) v) robot = ((px, py), (vx, vy));
                    robots.Add(robot);
                }
            }

            PartOne(robots);

            PartTwo(robots);

            Console.ReadKey();
        }

        static void PartOne(List<((int x, int y) p, (int x, int y) v)> robots)
        {
            int topLeft = 0;
            int topRight = 0;
            int bottomLeft = 0;
            int bottomRight = 0;

            foreach (((int x, int y) p, (int x, int y) v) robot in robots)
            {
                (int newpx, int newpy) = Move(robot, 100);

                if (newpx + 1 < (WIDTH + 1) / 2 && newpy + 1 < (HEIGHT + 1) / 2) topLeft++;
                if (newpx + 1 < (WIDTH + 1) / 2 && newpy + 1 > (HEIGHT + 1) / 2) bottomLeft++;
                if (newpx + 1 > (WIDTH + 1) / 2 && newpy + 1 < (HEIGHT + 1) / 2) topRight++;
                if (newpx + 1 > (WIDTH + 1) / 2 && newpy + 1 > (HEIGHT + 1) / 2) bottomRight++;
            }


            Console.WriteLine(topLeft * topRight * bottomLeft * bottomRight);
        }

        static void PartTwo(List<((int x, int y) p, (int x, int y) v)> robots)
        {
            int seconds = 0;

            while (true)
            {
                List<(int x, int y)> positions = new List<(int x, int y)>();

                foreach (var robot in robots)
                {
                    (int newpx, int newpy) = Move(robot, seconds);

                    positions.Add((newpx, newpy));
                }

                int[] xPositions = positions.OrderBy(p => p.x).Select(p => p.x).ToArray();
                int xFirstQuartileIndex = xPositions.Count() / 4;
                int xThirdQuartileIndex = xFirstQuartileIndex * 3;
                int xFirstQuartile = xPositions[xFirstQuartileIndex];
                int xThirdQuartile = xPositions[xThirdQuartileIndex];
                int xIQR = xThirdQuartile - xFirstQuartile;

                int[] yPositions = positions.OrderBy(p => p.y).Select(p => p.y).ToArray();
                int yFirstQuartileIndex = yPositions.Count() / 4;
                int yThirdQuartileIndex = yFirstQuartileIndex * 3;
                int yFirstQuartile = yPositions[yFirstQuartileIndex];
                int yThirdQuartile = yPositions[yThirdQuartileIndex];
                int yIQR = yThirdQuartile - yFirstQuartile;

                if (xIQR < 40 && yIQR < 40)
                {
                    DisplayPositions(positions);
                    Console.ReadKey();
                    Console.WriteLine($"{seconds} seconds - x IQR = {xIQR}, y IQR = {yIQR}");
                    Console.ReadKey();
                }

                seconds += 1;
            }
        }

        static (int x, int y) Move(((int x, int y) p, (int x, int y) v) robot, int seconds)
        {
            int px = robot.p.x;
            int py = robot.p.y;
            int vx = robot.v.x;
            int vy = robot.v.y;

            int newpx = Mod(px + seconds * vx, WIDTH);
            int newpy = Mod(py + seconds * vy, HEIGHT);

            return (newpx, newpy);
        }

        static void DisplayPositions(List<(int x, int y)> positions)
        {
            Console.Clear();

            for (int i = 0; i < HEIGHT; i++)
            {
                Console.WriteLine();
                for (int j = 0; j < WIDTH; j++)
                {
                    if (positions.Contains((j, i)))
                    {
                        Console.Write("X");
                    } else
                    {
                        Console.Write(" ");
                    }
                }
            }
        }

        static int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }
    }
}
