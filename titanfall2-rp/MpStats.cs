using System;
using System.Reflection;
using System.Text;
using log4net;
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
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
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
        /// Determine whether the user is searching for match or just sitting in a lobby.
        ///
        /// Some offsets that correspond to this behavior (lots of these may be wrong. This is just the list
        /// I whittled down after a lot of address scanning)
        /// client.dll+B221E1
        /// client.dll+B221E1
        /// client.dll+B221E2
        /// client.dll+B221E5
        /// client.dll+B221EE
        /// client.dll+B221F5
        /// client.dll+B221F5
        /// client.dll+B221F6
        /// client.dll+B221F6
        /// client.dll+B22221
        /// client.dll+B22221
        /// client.dll+B22222
        /// client.dll+B22225
        /// client.dll+B2222E
        /// client.dll+B22235
        /// client.dll+B22235
        /// client.dll+B22236
        /// client.dll+B22236
        /// client.dll+B2223E
        /// client.dll+BFCAA8
        /// client.dll+BFCAA8
        /// client.dll+BFCAA9
        /// client.dll+BFCAA9
        /// client.dll+11FC2C6
        /// client.dll+11FC2CA
        /// client.dll+11FC2CA
        /// client.dll+11FC2D4
        /// client.dll+11FC2D6
        /// client.dll+11FC2D6
        /// client.dll+11FC2DA
        /// client.dll+11FC2E9
        /// client.dll+11FC2E9
        /// client.dll+11FC2E9
        /// client.dll+11FC2EA
        /// client.dll+11FC35C
        /// client.dll+11FC39A
        /// client.dll+11FC3A3
        /// client.dll+11FC3A8
        /// client.dll+11FC7A4
        /// client.dll+11FC7A6
        /// client.dll+11FCBA1
        /// client.dll+11FCBC1
        /// client.dll+11FCBC2
        /// client.dll+11FCBC3
        /// client.dll+11FCBC6
        /// client.dll+11FCBC7
        /// client.dll+11FCBC9
        /// client.dll+11FCBCA
        /// client.dll+11FCBCB
        /// client.dll+11FCBCC
        /// client.dll+11FCBD2
        /// client.dll+11FCBDA
        /// client.dll+11FCBE4
        /// client.dll+11FCBEB
        /// client.dll+11FDBA2
        /// client.dll+11FDBA6
        /// client.dll+11FDBB1
        /// client.dll+11FDBB7
        /// client.dll+11FDBC5
        /// client.dll+11FDBC9
        /// client.dll+11FDBC9
        /// client.dll+11FDBD1
        /// client.dll+11FDBD1
        /// client.dll+11FDBD2
        /// client.dll+11FDBD6
        /// client.dll+1200C32
        /// client.dll+1200C32
        /// client.dll+1200C36
        /// client.dll+1200C36
        /// client.dll+1200C36
        /// client.dll+1200C39
        /// client.dll+1200C42
        /// client.dll+1200C42
        /// client.dll+1200C46
        /// client.dll+1200C49
        /// client.dll+1200C52
        /// client.dll+1200C59
        /// client.dll+1200C61
        /// client.dll+1200C62
        /// client.dll+1200C62
        /// client.dll+1200C62
        /// client.dll+1200C62
        /// client.dll+1200C65
        /// client.dll+1200C65
        /// client.dll+1200C65
        /// client.dll+1200C66
        /// client.dll+1300CE6
        /// client.dll+1300CEA
        /// client.dll+1300CF2
        /// client.dll+1300CF9
        /// client.dll+1300CF9
        /// client.dll+1300CF9
        /// client.dll+1300CFA
        /// client.dll+1300D06
        /// client.dll+1300D13
        /// client.dll+1380CE2
        /// client.dll+1380CE2
        /// client.dll+1380CE6
        /// client.dll+1380CE9
        /// client.dll+1380CF2
        /// client.dll+1380CF2
        /// client.dll+1380D02
        /// client.dll+1380D11
        /// client.dll+1380D12
        /// client.dll+1380D12
        /// client.dll+1380D12
        /// client.dll+1380D12
        /// client.dll+1380D15
        /// client.dll+1380D15
        /// client.dll+1380D62
        /// client.dll+1380D62
        /// client.dll+1380D66
        /// client.dll+1380D69
        /// client.dll+1380D72
        /// client.dll+1380D72
        /// client.dll+1380D82
        /// client.dll+1380D91
        /// client.dll+1380D92
        /// client.dll+1380D92
        /// client.dll+1380D92
        /// client.dll+1380D92
        /// client.dll+1380D95
        /// client.dll+1380D95
        /// client.dll+1380DCB
        /// client.dll+1380DE2
        /// client.dll+1380DE2
        /// client.dll+1380DF1
        /// client.dll+1380DF1
        /// client.dll+1380DF2
        /// client.dll+1380DF6
        /// client.dll+1380DF9
        /// client.dll+1380E02
        /// client.dll+1380E11
        /// client.dll+1380E12
        /// client.dll+1380E12
        /// client.dll+1380E12
        /// client.dll+1380E12
        /// client.dll+1380E15
        /// client.dll+1380E16
        /// client.dll+1380E20
        /// client.dll+1380E22
        /// client.dll+1380E23
        /// client.dll+1380E24
        /// client.dll+1380E25
        /// client.dll+1380E27
        /// client.dll+1380E29
        /// client.dll+1380E2B
        /// client.dll+1380E2C
        /// client.dll+1380E4F
        /// client.dll+1380E50
        /// client.dll+1380E54
        /// client.dll+1380E56
        /// client.dll+1380E62
        /// client.dll+1380E62
        /// client.dll+1380E71
        /// client.dll+1380E72
        /// client.dll+1380E77
        /// client.dll+1380E85
        /// client.dll+1380E91
        /// client.dll+1380E91
        /// client.dll+1380EE5
        /// client.dll+1380EF2
        /// client.dll+1380EFA
        /// client.dll+1E4917C
        /// client.dll+1E4917C
        /// client.dll+1E4917C
        /// client.dll+1E4917C
        /// client.dll+1E4917C
        /// client.dll+1E4917D
        /// client.dll+1E4917D
        /// client.dll+1E4917D
        /// client.dll+1E4917D
        /// client.dll+1E4917D
        /// client.dll+1E4917D
        /// client.dll+1E4917D
        /// client.dll+1E4917D
        /// client.dll+1E4917E
        /// client.dll+1E4917E
        /// client.dll+1E4917E
        /// client.dll+1E4917E
        /// client.dll+1E4917E
        /// client.dll+1E4917E
        /// client.dll+1E4917E
        /// client.dll+1E4917E
        /// client.dll+1E4917F
        /// client.dll+1E4917F
        /// client.dll+1E4917F
        /// client.dll+1E4917F
        /// client.dll+1E4917F
        /// client.dll+1E4917F
        /// client.dll+1E4917F
        /// client.dll+1E4917F
        /// client.dll+1E49194
        /// client.dll+1E511A4
        /// client.dll+1E511B0
        /// client.dll+1E511B0
        /// client.dll+1E61179
        /// client.dll+1E61187
        /// client.dll+1E61193
        /// client.dll+1E61195
        /// client.dll+1E61197
        /// client.dll+1E61199
        /// client.dll+1E6119B
        /// client.dll+1E6119C
        /// client.dll+1E611A1
        /// client.dll+1E611A2
        /// client.dll+1E611A2
        /// client.dll+1E611A6
        /// client.dll+1E611A6
        /// client.dll+1E611B3
        /// client.dll+1E611C2
        /// client.dll+1E611C5
        /// client.dll+1E611C5
        /// client.dll+1E611C6
        /// client.dll+1E611D1
        /// client.dll+1E611D1
        /// client.dll+1E611D2
        /// client.dll+1E611E3
        /// client.dll+1E611E5
        /// client.dll+1E611E5
        /// client.dll+1E611E7
        /// client.dll+1E611EB
        /// client.dll+1E611EC
        /// client.dll+1E61202
        /// client.dll+1E61216
        /// client.dll+1E6121A
        /// client.dll+1E74180
        /// client.dll+1E74180
        /// client.dll+1E74180
        /// client.dll+1E74180
        /// client.dll+1E74180
        /// client.dll+1E74180
        /// client.dll+1E74180
        /// client.dll+1E74180
        /// client.dll+1E74184
        /// client.dll+1E74185
        /// client.dll+1E74185
        /// client.dll+1E74185
        /// client.dll+1E74185
        /// client.dll+1E74185
        /// client.dll+216FD19
        /// client.dll+216FD1A
        /// client.dll+216FD1A
        /// client.dll+216FD1A
        /// client.dll+216FD1A
        /// client.dll+2173D9C
        /// client.dll+2173D9C
        /// client.dll+2173D9D
        /// client.dll+2173D9D
        /// client.dll+2173DA1
        /// client.dll+2173DA1
        /// client.dll+2215102
        /// client.dll+2215102
        /// client.dll+2215114
        /// client.dll+221514E
        /// client.dll+22151C2
        /// client.dll+22151C2
        /// client.dll+22151D4
        /// client.dll+2215200
        /// client.dll+2215200
        /// client.dll+2215201
        /// client.dll+2215201
        /// client.dll+2215204
        /// client.dll+2215204
        /// client.dll+2215204
        /// client.dll+2215205
        /// client.dll+2215206
        /// client.dll+2215206
        /// client.dll+221522C
        /// client.dll+221522C
        /// client.dll+221522C
        /// client.dll+221522D
        /// client.dll+221522E
        /// client.dll+221522E
        /// client.dll+221522E
        /// client.dll+221522E
        /// client.dll+22A2A02
        /// client.dll+22A2A02
        /// client.dll+22A2A14
        /// client.dll+22A2A4E
        /// client.dll+22A2AC2
        /// client.dll+22A2AC2
        /// client.dll+22A2AD4
        /// client.dll+22A2B00
        /// client.dll+22A2B00
        /// client.dll+22A2B01
        /// client.dll+22A2B01
        /// client.dll+22A2B04
        /// client.dll+22A2B04
        /// client.dll+22A2B04
        /// client.dll+22A2B05
        /// client.dll+22A2B06
        /// client.dll+22A2B06
        /// client.dll+22A2B2C
        /// client.dll+22A2B2C
        /// client.dll+22A2B2C
        /// client.dll+22A2B2D
        /// client.dll+22A2B2E
        /// client.dll+22A2B2E
        /// client.dll+22A2B2E
        /// client.dll+22A2B2E
        /// client.dll+22A2B45
        /// client.dll+22A2B45
        /// client.dll+22A2BD1
        /// client.dll+22A2BD1
        /// client.dll+22A2BD2
        /// client.dll+22A2BD5
        /// client.dll+22A2BDE
        /// client.dll+22A2BE5
        /// client.dll+22A2BE5
        /// client.dll+22A2BE6
        /// client.dll+22A2BE6
        /// client.dll+22A2C11
        /// client.dll+22A2C11
        /// client.dll+22A2C12
        /// client.dll+22A2C15
        /// client.dll+22A2C1E
        /// client.dll+22A2C25
        /// client.dll+22A2C25
        /// client.dll+22A2C26
        /// client.dll+22A2C26
        /// client.dll+22A2C2E
        /// client.dll+22A2DDE
        /// client.dll+2E55F96
        /// client.dll+2E55F9E
        /// client.dll+2E55F9E
        /// client.dll+2E55F9E
        /// client.dll+2E55FA6
        /// client.dll+2E55FA6
        /// client.dll+2E55FA9
        /// client.dll+2E55FAA
        /// client.dll+2E55FAA
        /// client.dll+2E55FAA
        /// client.dll+2E55FAD
        /// client.dll+2E55FB1
        /// client.dll+2E55FB1
        /// client.dll+2E55FB2
        /// client.dll+2E55FB5
        /// client.dll+2E55FB5
        /// client.dll+2E55FB6
        /// client.dll+2E55FB9
        /// client.dll+2E55FBA
        /// client.dll+2E55FBD
        /// client.dll+2E55FC0
        /// client.dll+2E55FC1
        /// 
        /// AS WELL AS
        /// 
        /// engine.dll+8A9D68
        /// engine.dll+8A9D68
        /// engine.dll+8A9D69
        /// engine.dll+8A9D69
        /// engine.dll+8A9D69
        /// engine.dll+8A9D69
        /// engine.dll+8A9D6A
        /// engine.dll+8A9D6A
        /// engine.dll+8A9D6A
        /// engine.dll+8A9D6A
        /// engine.dll+8A9D6B
        /// engine.dll+8A9D6B
        /// engine.dll+8A9D6B
        /// engine.dll+8A9D6C
        /// engine.dll+8AA1BA
        /// engine.dll+8AA1BB
        /// engine.dll+8AA1BB
        /// engine.dll+8AA1BB
        /// engine.dll+8AA1BB
        /// engine.dll+8AA1BB
        /// engine.dll+8AA1BB
        /// engine.dll+8AA1BC
        /// engine.dll+12A26810
        /// engine.dll+12A26828
        /// engine.dll+12A2684A
        /// engine.dll+12A26861
        /// engine.dll+12A2686B
        /// engine.dll+12A26872
        /// engine.dll+12A26872
        /// engine.dll+12A26873
        /// engine.dll+12A26873
        /// engine.dll+12A2688A
        /// engine.dll+12A2688B
        /// engine.dll+12A26891
        /// engine.dll+12A26899
        /// engine.dll+12A26899
        /// engine.dll+12A2689A
        /// engine.dll+12A268A1
        /// engine.dll+12A268A2
        /// engine.dll+12A268AA
        /// engine.dll+12A268B0
        /// engine.dll+12A268BA
        /// engine.dll+12A268C1
        /// engine.dll+12A268C2
        /// engine.dll+12A268D2
        /// engine.dll+12A268DB
        /// engine.dll+12A268E1
        /// engine.dll+12A268E3
        /// engine.dll+12A268E9
        /// engine.dll+12A268EA
        /// engine.dll+12A26900
        /// engine.dll+12A26AEC
        /// engine.dll+12A2A690
        /// engine.dll+12A2A6A8
        /// engine.dll+12A2A6CA
        /// engine.dll+12A2A6E1
        /// engine.dll+12A2A6EB
        /// engine.dll+12A2A6F2
        /// engine.dll+12A2A6F2
        /// engine.dll+12A2A6F3
        /// engine.dll+12A2A6F3
        /// engine.dll+12A2A70A
        /// engine.dll+12A2A70B
        /// engine.dll+12A2A711
        /// engine.dll+12A2A719
        /// engine.dll+12A2A719
        /// engine.dll+12A2A71A
        /// engine.dll+12A2A721
        /// engine.dll+12A2A722
        /// engine.dll+12A2A72A
        /// engine.dll+12A2A730
        /// engine.dll+12A2A73A
        /// engine.dll+12A2A741
        /// engine.dll+12A2A742
        /// engine.dll+12A2A752
        /// engine.dll+12A2A75B
        /// engine.dll+12A2A761
        /// engine.dll+12A2A763
        /// engine.dll+12A2A769
        /// engine.dll+12A2A76A
        /// engine.dll+12A2A780
        /// engine.dll+12A2A96C
        /// engine.dll+130D9AFE
        /// engine.dll+130D9AFE
        /// engine.dll+130D9AFE
        /// engine.dll+130D9B00
        /// </summary>
        /// <returns></returns>
        public bool IsSearchingForMatch()
        {
            // TODO: There are duplicates in the address list above. Try to narrow it down.
            return Sharp.Memory.Read(Tf2Api.ClientDllBaseAddress + 0xB221E1, 1)[0] == 1;
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
        /// The <see cref="Name"/> address is really the address to a pointer. Applying these offsets to that pointer
        /// points to the address with the actual desired value.
        /// </summary>
        internal static int[] NamePointerOffsets
        {
            // Use a property instead of a field to prevent the array from being modified by my dumb ass
            get { return new[] { 0x18, 0x50, 0x38, 0x38 }; }
        }

        /// <summary>
        /// This value (multiplied by the player's ID) is added to the last of the <see cref="NamePointerOffsets"/>
        /// when finding a player's name.
        /// </summary>
        internal const int NamePlayerIdIncrement = 0x58;

        public static class Attrition
        {
            internal const int Kills = 0x1123D27C;
            internal const int MinionKills = 0x1123D384;
            internal const int Score = 0x1123D510;
            internal const int AttritionStatsPlayerIdOffset = 0x4;
        }
    }
}