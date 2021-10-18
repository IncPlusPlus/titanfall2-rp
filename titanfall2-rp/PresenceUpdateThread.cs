using System;
using System.Reflection;
using System.Threading;
using System.Timers;
using DiscordRPC;
using log4net;
using titanfall2_rp.enums;
using titanfall2_rp.SegmentManager;
using titanfall2_rp.updater;

namespace titanfall2_rp
{
    public class PresenceUpdateThread
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private static System.Timers.Timer? _presenceUpdateTimer;
        private const int ProcessOpenWaitTimeInMinutes = 1;
        private readonly DiscordRpcClient _discordRpcClient;
        private readonly Titanfall2Api _tf2Api;
        private readonly EventWaitHandle _userExitEvent;

        public PresenceUpdateThread(DiscordRpcClient discordRpcClient, Titanfall2Api tf2Api,
            AutoResetEvent userExitEvent)
        {
            _discordRpcClient = discordRpcClient;
            _tf2Api = tf2Api;
            _userExitEvent = userExitEvent;
        }

        public void Run()
        {
            // Set up a timer that'll regularly run our method that updates the rich presence
            SetTimer();

            // Wait for the main method in Program to inform us that it's time to exit
            _userExitEvent.WaitOne();
            Log.Info("User has requested that the program exits. Stopping presence updates.");

            // Stop anc clean up the timer
            _presenceUpdateTimer!.Stop();
            _presenceUpdateTimer.Dispose();
        }

        private void SetTimer()
        {
            // Create a timer with a two second interval.
            _presenceUpdateTimer = new System.Timers.Timer(RichPresenceManager.StatusRefreshTimeInMs);
            // Hook up the Elapsed event for the timer. 
            _presenceUpdateTimer.Elapsed += OnTimedEvent;
            _presenceUpdateTimer.AutoReset = false;
            _presenceUpdateTimer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Thread.CurrentThread.Name = "TimedPersistenceUpdate-" + Thread.CurrentThread.ManagedThreadId;
            try
            {
                SetCurrentPresence(_discordRpcClient, _tf2Api);
                // See https://stackoverflow.com/a/1650120/1687436 for determining equality between double and int
                if (Math.Abs(_presenceUpdateTimer!.Interval - RichPresenceManager.StatusRefreshTimeInMs) > 0.0000001)
                {
                    // We must've changed the interval previously.
                    // It should only be a one-time operation so let's set it back to the correct value. 
                    _presenceUpdateTimer.Interval = RichPresenceManager.StatusRefreshTimeInMs;
                }
            }
            catch (Exception exception)
            {
                if (exception is InvalidOperationException)
                {
                    Log.WarnFormat("Titanfall 2 process not found. Waiting {0} minute(s) and trying again.",
                        ProcessOpenWaitTimeInMinutes);
                    // Set the timer to wait longer
                    _presenceUpdateTimer!.Interval = ProcessOpenWaitTimeInMinutes * 60 * 1000;
                    // Clearing the current presence. This should be fine to call every minute or so.
                    // The purpose of this is to clear the status if the game closes.
                    _discordRpcClient.ClearPresence();
                }
                else
                {
                    Log.Warn("Failed to perform Titanfall 2 rich presence update. Waiting " +
                             RichPresenceManager.StatusRefreshTimeInSeconds + " seconds and trying again.", exception);
                    SegmentManager.SegmentManager.TrackEvent(TrackableEvent.GameplayInfoFailure, exception);
                    // No need to change the timer interval before trying again
                }
            }
            finally
            {
                // Only allow the timer to start again after OnTimedEvent has completed executing
                // https://stackoverflow.com/a/56442085/1687436
                _presenceUpdateTimer!.Enabled = true;
            }
        }

        private static void SetCurrentPresence(DiscordRpcClient client, Titanfall2Api tf2Api)
        {
            var (gameDetails, gameState, timestamps, assets) = GetDetailsAndState(tf2Api);
            //Set the rich presence
            //Call this as many times as you want and anywhere in your code.
            client.SetPresence(new RichPresence
            {
                Details = gameDetails,
                State = gameState,
                Timestamps = timestamps,
                Assets = assets ?? new Assets
                {
                    LargeImageKey = "icon-900x900",
                    LargeImageText = "titanfall2-rp " + UpdateHelper.AppVersion + " by IncPlusPlus",
                }
            });
        }

        private static (string, string, Timestamps?, Assets? assets) GetDetailsAndState(Titanfall2Api tf2Api)
        {
            string gameDetails = "";
            string gameState = "";
            Timestamps? timestamps = null;
            Assets? assets = null;

            if (tf2Api.GetMultiplayerMapName() == "")
            {
                gameDetails = "Main Menu";
                timestamps = new Timestamps(ProcessNetApi.StartTimestamp);
            }
            else if (tf2Api.GetMultiplayerMapName().Equals("mp_lobby"))
            {
                gameDetails = "In a lobby";
                timestamps = new Timestamps(ProcessNetApi.StartTimestamp);
            }
            else if (tf2Api.GetGameMode() == GameMode.solo)
            {
                gameDetails = $"{tf2Api.GetGameModeName()} ({tf2Api.GetSinglePlayerDifficulty()})";
                gameState = Map.FromName(tf2Api.GetSinglePlayerMapName()).InEnglish();
                timestamps = new Timestamps(ProcessNetApi.StartTimestamp);
                assets = GameDetailsProvider.GetSinglePlayerAssets(tf2Api);
            }
            // Besides mp_lobby, any mp map will be prefixed with mp_. Grab the specific details then!
            else if (tf2Api.GetMultiplayerMapName().StartsWith("mp_"))
            {
                return GameDetailsProvider.GetMultiplayerDetails(tf2Api, ProcessNetApi.StartTimestamp);
            }
            // Could be main menu, might be some other random thing. This can be cleaned up later
            else
            {
                gameDetails = tf2Api.GetGameModeName();
                timestamps = new Timestamps(ProcessNetApi.StartTimestamp);
            }

            return (gameDetails, gameState, timestamps, assets);
        }
    }
}