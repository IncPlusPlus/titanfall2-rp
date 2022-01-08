using System;
using System.Diagnostics.CodeAnalysis;

namespace titanfall2_rp.enums
{
    /// <summary>
    /// Per englishclient_frontend.bsp.pak000_dir.vpk/scripts/vscripts/sh_consts.gnut
    /// and my own testing. Original declaration below.
    ///
    ///
    /// <code>
    /// global const T_DAY = "tday"
    /// global const TEAM_DEATHMATCH = "tdm"
    /// global const PILOT_SKIRMISH = "ps"
    /// global const CAPTURE_POINT = "cp"
    /// global const ATTRITION = "at"
    /// global const CAPTURE_THE_FLAG = "ctf"
    /// global const MARKED_FOR_DEATH = "mfd"
    /// global const MARKED_FOR_DEATH_PRO = "mfdp"
    /// global const LAST_TITAN_STANDING = "lts"
    /// global const WINGMAN_LAST_TITAN_STANDING = "wlts"
    /// global const LTS_BOMB = "ltsbomb"
    /// global const AI_TDM = "aitdm"
    /// global const BOMB = "bomb"
    /// global const FFA = "ffa"
    /// global const SST = "sst"
    /// global const COLISEUM = "coliseum"
    /// global const WINGMAN_PILOT_SKIRMISH = "wps"
    /// global const HARDCORE_TDM = "htdm"
    /// global const FREE_AGENCY = "fra"
    /// global const FORT_WAR = "fw"
    /// global const HUNTED = "hunted"
    /// global const DON = "don"
    /// global const SPEEDBALL = "speedball"
    /// global const RAID = "raid"
    /// global const ATCOOP = "atcoop"
    /// global const CONQUEST = "cq"
    /// global const FD = "fd"
    /// global const FD_EASY = "fd_easy"
    /// global const FD_NORMAL = "fd_normal"
    /// global const FD_HARD = "fd_hard"
    /// global const FD_MASTER = "fd_master"
    /// global const FD_INSANE = "fd_insane"
    /// global const PVE_SANDBOX = "pve_sandbox"
    /// global const TITAN_BRAWL = "ttdm"
    ///
    /// global const GAMEMODE_SP = "solo"
    /// </code>
    ///
    ///
    /// In the same VPK, there is also scripts/vscripts/gamemodes/sh_gamemodes.gnut which contains a similar structure
    /// shown below.
    ///
    /// <code>
    ///
    /// // Don't remove items from this list once the game is in production
    /// // Durango online analytics needs the numbers for each mode to stay the same
    /// // DO NOT CHANGE THESE VALUES AFTER THEY HAVE GONE LIVE
    /// global enum eGameModes
    /// {
    ///     invalid =							-1,
    ///     TEAM_DEATHMATCH_ID =				0,
    ///     CAPTURE_POINT_ID =					1,
    ///     ATTRITION_ID =						2,
    ///     CAPTURE_THE_FLAG_ID =				3,
    ///     MARKED_FOR_DEATH_ID =				4,
    ///     LAST_TITAN_STANDING_ID =			5,
    ///     WINGMAN_LAST_TITAN_STANDING_ID =	6,
    ///     PILOT_SKIRMISH_ID =					7,
    ///     MARKED_FOR_DEATH_PRO_ID =			8,
    ///     COOPERATIVE_ID =					9,
    ///     GAMEMODE_SP_ID =					10,
    ///     TITAN_BRAWL_ID =					11,
    ///     FFA_ID =							12,
    ///     PROTOTYPE2 =						13,
    ///     WINGMAN_PILOT_SKIRMISH_ID =			14,
    ///     PROTOTYPE3 = 						15,
    ///     PROTOTYPE4 = 						16,
    ///     FREE_AGENCY_ID = 					17,
    ///     PROTOTYPE6 =						18,
    ///     COLISEUM_ID =						19,
    ///     PROTOTYPE7 =						20,
    ///     AI_TDM_ID =							21,
    ///     PROTOTYPE8 =						22,
    ///     PROTOTYPE9 = 						23,
    ///     SPEEDBALL_ID =						24,
    ///     PROTOTYPE10 = 						25,
    ///     PROTOTYPE11 = 						26,
    ///     PROTOTYPE12 = 						27,
    ///     FD_ID = 							28,
    ///     PROTOTYPE14 =						29,
    /// }
    /// </code>
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public enum GameMode
    {
        /// <summary>
        /// This enum value represents a game mode that is unrecognized
        /// </summary>
        UNKNOWN_GAME_MODE,

        /// <summary>
        /// Coliseum
        /// </summary>
        coliseum,

        /// <summary>
        /// Attrition
        /// </summary>
        aitdm,

        /// <summary>
        /// Skirmish
        /// </summary>
        tdm,

        /// <summary>
        /// Amped Hardpoint
        /// </summary>
        cp,

        /// <summary>
        /// Bounty Hunt
        /// </summary>
        at,

        /// <summary>
        /// Capture the Flag
        /// </summary>
        ctf,

        /// <summary>
        /// Last Titan Standing
        /// </summary>
        lts,

        /// <summary>
        /// Pilots vs. Pilots
        /// </summary>
        ps,

        /// <summary>
        /// Live Fire
        /// </summary>
        speedball,

        /// <summary>
        /// Marked For Death
        /// </summary>
        mfd,

        /// <summary>
        /// Titan Brawl
        /// </summary>
        ttdm,

        /// <summary>
        /// Frontier Defense (Easy)
        /// </summary>
        fd_easy,

        /// <summary>
        /// Frontier Defense (Regular)
        /// </summary>
        fd_normal,

        /// <summary>
        /// Frontier Defense (Hard)
        /// </summary>
        fd_hard,

        /// <summary>
        /// Frontier Defense (Master)
        /// </summary>
        fd_master,

        /// <summary>
        /// Frontier Defense (Insane)
        /// </summary>
        fd_insane,

        /// <summary>
        /// Single Player (Campaign)
        /// </summary>
        solo,

        #region Featured-Only Modes (i.e. ones that are used with Northstar)

        /// <summary>
        /// Free for All
        /// </summary>
        ffa,

        /// <summary>
        /// Free Agents
        /// </summary>
        fra,

        /// <summary>
        /// Aegis Titan Brawl
        /// </summary>
        attdm,

        /// <summary>
        /// Turbo Titan Brawl
        /// </summary>
        turbo_ttdm,

        /// <summary>
        /// Aegis Last Titan Standing
        /// </summary>
        alts,

        /// <summary>
        /// Turbo Last Titan Standing
        /// </summary>
        turbo_lts,

        /// <summary>
        /// Rocket Arena
        /// </summary>
        rocket_lf,

        /// <summary>
        /// The Great Bamboozle
        /// </summary>
        holopilot_lf,

        /// <summary>
        /// Gun Game
        /// </summary>
        gg,

        /// <summary>
        /// Titan Tag
        /// </summary>
        tt,

        /// <summary>
        /// Infection
        /// </summary>
        inf,

        /// <summary>
        /// Amped Killrace
        /// </summary>
        kr,

        /// <summary>
        /// Fastball
        /// </summary>
        fastball,

        /// <summary>
        /// Hide and Seek
        /// </summary>
        hs,

        /// <summary>
        /// Competitive CTF
        /// </summary>
        ctf_comp,

        #endregion
    }

    internal static class GameModeMethods
    {
        public static GameMode GetGameMode(string gameModeCodeName)
        {
            var parseSuccess = Enum.TryParse(typeof(GameMode), gameModeCodeName, true, out var mode);
            if (!parseSuccess)
                return GameMode.UNKNOWN_GAME_MODE;
            return (GameMode)((mode) ?? GameMode.UNKNOWN_GAME_MODE);
        }

        /// <summary>
        /// Get a human-friendly name for a given GameMode
        /// </summary>
        /// <param name="gameMode">any valid GameMode</param>
        /// <returns>the provided GameMode as a human-readable string</returns>
        /// <exception cref="ArgumentOutOfRangeException">if you somehow managed to screw this up</exception>
        public static string ToFriendlyString(this GameMode gameMode)
        {
            return gameMode switch
            {
                GameMode.coliseum => "Coliseum",
                GameMode.aitdm => "Attrition",
                GameMode.tdm => "Skirmish",
                GameMode.cp => "Amped Hardpoint",
                GameMode.at => "Bounty Hunt",
                GameMode.ctf => "Capture the Flag",
                GameMode.lts => "Last Titan Standing",
                GameMode.ps => "Pilots vs. Pilots",
                GameMode.speedball => "Live Fire",
                GameMode.mfd => "Marked For Death",
                GameMode.ttdm => "Titan Brawl",
                GameMode.fd_easy => "Frontier Defense (Easy)",
                GameMode.fd_normal => "Frontier Defense (Regular)",
                GameMode.fd_hard => "Frontier Defense (Hard)",
                GameMode.fd_master => "Frontier Defense (Master)",
                GameMode.fd_insane => "Frontier Defense (Insane)",
                GameMode.solo => "Campaign",
                GameMode.ffa => "Free for All",
                GameMode.fra => "Free Agents",
                GameMode.attdm => "Aegis Titan Brawl",
                GameMode.turbo_ttdm => "Turbo Titan Brawl",
                GameMode.alts => "Aegis Last Titan Standing",
                GameMode.turbo_lts => "Turbo Last Titan Standing",
                GameMode.rocket_lf => "Rocket Arena",
                GameMode.holopilot_lf => "The Great Bamboozle",
                GameMode.gg => "Gun Game",
                GameMode.tt => "expr",
                GameMode.inf => "Infection",
                GameMode.kr => "Amped Killrace",
                GameMode.fastball => "Fastball",
                GameMode.hs => "Hide and Seek",
                GameMode.ctf_comp => "Competitive CTF",
                _ => throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode,
                    "No friendly string for gamemode")
            };
        }
    }
}