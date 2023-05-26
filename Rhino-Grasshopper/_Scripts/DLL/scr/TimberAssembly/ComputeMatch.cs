using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TimberAssembly.Entities;

namespace TimberAssembly
{
    public class ComputeMatch
    {
        /// <summary>
        /// Check if two agents are exactly matched.
        /// </summary>
        public static bool IsAgentExactMatched(Agent agent1, Agent agent2, double tolerance = 0.1)
        {
            bool matched = Math.Abs(agent1.Dimension.Length - agent2.Dimension.Length) < tolerance &&
                           Math.Abs(agent1.Dimension.Width - agent2.Dimension.Width) < tolerance &&
                           Math.Abs(agent1.Dimension.Height - agent2.Dimension.Height) < tolerance;
            return matched;
        }


        /// <summary>
        /// Check if two agents combined are matched with the target agent.
        /// </summary>
        public static bool IsAgentSecondMatched(Agent target, Agent agent1, Agent agent2, double tolerance = 0.1)
        {
            int differenceCount1 = 0;
            int differenceCount2 = 0;

            if (Math.Abs(agent1.Dimension.Length - target.Dimension.Length) > tolerance)
                differenceCount1++;
            if (Math.Abs(agent1.Dimension.Height - target.Dimension.Height) > tolerance)
                differenceCount1++;
            if (Math.Abs(agent1.Dimension.Width - target.Dimension.Width) > tolerance)
                differenceCount1++;

            if (Math.Abs(agent2.Dimension.Length - target.Dimension.Length) > tolerance)
                differenceCount2++;
            if (Math.Abs(agent2.Dimension.Height - target.Dimension.Height) > tolerance)
                differenceCount2++;
            if (Math.Abs(agent2.Dimension.Width - target.Dimension.Width) > tolerance)
                differenceCount2++;

            // Linear comparison only for now. This means there should be only one dimension difference.
            if (differenceCount1 > 1 || differenceCount2 > 1)
                return false;

            Dimension TargetAgent1Difference = Dimension.GetDifference(target.Dimension, agent1.Dimension);
            Dimension TargetAgent2Difference = Dimension.GetDifference(target.Dimension, agent2.Dimension);

            if (TargetAgent1Difference.IsAnySmallerThan(Dimension.Zero()) ||
                TargetAgent2Difference.IsAnySmallerThan(Dimension.Zero()))
            {
                return false;
            }

            // check if two agents dimension (l,h,w) can be combined to match the target agent.
            int CombDifferentceCount = 0;

            if (Math.Abs(agent1.Dimension.Length + agent2.Dimension.Length - target.Dimension.Length) < tolerance)
            {
                CombDifferentceCount++;
            }

            if (Math.Abs(agent1.Dimension.Height + agent2.Dimension.Height - target.Dimension.Height) < tolerance)
            {
                CombDifferentceCount++;
            }

            if (Math.Abs(agent1.Dimension.Width + agent2.Dimension.Width - target.Dimension.Width) < tolerance)
            {
                CombDifferentceCount++;
            }

            // if any dimension component (l,h,w) is matched, return true.
            if (CombDifferentceCount >= 1)
                return true;

            return false;
        }

        public static List<Agent> CalculateResiduals(Agent target, Agent subject)
        {
            var residuals = new List<Agent>();
            var targetBinsPerm = Processor.Permutations(target.Dimension.ToList());

            var targetBinOpt = targetBinsPerm
                .OrderByDescending(x => x.Min(t => subject.Dimension.ToList()[x.IndexOf(t)] / t))
                .First();

            var tempBin = subject.Dimension.ToList();

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
