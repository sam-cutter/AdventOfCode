using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day_19
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] input = File.ReadAllLines("input.txt");

            string[] patterns = input.First().Split(',').Select(p => p.Trim()).ToArray();

            Dictionary<char, List<string>> possibleSubDesigns = new Dictionary<char, List<string>>();

            foreach (string pattern in patterns)
            {
                if (possibleSubDesigns.ContainsKey(pattern.First())) possibleSubDesigns[pattern.First()].Add(pattern);
                else possibleSubDesigns[pattern.First()] = new List<string> { pattern };
            }

            Dictionary<char, List<string>> impossibleSubDesigns = new Dictionary<char, List<string>>();

            string[] designs = input.Skip(2).ToArray();

            int possibleDesigns = 0;

            foreach (string design in designs)if (Possible(design, possibleSubDesigns, impossibleSubDesigns)) possibleDesigns++;

            Console.WriteLine(possibleDesigns);

            Console.ReadKey();
        }

        static bool Possible(string design, Dictionary<char, List<string>> possibleSubDesigns, Dictionary<char, List<string>> impossibleSubDesigns)
        {
            if (!possibleSubDesigns.ContainsKey(design.First())) return false;
            if (possibleSubDesigns[design.First()].Contains(design)) return true;
            if (impossibleSubDesigns.ContainsKey(design.First()) && impossibleSubDesigns[design.First()].Contains(design)) return false;

            foreach (string pattern in possibleSubDesigns[design.First()])
            {
                if (pattern.Length > design.Length) continue;
                if (design.Substring(0, pattern.Length) != pattern) continue;

                string remainingDesign = design.Substring(pattern.Length);
                if (remainingDesign.Length == 0) return true;

                bool possible = Possible(remainingDesign, possibleSubDesigns, impossibleSubDesigns);

                if (possible)
                {
                    possibleSubDesigns[design.First()].Add(design);
                    return true;
                } if (!possible)
                {
                    if (impossibleSubDesigns.ContainsKey(design.First())) impossibleSubDesigns[design.First()].Add(design);
                    else impossibleSubDesigns[design.First()] = new List<string> { design };
                }
            }

            return false;
        }
    }
}
