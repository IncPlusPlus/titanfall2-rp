using System;
using System.Text;
using System.Text.RegularExpressions;
using Process.NET;
using static titanfall2_rp.ProcessApi;

namespace titanfall2_rp
{
    // This is probably not thread safe. Multiple threads could potentially attempt to initialize this class.
    // However, this is unlikely given that the presence update time is multiple seconds.
    public partial class Titanfall2Api
    {
        private static readonly Regex GameModeAndMapRegex = new Regex("Playing (.*) on (.*)");
        private ProcessSharp? _sharp;
        private IntPtr _engineDllBaseAddress;
        private IntPtr _clientDllBaseAddress;
        private IntPtr _serverDllBaseAddress;
        private MpGameStats? _multiplayerGameStats;

        public MpGameStats GetMultiPlayerGameStats()
        {
            _ensureInit();
            return this._multiplayerGameStats!;
        }

        public int GetPlayerHealth()
        {
            _ensureInit();
            return _sharp!.Memory.Read<int>(_engineDllBaseAddress + 0x1122A8DC);
        }

        public int GetPlayerVelocity()
        {
            _ensureInit();
            return _sharp!.Memory.Read<int>(_clientDllBaseAddress + 0x2A9F704);
        }

        public string GetGameModeAndMapName()
        {
            _ensureInit();
            return _sharp!.Memory.Read(_engineDllBaseAddress + 0x1397AC46, Encoding.UTF8, 50);
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
            string gameModeCodeName = _sharp!.Memory.Read(_engineDllBaseAddress + 0x13984088, Encoding.UTF8, 15);
            return GameModeMethods.GetGameMode(gameModeCodeName);
        }

        public string GetMultiplayerMapName()
        {
            _ensureInit();
            return _sharp!.Memory.Read(_clientDllBaseAddress + 0x23E0FA0, Encoding.UTF8, 50);
        }

        public string GetSinglePlayerMapName()
        {
            _ensureInit();
            return _sharp!.Memory.Read(_clientDllBaseAddress + 0xB34522, Encoding.UTF8, 50);
        }

        public string GetSinglePlayerDifficulty()
        {
            _ensureInit();
            byte difficulty = _sharp!.Memory.Read(_serverDllBaseAddress + 0xC0963C, 1)[0];
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
                _populateFields(ProcessNetApi.GetProcess());
            }
        }

        private void _populateFields(ProcessSharp sharp)
        {
            this._sharp = sharp;
            this._engineDllBaseAddress = GetModuleBaseAddress(sharp.Native, "engine.dll");
            this._clientDllBaseAddress = GetModuleBaseAddress(sharp.Native, "client.dll");
            this._serverDllBaseAddress = GetModuleBaseAddress(sharp.Native, "server.dll");
            this._multiplayerGameStats = new MpGameStats(this);
        }
    }
}