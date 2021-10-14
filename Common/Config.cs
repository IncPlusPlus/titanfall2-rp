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
                return _config[Props.General][Props.InstalledThroughSteam]
                    .GetValueOrDefault(Defaults.InstalledThroughSteam);
            }
            set
            {
                EnsureInit();
                _config[Props.General][Props.InstalledThroughSteam].BoolValue = value;
                Save();
            }
        }

        public static string Titanfall2ExecutablePath
        {
            get
            {
                EnsureInit();
                return _config[Props.General][Props.Titanfall2ExePath].GetValueOrDefault(Defaults.Titanfall2ExePath);
            }
            set
            {
                EnsureInit();
                _config[Props.General][Props.Titanfall2ExePath].StringValue = value;
                Save();
            }
        }

        public static bool IsAnalyticsAllowed
        {
            get
            {
                EnsureInit();
                return _config[Props.Internals][Props.AllowAnalytics].GetValueOrDefault(Defaults.AllowAnalytics);
            }
            set
            {
                EnsureInit();
                _config[Props.Internals][Props.AllowAnalytics].BoolValue = value;
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

        public static void ReloadFromFile()
        {
            Init();
        }

        private static void ConfigureDefaults()
        {
            _config = new Configuration
            {
                [Props.General] =
                {
                    PreComment = "Configuration file for IncPlusPlus's Titanfall 2 Discord Rich Presence tool",
                    [Props.InstalledThroughSteam] =
                    {
                        PreComment = Defaults.InstalledThroughSteamComment,
                        BoolValue = Defaults.InstalledThroughSteam,
                    },
                    [Props.Titanfall2ExePath] =
                    {
                        PreComment = Defaults.Titanfall2ExePathComment,
                        StringValue = Defaults.Titanfall2ExePath,
                    },
                },
                [Props.Internals] =
                {
                    PreComment = "Settings that have to do with how this program runs. Be careful here.",
                    [Props.AllowAnalytics] =
                    {
                        PreComment = Defaults.AllowAnalyticsComment,
                        BoolValue = Defaults.AllowAnalytics,
                    },
                },
            };
        }

        private static void EnsureInit()
        {
            if (!_initialized)
                Init();
        }

        /// <summary>
        /// A class to hold strings to avoid any typos causing me headaches later.
        /// </summary>
        public static class Props
        {
            public const string General = "General";
            public const string InstalledThroughSteam = "InstalledThroughSteam";
            public const string Titanfall2ExePath = "Titanfall2ExecutablePath";
            public const string Internals = "Internals";
            public const string AllowAnalytics = "AllowAnalytics";
        }

        public static class Defaults
        {
            public const bool InstalledThroughSteam = false;

            public const string InstalledThroughSteamComment =
                "Set this to True if you installed Titanfall 2 through Steam.";

            public const string Titanfall2ExePath = "";

            public const string Titanfall2ExePathComment =
                "Set this to the full path to 'Titanfall2.exe'. If you installed Titanfall 2 through Steam, you can " +
                "ignore this setting. If you installed Titanfall 2 through Origin, set this value to something like " +
                "\"E:\\Origin Games\\Titanfall2\\Titanfall2.exe\" (without quotation marks). Obviously, you shouldn't" +
                " use this exact value. You need to find wherever you put your 'Origin Games' folder.";

            public const bool AllowAnalytics = true;

            public const string AllowAnalyticsComment = "Set this to False to opt out of analytics. Unless you have " +
                                                        "good reason to do so, please leave this set to True. Sends " +
                                                        "information about the state of your game as well as basic " +
                                                        "information about the runtime environment like the TF|2 RP " +
                                                        "app version as well as your computer's Operating System. " +
                                                        "This is helpful for me finding and fixing errors.";
        }
    }
}