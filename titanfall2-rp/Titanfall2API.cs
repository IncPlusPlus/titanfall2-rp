using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using log4net;
using Process.NET;
using titanfall2_rp.enums;
using titanfall2_rp.SegmentManager;
using static titanfall2_rp.ProcessApi;

namespace titanfall2_rp
{
    // This is probably not thread safe. Multiple threads could potentially attempt to initialize this class.
    // However, this is unlikely given that the presence update time is multiple seconds.
    public class Titanfall2Api
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private static readonly Regex GameModeAndMapRegex = new Regex("Playing (.*) on (.*)");
        private ProcessSharp? _sharp;
        public IntPtr EngineDllBaseAddress { get; private set; }
        public IntPtr ClientDllBaseAddress { get; private set; }
        public IntPtr ServerDllBaseAddress { get; private set; }

        public MpStats GetMultiPlayerGameStats()
        {
            _ensureInit();
            return MpStats.Of(this, _sharp!);
        }

        /// <summary>
        /// This gets the user's health. This only works in single-player. For multi-player, use
        /// <see cref="MpStats.GetPlayerHealth"/>.
        /// </summary>
        /// <returns></returns>
        public int GetPlayerHealth()
        {
            _ensureInit();
            return _sharp!.Memory.Read<int>(EngineDllBaseAddress + 0x1122A8DC);
        }

        /// <summary>
        /// This function currently supports multiplayer only and hasn't been tested in single-player.
        /// </summary>
        /// <returns>true if the pilot is in a titan; else false</returns>
        /// <remarks>This function requires more testing but should be okay. It guesses whether the user is in a titan
        /// based on whether their health is over 100 (the default for a pilot).</remarks>
        public bool IsPlayerInTitan()
        {
            _ensureInit();
            return _sharp!.Memory.Read<int>(
                ResolvePointerAddress(_sharp!, (ClientDllBaseAddress + EntityOffsets.LocalPlayerBase)) +
                EntityOffsets.LocalPlayer.m_iMaxHealth) > 100;
        }

        /// <returns>the currently equipped titan</returns>
        /// <remarks>Keep in mind that this isn't the titan that's currently in use.
        /// It's only the one that's currently <b>equipped</b>. This may be improved in the future.</remarks>
        public Titan GetTitan()
        {
            _ensureInit();
            return TitanMethods.GetTitan(_sharp!.Memory.Read(EngineDllBaseAddress + 0x7A7429, 1)[0]);
        }

        public int GetPlayerVelocity()
        {
            _ensureInit();
            return _sharp!.Memory.Read<int>(ClientDllBaseAddress + 0x2A9F704);
        }

        public string GetGameModeAndMapName()
        {
            _ensureInit();
            return _sharp!.Memory.Read(EngineDllBaseAddress + 0x1397AC46, Encoding.UTF8, 50);
        }

        public string GetFriendlyMapName()
        {
            _ensureInit();
            var gameModeAndMapName = GetGameModeAndMapName();
            var m = GameModeAndMapRegex.Match(gameModeAndMapName);
            return m.Success
                ? m.Groups[2].Value
                : throw new ApplicationException($"Failed to recognize map name from string '{gameModeAndMapName}'.");
        }

        public string GetGameModeName()
        {
            _ensureInit();
            var gameModeAndMapName = GetGameModeAndMapName();
            var m = GameModeAndMapRegex.Match(gameModeAndMapName);
            return m.Success
                ? m.Groups[1].Value
                : throw new ApplicationException($"Failed to recognize game mode from string '{gameModeAndMapName}'.");
        }

        public GameMode GetGameMode()
        {
            _ensureInit();
            string gameModeCodeName = _sharp!.Memory.Read(EngineDllBaseAddress + 0x13984088, Encoding.UTF8, 15);
            return GameModeMethods.GetGameMode(gameModeCodeName);
        }

        public string GetMultiplayerMapName()
        {
            _ensureInit();
            return _sharp!.Memory.Read(ClientDllBaseAddress + 0x23E0FA0, Encoding.UTF8, 50);
        }

        public string GetSinglePlayerMapName()
        {
            _ensureInit();
            return _sharp!.Memory.Read(ClientDllBaseAddress + 0xB34522, Encoding.UTF8, 50);
        }

        public string GetSinglePlayerDifficulty()
        {
            _ensureInit();
            byte difficulty = _sharp!.Memory.Read(ServerDllBaseAddress + 0xC0963C, 1)[0];
            return difficulty switch
            {
                0 => "easy",
                1 => "regular",
                2 => "hard",
                3 => "master",
                _ => "UNKNOWN DIFFICULTY"
            };
        }

        public string GetUserId()
        {
            _ensureInit();
            var tryCount = 5;
            while (!_sharp!.Memory.Read(EngineDllBaseAddress + 0x139119EC, Encoding.UTF8, 250).Contains("?") &&
                   tryCount > 0)
            {
                Log.DebugFormat("Couldn't find the user ID. Waiting 1 second and trying again. ({0} tries left)",
                    tryCount);
                //If the value isn't there yet. Try waiting 5 seconds to grab it. This might be necessary for game startup
                Thread.Sleep(1000);
                tryCount--;
            }

            var stryderNucleusOauthString = _sharp!.Memory.Read(EngineDllBaseAddress + 0x139119EC, Encoding.UTF8, 250);
            var queryStringIndex = stryderNucleusOauthString.IndexOf("?", StringComparison.Ordinal);
            if (queryStringIndex < 0)
            {
                throw new ApplicationException(
                    "Expected to find a query string in the Stryder nucleus oauth string but didn't find one.");
            }

            // Edge case for if there's a question mark but nothing else following it.
            var querystring = (queryStringIndex < stryderNucleusOauthString.Length - 1)
                ? stryderNucleusOauthString[(queryStringIndex + 1)..]
                : string.Empty;
            NameValueCollection queryStringCollection = HttpUtility.ParseQueryString(querystring);
            return queryStringCollection["userId"] ?? "";
        }

        /// <summary>
        /// Reads the user's Origin name through engine.dll. The primary offset used is engine.dll+13F8E310.
        /// There appears to be another consistently working offset. That being engine.dll+1397A180. There's also one
        /// more offset, engine.dll+130DA1CF, which has the string format 
        /// </summary>
        /// <returns>the user's name on Origin</returns>
        public string GetOriginName()
        {
            _ensureInit();
            return _sharp!.Memory.Read(EngineDllBaseAddress + 0x13F8E310, Encoding.UTF8, 64);
        }


        private void _ensureInit()
        {
            if (ProcessNetApi.NeedsInit())
            {
                if (!ProcessNetApi.Init())
                {
                    throw new InvalidOperationException(
                        "Couldn't initialize Titanfall2Api. Make sure the process is running!");
                }

                Log.Info("Found a running instance of Titanfall 2.");
                _populateFields(ProcessNetApi.GetProcess());
                SegmentManager.SegmentManager.TrackEvent(TrackableEvent.GameOpened);
                _sharp!.Native.Exited += (sender, args) =>
                {
                    Log.Info("Titanfall 2 has closed.");
                    SegmentManager.SegmentManager.TrackEvent(TrackableEvent.GameClosed);
                };
            }
        }

        private void _populateFields(ProcessSharp sharp)
        {
            _sharp = sharp;
            EngineDllBaseAddress = GetModuleBaseAddress(sharp.Native, "engine.dll");
            ClientDllBaseAddress = GetModuleBaseAddress(sharp.Native, "client.dll");
            ServerDllBaseAddress = GetModuleBaseAddress(sharp.Native, "server.dll");
        }
    }
}