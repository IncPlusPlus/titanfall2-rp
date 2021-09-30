using System;
using Process.NET;

namespace titanfall2_rp.MpGameStats
{
    public class LastTitanStanding : MpStats
    {
        public LastTitanStanding(Titanfall2Api tf2Api, ProcessSharp processSharp) : base(tf2Api, processSharp)
        {
        }

        /// <returns>the current round number</returns>
        public int GetRoundNumber()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <returns>the total number of rounds (to give the current round number some context)</returns>
        public int GetTotalRounds()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <returns>the time remaining in the current round (measured in seconds)</returns>
        public int GetTimeRemainingInRoundInSeconds()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <returns>the number of titans the user has killed</returns>
        public int GetMyTitanKills()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <returns>the amount of damage the user has done to other titans</returns>
        public int GetMyTitanDamage()
        {
            throw new NotImplementedException(HelpMeBruh);
        }
    }
}