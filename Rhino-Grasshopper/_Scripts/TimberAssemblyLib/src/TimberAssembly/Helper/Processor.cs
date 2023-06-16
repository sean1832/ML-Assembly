using System.Collections.Generic;

namespace TimberAssembly.Helper
{
    internal class Processor
    {
        /// <summary>
        /// Get all permutations of a list of doubles
        /// </summary>
        /// <param name="list">List of doubles</param>
        /// <returns>All permutations of the list</returns>
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
