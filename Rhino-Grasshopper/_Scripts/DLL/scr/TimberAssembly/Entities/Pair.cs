using System.Collections.Generic;
using System.Linq;

namespace TimberAssembly.Entities
{
    public class Pair
    {
        public Agent Target { get; set; }
        public List<Agent> Subjects { get; set; }

        public Pair(Agent target = null, List<Agent> subjects = null)
        {
            Target = target;
            Subjects = subjects;
        }

        public List<Agent> CalculateResiduals()
        {
            if (Subjects.Count > 1)
                throw new System.NotImplementedException("Only one subject is currently allowed.");

            var residuals = new List<Agent>();
            var targetBinsPerm = Processor.Permutations(Target.Dimension.ToList());

            var targetBinOpt = targetBinsPerm
                .OrderByDescending(x => x.Min(t => Subjects[0].Dimension.ToList()[x.IndexOf(t)] / t))
                .First();

            var tempBin = Subjects[0].Dimension.ToList();

            for (int i = 0; i < 3; i++)
            {
                if (tempBin[i] > targetBinOpt[i])
                {
                    tempBin[i] -= targetBinOpt[i];
                    residuals.Add(new Agent($"Offcut{i}", new Dimension(tempBin[0], tempBin[1], tempBin[2])));
                    tempBin[i] = targetBinOpt[i];
                }
            }

            return residuals;
        }
    }

    
}
