using Process.NET;

namespace titanfall2_rp.MpGameStats
{
    public class FreeForAll : MpStats
    {
        public FreeForAll(Titanfall2Api tf2Api, ProcessSharp processSharp) : base(tf2Api, processSharp)
        {
        }

        /// <summary>
        /// The score of "team 1" is actually just the current user's score
        /// </summary>
        /// <returns></returns>
        public override int GetTeam1Score()
        {
            return GetScore(GetMyIdOnServer());
        }

        /// <summary>
        /// The score of "team 2" is actually the score of the player with the highest score
        /// </summary>
        /// <returns></returns>
        public override int GetTeam2Score()
        {
            return GetHighestScoreInGameIgnoringPlayer(GetMyIdOnServer());
        }

        public override int GetMyTeamScore()
        {
            return GetTeam1Score();
        }

        public override int GetEnemyTeamScore()
        {
            return GetTeam2Score();
        }

        /// <summary>
        /// Get the score of a user.
        /// </summary>
        /// <param name="playerId">the id of the player, leave blank to use the current player</param>
        /// <returns>the user's score</returns>
        private int GetScore(int playerId = -1)
        {
            var id = playerId < 0 ? GetMyIdOnServer() : playerId;
            return Sharp.Memory.Read<int>(Tf2Api.EngineDllBaseAddress + MpOffsets.FreeForAll.Score +
                                          (id * MpOffsets.FreeForAll.AttritionStatsPlayerIdOffset));
        }

        private int GetHighestScoreInGameIgnoringPlayer(int playerIdToIgnore = -1)
        {
            var currentHighest = int.MinValue;
            // Loop through all the score slots
            for (var i = 0; i < 64; i++)
            {
                // Skip the player whose ID we were instructed to ignore
                if (i == playerIdToIgnore)
                {
                    continue;
                }

                var playerScore = GetScore(i);
                // If the known highest score turns out to be lower than this player's score...
                if (playerScore > currentHighest)
                {
                    // Set the known highest to be that new highest score
                    currentHighest = playerScore;
                }
            }

            return currentHighest;
        }
    }
}