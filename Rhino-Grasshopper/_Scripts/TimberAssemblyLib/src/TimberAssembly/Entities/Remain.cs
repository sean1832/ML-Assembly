using System.Collections.Generic;

namespace TimberAssembly.Entities
{
    /// <summary>
    /// Store remain of targets and subjects.
    /// <para>These are targets and subjects that's left from the matching operation.</para>
    /// </summary>
    public class Remain
    {
        /// <summary>
        /// Remaining targets.
        /// </summary>
        public List<Agent> Targets { get; set; }
        /// <summary>
        /// Remaining subjects.
        /// </summary>
        public List<Agent> Subjects { get; set; }
    }
}
