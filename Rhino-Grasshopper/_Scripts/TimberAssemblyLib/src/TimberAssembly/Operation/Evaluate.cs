using System.Collections.Generic;
using TimberAssembly.Entities;

namespace TimberAssembly.Operation
{
    /// <summary>
    /// Evaluation of the matched results.
    /// </summary>
    public class Evaluate
    {
        private readonly List<Pair> _pairs;
        private readonly Remain _remains;
        private readonly List<Agent> _totalInitialSubjects;

        /// <summary>
        /// Evaluation of the matched results.
        /// </summary>
        /// <param name="pairs">Resulted pairs</param>
        /// <param name="remains">Resulted remains</param>
        /// <param name="totalInitialSubjects">Initial subjects before matching</param>
        public Evaluate(List<Pair> pairs, Remain remains, List<Agent> totalInitialSubjects)
        {
            _pairs = pairs;
            _remains = remains;
            _totalInitialSubjects = totalInitialSubjects;
        }

        /// <summary>
        /// Get the total number of cuts.
        /// </summary>
        /// <returns>Number of cuts</returns>
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

        /// <summary>
        /// Get the total volume of used subjects.
        /// </summary>
        /// <returns>Volume of used subjects</returns>
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

        /// <summary>
        /// Get the total volume that is wasted (not used in the matching process).
        /// </summary>
        /// <returns>Total volume of remained subjects</returns>
        public double GetWasteVolume()
        {
            double remainVolume = 0;
            foreach (var remain in _remains.Subjects)
            {
                remainVolume += remain.Dimension.GetVolume();
            }
            return remainVolume;
        }

        /// <summary>
        /// Get the total volume of initial subjects.
        /// </summary>
        /// <returns>Volume of initial subjects</returns>
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
        /// <returns>Waste Rate</returns>
        public double GetWasteRateByVolume()
        {
            double totalVolume = GetUsedSubjectVolume();

            double wasteVolume = GetWasteVolume();

            return (totalVolume == 0) ? 0 : wasteVolume / totalVolume; // avoid divide by zero
        }

        /// <summary>
        /// Get total volume of new timbers.
        /// </summary>
        /// <returns>Volume of new timbers</returns>
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
        /// <returns>Percentage of used timbers</returns>
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
        /// <returns>Material Efficiency</returns>
        public double EvaluateEfficiencyByVolume()
        {
            return 1 - GetWasteRateByVolume();
        }

        /// <summary>
        /// Labor Efficiency = (Number of timbers - Number of cuts) / Number of timbers
        /// (Higher the better)
        /// </summary>
        /// <returns>Labor Efficiency</returns>
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
        /// <returns>Time Efficiency</returns>
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
