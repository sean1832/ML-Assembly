namespace TimberAssembly.Entities
{
    public class Agent
    {
        public string Name { get; set; }
        public Dimension Dimension { get; set; }

        public Agent(string name = null, Dimension dimension = null)
        {
            Name = name;
            Dimension = dimension;
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
