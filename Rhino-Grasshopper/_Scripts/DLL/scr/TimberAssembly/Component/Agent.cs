using System;
using System.Collections.Generic;

namespace TimberAssembly.Component
{
    public class Agent
    {
        public string Name { get; set; }
        public Dimension Dimension { get; set; }
    }

    //public class Material
    //{
    //    // reference https://www.engineeringtoolbox.com/timber-mechanical-properties-d_1789.html
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public float YoungModulusElasticity { get; set; }
    //}

    public class MatchPair
    {
        public Agent Target { get; set; }
        public List<Agent> Subjects { get; set; }
        public Agent OffcutsAgent { get; set; }
    }


    public class Remain
    {
        public List<Agent> Targets { get; set; }
        public List<Agent> Subjects { get; set; }
    }
}
