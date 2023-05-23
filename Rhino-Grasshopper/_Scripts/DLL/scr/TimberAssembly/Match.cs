using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimberAssembly.Component;
using Newtonsoft.Json;


namespace TimberAssembly
{
    /// <summary>
    /// Matching algorithm for timber assembly.
    /// </summary>
    public class Match
    {
        public List<Agent> TargetAgents { get; set; }
        public List<Agent> SalvageAgents { get; set; }
        public double Tolerance { get; set; }

        public Match(List<Agent> targetAgents, List<Agent> salvageAgents, double tolerance)
        {
            TargetAgents = targetAgents;
            SalvageAgents = salvageAgents;
            Tolerance = tolerance;
        }

        /// <summary>
        /// One subject is exactly matched to one target.
        /// </summary>
        /// <param name="remains">Output remainders</param>
        public List<MatchPair> ExactMatch(out Remain remains)
        {
            remains = new Remain();
            List<Agent> remainTargets = TargetAgents.ToList();
            List<Agent> remainSalvages = SalvageAgents.ToList();

            List<MatchPair> pairs = new List<MatchPair>();
            foreach (var target in TargetAgents)
            {
                foreach (var salvage in SalvageAgents)
                {
                    if (!Utilities.IsAgentExactMatched(target, salvage, Tolerance)) continue;

                    MatchPair pair  = new MatchPair
                    {
                        Target = target,
                        Subjects = new List<Agent>(){salvage}
                    };

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
        /// WARNING: This is a slow method!
        /// Two subjects from the remainders of ExactMatch are combined to match one target.
        /// </summary>
        /// <param name="previousRemains">Remainders from ExactMatch</param>
        /// <param name="remains">Output remainders</param>
        public List<MatchPair> SecondMatchSlow(Remain previousRemains, out Remain remains)
        {
            remains = new Remain();
            List<Agent> remainTargets = previousRemains.Targets.ToList();
            List<Agent> remainSalvages = previousRemains.Subjects.ToList();

            List<Agent> matchedSubjects = new List<Agent>();

            List<MatchPair> pairs = new List<MatchPair>();
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

                        if (Utilities.IsAgentSecondMatched(target, salvage1, salvage2, Tolerance))
                        {
                            isMatched = true;

                            MatchPair pair = new MatchPair
                            {
                                Target = target,
                                Subjects = new List<Agent>() { salvage1, salvage2 }
                            };

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
        /// Two subjects from the remainder of ExactMatch are combined to match one target.
        /// </summary>
        /// <param name="previousRemains">Remainder from ExactMatch</param>
        /// <param name="remains">Output remainder</param>
        public List<MatchPair> SecondMatchFast(Remain previousRemains, out Remain remains)
        {
            remains = new Remain();
            List<Agent> remainTargets = previousRemains.Targets.ToList();
            List<Agent> remainSalvages = previousRemains.Subjects.ToList();

            Dictionary<(Agent target, Agent firstAgent), Agent> pairDict = new Dictionary<(Agent target, Agent firstAgent), Agent>();

            // Loop over targets and the first agents to find pairs that match the criteria
            foreach (var target in remainTargets)
            {
                for (int i = 0; i < remainSalvages.Count; i++)
                {
                    var salvage1 = remainSalvages[i];
                    for (int j = i + 1; j < remainSalvages.Count; j++)
                    {
                        var salvage2 = remainSalvages[j];
                        if (Utilities.IsAgentSecondMatched(target, salvage1, salvage2, Tolerance))
                        {
                            pairDict[(target, salvage1)] = salvage2;
                            break;
                        }
                    }
                }
            }

            HashSet<Agent> matchedSubjects = new HashSet<Agent>();
            List<MatchPair> pairs = new List<MatchPair>();

            // Loop over the dictionary to create the pairs and remove the matched agents
            foreach (var pair in pairDict)
            {
                MatchPair newPair = new MatchPair
                {
                    Target = pair.Key.target,
                    Subjects = new List<Agent> { pair.Key.firstAgent, pair.Value }
                };
                pairs.Add(newPair);

                matchedSubjects.Add(pair.Key.firstAgent);
                matchedSubjects.Add(pair.Value);

                remainTargets.Remove(pair.Key.target);
                remainSalvages.Remove(pair.Key.firstAgent);
                remainSalvages.Remove(pair.Value);
            }

            remains.Targets = remainTargets;
            remains.Subjects = remainSalvages;

            return pairs;
        }

        /// <summary>
        /// Match the rest of the targets with the rest of the subjects.
        /// Introduce offcuts if necessary.
        /// </summary>
        /// <param name="previousRemains">Remainder from SecondMatch</param>
        public List<MatchPair> RemainMatch(Remain previousRemains)
        {
            List<Agent> remainTargets;
            List<Agent> remainSalvages;
            try
            {
                remainTargets = previousRemains.Targets.ToList();
                remainSalvages = previousRemains.Subjects.ToList();
            }
            catch (Exception e)
            {
                return null;
            }
            
            List<MatchPair> pairs = new List<MatchPair>();

            // Match each target with a suitable subject
            foreach (Agent target in remainTargets)
            {
                Dictionary<Agent, Dimension> potentialMatches = new Dictionary<Agent, Dimension>();

                foreach (Agent salvage in remainSalvages)
                {

                    Dimension difference = Dimension.Subtract(target.Dimension, salvage.Dimension);
                    difference = Dimension.Absolute(difference);
                    potentialMatches.Add(salvage, difference);
                }

                var sortedMatches = potentialMatches.OrderBy(x => x.Value.Length + x.Value.Width + x.Value.Height).ToList();

                Agent selectedSalvage = sortedMatches[0].Key;
                Dimension remainingDimension = sortedMatches[0].Value;

                MatchPair matchPair = new MatchPair 
                { 
                    Target = target,
                    Subjects = new List<Agent> { selectedSalvage },
                    OffcutsAgent = new Agent(){Dimension = remainingDimension}
                };
                pairs.Add(matchPair);

                remainSalvages.Remove(selectedSalvage);
            }
            return pairs;
        }

    }
}
