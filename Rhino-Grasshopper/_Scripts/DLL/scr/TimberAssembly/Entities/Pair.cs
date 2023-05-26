using System.Collections.Generic;
using System.Linq;

namespace TimberAssembly.Entities
{
    public class Pair
    {
        public Agent Target { get; set; }
        public List<Agent> Subjects { get; set; }

        public Pair(Agent target = null, List<Agent> subjects = null)
        {
            Target = target;
            Subjects = subjects;
        }
    }
}
