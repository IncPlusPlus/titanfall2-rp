using Process.NET;

namespace titanfall2_rp.MpGameStats
{
    public class GunGame : MpStats
    {
        public GunGame(Titanfall2Api tf2Api, ProcessSharp processSharp) : base(tf2Api, processSharp)
        {
        }

        /// <summary>
        /// The score of "my team" is actually just the current user's score
        /// </summary>
        /// <returns>the current user's score</returns>
        protected override int GetMyTeamScore()
        {
            return GetScore();
        }

        /// <summary>
        /// The score of the "enemy team" is actually the score of the player with the highest score that isn't myself
        /// </summary>
        /// <returns>the score of the player with the highest score that isn't myself</returns>
        protected override int GetEnemyTeamScore()
        {
            return GetHighestScoreInGame(GetMyIdOnServer());
        }
    }
}