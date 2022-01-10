using Process.NET;

namespace titanfall2_rp.MpGameStats
{
    public class HideAndSeek : MpStats
    {
        public HideAndSeek(Titanfall2Api tf2Api, ProcessSharp processSharp) : base(tf2Api, processSharp)
        {
        }

        public override string GetGameState()
        {
            return GetMyTeam() == 2 ? "Seeking..." : "Hiding...";
        }
    }
}