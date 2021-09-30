using System;
using Process.NET;

namespace titanfall2_rp.MpGameStats
{
    public class LiveFire : MpStats
    {
        public LiveFire(Titanfall2Api tf2Api, ProcessSharp processSharp) : base(tf2Api, processSharp)
        {
        }

        /// <summary>
        /// Get the number of players the user has killed
        /// </summary>
        /// <returns>the number of players the user has killed</returns>
        public int GetMyKills()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <summary>
        /// Get the number of flags the user has secured
        /// </summary>
        /// <returns>the number of flags the user has secured</returns>
        public int GetMyFlags()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <summary>
        /// Get the number of times the user has died
        /// </summary>
        /// <returns>the number of times the user has died</returns>
        public int GetMyDeaths()
        {
            throw new NotImplementedException(HelpMeBruh);
        }
    }
}