using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_11
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<ulong> initialStonesList = File
                .ReadAllText("input.txt")
                .Trim()
                .Split(' ')
                .Select(ulong.Parse)
                .ToList();

            Dictionary<ulong, ulong> initialStones = new Dictionary<ulong, ulong>();

            foreach (ulong stone in initialStonesList)
            {
                if (initialStones.ContainsKey(stone)) initialStones[stone] += 1;
                else initialStones.Add(stone, 1);
            }

            // For part one
            PartOne(initialStones);

            // For part two
            PartTwo(initialStones);

            Console.ReadKey();
        }

        static void PartOne(Dictionary<ulong, ulong> initialStones)
        {
            Console.WriteLine(PerformBlinks(initialStones, 25));
        }

        static void PartTwo(Dictionary<ulong, ulong> initialStones)
        {
            Console.WriteLine(PerformBlinks(initialStones, 75));
        }

        static ulong PerformBlinks(Dictionary<ulong, ulong> initialStones, int blinks)
        {
            Dictionary<ulong, ulong> stones = initialStones.ToDictionary(x => x.Key, x => x.Value);

            for (int i = 0; i < blinks; i++)
            {
                stones = Blink(stones);
            }

            ulong total = 0;

            foreach (ulong stoneFrequency in stones.Values)
            {
                total += stoneFrequency;
            }

            return total;
        }

        static Dictionary<ulong, ulong> Blink(Dictionary<ulong, ulong> initialStones)
        {
            Dictionary<ulong, ulong> newStones = new Dictionary<ulong, ulong>();

            foreach (KeyValuePair<ulong, ulong> stoneFrequencyPair in initialStones)
            {
                int log10stone = (int)Math.Floor(Math.Log10(stoneFrequencyPair.Key));

                if (stoneFrequencyPair.Key == 0) {
                    if (!newStones.ContainsKey(1)) newStones.Add(1, stoneFrequencyPair.Value);
                    else newStones[1] += stoneFrequencyPair.Value;
                }

                else if (log10stone % 2 == 1)
                {
                    (ulong, ulong) halves = GetHalves(stoneFrequencyPair.Key);

                    if (!newStones.ContainsKey(halves.Item1)) newStones.Add(halves.Item1, stoneFrequencyPair.Value);
                    else newStones[halves.Item1] += stoneFrequencyPair.Value;

                    if (!newStones.ContainsKey(halves.Item2)) newStones.Add(halves.Item2, stoneFrequencyPair.Value);
                    else newStones[halves.Item2] += stoneFrequencyPair.Value;
                }

                else
                {
                    if (!newStones.ContainsKey(stoneFrequencyPair.Key * 2024)) newStones.Add(stoneFrequencyPair.Key * 2024, stoneFrequencyPair.Value);
                    else newStones[stoneFrequencyPair.Key * 2024] += stoneFrequencyPair.Value;
                }
            }

            return newStones;
        }

        static (ulong, ulong) GetHalves(ulong stone)
        {
            int log10stone = (int)Math.Floor(Math.Log10(stone));

            ulong leftHalf = (ulong)Math.Floor(stone / Math.Pow(10, (log10stone + 1) / 2));
            ulong rightHalf = stone - leftHalf * (ulong)(Math.Pow(10, (log10stone + 1) / 2));

            return (leftHalf, rightHalf);
        }
    }
}
