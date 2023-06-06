using System;

namespace TimberAssembly.Entities
{
    public class Agent
    {
        public string Name { get; set; }
        public int Trimmed { get; set; }
        public bool IsNew { get; set; }
        public Dimension Dimension { get; set; }

        public Agent(string name = null, Dimension dimension = null, int trimmed = 0, bool isNew = false)
        {
            Name = name;
            Dimension = dimension;
            Trimmed = trimmed;
            IsNew = isNew;
        }

        public override string ToString()
        {
            return $"({Name}, {Dimension.ToString()})";
        }

        public double Volume()
        {
            return Dimension.GetVolume();
        }
    }
}
