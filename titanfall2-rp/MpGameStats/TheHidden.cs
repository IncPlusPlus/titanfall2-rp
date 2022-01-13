using Process.NET;

namespace titanfall2_rp.MpGameStats
{
    public class TheHidden : MpStats
    {
        public TheHidden(Titanfall2Api tf2Api, ProcessSharp processSharp) : base(tf2Api, processSharp)
        {
        }

        public override string GetGameState()
        {
            return GetMyTeam() == 2 ? "Hidden and hunting..." : "Fleeing from The Hidden...";
        }
    }
}