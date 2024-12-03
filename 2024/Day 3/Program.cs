using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day_3
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string inputString = File.ReadAllText("input.txt");

            // For part one
            PartOne(inputString);

            // For part two
            PartTwo(inputString);

            Console.ReadKey();
        }

        static int PerformMultiplication(string mulString)
        {
            mulString = mulString.Substring(4);

            mulString = mulString.Remove(mulString.Length - 1);

            int numberOne = int.Parse(mulString.Split(',')[0]);
            int numberTwo = int.Parse(mulString.Split(',')[1]);

            return numberOne * numberTwo;
        }

        static void PartOne(string inputString)
        {
            string mulPattern = @"mul\([0-9]{1,3},[0-9]{1,3}\)";

            int total = 0;

            foreach (Match mulExpression in Regex.Matches(inputString, mulPattern))
            {
                string mulString = mulExpression.ToString();

                total += PerformMultiplication(mulString);
            }

            Console.WriteLine(total);
        }

        static void PartTwo(string inputString)
        {
            string mulDoDontPattern = @"mul\([0-9]{1,3},[0-9]{1,3}\)|do\(\)|don't\(\)";

            int total = 0;
            bool execute = true;

            foreach (Match expression in Regex.Matches(inputString, mulDoDontPattern))
            {
                string expressionString = expression.ToString();

                switch (expressionString)
                {
                    case "do()":
                        execute = true;
                        break;
                    case "don't()":
                        execute = false;
                        break;
                    default:
                        if (execute)
                        {
                            total += PerformMultiplication(expressionString);
                        }
                        break;
                }
            }

            Console.WriteLine(total);
        }
    }
}
