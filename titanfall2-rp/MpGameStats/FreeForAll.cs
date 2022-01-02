using Process.NET;

namespace titanfall2_rp.MpGameStats
{
    public class FreeForAll : MpStats
    {
        public FreeForAll(Titanfall2Api tf2Api, ProcessSharp processSharp) : base(tf2Api, processSharp)
        {
        }

        /// <summary>
        /// The score of "my team" is actually just the current user's score
        /// </summary>
        /// <returns></returns>
        protected override int GetMyTeamScore()
        {
            return GetScore();
        }

        /// <summary>
        /// The score of the "enemy team" is actually the score of the player with the highest score
        /// </summary>
        /// <returns></returns>
        protected override int GetEnemyTeamScore()
        {
            return GetHighestScoreInGameIgnoringPlayer(GetMyIdOnServer());
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