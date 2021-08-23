using System;
using System.Diagnostics.CodeAnalysis;

namespace titanfall2_rp
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
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public enum GameMode
    {
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
        /// Life Fire
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
        /// Frontier Defense (Insane)
        /// </summary>
        fd_insane,

        /// <summary>
        /// Single Player (Campaign)
        /// </summary>
        solo,
    }

    internal static class GameModeMethods
    {
        public static GameMode GetGameMode(string gameModeCodeName)
        {
            var parseSuccess = Enum.TryParse(typeof(GameMode), gameModeCodeName, true, out var mode);
            if (!parseSuccess)
                throw new ArgumentException("Unrecognized game mode '" + gameModeCodeName + "'.");
            return (GameMode)(mode ?? throw new ArgumentException("Unrecognized game mode '" + gameModeCodeName + "'."));
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
                GameMode.aitdm => "Attrition",
                GameMode.tdm => "Skirmish",
                GameMode.cp => "Amped Hardpoint",
                GameMode.at => "Bounty Hunt",
                GameMode.ctf => "Capture the Flag",
                GameMode.lts => "Last Titan Standing",
                GameMode.ps => "Pilots vs. Pilots",
                GameMode.speedball => "Life Fire",
                GameMode.mfd => "Marked For Death",
                GameMode.ttdm => "Titan Brawl",
                GameMode.fd_easy => "Frontier Defense (Easy)",
                GameMode.fd_normal => "Frontier Defense (Regular)",
                GameMode.fd_hard => "Frontier Defense (Hard)",
                GameMode.fd_insane => "Frontier Defense (Insane)",
                GameMode.solo => "Campaign",
                _ => throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null)
            };
        }
    }
}