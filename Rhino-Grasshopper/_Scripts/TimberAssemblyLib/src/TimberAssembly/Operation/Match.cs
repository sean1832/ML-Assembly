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
        /// One subject is exactly matched to one target.
        /// </summary>
        /// <param name="remains">Output remainders.
        /// This contains subject and targets that did not meet the condition of this method.</param>
        /// <returns>Resulted pairs</returns>
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
        /// Two subjects from the remainders are combined to match one target. 1 dimensional matching.
        /// </summary>
        /// <param name="remains">Output remainders</param>
        /// <returns>Resulted pairs</returns>
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
        /// Max four subjects from the remainders are combined to match one target regardless of orientations.
        /// This method is the most robust but also the slowest.
        /// <para>WARNING: This method can be very slow! It is recommended to use after Double Match and Exact Match to reduce timber to match.</para>
        /// </summary>
        /// <param name="remains">Output remainders</param>
        /// <returns>Resulted pairs</returns>
        public List<Pair> UniMatch(ref Remain remains)
        {
            Remain previousRemains = remains;
            var (remainTargets, remainSalvages) = CloneAgents(previousRemains);
            var (remainTargetsTemp, remainSalvagesTemp) = CloneAgents(previousRemains);
            List<Agent> matchedSubjects = new List<Agent>();
            List<Pair> pairs = new List<Pair>();

            foreach (var target in remainTargetsTemp)
            {
                bool found = false;
                for (var i = 0; i < remainSalvagesTemp.Count; i++)
                {
                    var salvage = remainSalvagesTemp[i];
                    if (matchedSubjects.Contains(salvage)) continue;
                    
                    // permutation of all aggregation orientation and trimmed order
                    // to see if any of these matches subjects.
                    List<List<List<Agent>>> allAggregations = ComputeMatch.CalculateAllAggregation(target, salvage);

                    foreach (var orientations in allAggregations)
                    {
                        foreach (var combination in orientations)
                        {
                            var count = 0;
                            List<Agent> matchedAggSubjects = new List<Agent>();
                            foreach (var item in combination)
                            {
                                // check if any of the aggregation matches subjects
                                for (int j = i + 1; j < remainSalvagesTemp.Count; j++)
                                {
                                    var aggregateSubject = remainSalvagesTemp[j];
                                    if (matchedAggSubjects.Contains(aggregateSubject)) continue;
                                    if (matchedSubjects.Contains(aggregateSubject)) continue;
                                    var aggregatePerm = Processor.Permutations(aggregateSubject.Dimension.ToList());

                                    // check if any of the permutation matches the aggregation
                                    if (aggregatePerm.Any(perm => perm.SequenceEqual(item.Dimension.ToList())))
                                    {
                                        count++;
                                        matchedAggSubjects.Add(aggregateSubject);
                                        break;
                                    }
                                }
                            }

                            // if count equals to the combination element count, it means the subjects are a match.
                            if (count == combination.Count)
                            {
                                found = true;
                                matchedAggSubjects.Add(salvage);
                                Pair pair = new Pair(target, matchedAggSubjects);
                                pairs.Add(pair);

                                remainTargets.Remove(target);
                                foreach (var item in matchedAggSubjects)
                                {
                                    remainSalvages.Remove(item);
                                }
                                matchedSubjects.AddRange(matchedAggSubjects);
                                break;
                            }
                        }
                        if (found) break;
                    }
                    if (found) break;
                }
            }
            remains.Targets = remainTargets;
            remains.Subjects = remainSalvages;
            return pairs;
        }


        /// <summary>
        /// Cut the remainders to the target and create offcuts.
        /// (when target is smaller than subject)
        /// </summary>
        /// <param name="remain">Remainders</param>
        /// <returns>Resulted pairs</returns>
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
        /// <para>(DEPRECATED! Use ExtendToTarget Instead.) </para>
        /// Match the rest of the targets with the rest of the subjects.
        /// Introduce offcuts if necessary.
        /// </summary>
        /// <param name="remain">Remainders</param>
        /// <returns>Resulted pairs</returns>
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
        /// <param name="remain">Remainders</param>
        /// <returns>Resulted pairs</returns>
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
