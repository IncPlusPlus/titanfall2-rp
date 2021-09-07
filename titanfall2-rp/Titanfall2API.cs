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

        /// <summary>
        /// Here's the list of addresses that all reflected the pilot being inside a titan:
        /// engine.dll+111E18DC
        /// engine.dll+111E18E0
        /// engine.dll+111E1CB8
        /// engine.dll+111E1D10
        /// engine.dll+111E1D34
        /// engine.dll+111E1DAC
        /// engine.dll+111E1DB8
        /// engine.dll+111E1DC4
        /// engine.dll+1128D850
        /// engine.dll+112910F4
        /// client.dll+21720D7
        /// client.dll+22BFEC9
        /// client.dll+22BFED5
        /// client.dll+22BFEE5
        /// client.dll+22BFF01
        /// client.dll+22BFF0D
        /// client.dll+22BFF1D
        /// client.dll+22BFF39
        /// client.dll+22BFF45
        /// client.dll+22BFF55
        /// client.dll+22C75E5
        /// client.dll+22C7605
        /// client.dll+22C7780
        /// client.dll+22C78A0
        /// client.dll+23F5B48
        /// client.dll+23F5B68
        /// client.dll+23F5B88
        /// client.dll+23F5BA8
        /// client.dll+23F5BC8
        /// client.dll+23F5BE8
        /// client.dll+23F5C08
        /// client.dll+23F5C28
        /// client.dll+23F5C48
        /// materialsystem_dx11.dll+1A98090
        /// </summary>
        /// <returns>true if the pilot is in a titan; else false</returns>
        /// <remarks>This shit ain't stable, chief</remarks>
        public bool IsPlayerInTitan()
        {
            _ensureInit();
            return _sharp!.Memory.Read<int>(_engineDllBaseAddress + 0x111E18DC) != 0;
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