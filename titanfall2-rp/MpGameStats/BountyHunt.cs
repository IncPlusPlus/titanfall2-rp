using System;
using Process.NET;

namespace titanfall2_rp.MpGameStats
{
    public class BountyHunt: MpStats
    {
        public BountyHunt(Titanfall2Api tf2Api, ProcessSharp processSharp) : base(tf2Api, processSharp)
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
        /// Get the bonus of the current user.
        /// </summary>
        /// <returns>the user's bonus</returns>
        public int GetMyBonus()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <summary>
        /// Get the kills of the current user.
        /// </summary>
        /// <returns>the user's kills</returns>
        public int GetMyKills()
        {
            throw new NotImplementedException(HelpMeBruh);
        }
    }
}