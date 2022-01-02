using System;
using Process.NET;

namespace titanfall2_rp.MpGameStats
{
    public class Attrition : MpStats
    {
        public Attrition(Titanfall2Api tf2Api, ProcessSharp processSharp) : base(tf2Api, processSharp)
        {
        }

        /// <summary>
        /// Get the pilot kills of a user.
        /// </summary>
        /// <param name="playerId">the id of the player, leave blank to use the current player</param>
        /// <returns>the number of pilots the user has killed</returns>
        public int GetPilotKills(int playerId = -1)
        {
            var id = playerId < 0 ? GetMyIdOnServer() : playerId;
            return Sharp.Memory.Read<int>(Tf2Api.EngineDllBaseAddress + MpOffsets.PilotKills +
                                          (id * MpOffsets.ScoringStatsPlayerIdOffset));
        }

        /// <summary>
        /// Get the titan kills of a user.
        /// </summary>
        /// <returns>the number of titans the user has killed</returns>
        public int GetTitanKills()
        {
            throw new NotImplementedException(HelpMeBruh);
        }

        /// <summary>
        /// Get the minion kills of a user.
        /// </summary>
        /// <param name="playerId">the id of the player, leave blank to use the current player</param>
        /// <returns>the number of minions the user has killed</returns>
        public int GetMinionKills(int playerId)
        {
            var id = playerId < 0 ? GetMyIdOnServer() : playerId;
            return Sharp.Memory.Read<int>(Tf2Api.EngineDllBaseAddress + MpOffsets.MinionKills +
                                          (id * MpOffsets.ScoringStatsPlayerIdOffset));
        }
    }
}