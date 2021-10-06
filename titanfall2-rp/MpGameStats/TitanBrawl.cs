using System;
using Process.NET;

namespace titanfall2_rp.MpGameStats
{
    public class TitanBrawl : MpStats
    {
        public TitanBrawl(Titanfall2Api tf2Api, ProcessSharp processSharp) : base(tf2Api, processSharp)
        { }

        /// <summary>
        /// Gets the round number (out of 5) of the current match.
        ///
        /// Candidates for this value are the following offsets:
        /// client.dll+FB83BC
        /// client.dll+FB83E4
        /// client.dll+FB840C
        ///
        /// If one of these is returning an erroneous result, the offset used by this method might need
        /// to be replaced with another one of these offsets.
        /// </summary>
        /// <returns>the current round number of the match</returns>
        public int GetRoundNumber()
        {
            return Sharp.Memory.Read<int>(Tf2Api.ClientDllBaseAddress + 0xFB83BC);
        }

        /// <summary>
        /// Get the kills of the current user.
        /// </summary>
        /// <returns>the user's number of kills</returns>
        public int GetMyKills()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <summary>
        /// Get the deaths of the current user.
        /// </summary>
        /// <returns>the user's number of deaths</returns>
        public int GetMyDeaths()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <summary>
        /// Get the titan damage of the current user.
        /// </summary>
        /// <returns>the amount of titan damage the current user has inflicted</returns>
        public int GetMyTitanDamage()
        {
            throw new NotImplementedException(HelpMeBruh);
        }
    }
}