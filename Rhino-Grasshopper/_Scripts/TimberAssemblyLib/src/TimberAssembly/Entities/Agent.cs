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
        public Vector3D Dimension { get; set; }

        /// <summary>
        /// Create an agent
        /// </summary>
        /// <param name="name">Name of the agent</param>
        /// <param name="dimension">Dimension of the agent</param>
        /// <param name="trimmed">How many times the agent has been trimmed</param>
        /// <param name="isNew">Is the agent newly introduced</param>
        public Agent(string name = null, Vector3D dimension = null, int trimmed = 0, bool isNew = false)
        {
            Name = name;
            Dimension = dimension;
            Trimmed = trimmed;
            IsNew = isNew;
        }

        /// <summary>
        /// Convert agent to string.
        /// </summary>
        public override string ToString()
        {
            return $"({Name}, {Dimension.ToString()}, Trimmed: {Trimmed}, IsNew: {IsNew})";
        }

        /// <summary>
        /// Get the volume of the agent.
        /// </summary>
        /// <returns>Volume of the agent dimension</returns>
        public double Volume()
        {
            return Dimension.GetVolume();
        }
    }
}
