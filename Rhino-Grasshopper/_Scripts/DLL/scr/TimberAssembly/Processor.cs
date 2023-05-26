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
        internal static List<List<double>> Permutations(List<double> list)
        {
            if (list.Count == 1)
                return new List<List<double>> { list };

            var result = new List<List<double>>();
            for (int i = 0; i < list.Count; i++)
            {
                var remaining = new List<double>(list);
                remaining.RemoveAt(i);

                foreach (var perm in Permutations(remaining))
                {
                    perm.Insert(0, list[i]);
                    result.Add(perm);
                }
            }

            return result;
        }

        internal static List<List<double>> Permutations(Dimension dimension)
        {
            return Permutations(new List<double> { dimension.Length, dimension.Width, dimension.Height });
        }
    }
}
