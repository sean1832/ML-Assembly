using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using TimberAssembly.Entities;

namespace TimberAssembly.Helper
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
        /// Linear check if two agents combined are matched with the target agent.
        /// </summary>
        public static bool IsAgentDoubleMatched(Agent target, Agent agent1, Agent agent2, double tolerance = 0.1)
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
        /// <summary>
        /// Get closest Agent dimension from a list of agents. 
        /// Only accept one dimensional difference
        /// </summary>
        public static (Agent, Dimension) GetClosestAgent(Agent target, List<Agent> subjects)
        {
            double minDistance = double.MaxValue;

            Agent closestAgent = null;

            foreach (Agent subject in subjects)
            {
                foreach (var permutation in Processor.Permutations(subject.Dimension.ToList()))// test all orientation
                {
                    List<double> targetVal = target.Dimension.ToList();
                    double distance = Distance(targetVal, permutation);
                    int differenceCount = DimensionDifferenceCount(targetVal, permutation);
                    if (differenceCount <= 1 && distance < minDistance) // only accept one dimensional difference
                    {
                        minDistance = distance;
                        closestAgent = subject;
                    }
                }
            }

            if (closestAgent == null)
            {
                return (null, null);
            }
            List<double> different = CreateDimensionDifferent(target.Dimension.ToList(), closestAgent.Dimension.ToList(), minDistance);
            return (closestAgent, new Dimension(different));
        }

        private static double Distance(List<double> target, List<double> permutation)
        {
            double totalDistance = 0;
            for (int i = 0; i < target.Count; i++)
            {
                totalDistance += Math.Abs(target[i] - permutation[i]);
            }
            return totalDistance;
        }
        private static int DimensionDifferenceCount(List<double> target, List<double> permutation)
        {
            int diffCount = 0;
            for (int i = 0; i < target.Count; i++)
            {
                if (target[i] != permutation[i])
                {
                    diffCount++;
                }
            }
            return diffCount;
        }
        private static List<double> CreateDimensionDifferent(List<double> target, List<double> closestSubject, double minDifference)
        {
            List<double> result = new List<double>();

            foreach (var targetVal in target)
            {
                if (closestSubject.Contains(targetVal))
                {
                    result.Add(targetVal);
                    closestSubject.Remove(targetVal);
                }
                else
                {
                    result.Add(minDifference);
                }
            }

            return result;
        }


        public static List<Agent> CalculateResiduals(Agent target, Agent subject)
        {
            var residuals = new List<Agent>();

            // Calculate all permutations of the target's dimensions.
            var targetBinsPerm = Processor.Permutations(target.Dimension.ToList());

            // Find the permutation of the target's dimensions which maximizes the minimum ratio of 
            // the subject's dimensions to the target's dimensions. This is to find the orientation 
            // of the target that will allow the subject to fit inside with the least waste.
            var targetBinOpt = targetBinsPerm
                .OrderByDescending(x => x.Min(t => subject.Dimension.ToList()[x.IndexOf(t)] / t))
                .First();

            // Make a temporary copy of the subject's dimensions.
            var tempBin = subject.Dimension.ToList();

            // Iterate over three dimensions (width, height, and depth).
            for (int i = 0; i < 3; i++)
            {
                // If a dimension of the subject exceeds the corresponding dimension of the optimal
                // orientation of the target, calculate the residual in that dimension.
                if (tempBin[i] > targetBinOpt[i])
                {
                    // Reduce the dimension of the subject by the size of the target's corresponding dimension.
                    tempBin[i] -= targetBinOpt[i];
                    residuals.Add(new Agent($"Offcut{i} form {subject.Name}", new Dimension(tempBin[0], tempBin[1], tempBin[2]), 1));
                    tempBin[i] = targetBinOpt[i];
                }
            }

            return residuals;
        }
    }
}
