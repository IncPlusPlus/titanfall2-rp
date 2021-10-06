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

        /// <summary>
        /// When connected to multiplayer, there is always an ID that is assigned to the user which can be used
        /// to identify them on the server. This also applies to lobbies. Any of the array-like structures for keeping
        /// track of scores can typically be navigated by way of the user's ID. Please note that IDs start at 0.
        /// </summary>
        /// <returns>the user's ID on the server</returns>
        public int GetMyIdOnServer()
        {
            return Sharp.Memory.Read<int>(Tf2Api.EngineDllBaseAddress + 0x7A6630);
        }

        /// <summary>
        /// TODO bruh!
        /// </summary>
        /// <returns>the user's health (in multi-player)</returns>
        public int GetPlayerHealth()
        {
            return Sharp.Memory.Read<int>(Tf2Api.EngineDllBaseAddress + 0x1123CF64 + (GetMyIdOnServer() * 0x4));
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
        /// <returns>the score of team 1, -1 if not applicable to this game mode</returns>
        public virtual int GetTeam1Score()
        {
            return Sharp.Memory.Read<int>(Tf2Api.EngineDllBaseAddress + 0x1121814C);
        }

        /// <summary>
        /// Get the score of team 2. Whether this is your team or the enemy's doesn't always stay the same.
        /// I'm not sure why. This is something that I need some help figuring out.
        /// </summary>
        /// <returns>the score of team 2, -1 if not applicable to this game mode</returns>
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

    /// <summary>
    /// Certain pieces of multiplayer data are kept in some kind of sequential structure in memory. The constants in
    /// this class all have one thing in common, the data they can point to exists for all players. The data can be
    /// accessed using the offset here summed with the player's ID times some other constant. The other constant is
    /// the distance between these sequential entries.
    ///
    /// To put this into practice, consider getting the health of the player. To do this, you'd try to read
    /// <see cref="Titanfall2Api.EngineDllBaseAddress"/> + <see cref="Health"/> + X
    /// where X is <see cref="HealthPlayerIdOffset"/> * <see cref="MpStats.GetMyIdOnServer"/>. Breaking this down a
    /// little further, the player ID starts at 0 meaning that if their ID is zero, the original health offset points
    /// to their health. However, if their ID is 5, their offset is 5 increments by <see cref="HealthPlayerIdOffset"/>
    /// away from the original <see cref="Health"/> offset.
    /// </summary>
    internal static class MpOffsets
    {
        /// <summary>
        /// Points to the player's health if their ID is 0. To be used with engine.dll. If their ID isn't 0, this needs
        /// to be offset by <see cref="HealthPlayerIdOffset"/> * their ID.
        /// </summary>
        private const int Health = 0x1123CF64;
        /// <summary>
        /// <see cref="Health"/>
        /// </summary>
        private const int HealthPlayerIdOffset = 0x4;

        /// <summary>
        /// To be used with engine.dll.
        /// </summary>
        private const int Name = 0x13fa6eb8; //TODO Figure out how multiple offsets work

        private const int NamePlayerIdOffset = 0x58;

        static class Attrition
        {
            private const int Kills = 0x1123D27C;
            private const int MinionKills = 0x1123D384;
            private const int Score = 0x1123D510;
            private const int AttritionStatsPlayerIdOffset = 0x4;
        }
    }
}