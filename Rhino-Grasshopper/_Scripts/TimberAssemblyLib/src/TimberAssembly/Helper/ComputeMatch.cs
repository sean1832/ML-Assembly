﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using TimberAssembly.Entities;

namespace TimberAssembly.Helper
{
    /// <summary>
    /// Calculate if agents are matched.
    /// </summary>
    public class ComputeMatch
    {
        /// <summary>
        /// Check if two agents are exactly matched.
        /// </summary>
        /// <param name="agent1">Agent 1</param>
        /// <param name="agent2">Agent 2</param>
        /// <param name="tolerance">Tolerance for comparison</param>
        /// <returns>True if matched, false if not</returns>
        public static bool IsAgentExactMatched(Agent agent1, Agent agent2, double tolerance = 0.1)
        {
            bool matched = Math.Abs(agent1.Dimension.X - agent2.Dimension.X) < tolerance &&
                           Math.Abs(agent1.Dimension.Y - agent2.Dimension.Y) < tolerance &&
                           Math.Abs(agent1.Dimension.Z - agent2.Dimension.Z) < tolerance;
            return matched;
        }


        /// <summary>
        /// Linear check if two agents combined are matched with the target agent.
        /// </summary>
        /// <param name="target">Target agent</param>
        /// <param name="agent1">Agent 1</param>
        /// <param name="agent2">Agent 2</param>
        /// <param name="tolerance">Tolerance for comparison</param>
        /// <returns>True if matched, false if not</returns>
        public static bool IsAgentDoubleMatched(Agent target, Agent agent1, Agent agent2, double tolerance = 0.1)
        {
            int differenceCount1 = 0;
            int differenceCount2 = 0;

            if (Math.Abs(agent1.Dimension.X - target.Dimension.X) > tolerance)
                differenceCount1++;
            if (Math.Abs(agent1.Dimension.Z - target.Dimension.Z) > tolerance)
                differenceCount1++;
            if (Math.Abs(agent1.Dimension.Y - target.Dimension.Y) > tolerance)
                differenceCount1++;

            if (Math.Abs(agent2.Dimension.X - target.Dimension.X) > tolerance)
                differenceCount2++;
            if (Math.Abs(agent2.Dimension.Z - target.Dimension.Z) > tolerance)
                differenceCount2++;
            if (Math.Abs(agent2.Dimension.Y - target.Dimension.Y) > tolerance)
                differenceCount2++;

            // Linear comparison only for now. This means there should be only one dimension difference.
            if (differenceCount1 > 1 || differenceCount2 > 1)
                return false;

            Vector3D TargetAgent1Difference = target.Dimension - agent1.Dimension;
            Vector3D TargetAgent2Difference = target.Dimension - agent2.Dimension;

            if (TargetAgent1Difference.IsAnySmaller(Vector3D.Zero()) ||
                TargetAgent2Difference.IsAnySmaller(Vector3D.Zero()))
            {
                return false;
            }

            // check if two agents dimension (l,h,w) can be combined to match the target agent.
            int CombDifferentceCount = 0;

            if (Math.Abs(agent1.Dimension.X + agent2.Dimension.X - target.Dimension.X) < tolerance)
            {
                CombDifferentceCount++;
            }

            if (Math.Abs(agent1.Dimension.Z + agent2.Dimension.Z - target.Dimension.Z) < tolerance)
            {
                CombDifferentceCount++;
            }

            if (Math.Abs(agent1.Dimension.Y + agent2.Dimension.Y - target.Dimension.Y) < tolerance)
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
        /// <para>Only accept one dimensional difference.</para>
        /// </summary>
        /// <param name="target">Target agent</param>
        /// <param name="subjects">List of subject agent</param>
        /// <returns>Closest agent and its dimension</returns>
        public static (Agent, Vector3D) GetClosestAgent(Agent target, List<Agent> subjects)
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
            return (closestAgent, new Vector3D(different));
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

        /// <summary>
        /// Calculate the residuals of the subject agent after fitting it inside the target agent.
        /// <para>When target is smaller than subject.</para>
        /// </summary>
        /// <param name="target">Target agent to fit</param>
        /// <param name="subject">Subject agent to cut</param>
        /// <returns></returns>
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
                    residuals.Add(new Agent($"Offcut{i} form {subject.Name}", new Vector3D(tempBin[0], tempBin[1], tempBin[2]), 1));
                    tempBin[i] = targetBinOpt[i];
                }
            }

            return residuals;
        }

        /// <summary>
        /// calculate all possible aggregation combination of the subject agent to fit inside the target agent.
        /// <para>When target is larger than subject.</para>
        /// </summary>
        /// <param name="target">Target agent to fit</param>
        /// <param name="subject">Subject agent to aggregate</param>
        /// <returns>
        /// All possible aggregation combination.
        /// <para>Any one of combination (Agent List) should be able to add up to target agent.</para>
        /// </returns>
        public static List<List<List<Agent>>> CalculateAllAggregation(Agent target, Agent subject)
        {
            var allAggregations = new List<List<List<Agent>>>();

            // Calculate all permutations of the subject's dimensions.
            var subjectBinsPerm = Processor.Permutations(subject.Dimension.ToList());
            
            // Calculate all permutations of the target's dimensions.
            var targetBinsPerm = Processor.Permutations(target.Dimension.ToList());

            // Iterate over all permutations
            foreach (var subjectBin in subjectBinsPerm)
            {
                var aggregations = new List<List<Agent>>();

                foreach (var targetBin in targetBinsPerm)
                {
                    bool isOversize = false;
                    // Make a temporary copy of the target's dimensions.
                    var tempTargetBin = new List<double>(targetBin);
                    var tempSubjectBin = new List<double>(subjectBin);
                    var combination = new List<Agent>();

                    // Iterate over three dimensions (width, height, and depth).
                    for (int i = 0; i < 3; i++)
                    {
                        if (tempTargetBin[i] < tempSubjectBin[i])
                        {
                            isOversize = true;
                            break;
                        }
                        // If a the subject's combination volume is equal to the target volume, break.
                        double totalVolume = subject.Volume();
                        foreach (var combAgent in combination)
                        {
                            totalVolume += combAgent.Volume();
                        }
                        if (Math.Abs(totalVolume - target.Volume()) < 0.00001)
                        {
                            break;
                        }
                        if (totalVolume > target.Volume())
                        {
                            isOversize = true;
                            break;
                        }
                        // If a dimension of the target exceeds the corresponding dimension of the optimal
                        // orientation of the target, calculate the residual in that dimension.
                        if (tempTargetBin[i] > tempSubjectBin[i])
                        {
                            // Reduce the dimension of the target by the size of the subject's corresponding dimension.
                            tempTargetBin[i] -= tempSubjectBin[i];
                            combination.Add(new Agent(null, new Vector3D(tempTargetBin[0], tempTargetBin[1], tempTargetBin[2]), 1));
                            tempTargetBin[i] = tempSubjectBin[i];
                        }
                        else
                        {
                            // Reduce the dimension of the target by the size of the subject's corresponding dimension.
                            tempTargetBin[i] = tempSubjectBin[i];
                            combination.Add(new Agent(null, new Vector3D(tempTargetBin[0], tempTargetBin[1], tempTargetBin[2]), 1));
                        }
                    }

                    if (isOversize) continue;
                    // valid combination
                    aggregations.Add(combination);
                }

                if (aggregations.Count == 0) continue;
                allAggregations.Add(aggregations);
            }
            return allAggregations;
        }
    }
}
