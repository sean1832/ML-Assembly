using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TimberAssembly.Entities;

namespace TimberAssembly
{
    /// <summary>
    /// Parse json strings to objects.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Convert a list of json strings to a list of agents.
        /// </summary>
        /// <param name="jsons">Json strings</param>
        /// <returns>Converted list of agents</returns>
        public static List<Agent> DeserializeToAgents(List<string> jsons)
        {
            List<Agent> agents = new List<Agent>();
            foreach (var json in jsons)
            {
                agents.Add(JsonConvert.DeserializeObject<Agent>(json));
            }
            return agents;
        }

        /// <summary>
        /// Convert a list of pairs to a list of json strings.
        /// </summary>
        /// <param name="pairs">List of pairs</param>
        /// <param name="indent">Do you want to indent json string?</param>
        /// <returns>Converted json strings</returns>
        public static List<string> SerializeAgentPairs(List<Pair> pairs, bool indent = false)
        {
            List<string> jsons = new List<string>();

            if (indent)
            {
                foreach (var pair in pairs)
                {
                    jsons.Add(JsonConvert.SerializeObject(pair, Formatting.Indented));
                }
            }
            else
            {
                foreach (var pair in pairs)
                {
                    jsons.Add(JsonConvert.SerializeObject(pair));
                }
            }

            return jsons;
        }

        /// <summary>
        /// Convert remain to json string.
        /// </summary>
        /// <param name="remains">Input remain object</param>
        /// <param name="indent"></param>
        /// <returns></returns>
        public static List<string> SerializeAgentRemains(Remain remains, bool indent = false)
        {
            List<string> jsons = new List<string>();

            if (indent)
            {
                jsons.Add(JsonConvert.SerializeObject(remains, Formatting.Indented));
            }
            else
            {
                jsons.Add(JsonConvert.SerializeObject(remains));
            }

            return jsons;
        }
    }
}
