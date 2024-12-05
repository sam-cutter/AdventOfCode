using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Xml.Schema;

namespace Day_5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt");

            int breakIndex = lines.ToList().IndexOf("");

            (int, int)[] rules = lines
                .Take(breakIndex)
                .Select(line => line.Split('|'))
                .Select(tokens => tokens.Select(token => int.Parse(token)).ToArray())
                .Select(numbers => (numbers[0], numbers[1]))
                .ToArray();

            int[][] updates = lines
                .Reverse()
                .Take(lines.Length - breakIndex - 1)
                .Select(line => line.Split(','))
                .Select(tokens => tokens.Select(token => int.Parse(token)).ToArray())
                .ToArray();

            // For part one
            PartOne(updates, rules);

            // For part two
            PartTwo(updates, rules);

            Console.ReadKey();
        }

        static void PartOne(int[][] updates, (int, int)[] rules)
        {
            int total = 0;

            foreach (int[] update in updates)
            {
                if (Valid(update, rules))
                {
                    total += update[(update.Length - 1) / 2];
                }
            }

            Console.WriteLine(total);
        }

        static void PartTwo(int[][] updates, (int, int)[] rules)
        {
            int[][] invalidUpdates = updates
                .Where(update => !Valid(update, rules))
                .ToArray();

            int total = 0;

            foreach (int[] update in invalidUpdates)
            {
                int[] fixedUpdate = FixUpdate(update, rules);

                total += fixedUpdate[(update.Length - 1) / 2];
            }

            Console.WriteLine(total);
        }

        static bool Valid(int[] update, (int, int)[] rules)
        {
            foreach ((int, int) rule in rules)
            {
                if (update.Contains(rule.Item1) && update.Contains(rule.Item2))
                {
                    if (update.ToList().IndexOf(rule.Item1) > update.ToList().IndexOf(rule.Item2))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        static int[] FixUpdate(int[] update, (int, int)[] rules)
        {
            List<int> fixedUpdate = new List<int>();

            foreach (int pageNumber in update)
            {
                (int, int)[] relevantRules = rules
                    .Where(rule => (pageNumber == rule.Item1 || pageNumber == rule.Item2) && (update.Contains(rule.Item1) && update.Contains(rule.Item2)))
                    .ToArray();

                for (int insertIndex = 0; insertIndex <= fixedUpdate.Count; insertIndex++)
                {
                    List<int> testUpdate = fixedUpdate.ToList();

                    testUpdate.Insert(insertIndex, pageNumber);

                    if (Valid(testUpdate.ToArray(), relevantRules))
                    {
                        fixedUpdate = testUpdate;

                        break;
                    }
                }
            }

            return fixedUpdate.ToArray();
        }
    }
}
