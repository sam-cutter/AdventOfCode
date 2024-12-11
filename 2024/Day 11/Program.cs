using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day_11
{
    internal class Program
    {
        const int blinks = 25;

        static void Main(string[] args)
        {
            List<ulong> initialStones = File.ReadAllText("input.txt").Trim().Split(' ').Select(ulong.Parse).ToList();

            List<ulong> stones = initialStones.ToList();

            for (int i = 0; i < blinks; i++)
            {
                stones = Blink(stones);

                Console.WriteLine($"Blink {i + 1}/{blinks}");
            }

            Console.WriteLine(stones.Count);

            Console.ReadKey();
        }

        static List<ulong> Blink(List<ulong> initialStones)
        {
            List<ulong> newStones = new List<ulong>();

            foreach (ulong stone in initialStones)
            {
                int log10stone = (int)Math.Floor(Math.Log10(stone));

                if (stone == 0) newStones.Add(1);

                else if (log10stone % 2 == 1) newStones.AddRange(new List<ulong>() { GetHalves(stone).Item1, GetHalves(stone).Item2 });

                else newStones.Add(stone * 2024);
            }

            return newStones;
        }

        static (ulong, ulong) GetHalves(ulong stone)
        {
            int log10stone = (int)Math.Floor(Math.Log10(stone));

            ulong leftHalf = (ulong)Math.Floor(stone / Math.Pow(10, (log10stone + 1) / 2));
            ulong rightHalf = stone - leftHalf * (ulong)(Math.Pow(10, (log10stone + 1) / 2));

            int leftPowerOf10 = 0;

            while (leftHalf % (ulong)Math.Pow(10, leftPowerOf10 + 2) == 0) leftPowerOf10++;

            leftHalf /= (ulong)Math.Pow(10, leftPowerOf10);

            if (!(rightHalf == 0))
            {
                int rightPowerOf10 = 0;

                while (rightHalf % (ulong)Math.Pow(10, rightPowerOf10 + 2) == 0) rightPowerOf10++;

                rightHalf /= (ulong)Math.Pow(10, rightPowerOf10);
            }

            return (leftHalf, rightHalf);
        }
    }
}
