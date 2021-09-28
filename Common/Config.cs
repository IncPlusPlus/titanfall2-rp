using System;
using System;
using System.IO;
using System.Reflection;
using log4net;
using SharpConfig;

namespace Common
{
    public static class Config
    {
        public const string ConfigFileName = "titanfall2-rp.cfg";
        public static readonly FileInfo ConfigFileInfo = new FileInfo(ConfigFileName);
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private static bool _initialized = false;
        private static Configuration _config;

        public static bool IsInstalledThroughSteam
        {
            get
            {
                EnsureInit();
                return _config["General"]["InstalledThroughSteam"].GetValueOrDefault(Defaults.InstalledThroughSteam);
            }
            set
            {
                EnsureInit();
                _config["General"]["InstalledThroughSteam"].BoolValue = value;
                Save();
            }
        }

        private static void Init()
        {
            Log.DebugFormat("Loading config file from '{0}'", ConfigFileInfo.FullName);
            if (!ConfigFileInfo.Exists)
            {
                Log.WarnFormat("Couldn't find '{0}'! Creating it (this only needs to happen once)...", ConfigFileName);
                ConfigureDefaults();
                Save();
            }
            else
            {
                _config = Configuration.LoadFromFile(ConfigFileName);
                Log.DebugFormat("Loaded config file '{0}'", ConfigFileInfo.FullName);
            }

            _initialized = true;
        }

        private static void Save()
        {
            try
            {
                Log.DebugFormat("Attempting to save config to '{0}'...", ConfigFileInfo.FullName);
                _config.SaveToFile(ConfigFileName);
                Log.DebugFormat("Saved '{0}' successfully!", ConfigFileInfo.FullName);
            }
            catch (Exception e)
            {
                Log.Error("Unable to save application config", e);
                Log.Error("Continuing to use built-in default application configuration.");
            }
        }

        private static void ConfigureDefaults()
        {
            _config = new Configuration
            {
                ["General"] =
                {
                    PreComment = "Configuration file for IncPlusPlus's Titanfall 2 Discord Rich Presence tool",
                    ["InstalledThroughSteam"] =
                    {
                        PreComment = Defaults.InstalledThroughSteamComment,
                        BoolValue = Defaults.InstalledThroughSteam,
                    },
                    ["Titanfall2ExecutablePath"] =
                    {
                        PreComment = Defaults.Titanfall2ExecutablePathComment,
                        StringValue = Defaults.Titanfall2ExecutablePath,
                    },
                },
            };
        }

        private static void EnsureInit()
        {
            if (!_initialized)
                Init();
        }

        public static class Defaults
        {
            public const bool InstalledThroughSteam = false;

            public const string InstalledThroughSteamComment =
                "Set this to True if you installed Titanfall 2 through Steam.";

            public const string Titanfall2ExecutablePath = "";

            public const string Titanfall2ExecutablePathComment =
                "Set this to the full path to 'Titanfall2.exe'. If you installed Titanfall 2 through Steam, you can " +
                "ignore this setting. If you installed Titanfall 2 through Origin, set this value to something like " +
                "\"E:\\Origin Games\\Titanfall2\\Titanfall2.exe\" (without quotation marks). Obviously, you shouldn't" +
                " use this exact value. You need to find wherever you put your 'Origin Games' folder.";
        }
    }
}