using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using log4net;
using Process.NET;
using titanfall2_rp.enums;
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

        public int GetPlayerHealth()
        {
            _ensureInit();
            return _sharp!.Memory.Read<int>(EngineDllBaseAddress + 0x1122A8DC);
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
            var m = GameModeAndMapRegex.Match(GetGameModeAndMapName());
            return m.Success ? m.Groups[2].Value : "UNKNOWN MAP NAME";
        }

        public string GetGameModeName()
        {
            _ensureInit();
            var m = GameModeAndMapRegex.Match(GetGameModeAndMapName());
            return m.Success ? m.Groups[1].Value : "UNKNOWN GAME MODE";

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