using Process.NET;

namespace titanfall2_rp.MpGameStats
{
    public class Infection : MpStats
    {
        public Infection(Titanfall2Api tf2Api, ProcessSharp processSharp) : base(tf2Api, processSharp)
        {
        }

        public override string GetGameState()
        {
            return GetMyTeam() == 2 ? "Infected and hunting..." : "Fleeing from infected...";
        }
    }
}