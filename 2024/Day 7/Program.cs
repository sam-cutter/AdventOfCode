using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day_7
{
    internal class Program
    {
        enum Operator
        {
            Addition,
            Multiplication,
            Concatenation
        }

        static void Main(string[] args)
        {
            (long, long[])[] equations = File
                .ReadAllLines("input.txt")
                .Select(line => line.Split(':'))
                .Select(halves => (long.Parse(halves[0]), halves[1]))
                .Select(halves => (halves.Item1, halves.Item2
                    .Trim()
                    .Split(' ')
                    .Select(long.Parse)
                    .ToArray()
                ))
                .ToArray();

            // For part one
            PartOne(equations);

            // For part two
            PartTwo(equations);

            Console.ReadKey();
        }

        static void PartOne((long, long[])[] equations)
        {
            long total = 0;

            foreach ((long, long[]) equation in equations)
            {
                long constant = equation.Item1;
                long[] terms = equation.Item2;

                int operatorsCount = terms.Length - 1;

                for (int i = 0; i < Math.Pow(2, operatorsCount); i++)
                {
                    Operator[] operators = GetOperators(i, operatorsCount, false);

                    if (Evaluate(terms, operators) == constant)
                    {
                        total += constant;
                        break;
                    }
                }
            }

            Console.WriteLine(total);
        }

        static void PartTwo((long, long[])[] equations)
        {
            long total = 0;

            foreach ((long, long[]) equation in equations)
            {
                long constant = equation.Item1;
                long[] terms = equation.Item2;

                int operatorsCount = terms.Length - 1;

                for (int i = 0; i < Math.Pow(3, operatorsCount); i++)
                {
                    Operator[] operators = GetOperators(i, operatorsCount, true);

                    if (Evaluate(terms, operators) == constant)
                    {
                        total += constant;
                        break;
                    }
                }
            }

            Console.WriteLine(total);
        }

        static Operator[] GetOperators(int i, int operatorsCount, bool partTwo)
        {
            Operator[] operators = new Operator[operatorsCount];

            if (!partTwo)
            {
                for (int j = 0; j < operatorsCount; j++)
                {
                    switch (i & (int)Math.Pow(2, j))
                    {
                        case 0:
                            operators[j] = Operator.Addition;
                            break;
                        default:
                            operators[j] = Operator.Multiplication;
                            break;
                    }
                }
            }
            else
            {
                for (int j = 0; j < operatorsCount; j++)
                {
                    int[] baseThreeDigits = BaseThreeDigits(i, operatorsCount);

                    switch (baseThreeDigits[j])
                    {
                        case 0:
                            operators[j] = Operator.Addition;
                            break;
                        case 1:
                            operators[j] = Operator.Multiplication;
                            break;
                        case 2:
                            operators[j] = Operator.Concatenation;
                            break;
                    }
                }
            }

            return operators;
        }

        static long Evaluate(long[] terms, Operator[] operators)
        {
            long previousValue = Calculate(terms[0], operators[0], terms[1]);

            for (int i = 2; i < terms.Length; i++)
            {
                previousValue = Calculate(previousValue, operators[i - 1], terms[i]);
            }

            return previousValue;
        }

        static long Calculate(long termOne, Operator @operator, long termTwo)
        {
            switch (@operator)
            {
                case Operator.Addition:
                    return termOne + termTwo;
                case Operator.Multiplication:
                    return termOne * termTwo;
                case Operator.Concatenation:
                    return long.Parse($"{termOne}{termTwo}");
                default:
                    throw new Exception();
            }
        }

        static int[] BaseThreeDigits(int x, int operatorsCount)
        {
            int remaining = x;

            int[] digits = new int[operatorsCount];

            for (int i = 0; i < digits.Length; i++)
            {
                for (int j = 2; j >= 0; j--)
                {
                    int value = j * (int)Math.Pow(3, digits.Length - i - 1);

                    if (remaining - value >= 0)
                    {
                        digits[i] = j;
                        remaining -= value;

                        break;
                    }
                }
            }

            return digits;
        }
    }
}
