using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Day_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<int> leftList = new List<int>();
            List<int> rightList = new List<int>();

            List<(int, int)> numberPairs = File
                .ReadAllLines("input.txt")
                .Select(line => line.Trim())
                .Select(line => line.Split().Where(token => token != "").ToArray())
                .Select(tokens => (tokens[0], tokens[1]))
                .Select(tokens => (int.Parse(tokens.Item1), int.Parse(tokens.Item2)))
                .ToList();

            foreach ((int, int) numberPair in numberPairs)
            {
                leftList.Add(numberPair.Item1);
                rightList.Add(numberPair.Item2);
            }

            // For part one
            PartOne(leftList, rightList);

            // For part two
            PartTwo(leftList, rightList);

            Console.ReadKey();
        }

        static void PartOne(List<int> leftList, List<int> rightList)
        {
            leftList.Sort();
            rightList.Sort();

            int distance = 0;

            foreach ((int, int) numberPair in leftList.Zip(rightList, (numberOne, numberTwo) => (numberOne, numberTwo)))
            {
                distance += Math.Abs(numberPair.Item2 - numberPair.Item1);
            }

            Console.WriteLine(distance);
        }

        static void PartTwo(List<int> leftList, List<int> rightList)
        {
            Dictionary<int, int> rightListFrequencies = new Dictionary<int, int>();

            foreach (int number in rightList)
            {
                if (rightListFrequencies.ContainsKey(number))
                {
                    rightListFrequencies[number]++;
                } else
                {
                    rightListFrequencies.Add(number, 1);
                }
            }

            int similarityScore = 0;

            foreach (int number in leftList)
            {
                if (!rightListFrequencies.ContainsKey(number))
                {
                    similarityScore += 0;
                } else
                {
                    similarityScore += number * rightListFrequencies[number];
                }
            }

            Console.WriteLine(similarityScore);
        }
    }
}
