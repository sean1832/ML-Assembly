namespace TimberAssembly.Entities
{
    public class Agent
    {
        public string Name { get; set; }
        public bool Trimmed { get; set; }
        public Dimension Dimension { get; set; }

        public Agent(string name = null, Dimension dimension = null, bool trimmed = false)
        {
            Name = name;
            Dimension = dimension;
            Trimmed = trimmed;
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
