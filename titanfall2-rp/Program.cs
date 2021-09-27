using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Common;
using DiscordRPC;
using DiscordRPC.Logging;
using log4net;
using log4net.Config;
using titanfall2_rp.misc;
using titanfall2_rp.updater;

namespace titanfall2_rp
{
    static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private const string LogFileName = "titanfall2-rp.log";
        private const string LoggerConfigFileName = "log4net.config";
        public const int StatusRefreshTimeInSeconds = 5;
        public const int StatusRefreshTimeInMs = StatusRefreshTimeInSeconds * 1000;
        private static AutoResetEvent _userRequestedExit = new AutoResetEvent(false);
        private static readonly DiscordRpcClient DiscordRpcClient = new DiscordRpcClient("877931149740089374");
        private static Thread? _presenceUpdatingThread;

        static void Main(string[] args)
        {
            Log4NetConfig.ConfigureLogger(LogFileName, LoggerConfigFileName);
            // Set this thread's name. This way it indicates which thread is the main thread (although this is usually 1)
            Thread.CurrentThread.Name = "Main-" + Thread.CurrentThread.ManagedThreadId;

            Log.InfoFormat("IncPlusPlus' TF|2 Discord Rich Presence Tool version {0} (https://github.com/IncPlusPlus/titanfall2-rp)", UpdateHelper.AppVersion);

            if (!EnvironmentUtils.AppDirectoryWritable())
            {
                Log.ErrorFormat(
                    "This program needs write permissions to the current directory." +
                    "\nThe directory '{0}' is not writeable!!!" +
                    "\nThis could affect logging!",
                    AppContext.BaseDirectory!);
            }

            UpdateHelper.Updater.Update();

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

        // With some help from https://stackoverflow.com/a/10669337/1687436
        private static void StartThread(DiscordRpcClient discordRpcClient, Titanfall2Api tf2Api, AutoResetEvent userExitEvent)
        {
            _presenceUpdatingThread = new Thread(new PresenceUpdateThread(discordRpcClient, tf2Api, userExitEvent).Run)
            {
                Name = "Discord RP Updating Thread"
            };
            _presenceUpdatingThread.Start();
        }
    }
}