using System;
using Process.NET;

namespace titanfall2_rp.MpGameStats
{
    public class AmpedHardpoint: MpStats
    {
        public AmpedHardpoint(Titanfall2Api tf2Api, ProcessSharp processSharp) : base(tf2Api, processSharp)
        { }

        /// <summary>
        /// Get the assault score of the current user.
        /// </summary>
        /// <returns>the user's assault score</returns>
        public int GetMyAssaultScore()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <summary>
        /// Get the defense score of the current user.
        /// </summary>
        /// <returns>the user's defense score</returns>
        public int GetMyDefenseScore()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <summary>
        /// Get the kill count of the current user.
        /// </summary>
        /// <returns>the user's kill count</returns>
        public int GetMyKills()
        {
            throw new NotImplementedException(HelpMeBruh);
        }
    }
}