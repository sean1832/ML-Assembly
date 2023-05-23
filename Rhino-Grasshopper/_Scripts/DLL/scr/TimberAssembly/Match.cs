using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimberAssembly.Component;
using Newtonsoft.Json;


namespace TimberAssembly
{
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

        public List<MatchPair> SecondMatch(Remain previousRemains, out Remain remains)
        {
            remains = new Remain();
            HashSet<Agent> remainTargets = new HashSet<Agent>(previousRemains.Targets);
            HashSet<Agent> remainSalvages = new HashSet<Agent>(previousRemains.Subjects);

            List<MatchPair> pairs = new List<MatchPair>();

            // a dictionary to store pairs of salvages that have already been checked with each target
            Dictionary<(Agent, Agent), bool> checkedPairs = new Dictionary<(Agent, Agent), bool>();

            foreach (var target in remainTargets)
            {
                // retrieve the list of salvages for the target from the dictionary
                var salvagePairs = remainSalvages
                    .SelectMany((v, i) => remainSalvages
                        .Skip(i + 1)
                        .Select(w => (v, w)));

                foreach (var salvagePair in salvagePairs)
                {
                    // check if we have already checked this pair with the target
                    if (checkedPairs.TryGetValue((salvagePair.v, salvagePair.w), out bool isMatched))
                    {
                        if (!isMatched) continue;
                        MatchPair pair = new MatchPair
                        {
                            Target = target,
                            Subjects = new List<Agent>() { salvagePair.v, salvagePair.w }
                        };
                        pairs.Add(pair);

                        remainSalvages.Remove(salvagePair.v);
                        remainSalvages.Remove(salvagePair.w);
                        break;
                    }
                    else
                    {
                        // if we have not checked this pair, then we do the check and store the result
                        isMatched = Utilities.IsAgentSecondMatched(target, salvagePair.v, salvagePair.w, Tolerance);
                        checkedPairs[(salvagePair.v, salvagePair.w)] = isMatched;

                        if (!isMatched) continue;
                        MatchPair pair = new MatchPair
                        {
                            Target = target,
                            Subjects = new List<Agent>() { salvagePair.v, salvagePair.w }
                        };
                        pairs.Add(pair);

                        remainSalvages.Remove(salvagePair.v);
                        remainSalvages.Remove(salvagePair.w);
                        break;
                    }
                }
            }

            remains.Targets = remainTargets.ToList();
            remains.Subjects = remainSalvages.ToList();

            return pairs;
        }

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
                Console.WriteLine(e);
                throw new NullReferenceException("Previous Remain is null, please ensure you have the right input.");
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
