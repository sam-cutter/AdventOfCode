using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace Day_9
{
    internal class Program { 
        static void Main(string[] args)
        {
            // List of blocks, where -1 represents free space, and any other number is the file id number
            List<int> blocks = new List<int>();

            string input = File.ReadAllText("input.txt").Trim();

            for (int i = 0; i < input.Length; i++)
            {
                int fileSize = int.Parse(input[i].ToString());

                if (i % 2 == 0)
                {
                    blocks.AddRange(Enumerable.Repeat(i / 2, fileSize));
                } else
                {
                    blocks.AddRange(Enumerable.Repeat(-1, fileSize));
                }
            }

            // For part one
            List<int> partOneBlocks = blocks.ToList();
            PartOne(partOneBlocks);

            // For part one
            List<int> partTwoBlocks = blocks.ToList();
            PartTwo(partTwoBlocks);

            Console.ReadKey();
        }

        static void PartOne(List<int> blocks)
        {
            List<int> compacted = CompactFragment(blocks);

            long checksum = CalculateChecksum(compacted);

            Console.WriteLine(checksum);
        }

        static void PartTwo(List<int> blocksIn)
        {
            List<int> compacted = CompactWhole(blocksIn);

            long checksum = CalculateChecksum(compacted);

            Console.WriteLine(checksum);
        }

        static List<int> CompactFragment(List<int> blocks)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i] == -1)
                {
                    int lastFileBlockIndex = blocks.Count - 1;

                    while (blocks[lastFileBlockIndex] == -1)
                    {
                        lastFileBlockIndex--;
                    }

                    if (lastFileBlockIndex < i)
                    {
                        break;
                    }

                    blocks[i] = blocks[lastFileBlockIndex];
                    blocks[lastFileBlockIndex] = -1;
                }
            }

            return blocks;
        }

        static List<int> CompactWhole(List<int> blocks)
        {
            int lastFileId = blocks.Where(block => block != -1).Last();

            for (int fileId = lastFileId; fileId >= 0; fileId--)
            {
                int fileStart = blocks.FindIndex(block => block == fileId);
                int fileEnd = blocks.FindLastIndex(block => block == fileId);
                int fileSize = fileEnd - fileStart + 1;

                List<(int, int)> freeSpacePockets = CalculateFreeSpacePockets(blocks);

                List<(int, int)> possiblePockets = freeSpacePockets.Where(p => p.Item2 >= fileSize).ToList();

                if (possiblePockets.Count == 0) continue;

                (int, int) pocket = possiblePockets.First();

                if (pocket.Item1 > fileStart) continue;

                int blocksMoved = 0;

                while (blocksMoved < fileSize)
                {
                    blocks[pocket.Item1 + blocksMoved] = fileId;
                    blocks[fileStart + blocksMoved] = -1;

                    blocksMoved++;
                }
            }

            return blocks;
        }

        static List<(int, int)> CalculateFreeSpacePockets(List<int> blocks)
        {
            List<(int, int)> freeSpacePockets = new List<(int, int)>();

            for (int i = 0; i < blocks.Count; i++)
            {
                int block = blocks[i];

                if (block != -1) continue;

                if (freeSpacePockets.Count == 0)
                {
                    freeSpacePockets.Add((i, 1));
                    continue;
                }

                (int, int) lastPocket = freeSpacePockets.Last();

                if (lastPocket.Item1 + lastPocket.Item2 != i)
                {
                    freeSpacePockets.Add((i, 1));
                    continue;
                }

                lastPocket.Item2 += 1;

                freeSpacePockets.RemoveAt(freeSpacePockets.Count - 1);

                freeSpacePockets.Add(lastPocket);
            }

            return freeSpacePockets;
        }

        static void DisplayBlocks(List<int> blocks)
        {
            Console.WriteLine();
            foreach (int block in blocks) {
                if (block == -1)
                {
                    Console.Write(".");
                } else
                {
                    Console.Write(block);
                }
            }
        }

        static long CalculateChecksum(List<int> blocks)
        {
            long checksum = 0;

            for (int i = 0; i < blocks.Count; i++)
            {

                if (blocks[i] > -1)
                {
                    checksum += (blocks[i] * i);
                }
            }

            return checksum;
        }
    }
}
