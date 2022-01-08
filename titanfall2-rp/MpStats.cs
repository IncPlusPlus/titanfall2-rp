using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using log4net;
using Process.NET;
using titanfall2_rp.enums;
using titanfall2_rp.MpGameStats;
using titanfall2_rp.SegmentManager;

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

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private protected readonly ProcessSharp Sharp;

        private protected readonly Titanfall2Api Tf2Api;

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
        /// Gets the health of the specified player. This is a multiplayer-only API.
        /// </summary>
        /// <param name="playerId">the ID of a given player. To find your own ID, call <see cref="GetMyIdOnServer"/></param>
        /// <returns>the specified player's health</returns>
        public int GetPlayerHealth(int playerId)
        {
            return Sharp.Memory.Read<int>(Tf2Api.EngineDllBaseAddress + MpOffsets.Health +
                                          (playerId * MpOffsets.HealthPlayerIdOffset));
        }

        /// <summary>
        /// Get the name of a specific player
        /// </summary>
        /// <param name="playerId">the ID of a given player. To find your own ID, call <see cref="GetMyIdOnServer"/></param>
        /// <returns>the specified player's name or "???" if it could not be found</returns>
        public string GetPlayerName(int playerId)
        {
            var offsets = MpOffsets.NamePointerOffsets;
            // The last of the offsets gets incremented by NamePlayerIdIncrement depending on what the ID is
            offsets[^1] += playerId * MpOffsets.NamePlayerIdIncrement;
            try
            {
                return Sharp.Memory.Read(
                    ProcessApi.ResolvePointerAddress(
                        Sharp,
                        Tf2Api.EngineDllBaseAddress + MpOffsets.Name,
                        offsets),
                    Encoding.UTF8, 100);
            }
            catch (Exception e)
            {
                Log.Warn($"Failed to get the name of player with ID '{playerId}'", e);
                return "???";
            }
        }

        /// <summary>
        /// Get the clan tag of a specific player
        /// </summary>
        /// <param name="playerId">the ID of a given player. To find your own ID, call <see cref="GetMyIdOnServer"/></param>
        /// <returns>the clan tag of the specified player</returns>
        public string GetPlayerClanTag(int playerId)
        {
            var offsets = MpOffsets.NamePointerOffsets;
            // The last of the offsets gets incremented by NamePlayerIdIncrement depending on what the ID is
            offsets[^1] += playerId * MpOffsets.NamePlayerIdIncrement;
            try
            {
                return Sharp.Memory.Read(
                    ProcessApi.ResolvePointerAddress(
                        Sharp,
                        Tf2Api.EngineDllBaseAddress + MpOffsets.Name,
                        offsets) + 0x100,
                    Encoding.UTF8, 100);
            }
            catch (Exception e)
            {
                Log.Warn($"Failed to get the clan tag of player with ID '{playerId}'", e);
                return "???";
            }
        }

        /// <summary>
        /// Get the clan name of a specific player
        /// </summary>
        /// <param name="playerId">the ID of a given player. To find your own ID, call <see cref="GetMyIdOnServer"/></param>
        /// <returns>the clan name of the specified player</returns>
        public string GetPlayerClanName(int playerId)
        {
            var offsets = MpOffsets.NamePointerOffsets;
            // The last of the offsets gets incremented by NamePlayerIdIncrement depending on what the ID is
            offsets[^1] += playerId * MpOffsets.NamePlayerIdIncrement;
            try
            {
                return Sharp.Memory.Read(
                    ProcessApi.ResolvePointerAddress(
                        Sharp,
                        Tf2Api.EngineDllBaseAddress + MpOffsets.Name,
                        offsets) + 0x110,
                    Encoding.UTF8, 100);
            }
            catch (Exception e)
            {
                Log.Warn($"Failed to get the clan name of player with ID '{playerId}'", e);
                return "???";
            }
        }

        public Faction GetCurrentFaction()
        {
            return FactionMethods.GetFaction(
                Sharp.Memory.Read(Tf2Api.EngineDllBaseAddress + 0x7A7383, 1)[0]);
        }

        /// <summary>
        /// Retrieve a string that represents the current state of the match. For all score-based game modes, this will
        /// show the current score. Any game modes that aren't score-based will override this method with a
        /// more appropriate string.
        /// </summary>
        /// <returns>a string representing the current state of the match</returns>
        public virtual string GetGameState()
        {
            return $"Score: {GetMyTeamScore()} - {GetEnemyTeamScore()}";
        }

        protected virtual int GetMyTeamScore()
        {
            return GetTeamScore(GetMyTeam(), true);
        }

        protected virtual int GetEnemyTeamScore()
        {
            return GetTeamScore(GetMyTeam(), false);
        }

        /// <summary>
        /// Get the score of team 1. Whether this is your team or the enemy's doesn't always stay the same.
        /// I'm not sure why. This is something that I need some help figuring out.
        /// </summary>
        /// <returns>the score of team 1, -1 if not applicable to this game mode</returns>
        public int GetTeam1Score()
        {
            return Sharp.Memory.Read<int>(Tf2Api.EngineDllBaseAddress + 0x1121814C);
        }

        /// <summary>
        /// Get the score of team 2. Whether this is your team or the enemy's doesn't always stay the same.
        /// I'm not sure why. This is something that I need some help figuring out.
        /// </summary>
        /// <returns>the score of team 2, -1 if not applicable to this game mode</returns>
        public int GetTeam2Score()
        {
            return Sharp.Memory.Read<int>(Tf2Api.EngineDllBaseAddress + 0x11218CA0);
        }

        /// <summary>
        /// Get the score of a user.
        /// </summary>
        /// <param name="playerId">the id of the player, leave blank to use the current player</param>
        /// <returns>the user's score</returns>
        protected int GetScore(int playerId = -1)
        {
            var id = playerId < 0 ? GetMyIdOnServer() : playerId;
            return Sharp.Memory.Read<int>(Tf2Api.EngineDllBaseAddress + MpOffsets.Score +
                                          (id * MpOffsets.ScoringStatsPlayerIdOffset));
        }

        /// <summary>
        /// Get the highest score in the match, spare the specified player ID. If no player ID is specified,
        /// then no player ID will be skipped and all players are included when finding who has the highest score.
        /// </summary>
        /// <param name="playerIdToIgnore">leave blank to just find the max. Otherwise, specify a player to ignore
        /// in this search</param>
        /// <returns>the score of the highest scoring player in the match</returns>
        protected int GetHighestScoreInGame(int playerIdToIgnore = -1)
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
            var gameMode = titanfall2Api.GetGameMode();
            return gameMode switch
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
                GameMode.ffa => new FreeForAll(titanfall2Api, sharp),
                GameMode.fra => new FreeAgents(titanfall2Api, sharp),
                GameMode.attdm => new TitanBrawl(titanfall2Api, sharp),
                GameMode.turbo_ttdm => new TitanBrawl(titanfall2Api, sharp),
                GameMode.alts => new LastTitanStanding(titanfall2Api, sharp),
                GameMode.turbo_lts => new LastTitanStanding(titanfall2Api, sharp),
                _ => ReportGameModeFailure(gameMode, titanfall2Api, sharp)
            };
        }

        private static MpStats ReportGameModeFailure(GameMode gameMode, Titanfall2Api titanfall2Api, ProcessSharp sharp)
        {
            var e = new ArgumentOutOfRangeException(nameof(gameMode), gameMode, $"Unknown game mode '{gameMode}'.");
            SegmentManager.SegmentManager.TrackEvent(TrackableEvent.GameplayInfoFailure, e);
            return new UnknownGameMode(titanfall2Api, sharp);
        }

        /// <summary>
        /// Get the score for a specific team. The team number argument isn't what you'd typically expect. Instead,
        /// the number is what you get from <see cref="EntityOffsets.LocalPlayer.m_iTeamNum"/>.
        /// </summary>
        /// <param name="teamNumber">The user's team number. This is the INTERNAL team number that the game uses. Not just 1 or 2.</param>
        /// <param name="myTeam">if true, get the specified team's score. If false, get the score of the team that is opposing the specified team</param>
        /// <returns>the score for the specified team</returns>
        private int GetTeamScore(int teamNumber, bool myTeam)
        {
            return (teamNumber, myTeam) switch
            {
                (2, true) => GetTeam1Score(),
                (3, true) => GetTeam2Score(),
                (2, false) => GetTeam2Score(),
                (3, false) => GetTeam1Score(),
                _ => throw new ArgumentOutOfRangeException(nameof(teamNumber), teamNumber, "Unrecognized team number")
            };
        }

        /// <summary>
        /// The user's internal team number. This tends to be 2 or 3.
        /// </summary>
        /// <returns>the internal representation of the current user's team</returns>
        private int GetMyTeam()
        {
            return Sharp.Memory.Read<int>(ProcessApi.ResolvePointerAddress(Sharp,
                                              (Tf2Api.ClientDllBaseAddress + EntityOffsets.LocalPlayerBase)) +
                                          EntityOffsets.LocalPlayer.m_iTeamNum);
        }
    }

    /// <summary>
    /// These offsets represent the base addresses and various offsets for
    /// </summary>
    internal static class EntityOffsets
    {
        /// <summary>
        /// This is the offset to find the pointer for LocalPlayer. The sum of this value + the base address of
        /// client.dll results in the address that stores a pointer to LocalPlayer.
        /// </summary>
        internal const int LocalPlayerBase = 0x28FA260;

        /// <summary>
        /// Offsets that use LocalPlayer as their base. These use the actual address of LocalPlayer. This is not to be
        /// confused with <see cref="EntityOffsets.LocalPlayerBase"/> which directs to the pointer FOR LocalPlayer.
        /// </summary>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static class LocalPlayer
        {
            /// <summary>
            /// The health of the LocalPlayer
            /// </summary>
            public const int m_iHealth = 0x390;

            /// <summary>
            /// LocalPlayer+m_iTeamNum is the address of a pointer. Generally, the value it points to is either 2 or 3.
            /// </summary>
            public const int m_iTeamNum = 0x3a4;

            /// <summary>
            /// The max health of the LocalPlayer
            /// </summary>
            public const int m_iMaxHealth = 0x04A8;
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
        internal const int Health = 0x1123CF64;

        /// <summary>
        /// <see cref="Health"/>
        /// </summary>
        internal const int HealthPlayerIdOffset = 0x4;

        /// <summary>
        /// To be used with engine.dll.
        /// </summary>
        internal const int Name = 0x13fa6eb8;

        /// <summary>
        /// This value (multiplied by the player's ID) is added to the last of the <see cref="NamePointerOffsets"/>
        /// when finding a player's name.
        /// </summary>
        internal const int NamePlayerIdIncrement = 0x58;

        /// <summary>
        /// <see cref="Health"/>
        /// </summary>
        internal const int ScoringStatsPlayerIdOffset = 0x4;

        internal const int Score = 0x1123D510;
        internal const int PilotKills = 0x1123D27C;
        internal const int MinionKills = 0x1123D384;

        /// <summary>
        /// The <see cref="Name"/> address is really the address to a pointer. Applying these offsets to that pointer
        /// points to the address with the actual desired value.
        /// </summary>
        internal static int[] NamePointerOffsets
        {
            // Use a property instead of a field to prevent the array from being modified by my dumb ass
            get { return new[] { 0x18, 0x50, 0x38, 0x38 }; }
        }
    }
}