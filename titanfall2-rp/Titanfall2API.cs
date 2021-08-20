using System;
using System.Text;
using System.Text.RegularExpressions;
using Process.NET;
using static titanfall2_rp.ProcessApi;

namespace titanfall2_rp
{
    public class Titanfall2Api
    {
        private static readonly Regex gameModeAndMapRegex = new Regex("Playing (.*) on (.*)");
        private ProcessSharp _sharp;
        private IntPtr _engineDllBaseAddress;
        private IntPtr _clientDllBaseAddress;
        private IntPtr _serverDllBaseAddress;

        public int GetPlayerHealth()
        {
            _ensureInit();
            return _sharp.Memory.Read<int>(_engineDllBaseAddress + 0x1122A8DC);
        }

        public int GetPlayerVelocity()
        {
            _ensureInit();
            return _sharp.Memory.Read<int>(_clientDllBaseAddress + 0x2A9F704);
        }

        public string GetGameModeAndMapName()
        {
            _ensureInit();
            return _sharp.Memory.Read(_engineDllBaseAddress + 0x1397AC46, Encoding.UTF8, 50);
        }

        public string GetFriendlyMapName()
        {
            _ensureInit();
            var m = gameModeAndMapRegex.Match(GetGameModeAndMapName());
            return m.Success ? m.Groups[2].Value : "UNKNOWN MAP NAME";
        }

        public string GetGameModeName()
        {
            _ensureInit();
            var m = gameModeAndMapRegex.Match(GetGameModeAndMapName());
            return m.Success ? m.Groups[1].Value : "UNKNOWN GAME MODE";

        }

        public string GetMultiplayerMapName()
        {
            _ensureInit();
            return _sharp.Memory.Read(_clientDllBaseAddress + 0x23E0FA0, Encoding.UTF8, 50);
        }

        public string GetSinglePlayerMapName()
        {
            _ensureInit();
            return _sharp.Memory.Read(_serverDllBaseAddress + 0xC9BE64, Encoding.UTF8, 50);
        }

        public string GetSinglePlayerDifficulty()
        {
            _ensureInit();
            byte difficulty = _sharp.Memory.Read(_serverDllBaseAddress + 0xC0963C, 1)[0];
            switch (difficulty)
            {
                case 0:
                    return "easy";
                case 1:
                    return "regular";
                case 2:
                    return "hard";
                case 3:
                    return "master";
            }
            return "UNKNOWN DIFFICULTY";
        }

        /// <summary>
        /// Inside this class are multiplayer subclasses for each game mode (because the stats are different
        /// based on the game type. They'll also be in different memory locations).
        /// </summary>
        class MultiPlayerGameStats
        {
            class Attrition { }
            class Skirmish { }
            class AmpedHardpoint { }
            class BountyHunt { }
            class CaptureTheFlag { }
            class LastTitanStanding { }
            class PilotsVersusPilots { }
            class LiveFire { }
            class MarkedForDeath { }
            class TitanBrawl { }
            class FrontierDefense { }
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
        }
    }
}