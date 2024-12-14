using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day_13
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // For part one
            Console.WriteLine(GetTokens(false));

            // For part two
            Console.WriteLine(GetTokens(true));

            Console.ReadKey();
        }

        static long GetTokens(bool partTwo)
        {
            long tokensUsed = 0;

            using (StreamReader reader = new StreamReader(File.OpenRead("input.txt")))
            {
                while (!reader.EndOfStream)
                {
                    string buttonALine = reader.ReadLine();
                    string[] buttonAParts = buttonALine.Split(',');
                    int buttonAx = int.Parse(buttonAParts[0].Substring(buttonAParts[0].IndexOf('+') + 1));
                    int buttonAy = int.Parse(buttonAParts[1].Substring(buttonAParts[1].IndexOf('+') + 1));

                    string buttonBLine = reader.ReadLine();
                    string[] buttonBParts = buttonBLine.Split(',');
                    int buttonBx = int.Parse(buttonBParts[0].Substring(buttonBParts[0].IndexOf('+') + 1));
                    int buttonBy = int.Parse(buttonBParts[1].Substring(buttonBParts[1].IndexOf('+') + 1));

                    string prizeLine = reader.ReadLine();
                    string[] prizeParts = prizeLine.Split(',');
                    long prizex = int.Parse(prizeParts[0].Substring(prizeParts[0].IndexOf('=') + 1));
                    long prizey = int.Parse(prizeParts[1].Substring(prizeParts[1].IndexOf('=') + 1));

                    if (partTwo)
                    {
                        prizex += 10000000000000;
                        prizey += 10000000000000;
                    }

                    reader.ReadLine();

                    decimal[,] augmentedMatrix = new decimal[2, 3];

                    augmentedMatrix[0, 0] = buttonAx; augmentedMatrix[0, 1] = buttonBx; augmentedMatrix[0, 2] = prizex;
                    augmentedMatrix[1, 0] = buttonAy; augmentedMatrix[1, 1] = buttonBy; augmentedMatrix[1, 2] = prizey;

                    Reduce(augmentedMatrix);

                    decimal[] solutions = Solve(augmentedMatrix);

                    decimal A = solutions[0];
                    decimal B = solutions[1];

                    if (A < 0 || B < 0) continue;

                    if (Math.Abs(A - Math.Round(A)) < (decimal)0.001) A = Math.Round(A);
                    if (Math.Abs(B - Math.Round(B)) < (decimal)0.001) B = Math.Round(B);

                    if (A % 1 != 0 || B % 1 != 0) continue;

                    tokensUsed += (3 * (long)A) + (1 * (long)B);
                }
            }

            return tokensUsed;
        }

        static decimal[] Solve(decimal[,] reducedAugmentedMatrix)
        {
            int variablesCount = reducedAugmentedMatrix.GetLength(0);

            decimal[] solutionValues = new decimal[variablesCount];
            solutionValues[variablesCount - 1] = reducedAugmentedMatrix[variablesCount - 1, variablesCount];

            for (int i = variablesCount - 2; i >= 0; i--)
            {
                decimal sum = 0;

                for (int j = i + 1; j < variablesCount; j++)
                {
                    sum += solutionValues[j] * reducedAugmentedMatrix[i, j];
                }


                solutionValues[i] = reducedAugmentedMatrix[i, variablesCount] - sum;
            }

            return solutionValues;
        }

        static decimal[,] Reduce(decimal[,] augmentedMatrix)
        {
            if (IsReduced(augmentedMatrix))
            {
                return augmentedMatrix;
            }

            for (int j = 0; j < augmentedMatrix.GetLength(1) - 1; j++)
            {
                for (int i = augmentedMatrix.GetLength(0) - 1; i > j; i--)
                {
                    decimal[] currentRow = GetRow(augmentedMatrix, i);
                    decimal[] otherRow = GetRow(augmentedMatrix, j);
                    decimal scalar = -currentRow[j] / otherRow[j];

                    decimal[] otherRowMultiplied = MultiplyRowByScalar(otherRow, scalar);

                    decimal[] newCurrentRow = AddRows(currentRow, otherRowMultiplied);

                    augmentedMatrix = ReplaceRow(augmentedMatrix, i, newCurrentRow);

                    if (IsReduced(augmentedMatrix))
                    {
                        return augmentedMatrix;
                    }
                }
            }

            for (int i = 0; i < augmentedMatrix.GetLength(0); i++)
            {
                decimal coefficient = augmentedMatrix[i, i];
                decimal scalar = 1 / coefficient;

                decimal[] currentRow = GetRow(augmentedMatrix, i);
                decimal[] newCurrentRow = MultiplyRowByScalar(currentRow, scalar);

                augmentedMatrix = ReplaceRow(augmentedMatrix, i, newCurrentRow);

                if (IsReduced(augmentedMatrix))
                {
                    return augmentedMatrix;
                }
            }

            return augmentedMatrix;
        }

        static bool IsReduced(decimal[,] augmentedMatrix)
        {
            for (int i = 0; i < augmentedMatrix.GetLength(0); i++)
            {
                if (augmentedMatrix[i, i] != 1)
                {
                    return false;
                }

                for (int j = 0; j < i; j++)
                {
                    if (augmentedMatrix[i, j] != 0)
                    {
                        return false;
                    }
                }
            }

            return true;

        }

        static decimal[] AddRows(decimal[] rowOne, decimal[] rowTwo)
        {
            return rowOne
                .Zip(rowTwo, (rowOneElement, rowTwoElement) => rowOneElement + rowTwoElement)
                .ToArray();
        }

        static decimal[] MultiplyRowByScalar(decimal[] row, decimal scalar)
        {
            return row
                .Select(element => element * scalar)
                .ToArray();
        }

        static decimal[] GetRow(decimal[,] augmentedMatrix, int row)
        {
            return Enumerable
                .Range(0, augmentedMatrix.GetLength(1))
                .Select(j => augmentedMatrix[row, j])
                .ToArray();
        }

        static decimal[,] ReplaceRow(decimal[,] augmentedMatrix, int rowToReplaceIndex, decimal[] row)
        {
            decimal[,] newAugmentedMatrix = augmentedMatrix;

            for (int j = 0; j < augmentedMatrix.GetLength(1); j++)
            {
                newAugmentedMatrix[rowToReplaceIndex, j] = row[j];
            }

            return newAugmentedMatrix;
        }
    }
}
