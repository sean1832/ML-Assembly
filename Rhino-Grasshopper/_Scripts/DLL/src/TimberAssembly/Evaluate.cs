using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimberAssembly.Entities;

namespace TimberAssembly
{
    public class Evaluate
    {
        private readonly List<Pair> _pairs;
        private readonly Remain _remains;
        private readonly List<Agent> _totalInitialSubjects;

        public Evaluate(List<Pair> pairs, Remain remains, List<Agent> totalInitialSubjects)
        {
            _pairs = pairs;
            _remains = remains;
            _totalInitialSubjects = totalInitialSubjects;
        }

        public int GetCutCount()
        {
            int cutCount = 0;
            foreach (var pair in _pairs)
            {
                foreach (var subject in pair.Subjects)
                {
                    cutCount += subject.Trimmed;
                }
            }
            return cutCount;
        }

        public double GetUsedSubjectVolume()
        {
            double usedVolume = 0;
            foreach (var pair in _pairs)
            {
                foreach (var subject in pair.Subjects)
                {
                    usedVolume += subject.Dimension.GetVolume();
                }
            }
            return usedVolume;
        }

        public double GetWasteVolume()
        {
            double remainVolume = 0;
            foreach (var remain in _remains.Subjects)
            {
                remainVolume += remain.Dimension.GetVolume();
            }
            return remainVolume;
        }

        public double GetSubjectInitVolume()
        {
            double subjectInitialVolume = 0;
            foreach (var subject in _totalInitialSubjects)
            {
                subjectInitialVolume += subject.Dimension.GetVolume();
            }
            return subjectInitialVolume;
        }

        /// <summary>
        /// Waste Rate = Waste Volume / Total Volume
        /// (Lower the better)
        /// </summary>
        public double GetWasteRateByVolume()
        {
            double totalVolume = GetUsedSubjectVolume();

            double wasteVolume = GetWasteVolume();

            return (totalVolume == 0) ? 0 : wasteVolume / totalVolume; // avoid divide by zero
        }

        /// <summary>
        /// Get total volume of new timbers.
        /// </summary>
        public double GetNewSubjectVolume()
        {
            double newSubjectVolume = 0;
            foreach (var pair in _pairs)
            {
                foreach (var subject in pair.Subjects)
                {
                    if (subject.IsNew)
                    {
                        newSubjectVolume += subject.Dimension.GetVolume();
                    }
                }
            }
            return newSubjectVolume;
        }

        /// <summary>
        /// Get the percentage of used timbers compare to initial timbers by volume.
        /// (Higher the better)
        /// </summary>
        public double GetRecycleRateVolume()
        {

            double totalUsedVolume = GetUsedSubjectVolume();

            double subjectInitialVolume = GetSubjectInitVolume();

            return (subjectInitialVolume == 0) ? 0 : totalUsedVolume / subjectInitialVolume; // avoid divide by zero
        }

        /// <summary>
        /// MaterialEfficiency = 1 - Waste Rate
        /// (Higher the better)
        /// </summary>
        public double EvaluateEfficiencyByVolume()
        {
            return 1 - GetWasteRateByVolume();
        }

        /// <summary>
        /// Labor Efficiency = (Number of timbers - Number of cuts) / Number of timbers
        /// (Higher the better)
        /// </summary>
        public double EvaluateEfficiencyByCutCount()
        {
            int totalSubjectsCount = _totalInitialSubjects.Count;
            int totalCutCount = GetCutCount();
            double efficiency = ((double) totalSubjectsCount - (double)totalCutCount)/(double)totalSubjectsCount;
            return efficiency;
        }


        /// <summary>
        /// Time Efficiency = Perfect Time / Actual Time
        /// (Higher the better)
        /// </summary>
        /// <param name="timePerSubject">Time taken for each timber to be install</param>
        /// <param name="timePerCut">Time taken for each timber to be cut</param>
        public double EvaluateEfficiencyByTime(double timePerSubject, double timePerCut)
        {
            int totalPairedSubjectCount = 0;
            foreach (var pair in _pairs)
            {
                totalPairedSubjectCount += pair.Subjects.Count;
            }

            double perfectTime = timePerSubject * _totalInitialSubjects.Count;
            double actualTime = timePerSubject * totalPairedSubjectCount + timePerCut * GetCutCount();

            double efficiency = perfectTime / actualTime;
            return efficiency;
        }
    }
}
