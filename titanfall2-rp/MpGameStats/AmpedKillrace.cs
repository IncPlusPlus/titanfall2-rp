using Process.NET;

namespace titanfall2_rp.MpGameStats
{
    public class AmpedKillrace : MpStats
    {
        public AmpedKillrace(Titanfall2Api tf2Api, ProcessSharp processSharp) : base(tf2Api, processSharp)
        {
        }

        // TODO: Implement these
        public override int GetMyTeamScore()
        {
            return -1;
        }

        public override int GetEnemyTeamScore()
        {
            return -1;
        }
    }
}