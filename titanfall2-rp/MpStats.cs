using System;
using Process.NET;
using titanfall2_rp.enums;
using titanfall2_rp.MpGameStats;

namespace titanfall2_rp
{
    /// <summary>
    /// This class serves as the base class for all the multiplayer game modes.
    /// </summary>
    public abstract class MpStats
    {
        private protected const string HelpMeBruh =
            "Getting this value is not supported. " +
            "If you want this to be possible, you'll need to contribute this yourself or tell me how the heck to get it.";
        private protected readonly Titanfall2Api Tf2Api;
        private protected readonly ProcessSharp Sharp;

        protected MpStats(Titanfall2Api titanfall2Api, ProcessSharp processSharp)
        {
            Tf2Api = titanfall2Api;
            Sharp = processSharp;
        }

        public Faction GetCurrentFaction()
        {
            return FactionMethods.GetFaction(
                Sharp.Memory.Read(Tf2Api.EngineDllBaseAddress + 0x7A7383, 1)[0]);
        }
        
        /// <summary>
        /// Get the score of team 1. Whether this is your team or the enemy's doesn't always stay the same.
        /// I'm not sure why. This is something that I need some help figuring out.
        /// </summary>
        /// <returns>the score of team 1</returns>
        public virtual int GetTeam1Score()
        {
            return Sharp.Memory.Read<int>(Tf2Api.EngineDllBaseAddress + 0x1121814C);
        }

        /// <summary>
        /// Get the score of team 2. Whether this is your team or the enemy's doesn't always stay the same.
        /// I'm not sure why. This is something that I need some help figuring out.
        /// </summary>
        /// <returns>the score of team 2</returns>
        public virtual int GetTeam2Score()
        {
            return Sharp.Memory.Read<int>(Tf2Api.EngineDllBaseAddress + 0x11218CA0);
        }

        /// <summary>
        /// Get the number of points one team needs to win the match 
        /// </summary>
        /// <returns>the number of points one team needs to win the match, -1 if not applicable (like in frontier defense)</returns>
        // public abstract int MaxScore();

        /// <summary>
        /// Get an instance of the abstract MpStats class.
        /// </summary>
        /// <param name="titanfall2Api">a valid TF|2 API instance</param>
        /// <param name="sharp">a valid ProcessSharp that points to the current TF|2 process</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if the specified game mode isn't valid here</exception>
        /// <exception cref="ArgumentOutOfRangeException">if you screw up big time</exception>
        public static MpStats Of(Titanfall2Api titanfall2Api, ProcessSharp sharp)
        {
            return titanfall2Api.GetGameMode() switch
            {
                GameMode.coliseum => new Coliseum(titanfall2Api, sharp),
                GameMode.aitdm => new Attrition(titanfall2Api, sharp),
                GameMode.tdm => new Skirmish(titanfall2Api, sharp),
                GameMode.cp => new AmpedHardpoint(titanfall2Api, sharp),
                GameMode.at => new BountyHunt(titanfall2Api, sharp),
                GameMode.ctf => new CaptureTheFlag(titanfall2Api, sharp),
                GameMode.lts => new LastTitanStanding(titanfall2Api, sharp),
                GameMode.ps => new PilotsVersusPilots(titanfall2Api, sharp),
                GameMode.speedball => new LiveFire(titanfall2Api, sharp),
                GameMode.mfd => new MarkedForDeath(titanfall2Api, sharp),
                GameMode.ttdm => new TitanBrawl(titanfall2Api, sharp),
                GameMode.fd_easy => new FrontierDefense(titanfall2Api, sharp),
                GameMode.fd_normal => new FrontierDefense(titanfall2Api, sharp),
                GameMode.fd_hard => new FrontierDefense(titanfall2Api, sharp),
                GameMode.fd_master => new FrontierDefense(titanfall2Api, sharp),
                GameMode.fd_insane => new FrontierDefense(titanfall2Api, sharp),
                GameMode.solo => throw new ArgumentException("Tried to get multiplayer details for the campaign"),
                _ => throw new ArgumentOutOfRangeException($"Unknown game mode '{titanfall2Api.GetGameMode()}'.")
            };
        }
    }
}