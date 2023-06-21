using System;
using System.Collections.Generic;
using System.Linq;
using TimberAssembly.Entities;
using TimberAssembly.Helper;

namespace TimberAssembly.Operation
{
    /// <summary>
    /// Matching algorithm for timber assembly.
    /// </summary>
    public class Match
    {
        /// <summary>
        /// Target agents to be matched. Subject agent will be matched to target agents.
        /// </summary>
        public List<Agent> TargetAgents { get; set; }
        /// <summary>
        /// Subject agents to be matched.
        /// </summary>
        public List<Agent> SubjectAgents { get; set; }
        /// <summary>
        /// Tolerance for matching. This number cannot be smaller than the smallest dimension of the agents.
        /// </summary>
        public double Tolerance { get; set; }

        /// <summary>
        /// Create a matching algorithm for timber assembly.
        /// </summary>
        /// <param name="targetAgents">Target agents to be matched. Subject agent will be matched to target agents.</param>
        /// <param name="subjectAgents">Subject agents to be matched.</param>
        /// <param name="tolerance">Tolerance for matching. This number cannot be smaller than the smallest dimension of the agents.</param>
        public Match(List<Agent> targetAgents, List<Agent> subjectAgents, double tolerance = 0.01)
        {
            TargetAgents = targetAgents;
            SubjectAgents = subjectAgents;
            Tolerance = tolerance;
        }

        /// <summary>
        /// Attempts to match each target to a single subject from the remainder based on exact one-to-one matching.
        /// <para>NOTE: 'ExactMatch' is the fastest method, but may not identify all possible matches. If there are complex combinations,
        /// consider using
        /// <see cref="DoubleMatch"/> or <see cref="UniMatch"/> methods after this.</para>
        /// </summary>
        /// <param name="remains">The remaining items to be processed. The method updates this parameter with any unmatched items after processing.</param>
        /// <returns>A list of matched pairs, where each pair consists of a target and a single matching subject.</returns>
        public List<Pair> ExactMatch(ref Remain remains)
        {
            List<Agent> remainTargets = TargetAgents.ToList();
            List<Agent> remainSalvages = SubjectAgents.ToList();

            List<Pair> pairs = new List<Pair>();
            foreach (var target in TargetAgents)
            {
                foreach (var salvage in SubjectAgents)
                {
                    if (!ComputeMatch.IsAgentExactMatched(target, salvage, Tolerance)) continue;

                    Pair pair = new Pair(target, new List<Agent>() { salvage });

                    pairs.Add(pair);

                    remainTargets.Remove(target);
                    remainSalvages.Remove(salvage);
                    break;
                }
            }
            remains.Targets = remainTargets;
            remains.Subjects = remainSalvages;

            return pairs;
        }

        /// <summary>
        /// Pairs a target with two subjects from the remaining items, based on a one-dimensional match.
        /// The method attempts to find two subjects that, when combined, match a target,
        /// thereby reducing the set of remaining items.
        /// <para>NOTE: Matches are determined using a set tolerance level.
        /// Items already matched will not be considered in subsequent iterations.</para>
        /// </summary>
        /// <param name="remains">The remaining items to be processed. The method modifies this parameter, removing matched items after processing.</param>
        /// <returns>A list of matched pairs, each consisting of a target and a list of two matching subjects. If no matches are found, an empty list is returned.</returns>
        public List<Pair> DoubleMatch(ref Remain remains)
        {
            Remain previousRemains = remains;

            var (remainTargets, remainSalvages) = CloneAgents(previousRemains);

            List<Agent> matchedSubjects = new List<Agent>();

            List<Pair> pairs = new List<Pair>();
            foreach (var target in previousRemains.Targets)
            {
                bool isMatched = false;

                for (int i = 0; i < previousRemains.Subjects.Count; i++)
                {
                    if (isMatched) break;

                    var salvage1 = previousRemains.Subjects[i];
                    if (matchedSubjects.Contains(salvage1)) continue;

                    for (int j = i + 1; j < previousRemains.Subjects.Count; j++)
                    {
                        var salvage2 = previousRemains.Subjects[j];
                        if (matchedSubjects.Contains(salvage2)) continue;
                        if (ComputeMatch.IsAgentDoubleMatched(target, salvage1, salvage2, Tolerance))
                        {
                            isMatched = true;

                            Pair pair = new Pair(target, new List<Agent>() { salvage1, salvage2 });

                            pairs.Add(pair);

                            matchedSubjects.Add(salvage1);
                            matchedSubjects.Add(salvage2);

                            remainTargets.Remove(target);
                            remainSalvages.Remove(salvage1);
                            remainSalvages.Remove(salvage2);
                            break;
                        }
                    }
                }
            }
            remains.Targets = remainTargets;
            remains.Subjects = remainSalvages;

            return pairs;
        }

        /// <summary>
        /// Matches targets and subjects from the remainders, combining up to four subjects to match a single target, regardless of orientations. 
        /// This method provides comprehensive results but it may be slow due to its exhaustive nature.
        /// <para>NOTE: This method can be computationally intensive! It is recommended to use it after
        /// <see cref="DoubleMatch"/> and <see cref="ExactMatch"/> methods have been used, to reduce the remaining unprocessed items.</para>
        /// </summary>
        /// <param name="remains">The remaining items to be processed. The method updates this parameter with any unmatched items after processing.</param>
        /// <returns>A list of matched pairs, where each pair consists of a target and its matching subjects.</returns>
        public List<Pair> UniMatch(ref Remain remains)
        {
            // Clone previous remains into two new collections for targets and salvages.
            Remain previousRemains = remains;
            var (remainTargets, remainSalvages) = CloneAgents(previousRemains);
            var (remainTargetsTemp, remainSalvagesTemp) = CloneAgents(previousRemains);

            // Initialize a list to store matched subjects and pairs.
            List<Agent> matchedSubjects = new List<Agent>();
            List<Pair> pairs = new List<Pair>();

            // Iterate over the targets.
            foreach (var target in remainTargetsTemp)
            {
                bool found = false;
                // Iterate over the salvages.
                for (var i = 0; i < remainSalvagesTemp.Count; i++)
                {
                    var salvage = remainSalvagesTemp[i];
                    if (matchedSubjects.Contains(salvage)) continue;

                    // Calculate all possible combinations of target and salvage aggregations.
                    List<List<List<Agent>>> allAggregations = ComputeMatch.CalculateAllAggregation(target, salvage);

                    // Iterate over all aggregations.
                    foreach (var orientations in allAggregations)
                    {
                        // Iterate over each combination in the current aggregation.
                        foreach (var combination in orientations)
                        {
                            var count = 0;
                            List<Agent> matchedAggSubjects = new List<Agent>();
                            // Iterate over each item in the current combination.
                            foreach (var item in combination)
                            {
                                // Check if any of the aggregation matches with other subjects.
                                for (int j = i + 1; j < remainSalvagesTemp.Count; j++)
                                {
                                    var aggregateSubject = remainSalvagesTemp[j];
                                    if (matchedAggSubjects.Contains(aggregateSubject)) continue;
                                    if (matchedSubjects.Contains(aggregateSubject)) continue;

                                    // Generate all permutations of the current aggregate subject dimensions.
                                    var aggregatePerm = Processor.Permutations(aggregateSubject.Dimension.ToList());

                                    // Check if any permutation matches the item's dimension.
                                    if (aggregatePerm.Any(perm => perm.SequenceEqual(item.Dimension.ToList())))
                                    {
                                        count++;
                                        matchedAggSubjects.Add(aggregateSubject);
                                        break;
                                    }
                                }
                            }

                            // If the count equals to the combination element count, it means the subjects are a match.
                            if (count == combination.Count)
                            {
                                found = true;
                                matchedAggSubjects.Add(salvage);

                                // Create a pair with the current target and matched subjects, add it to the pairs list.
                                Pair pair = new Pair(target, matchedAggSubjects);
                                pairs.Add(pair);

                                // Remove the matched elements from the remaining targets and salvages.
                                remainTargets.Remove(target);
                                foreach (var item in matchedAggSubjects)
                                {
                                    remainSalvages.Remove(item);
                                }
                                // Add the matched subjects to the overall matched subjects list.
                                matchedSubjects.AddRange(matchedAggSubjects);
                                break;
                            }
                        }
                        if (found) break;
                    }
                    if (found) break;
                }
            }

            // Update the remains with the current remainTargets and remainSalvages.
            remains.Targets = remainTargets;
            remains.Subjects = remainSalvages;
            // Return the list of pairs that have been matched.
            return pairs;
        }



        /// <summary>
        /// Cut the remainders to the target and create offcuts.
        /// (when target is smaller than subject)
        /// </summary>
        /// <param name="remain">The remaining items to be processed. The method updates this parameter with any unmatched items after processing.</param>
        /// <returns>A list of matched pairs, where each pair consists of a target and its matching subjects.</returns>
        public List<Pair> CutToTarget(ref Remain remain)
        {
            Remain previousRemains = remain;
            var usedSubjects = new HashSet<Agent>();
            var usedTargets = new HashSet<Agent>();

            var (targets, subjects) = CloneAgents(previousRemains);

            if (targets == null || subjects == null)
            {
                return null;
            }

            var resultOffcuts = new List<Agent>();
            var results = new List<Pair>();
            List<Agent> remainTargets = targets.ToList();


            foreach (var target in targets)
            {
                var matchedSubject = FindBestMatch(target, subjects, usedSubjects, usedTargets);

                if (matchedSubject == null) continue;

                var residuals = ComputeMatch.CalculateResiduals(target, matchedSubject);
                resultOffcuts.AddRange(residuals);

                // mutate matched subject
                matchedSubject.Trimmed = residuals.Count;
                matchedSubject.Dimension = target.Dimension;

                results.Add(new Pair(target, new List<Agent> { matchedSubject }));

                usedSubjects.Add(matchedSubject);
                usedTargets.Add(target);
                remainTargets.Remove(target);
            }

            remain.Targets = remainTargets;
            remain.Subjects = resultOffcuts;

            return results;
        }

        /// <summary>
        /// Clone the agents from the previousRemains.
        /// </summary>
        /// <param name="previousRemains"></param>
        /// <returns>(Targets, Subjects)</returns>
        private (List<Agent>, List<Agent>) CloneAgents(Remain previousRemains)
        {
            try
            {
                List<Agent> targets = new List<Agent>(previousRemains.Targets);
                List<Agent> subjects = new List<Agent>(previousRemains.Subjects);
                return (targets, subjects);
            }
            catch (NullReferenceException)
            {
                return (null, null);
            }
        }

        private Agent FindBestMatch(Agent target, List<Agent> subjects, HashSet<Agent> usedSubjects, HashSet<Agent> usedTargets)
        {
            double minDiff = double.MaxValue;
            Agent matchedSubject = null;

            foreach (var subject in subjects)
            {
                if (usedSubjects.Contains(subject) || usedTargets.Contains(target)) continue;

                if (subject.Volume() < target.Volume() || subject.Volume() - target.Volume() >= minDiff) continue;

                minDiff = subject.Volume() - target.Volume();
                matchedSubject = subject;
            }

            return matchedSubject;
        }


        /// <summary>
        /// <para>(DEPRECATED! Use <see cref="ExtendToTarget"/> Instead.) </para>
        /// Match the rest of the targets with the rest of the subjects.
        /// Introduce offcuts if necessary.
        /// </summary>
        /// <param name="remain">The remaining items to be processed. The method updates this parameter with any unmatched items after processing.</param>
        /// <returns>A list of matched pairs, where each pair consists of a target and its matching subjects.</returns>
        public List<Pair> RemainMatch(Remain remain)
        {
            List<Agent> remainTargets;
            List<Agent> remainSalvages;
            try
            {
                remainTargets = remain.Targets.ToList();
                remainSalvages = remain.Subjects.ToList();
            }
            catch (NullReferenceException e)
            {
                return new List<Pair>();
            }

            List<Pair> pairs = new List<Pair>();

            int count = 0;

            // Match each target with a suitable subject
            for (var i = 0; i < remainTargets.Count; i++)
            {
                var target = remainTargets[i];
                Dictionary<Agent, Vector3D> potentialMatches = new Dictionary<Agent, Vector3D>();

                foreach (Agent salvage in remainSalvages)
                {
                    if (salvage.Dimension.IsAnyGreater(target.Dimension)) continue;

                    Vector3D difference = target.Dimension - salvage.Dimension;
                    difference.Absolute();
                    potentialMatches.Add(salvage, difference);
                }

                var sortedMatches = potentialMatches.OrderBy(x => x.Value.X + x.Value.Y + x.Value.Z)
                    .ToList();

                // check if sortedMatches is empty
                if (sortedMatches.Count == 0)
                {
                    continue; // skip this iteration if there are no potential matches
                }

                Agent selectedSalvage = sortedMatches[0].Key;
                Vector3D remainingVector3D = sortedMatches[0].Value;

                // check if all dimensions have value, if not, assign the value from target
                foreach (var prop in remainingVector3D.GetType().GetProperties())
                {
                    if ((double)prop.GetValue(remainingVector3D) != 0) continue;

                    if (prop.Name == "Length")
                    {
                        prop.SetValue(remainingVector3D, target.Dimension.X);
                    }
                    else if (prop.Name == "Width")
                    {
                        prop.SetValue(remainingVector3D, target.Dimension.Y);
                    }
                    else if (prop.Name == "Height")
                    {
                        prop.SetValue(remainingVector3D, target.Dimension.Z);
                    }
                }

                Pair pair = new Pair ()   
                {
                    Target = target,
                    Subjects = new List<Agent>
                    {
                        selectedSalvage,
                        new Agent() 
                        {
                            Name = $"NewTimber{count:D2}",
                            Dimension = remainingVector3D
                        }
                    }
                };
                pairs.Add(pair);

                count++;

                remainSalvages.Remove(selectedSalvage);
            }
            return pairs;
        }

        /// <summary>
        /// Combining remainders with new subjects to match the targets. 
        /// (when target is larger than target)
        /// </summary>
        /// <param name="remain">The remaining items to be processed. The method updates this parameter with any unmatched items after processing.</param>
        /// <returns>A list of matched pairs, where each pair consists of a target and its matching subjects.</returns>
        public List<Pair> ExtendToTarget(ref Remain remain)
        {
            Remain previousRemains = remain;
            List<Agent> preRemainTargets = previousRemains.Targets;
            List<Agent> remainTargets = new List<Agent>(preRemainTargets);

            List<Agent> remainSubjects = previousRemains.Subjects;

            List<Pair> pairs = new List<Pair>();

            for (var i = 0; i < preRemainTargets.Count; i++)
            {
                var target = preRemainTargets[i];
                (Agent closestSubject, Vector3D different) = ComputeMatch.GetClosestAgent(target, remainSubjects);
                if (closestSubject != null)
                {
                    remainTargets.Remove(target);
                    remainSubjects.Remove(closestSubject);

                    Pair pair = new Pair(target, new List<Agent>()
                    {
                        closestSubject,
                        new Agent($"NewTimber{i:D2}", different, 1, true)
                    });
                    pairs.Add(pair);
                }
            }
            remain.Targets = remainTargets;
            remain.Subjects = remainSubjects;

            return pairs;
        }
    }
}
