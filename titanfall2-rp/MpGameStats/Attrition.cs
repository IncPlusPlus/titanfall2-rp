using System;
using Process.NET;

namespace titanfall2_rp.MpGameStats
{
    public class Attrition: MpStats
    {
        public Attrition(Titanfall2Api tf2Api, ProcessSharp processSharp) : base(tf2Api, processSharp)
        { }

        /// <summary>
        /// Get the score of the current user.
        /// </summary>
        /// <returns>the user's score</returns>
        public int GetMyScore()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <summary>
        /// Get the pilot kills of the current user.
        /// </summary>
        /// <returns>the number of pilots the user has killed</returns>
        public int GetMyPilotKills()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <summary>
        /// Get the titan kills of the current user.
        /// </summary>
        /// <returns>the number of titans the user has killed</returns>
        public int GetMyTitanKills()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <summary>
        /// Get the minion kills of the current user.
        /// </summary>
        /// <returns>the number of minions the user has killed</returns>
        public int GetMyMinionKills()
        {
            throw new NotImplementedException(HelpMeBruh);
        }
    }
}