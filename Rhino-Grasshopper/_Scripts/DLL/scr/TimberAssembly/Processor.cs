using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimberAssembly.Entities;

namespace TimberAssembly
{
    internal class Processor
    {
        /// <summary>
        /// Get all permutations of a list of doubles
        /// </summary>
        internal static List<List<double>> Permutations(List<double> list)
        {
            // Base case: If the list has only one item, return a list containing just the original list
            if (list.Count == 1)
                return new List<List<double>> { list };

            var result = new List<List<double>>();

            // Iterate over the input list
            for (int i = 0; i < list.Count; i++)
            {
                // Create a copy of the input list and remove the current item
                var remaining = new List<double>(list);
                remaining.RemoveAt(i);

                // Call Permutations recursively on the remaining list (i.e., the input list without the current item)
                foreach (var perm in Permutations(remaining))
                {
                    // Insert the removed item at the beginning of each permutation of the remaining list,
                    // and add the resulting list to the final result
                    perm.Insert(0, list[i]);
                    result.Add(perm);
                }
            }

            return result;
        }
    }
}
