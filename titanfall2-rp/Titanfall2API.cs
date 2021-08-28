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

        public bool SinglePlayerMapHasChapters()
        {
            try
            {
                GetSinglePlayerChapter();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public int GetSinglePlayerChapter()
        {
            var mapName = GetSinglePlayerMapName();
            switch (mapName)
            {
                case "sp_beacon":
                    /*
                     * Seems like The Beacon's chapter number can be found at the following client.dll offsets
                     * client.dll+22B0D59
                     * client.dll+22B0E79
                     * client.dll+22B0F99
                     * client.dll+22B10B9
                     * client.dll+22B11D9
                     * client.dll+22B12F9
                     * client.dll+22B1539
                     * client.dll+22B1659
                     *
                     * There also seem to be offsets that represent the chapter number but zero-indexed.
                     * engine.dll+FD9BD59
                     * engine.dll+FD9BD5F
                     * engine.dll+FD9BD63
                     * engine.dll+FD9BD65
                     * engine.dll+FD9BD67
                     * engine.dll+FD9BD69
                     * engine.dll+FD9BD6B
                     * engine.dll+FD9BD6D
                     * engine.dll+FD9BD6F
                     * engine.dll+FD9BD71
                     * engine.dll+111D4073
                     * engine.dll+111D4A57
                     * engine.dll+111D4A7B
                     * engine.dll+111D4A87
                     * engine.dll+111D4A9F
                     * engine.dll+111D4AAB
                     * engine.dll+111D4AB7
                     * engine.dll+111D4AC3
                     * engine.dll+111D4ACF
                     * engine.dll+111D4ADB
                     * engine.dll+111D4AFF
                     * server.dll+C12819
                     * server.dll+C1281D
                     * server.dll+C1282D
                     * server.dll+C12831
                     * server.dll+C12839
                     * server.dll+C1283D
                     * server.dll+C12841
                     * server.dll+C12845
                     * server.dll+C1284D
                     * server.dll+C12851
                     * server.dll+C12855
                     * server.dll+C12859
                     * server.dll+C128A5
                     * server.dll+C1693D
                     * server.dll+C16941
                     * server.dll+C16945
                     * server.dll+C16949
                     * server.dll+C1694D
                     * server.dll+C16951
                     * server.dll+C16955
                     */
                    return _sharp!.Memory.Read(_clientDllBaseAddress+0x22B0D59,1)[0];
                default:
                    throw new ApplicationException("There are no chapters in map '"+mapName+"'.");
            }
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