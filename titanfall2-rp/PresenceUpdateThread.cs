using System;
using System.Reflection;
using System.Threading;
using System.Timers;
using DiscordRPC;
using log4net;

namespace titanfall2_rp
{
    public class PresenceUpdateThread
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private static System.Timers.Timer? presenceUpdateTimer;
        private const int ProcessOpenWaitTimeInMinutes = 1;
        private readonly DiscordRpcClient _discordRpcClient;
        private readonly Titanfall2Api _tf2Api;
        private readonly EventWaitHandle _userExitEvent;

        public PresenceUpdateThread(DiscordRpcClient discordRpcClient, Titanfall2Api tf2Api, AutoResetEvent userExitEvent)
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
            presenceUpdateTimer!.Stop();
            presenceUpdateTimer.Dispose();
        }

        private void SetTimer()
        {
            // Create a timer with a two second interval.
            presenceUpdateTimer = new System.Timers.Timer(Program.StatusRefreshTimeInMs);
            // Hook up the Elapsed event for the timer. 
            presenceUpdateTimer.Elapsed += OnTimedEvent;
            presenceUpdateTimer.AutoReset = true;
            presenceUpdateTimer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Thread.CurrentThread.Name = "TimedPersistenceUpdate-" + Thread.CurrentThread.ManagedThreadId;
            try
            {
                SetCurrentPresence(_discordRpcClient, _tf2Api);
                // See https://stackoverflow.com/a/1650120/1687436 for determining equality between double and int
                if (Math.Abs(presenceUpdateTimer!.Interval - Program.StatusRefreshTimeInMs) > 0.0000001)
                {
                    // We must've changed the interval previously.
                    // It should only be a one-time operation so let's set it back to the correct value. 
                    presenceUpdateTimer.Interval = Program.StatusRefreshTimeInMs;
                }
            }
            catch (Exception exception)
            {
                if (exception is InvalidOperationException)
                {
                    Log.WarnFormat("Titanfall 2 process not found. Waiting {0} minute(s) and trying again.",
                        ProcessOpenWaitTimeInMinutes);
                    // Set the timer to wait longer
                    presenceUpdateTimer!.Interval = ProcessOpenWaitTimeInMinutes * 60 * 1000;
                }
                else
                {
                    Log.Warn("Failed to perform Titanfall 2 rich presence update. Waiting " +
                             Program.StatusRefreshTimeInSeconds + " seconds and trying again.", exception);
                    // No need to change the timer interval before trying again
                }
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
                    LargeImageText = "Large Image Text",
                }
            });
        }

        private static (string, string, Timestamps?, Assets? assets) GetDetailsAndState(Titanfall2Api tf2Api)
        {
            string gameDetails = "";
            string gameState = "";
            Timestamps? timestamps = null;
            Assets? assets = null;

            if (tf2Api.GetGameModeAndMapName().Equals("Main Menu"))
            {
                gameDetails = "Main Menu";
                timestamps = new Timestamps(Program.StartTimestamp);
            }
            else if (tf2Api.GetGameModeName().Contains("Campaign"))
            {
                gameDetails = "Campaign (" + tf2Api.GetSinglePlayerDifficulty() + ")";
                gameState = tf2Api.GetFriendlyMapName();
                timestamps = new Timestamps(Program.StartTimestamp);
            }
            else if (tf2Api.GetMultiplayerMapName().Equals("mp_lobby"))
            {
                gameDetails = "In a lobby";
                timestamps = new Timestamps(Program.StartTimestamp);
            }
            // Besides mp_lobby, any mp map will be prefixed with mp_. Grab the specific details then!
            else if (tf2Api.GetMultiplayerMapName().StartsWith("mp_"))
            {
                return GameDetailsProvider.GetMultiplayerDetails(tf2Api, Program.StartTimestamp);
            }
            // Could be main menu, might be some other random thing. This can be cleaned up later
            else
            {
                gameDetails = tf2Api.GetGameModeName();
                timestamps = new Timestamps(Program.StartTimestamp);
            }

            return (gameDetails, gameState, timestamps, assets);
        }
    }
}