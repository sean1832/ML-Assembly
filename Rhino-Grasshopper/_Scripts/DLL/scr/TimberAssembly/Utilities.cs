using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimberAssembly.Component;

namespace TimberAssembly
{
    public class Utilities
    {
        public static bool IsAgentExactMatched(Agent agent1, Agent agent2, double tolerance = 0.1)
        {
            bool matched = Math.Abs(agent1.Dimension.Length - agent2.Dimension.Length) < tolerance &&
                           Math.Abs(agent1.Dimension.Width - agent2.Dimension.Width) < tolerance &&
                           Math.Abs(agent1.Dimension.Height - agent2.Dimension.Height) < tolerance;
            return matched;
        }

        public static bool IsAgentSecondMatched(Agent target, Agent agent1, Agent agent2, double tolerance = 0.1)
        {
            int differenceCount1 = 0;
            int differenceCount2 = 0;

            if (Math.Abs(agent1.Dimension.Length - target.Dimension.Length) > tolerance)
                differenceCount1++;
            if (Math.Abs(agent1.Dimension.Height - target.Dimension.Height) > tolerance)
                differenceCount1++;
            if (Math.Abs(agent1.Dimension.Width - target.Dimension.Width) > tolerance)
                differenceCount1++;

            if (Math.Abs(agent2.Dimension.Length - target.Dimension.Length) > tolerance)
                differenceCount2++;
            if (Math.Abs(agent2.Dimension.Height - target.Dimension.Height) > tolerance)
                differenceCount2++;
            if (Math.Abs(agent2.Dimension.Width - target.Dimension.Width) > tolerance)
                differenceCount2++;


            if (differenceCount1 > 1 || differenceCount2 > 1)
                return false;


            if ((Math.Abs(agent1.Dimension.Length + agent2.Dimension.Length - target.Dimension.Length) < tolerance)
                && (Math.Abs(agent1.Dimension.Height + agent2.Dimension.Height - target.Dimension.Height) < tolerance)
                && (Math.Abs(agent1.Dimension.Width + agent2.Dimension.Width - target.Dimension.Width) < tolerance))
            {
                return true;
            }

            else if ((agent1.Dimension.Length + agent2.Dimension.Length >= target.Dimension.Length)
                     && (agent1.Dimension.Height + agent2.Dimension.Height >= target.Dimension.Height)
                     && (agent1.Dimension.Width + agent2.Dimension.Width >= target.Dimension.Width))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
