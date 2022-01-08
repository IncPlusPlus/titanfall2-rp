using Process.NET;

namespace titanfall2_rp.MpGameStats
{
    public class UnknownGameMode : MpStats
    {
        public UnknownGameMode(Titanfall2Api tf2Api, ProcessSharp processSharp) : base(tf2Api, processSharp)
        {
        }

        public override string GetGameState()
        {
            return "In a match";
        }
    }
}