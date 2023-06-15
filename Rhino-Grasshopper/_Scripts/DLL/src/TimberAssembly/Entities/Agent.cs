using System;

namespace TimberAssembly.Entities
{
    /// <summary>
    /// Agent to assemble
    /// </summary>
    public class Agent
    {
        /// <summary>
        /// name of the agent
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// How many times the agent has been trimmed
        /// </summary>
        public int Trimmed { get; set; }
        /// <summary>
        /// Is the agent newly introduced
        /// </summary>
        public bool IsNew { get; set; }
        /// <summary>
        /// Dimension of the agent
        /// </summary>
        public Dimension Dimension { get; set; }

        /// <summary>
        /// Create an agent
        /// </summary>
        /// <param name="name">name of the agent</param>
        /// <param name="dimension">How many times the agent has been trimmed</param>
        /// <param name="trimmed">Is the agent newly introduced</param>
        /// <param name="isNew">Dimension of the agent</param>
        public Agent(string name = null, Dimension dimension = null, int trimmed = 0, bool isNew = false)
        {
            Name = name;
            Dimension = dimension;
            Trimmed = trimmed;
            IsNew = isNew;
        }

        /// <summary>
        /// Convert agent to string
        /// </summary>
        public override string ToString()
        {
            return $"({Name}, {Dimension.ToString()}, Trimmed: {Trimmed}, IsNew: {IsNew})";
        }

        /// <summary>
        /// Get the volume of the agent
        /// </summary>
        public double Volume()
        {
            return Dimension.GetVolume();
        }
    }
}
