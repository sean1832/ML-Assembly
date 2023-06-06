﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TimberAssembly.Entities;

namespace TimberAssembly
{
    public static class Parser
    {
        /// <summary>
        /// Parse a list of json strings to a list of agents.
        /// </summary>
        public static List<Agent> DeserializeToAgents(List<string> jsons)
        {
            List<Agent> agents = new List<Agent>();
            foreach (var json in jsons)
            {
                agents.Add(JsonConvert.DeserializeObject<Agent>(json));
            }
            return agents;
        }

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
