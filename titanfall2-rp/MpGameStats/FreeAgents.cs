using Process.NET;

namespace titanfall2_rp.MpGameStats
{
    public class FreeAgents : MpStats
    {
        public FreeAgents(Titanfall2Api tf2Api, ProcessSharp processSharp) : base(tf2Api, processSharp)
        {
        }

        // TODO: Figure out how scores work for this gamemode
    }
}