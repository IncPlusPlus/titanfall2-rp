using System;
using Process.NET;

namespace titanfall2_rp.MpGameStats
{
    public class CaptureTheFlag : MpStats
    {
        public CaptureTheFlag(Titanfall2Api tf2Api, ProcessSharp processSharp) : base(tf2Api, processSharp)
        { }

        /// <summary>
        /// Get whether the game has passed halftime
        /// </summary>
        /// <returns>true if in the 2nd half, false if halftime hasn't happened yet</returns>
        public bool IsInSecondHalf()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <summary>
        /// Get the amount of time in seconds remaining in the current half
        /// </summary>
        /// <returns>the time in seconds remaining in the current half</returns>
        public int GetTimeRemainingInHalf()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <summary>
        /// Get the number of captures the user has achieved
        /// </summary>
        /// <returns>the number of captures the user has achieved</returns>
        public int GetMyCaptures()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <summary>
        /// Get the number of times the user returned their team's flag
        /// </summary>
        /// <returns>the number of times the user returned their team's flag</returns>
        public int GetMyReturns()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <summary>
        /// Get the number of kills the user has made
        /// </summary>
        /// <returns>the number of players the user has killed</returns>
        public int GetMyKills()
        {
            throw new NotImplementedException(HelpMeBruh);
        }
    }
}