using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimberAssembly.Component;
using Newtonsoft.Json;

namespace TimberAssembly
{
    public static class Parser
    {
        public static List<Agent> ParseAgents(List<string> jsons)
        {
            List<Agent> agents = new List<Agent>();
            foreach (var json in jsons)
            {
                agents.Add(JsonConvert.DeserializeObject<Agent>(json));
            }
            return agents;
        }
    }
}
