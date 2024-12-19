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
            string[] input = File.ReadAllLines("test1.txt");

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

            foreach (string design in designs) if (Possible(design, possibleSubDesigns, impossibleSubDesigns)) possibleDesigns++;

            Console.WriteLine(possibleDesigns);

            Dictionary<char, Dictionary<string, int>> subDesigns = new Dictionary<char, Dictionary<string, int>>();

            foreach (string pattern in patterns.OrderBy(p => p.Length).ToArray())
            {
                int waysOfMaking = WaysOfMaking(pattern, subDesigns, impossibleSubDesigns);

                if (!subDesigns.ContainsKey(pattern.First()))
                {
                    subDesigns[pattern.First()] = new Dictionary<string, int>();
                }

                if (waysOfMaking > 0)
                {
                    subDesigns[pattern.First()].Add(pattern, waysOfMaking);
                } else
                {
                    subDesigns[pattern.First()].Add(pattern, 1);
                }
            }

            Console.WriteLine(subDesigns.Count);

            Console.ReadKey();
        }

        static int WaysOfMaking(string design, Dictionary<char, Dictionary<string, int>> subDesigns, Dictionary<char, List<string>> impossibleSubDesigns)
        {
            int waysOfMaking = 0;

            if (!subDesigns.ContainsKey(design.First())) return 0;
            if (subDesigns[design.First()].ContainsKey(design)) return subDesigns[design.First()][design];
            if (impossibleSubDesigns.ContainsKey(design.First()) && impossibleSubDesigns[design.First()].Contains(design)) return 0;

            foreach (KeyValuePair<string, int> kvp in subDesigns[design.First()])
            {
                if (kvp.Key.Length > design.Length) continue;
                if (design.Substring(0, kvp.Key.Length) != kvp.Key) continue;

                string remainingDesign = design.Substring(kvp.Key.Length);

                int waysOfMakingRemaining = WaysOfMaking(remainingDesign, subDesigns, impossibleSubDesigns);

                waysOfMaking += kvp.Value * waysOfMakingRemaining;
            }

            return waysOfMaking;
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

                bool possible = Possible(remainingDesign, possibleSubDesigns, impossibleSubDesigns);

                if (possible)
                {
                    possibleSubDesigns[design.First()].Add(design);
                    return true;
                }
                if (!possible)
                {
                    if (impossibleSubDesigns.ContainsKey(design.First())) impossibleSubDesigns[design.First()].Add(design);
                    else impossibleSubDesigns[design.First()] = new List<string> { design };
                }
            }

            return false;
        }
    }
}
