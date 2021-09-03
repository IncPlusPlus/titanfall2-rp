using System;

namespace titanfall2_rp
{
    public partial class Titanfall2Api
    {
        private const string HelpMeBruh =
            "Getting this value is not supported. " +
            "If you want this to be possible, you'll need to contribute this yourself or tell me how the heck to get it.";

        /// <summary>
        /// Inside this class are multiplayer subclasses for each game mode (because the stats are different
        /// based on the game type. They'll also be in different memory locations).
        /// </summary>
        public class MpGameStats
        {
            private Coliseum _coliseum;
            private Attrition _attrition;
            private Skirmish _skirmish;
            private AmpedHardpoint _ampedHardpoint;
            private BountyHunt _bountyHunt;
            private CaptureTheFlag _captureTheFlag;
            private LastTitanStanding _lastTitanStanding;
            private PilotsVersusPilots _pilotsVersusPilots;
            private LiveFire _liveFire;
            private MarkedForDeath _markedForDeath;
            private TitanBrawl _titanBrawl;
            private FrontierDefense _frontierDefense;

            public MpGameStats(Titanfall2Api titanfall2Api)
            {
                _coliseum = new Coliseum(titanfall2Api);
                _attrition = new Attrition(titanfall2Api);
                _skirmish = new Skirmish(titanfall2Api);
                _ampedHardpoint = new AmpedHardpoint(titanfall2Api);
                _bountyHunt = new BountyHunt(titanfall2Api);
                _captureTheFlag = new CaptureTheFlag(titanfall2Api);
                _lastTitanStanding = new LastTitanStanding(titanfall2Api);
                _pilotsVersusPilots = new PilotsVersusPilots(titanfall2Api);
                _liveFire = new LiveFire(titanfall2Api);
                _markedForDeath = new MarkedForDeath(titanfall2Api);
                _titanBrawl = new TitanBrawl(titanfall2Api);
                _frontierDefense = new FrontierDefense(titanfall2Api);
            }

            public class Coliseum
            {
                private readonly Titanfall2Api _tf2Api;

                public Coliseum(Titanfall2Api tf2Api)
                {
                    this._tf2Api = tf2Api;
                }
            }

            public class Attrition
            {
                private readonly Titanfall2Api _tf2Api;

                public Attrition(Titanfall2Api tf2Api)
                {
                    this._tf2Api = tf2Api;
                }

                /// <summary>
                /// Get the score of team 1. Whether this is your team or the enemy's doesn't always stay the same.
                /// I'm not sure why. This is something that I need some help figuring out.
                /// </summary>
                /// <returns>the score of team 1</returns>
                public int GetTeam1Score()
                {
                    return _tf2Api._sharp!.Memory.Read<int>(_tf2Api._engineDllBaseAddress + 0x1121814C);
                }

                /// <summary>
                /// Get the score of team 2. Whether this is your team or the enemy's doesn't always stay the same.
                /// I'm not sure why. This is something that I need some help figuring out.
                /// </summary>
                /// <returns>the score of team 2</returns>
                public int GetTeam2Score()
                {
                    return _tf2Api._sharp!.Memory.Read<int>(_tf2Api._engineDllBaseAddress + 0x11218CA0);
                }

                /// <summary>
                /// Get the score of the current user.
                /// </summary>
                /// <returns>the user's score</returns>
                public int GetMyScore()
                {
                    throw new NotImplementedException(HelpMeBruh);
                }

                /// <summary>
                /// Get the pilot kills of the current user.
                /// </summary>
                /// <returns>the number of pilots the user has killed</returns>
                public int GetMyPilotKills()
                {
                    throw new NotImplementedException(HelpMeBruh);
                }

                /// <summary>
                /// Get the titan kills of the current user.
                /// </summary>
                /// <returns>the number of titans the user has killed</returns>
                public int GetMyTitanKills()
                {
                    throw new NotImplementedException(HelpMeBruh);
                }

                /// <summary>
                /// Get the minion kills of the current user.
                /// </summary>
                /// <returns>the number of minions the user has killed</returns>
                public int GetMyMinionKills()
                {
                    throw new NotImplementedException(HelpMeBruh);
                }
            }

            public class Skirmish
            {
                private readonly Titanfall2Api _tf2Api;

                public Skirmish(Titanfall2Api tf2Api)
                {
                    this._tf2Api = tf2Api;
                }
            }

            public class AmpedHardpoint
            {
                private readonly Titanfall2Api _tf2Api;

                public AmpedHardpoint(Titanfall2Api tf2Api)
                {
                    this._tf2Api = tf2Api;
                }
            }

            public class BountyHunt
            {
                private readonly Titanfall2Api _tf2Api;

                public BountyHunt(Titanfall2Api tf2Api)
                {
                    this._tf2Api = tf2Api;
                }
            }

            public class CaptureTheFlag
            {
                private readonly Titanfall2Api _tf2Api;

                public CaptureTheFlag(Titanfall2Api tf2Api)
                {
                    this._tf2Api = tf2Api;
                }
            }

            public class LastTitanStanding
            {
                private readonly Titanfall2Api _tf2Api;

                public LastTitanStanding(Titanfall2Api tf2Api)
                {
                    this._tf2Api = tf2Api;
                }
            }

            public class PilotsVersusPilots
            {
                private readonly Titanfall2Api _tf2Api;

                public PilotsVersusPilots(Titanfall2Api tf2Api)
                {
                    this._tf2Api = tf2Api;
                }
            }

            public class LiveFire
            {
                private readonly Titanfall2Api _tf2Api;

                public LiveFire(Titanfall2Api tf2Api)
                {
                    this._tf2Api = tf2Api;
                }
            }

            public class MarkedForDeath
            {
                private readonly Titanfall2Api _tf2Api;

                public MarkedForDeath(Titanfall2Api tf2Api)
                {
                    this._tf2Api = tf2Api;
                }
            }

            public class TitanBrawl
            {
                private readonly Titanfall2Api _tf2Api;

                public TitanBrawl(Titanfall2Api tf2Api)
                {
                    this._tf2Api = tf2Api;
                }

                /// <summary>
                /// Get the score of team 1. Whether this is your team or the enemy's doesn't always stay the same.
                /// I'm not sure why. This is something that I need some help figuring out.
                /// </summary>
                /// <returns>the score of team 1</returns>
                public int GetTeam1Score()
                {
                    return _tf2Api._sharp!.Memory.Read<int>(_tf2Api._engineDllBaseAddress + 0x1121814C);
                }

                /// <summary>
                /// Get the score of team 2. Whether this is your team or the enemy's doesn't always stay the same.
                /// I'm not sure why. This is something that I need some help figuring out.
                /// </summary>
                /// <returns>the score of team 2</returns>
                public int GetTeam2Score()
                {
                    return _tf2Api._sharp!.Memory.Read<int>(_tf2Api._engineDllBaseAddress + 0x11218CA0);
                }

                /// <summary>
                /// Gets the round number (out of 5) of the current match.
                ///
                /// Candidates for this value are the following offsets:
                /// client.dll+FB83BC
                /// client.dll+FB83E4
                /// client.dll+FB840C
                ///
                /// If one of these is returning an erroneous result, the offset used by this method might need
                /// to be replaced with another one of these offsets.
                /// </summary>
                /// <returns>the current round number of the match</returns>
                public int GetRoundNumber()
                {
                    return _tf2Api._sharp!.Memory.Read<int>(_tf2Api._clientDllBaseAddress + 0xFB83BC);
                }

                /// <summary>
                /// Get the kills of the current user.
                /// </summary>
                /// <returns>the user's number of kills</returns>
                public int GetMyKills()
                {
                    throw new NotImplementedException(HelpMeBruh);
                }

                /// <summary>
                /// Get the deaths of the current user.
                /// </summary>
                /// <returns>the user's number of deaths</returns>
                public int GetMyDeaths()
                {
                    throw new NotImplementedException(HelpMeBruh);
                }

                /// <summary>
                /// Get the titan damage of the current user.
                /// </summary>
                /// <returns>the amount of titan damage the current user has inflicted</returns>
                public int GetMyTitanDamage()
                {
                    throw new NotImplementedException(HelpMeBruh);
                }
            }

            public class FrontierDefense
            {
                private readonly Titanfall2Api _tf2Api;

                public FrontierDefense(Titanfall2Api tf2Api)
                {
                    this._tf2Api = tf2Api;
                }
            }

            public Coliseum GetColiseum()
            {
                return this._coliseum;
            }

            public Attrition GetAttrition()
            {
                return this._attrition;
            }

            public Skirmish GetSkirmish()
            {
                return this._skirmish;
            }

            public AmpedHardpoint GetAmpedHardpoint()
            {
                return this._ampedHardpoint;
            }

            public BountyHunt GetBountyHunt()
            {
                return this._bountyHunt;
            }

            public CaptureTheFlag GetCaptureTheFlag()
            {
                return this._captureTheFlag;
            }

            public LastTitanStanding GetLastTitanStanding()
            {
                return this._lastTitanStanding;
            }

            public PilotsVersusPilots GetPilotsVersusPilots()
            {
                return this._pilotsVersusPilots;
            }

            public LiveFire GetLiveFire()
            {
                return this._liveFire;
            }

            public MarkedForDeath GetMarkedForDeath()
            {
                return this._markedForDeath;
            }

            public TitanBrawl GetTitanBrawl()
            {
                return this._titanBrawl;
            }

            public FrontierDefense GetFrontierDefense()
            {
                return this._frontierDefense;
            }
        }
    }
}