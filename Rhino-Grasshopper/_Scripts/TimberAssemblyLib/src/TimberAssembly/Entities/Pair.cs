using System.Collections.Generic;
using System.Linq;

namespace TimberAssembly.Entities
{
    /// <summary>
    /// Pair of target and subjects.
    /// </summary>
    public class Pair
    {
        /// <summary>
        /// Target agent.
        /// </summary>
        public Agent Target { get; set; }

        /// <summary>
        /// Subject agents.
        /// <para>Subjects can contain multiple subjects that combined to the size of target</para>
        /// </summary>
        public List<Agent> Subjects { get; set; }

        /// <summary>
        /// Create a pair of target and subjects.
        /// </summary>
        /// <param name="target">Target agent.</param>
        /// <param name="subjects">Subject agents.
        /// <para>Subjects can contain multiple subjects that combined to the size of target</para></param>
        public Pair(Agent target = null, List<Agent> subjects = null)
        {
            Target = target;
            Subjects = subjects;
        }
    }
}
