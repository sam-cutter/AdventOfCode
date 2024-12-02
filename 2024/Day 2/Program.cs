using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Day_2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[][] reports = File.ReadAllLines("input.txt")
                .Select(line => line.Trim())
                .Select(line => line.Split(' '))
                .Select(line => line.Select(token => int.Parse(token)).ToArray())
                .ToArray();

            // For part one
            PartOne(reports);

            // For part two
            PartTwo(reports);

            Console.ReadKey();
        }

        static void PartOne(int[][] reports)
        {
            int safeCount = reports
                .Where(report =>
                {
                    return Safe(report);
                }).Count();

            Console.WriteLine(safeCount);
        }

        static void PartTwo(int[][] reports)
        {
            int safeCount = reports
                .Where(report =>
                {
                bool increasing = report[1] > report[0];

                    for (int removals = 0; removals <= 1; removals++) { 

                        if (removals == 0)
                        {
                            if (Safe(report))
                            {
                                return true;
                            }
                        }

                        else
                        {
                            for (int removalIndex = 0; removalIndex < report.Length; removalIndex++)
                            {
                                List<int> tempReport = report.ToList();

                                tempReport.RemoveAt(removalIndex);

                                int[] newReport = tempReport.ToArray();

                                if (Safe(newReport))
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    return false;


                }).Count();

            Console.WriteLine(safeCount);
        }

        static bool Safe(int[] report)
        {

            bool increasing = report[1] > report[0];

            for (int i = 1; i < report.Length; i++)
            {
                int difference = report[i] - report[i - 1];

                if (Math.Abs(difference) >= 1 && Math.Abs(difference) <= 3)
                {
                    if (increasing && difference < 0)
                    {
                        return false;
                    }
                    else if (!increasing && difference > 0)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
