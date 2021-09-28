﻿using System;
using System.Reflection;
using System.Threading;
using Common;
using DiscordRPC;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using log4net;
using titanfall2_rp.misc;
using titanfall2_rp.updater;

namespace titanfall2_rp
{
    public delegate void OnPresenceUpdateEvent(object sender, PresenceMessage args);

    public class RichPresenceManager
    {
        public event OnPresenceUpdateEvent? OnPresenceUpdate;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private const string LogFileName = "titanfall2-rp.log";
        private const string LoggerConfigFileName = "log4net.config";
        public const int StatusRefreshTimeInSeconds = 5;
        public const int StatusRefreshTimeInMs = StatusRefreshTimeInSeconds * 1000;
        private readonly AutoResetEvent _userRequestedExit;
        private readonly DiscordRpcClient _discordRpcClient;
        private readonly Thread _presenceUpdatingThread;

        public RichPresenceManager()
        {
            _discordRpcClient = new DiscordRpcClient("877931149740089374");
            _userRequestedExit = new AutoResetEvent(false);
            _presenceUpdatingThread = new Thread(new PresenceUpdateThread(_discordRpcClient, new Titanfall2Api(), _userRequestedExit).Run)
            {
                Name = "Discord RP Updating Thread"
            };
        }

        public void Begin()
        {
            Log4NetConfig.ConfigureLogger(LogFileName, LoggerConfigFileName);
            // Set this thread's name. This way it indicates which thread is the main thread (although this is usually 1)
            Thread.CurrentThread.Name = "Main-" + Thread.CurrentThread.ManagedThreadId;

            Log.InfoFormat(
                "IncPlusPlus' TF|2 Discord Rich Presence Tool version {0} (https://github.com/IncPlusPlus/titanfall2-rp)",
                UpdateHelper.AppVersion);

            if (!EnvironmentUtils.AppDirectoryWritable())
            {
                Log.ErrorFormat(
                    "This program needs write permissions to the current directory." +
                    "\nThe directory '{0}' is not writeable!!!" +
                    "\nThis could affect logging!",
                    AppContext.BaseDirectory!);
            }

            Log.Info("Starting Titanfall 2 Discord Rich Presence.");

            // Make the RPC library use a custom implemented logger (this is such a cool library feature)
            _discordRpcClient.Logger = new Log4NetDiscordLogger { Level = LogLevel.Warning };

            // Subscribe to events
            _discordRpcClient.OnReady += (sender, e) =>
            {
                Log.DebugFormat("Received Ready from user {0}", e.User.Username);
            };
            // _discordRpcClient.OnClose += (sender, e) => { Log.DebugFormat("Received Close with code {0}. Reason: {1}", e.Code, e.Reason); };
            // _discordRpcClient.OnError += (sender, message) => {Log.DebugFormat("Received error code {0}. Message was: {1}", message.Code, message.Message); };
            _discordRpcClient.OnPresenceUpdate += (sender, e) =>
            {
                Log.DebugFormat("Received Update! {0}", e.Presence);
                OnPresenceUpdate?.Invoke(this, e);
            };

            // Connect to the RPC
            _discordRpcClient.Initialize();

            // Start the thread continuously updates the RP status
            _presenceUpdatingThread.Start();

            UpdateHelper.Updater.Update();
        }

        public void Stop()
        {
            Log.Info("User has requested that the program exits.");

            // Cleanup
            Log.Debug("Disposing of Discord RPC client");
            _discordRpcClient.Dispose();
            Log.Debug("Informing the presence updating thread that the user has requested it's time to exit...");
            _userRequestedExit.Set();
            // Waits for the presence updating thread to close
            _presenceUpdatingThread.Join();
            // Releases the resources used for the events (only after the thread that was using this has exited)
            _userRequestedExit.Close();
            Log.Info("Closing...");
        }
    }
}