using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Day_8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<char, List<(int, int)>> antennaLocations = new Dictionary<char, List<(int, int)>>();

            string[] input = File.ReadAllLines("input.txt");

            int rows = input.Length;
            int columns = input[0].Length;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    char c = input[row][column];

                    if (c == '.') continue;

                    if (antennaLocations.ContainsKey(c)) antennaLocations[c].Add((row, column));

                    else antennaLocations.Add(c, new List<(int, int)> { (row, column) });
                }
            }

            // For part one
            PartOne(antennaLocations, rows, columns);

            // For part two
            PartTwo(antennaLocations, rows, columns);

            Console.ReadKey();
        }

        static ((int, int), (int, int)) CalculateAntinodes((int, int) locationOne, (int, int) locationTwo)
        {
            int col1 = locationOne.Item2;
            int col2 = locationTwo.Item2;

            int row1 = locationOne.Item1;
            int row2 = locationTwo.Item1;

            int deltaCol = col2 - col1;
            int deltaRow = row2 - row1;

            (int, int) antinodeOne;
            (int, int) antinodeTwo;

            antinodeOne.Item1 = row1 - deltaRow;
            antinodeOne.Item2 = col1 - deltaCol;

            antinodeTwo.Item1 = row2 + deltaRow;
            antinodeTwo.Item2 = col2 + deltaCol;

            return (antinodeOne, antinodeTwo);
        }

        static List<(int, int)> CalculateResonantAntinodes((int, int) locationOne, (int, int) locationTwo, int rows, int columns)
        {
            List<(int, int)> antinodes = new List<(int, int)>();

            int col1 = locationOne.Item2;
            int col2 = locationTwo.Item2;

            int row1 = locationOne.Item1;
            int row2 = locationTwo.Item1;

            int deltaCol = col2 - col1;
            int deltaRow = row2 - row1;

            int hcf = HCF(Math.Abs(deltaCol), Math.Abs(deltaRow));

            deltaCol = deltaCol / hcf;
            deltaRow = deltaRow / hcf;

            int i = 0;

            while (true)
            {
                int posCol = col1 + i * deltaCol;
                int posRow = row1 + i * deltaRow;
                int negCol = col1 - i * deltaCol;
                int negRow = row1 - i * deltaRow;

                bool posValid = posCol >= 0 && posCol < columns && posRow >= 0 && posRow < rows;
                bool negValid = negCol >= 0 && negCol < columns && negRow >= 0 && negRow < rows;

                if (!posValid && !negValid) break;

                if (posValid) antinodes.Add((posRow, posCol));
                if (negValid) antinodes.Add((negRow, negCol));

                i += 1;
            }

            antinodes = antinodes.Distinct().ToList();

            return antinodes;
        }

        private static int HCF(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a | b;
        }

        static void PartOne(Dictionary<char, List<(int, int)>> antennaLocations, int rows, int columns)
        {
            List<(int, int)> antinodeLocations = new List<(int, int)>();

            foreach (char key in antennaLocations.Keys)
            {
                List<(int, int)> locations = antennaLocations[key];

                foreach ((int, int) locationOne in locations)
                {
                    foreach ((int, int) locationTwo in locations)
                    {
                        if (locationOne == locationTwo) continue;

                        ((int, int), (int, int)) antinodes = CalculateAntinodes(locationOne, locationTwo);

                        (int, int) antinodeOne = antinodes.Item1;
                        (int, int) antinodeTwo = antinodes.Item2;

                        if (antinodeOne.Item1 >= 0 && antinodeOne.Item1 < rows && antinodeOne.Item2 >= 0 && antinodeOne.Item2 < columns)
                        {
                            antinodeLocations.Add(antinodeOne);
                        }

                        if (antinodeTwo.Item1 >= 0 && antinodeTwo.Item1 < rows && antinodeTwo.Item2 >= 0 && antinodeTwo.Item2 < columns)
                        {
                            antinodeLocations.Add(antinodeTwo);
                        }
                    }
                }
            }

            antinodeLocations = antinodeLocations.Distinct().ToList();

            Console.WriteLine(antinodeLocations.Count);
        }

        static void PartTwo(Dictionary<char, List<(int, int)>> antennaLocations, int rows, int columns)
        {
            List<(int, int)> antinodeLocations = new List<(int, int)>();

            foreach (char key in antennaLocations.Keys)
            {
                List<(int, int)> locations = antennaLocations[key];

                foreach ((int, int) locationOne in locations)
                {
                    foreach ((int, int) locationTwo in locations)
                    {
                        if (locationOne == locationTwo) continue;

                        List<(int, int)> antinodes = CalculateResonantAntinodes(locationOne, locationTwo, rows, columns);

                        antinodeLocations.AddRange(antinodes);
                    }
                }
            }

            antinodeLocations = antinodeLocations.Distinct().ToList();

            Console.WriteLine(antinodeLocations.Count);
        }
    }
}
