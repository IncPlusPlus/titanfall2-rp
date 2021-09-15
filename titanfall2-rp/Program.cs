using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using AutoUpdaterDotNET;
using DiscordRPC;
using DiscordRPC.Logging;
using log4net;
using log4net.Config;

namespace titanfall2_rp
{
    static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private const string LoggerConfigFileName = "log4net.config";
        public static readonly Version AppVersion = Assembly.GetEntryAssembly()!.GetName().Version!;
        public const int StatusRefreshTimeInSeconds = 5;
        public const int StatusRefreshTimeInMs = StatusRefreshTimeInSeconds * 1000;
        private static AutoResetEvent _userRequestedExit = new AutoResetEvent(false);
        private static readonly DiscordRpcClient DiscordRpcClient = new DiscordRpcClient("877931149740089374");
        private static Thread? _presenceUpdatingThread;

        static void Main(string[] args)
        {
            ConfigureLogger();
            // Set this thread's name. This way it indicates which thread is the main thread (although this is usually 1)
            Thread.CurrentThread.Name = "Main-" + Thread.CurrentThread.ManagedThreadId;

            Log.InfoFormat("IncPlusPlus' TF|2 Discord Rich Presence Tool version {0} (https://github.com/IncPlusPlus/titanfall2-rp)", AppVersion);

            var appDirectoryWritable = HasWritePermission(AppContext.BaseDirectory!);

            if (!appDirectoryWritable)
            {
                Log.ErrorFormat(
                    "This program needs write permissions to the current directory." +
                    "\nThe directory '{0}' is not writeable!!!" +
                    "\nThis could affect logging!",
                    AppContext.BaseDirectory!);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !IsRunningInWine())
            {
                if (appDirectoryWritable)
                {
                    // Don't give a UAC prompt if I don't have to (they're annoying)
                    AutoUpdater.RunUpdateAsAdmin = false;
                }
                Log.Info("Checking for updates...");
                AutoUpdater.Synchronous = true;
                AutoUpdater.ApplicationExitEvent += AutoUpdater_ApplicationExitEvent;
                AutoUpdater.Start("https://github.com/IncPlusPlus/titanfall2-rp/releases/latest/download/updater-helper-file.xml");
            }
            else
            {
                Log.Info("Skipped checking for updates as that's a Windows-only feature.");
            }

            Log.Info("Starting Titanfall 2 Discord Rich Presence. Press enter at any time to exit!");

            // Get an instance of Titanfall2Api. This doesn't do any initialization. That'll happen upon each method
            // call. Meaning, the first method call will do any initializing.
            var tf2Api = new Titanfall2Api();

            // Make the RPC library use a custom implemented logger (this is such a cool library feature)
            DiscordRpcClient.Logger = new Log4NetDiscordLogger { Level = LogLevel.Warning };

            // Subscribe to events
            DiscordRpcClient.OnReady += (sender, e) => { Log.DebugFormat("Received Ready from user {0}", e.User.Username); };
            // _discordRpcClient.OnClose += (sender, e) => { Log.DebugFormat("Received Close with code {0}. Reason: {1}", e.Code, e.Reason); };
            // _discordRpcClient.OnError += (sender, message) => {Log.DebugFormat("Received error code {0}. Message was: {1}", message.Code, message.Message); };
            DiscordRpcClient.OnPresenceUpdate += (sender, e) => { Log.DebugFormat("Received Update! {0}", e.Presence); };

            // Connect to the RPC
            DiscordRpcClient.Initialize();

            // Start the thread continuously updates the RP status
            StartThread(DiscordRpcClient, tf2Api, _userRequestedExit);

            // Waits for the user to hit enter. Then this will close
            Console.ReadLine();
            Log.Info("User has requested that the program exits.");

            // Cleanup
            Log.Debug("Disposing of Discord RPC client");
            DiscordRpcClient.Dispose();
            Log.Debug("Informing the presence updating thread that the user has requested it's time to exit...");
            _userRequestedExit.Set();
            // Waits for the presence updating thread to close
            _presenceUpdatingThread!.Join();
            // Releases the resources used for the events (only after the thread that was using this has exited)
            _userRequestedExit.Close();
            Log.Info("Closing...");
        }

        /// <summary>
        /// This method is somewhat deceiving. This will return true as long as it's not on windows. This should be
        /// used in tandem with an IfOnWindows() conditional. That'll probably return true when in wine so you'd want
        /// another method to make sure you're truly in a Windows environment. That's what this method is useful for.
        /// </summary>
        /// <returns>whether this program is running inside WINE</returns>
        private static bool IsRunningInWine()
        {
            return System.Diagnostics.Process.GetProcessesByName("winlogon").Length == 0;
        }

        private static void ConfigureLogger()
        {
            // Load logging configuration
            // Thanks to https://jakubwajs.wordpress.com/2019/11/28/logging-with-log4net-in-net-core-3-0-console-app/
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            // Start with the default logger before trying any fancy config file stuff
            XmlConfigurator.Configure(logRepository, Log4NetDefaultConfig.GetLoggerConfigAsXml());
            var loggerConfigFile = new FileInfo(LoggerConfigFileName);
            if (!loggerConfigFile.Exists)
            {
                Log.WarnFormat("Couldn't find '{0}'! Creating it (this only needs to happen once)...", LoggerConfigFileName);
                try
                {
                    File.WriteAllText(LoggerConfigFileName, Log4NetDefaultConfig.DefaultLog4NetConfig);
                }
                catch (Exception e)
                {
                    Log.Error("Unable to save logging config", e);
                    Log.Error("Using built-in default logging configuration.");
                    return;
                }
            }
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        private static void AutoUpdater_ApplicationExitEvent()
        {
            Log.Info("Closing application in preparation for update...");
            Environment.Exit(0);
        }

        // With some help from https://stackoverflow.com/a/10669337/1687436
        private static void StartThread(DiscordRpcClient discordRpcClient, Titanfall2Api tf2Api, AutoResetEvent userExitEvent)
        {
            _presenceUpdatingThread = new Thread(new PresenceUpdateThread(discordRpcClient, tf2Api, userExitEvent).Run)
            {
                Name = "Discord RP Updating Thread"
            };
            _presenceUpdatingThread.Start();
        }

        public static bool HasWritePermission(string tempfilepath)
        {
            try
            {
                File.Create(tempfilepath + "temp.txt").Close();
                File.Delete(tempfilepath + "temp.txt");
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }

            return true;
        }
    }
}