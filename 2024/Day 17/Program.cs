using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime;

namespace Day_17
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] input = File.ReadAllLines("input.txt");
            int breakIndex = Array.IndexOf(input, "");

            int[] registers = input
                .Take(breakIndex)
                .Select(line => line.Split(':')[1])
                .Select(int.Parse)
                .ToArray();

            long REGISTER_A = registers[0];
            long REGISTER_B = registers[1];
            long REGISTER_C = registers[2];

            int[] program = input
                .Skip(breakIndex + 1)
                .First()
                .Split(':')[1]
                .Split(',')
                .Select(int.Parse)
                .ToArray();

            List<long> output = new List<long>();

            output = Execute(program, REGISTER_A, REGISTER_B, REGISTER_C);

            for (int i = 0; i < output.Count; i++)
            {
                Console.Write(output[i]);
                if (i + 1 < output.Count) Console.Write(",");
            }

            Console.ReadKey();
        }

        static List<long> Execute(int[] program, long REGISTER_A, long REGISTER_B, long REGISTER_C)
        {
            int PC = 0;

            List<long> output = new List<long>();

            while (PC < program.Length)
            {
                int opcode = program[PC];
                int operand = program[PC + 1];

                switch (opcode)
                {
                    case 0:
                        REGISTER_A = REGISTER_A / (int)Math.Pow(2, ComboOperandValue(operand, REGISTER_A, REGISTER_B, REGISTER_C));
                        PC += 2;
                        break;
                    case 1:
                        REGISTER_B = REGISTER_B ^ operand;
                        PC += 2;
                        break;
                    case 2:
                        REGISTER_B = ComboOperandValue(operand, REGISTER_A, REGISTER_B, REGISTER_C) % 8;
                        PC += 2;
                        break;
                    case 3:
                        if (REGISTER_A != 0) PC = operand;
                        else PC += 2;
                        break;
                    case 4:
                        REGISTER_B = REGISTER_B ^ REGISTER_C;
                        PC += 2;
                        break;
                    case 5:
                        output.Add(ComboOperandValue(operand, REGISTER_A, REGISTER_B, REGISTER_C) % 8);
                        PC += 2;
                        break;
                    case 6:
                        REGISTER_B = REGISTER_A / (int)Math.Pow(2, ComboOperandValue(operand, REGISTER_A, REGISTER_B, REGISTER_C));
                        PC += 2;
                        break;
                    case 7:
                        REGISTER_C = REGISTER_A / (int)Math.Pow(2, ComboOperandValue(operand, REGISTER_A, REGISTER_B, REGISTER_C));
                        PC += 2;
                        break;
                }
            }

            return output;
        }

        static long ComboOperandValue(int operand, long REGISTER_A, long REGISTER_B, long REGISTER_C)
        {
            switch (operand)
            {
                case 0: return operand;
                case 1: return operand;
                case 2: return operand;
                case 3: return operand;
                case 4: return REGISTER_A;
                case 5: return REGISTER_B;
                case 6: return REGISTER_C;
                default: throw new Exception();
            }
        }
    }
}
