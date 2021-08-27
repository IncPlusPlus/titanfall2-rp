using System;
using System.Reflection;
using System.Threading;
using DiscordRPC;
using log4net;

namespace titanfall2_rp
{
    public class PresenceUpdateThread
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private const int ProcessOpenWaitTimeInMinutes = 1;
        private readonly DiscordRpcClient _discordRpcClient;
        private readonly Titanfall2Api _tf2Api;

        public PresenceUpdateThread(DiscordRpcClient discordRpcClient, Titanfall2Api tf2Api)
        {
            _discordRpcClient = discordRpcClient;
            _tf2Api = tf2Api;
        }

        public void Run()
        {
            while (true)
            {
                if (Program.HasUserRequestedExit())
                {
                    Log.Info("User has requested that the program exits. Stopping presence updates.");
                    return;
                }
                try
                {
                    SetCurrentPresence(_discordRpcClient, _tf2Api);
                }
                catch (Exception e)
                {
                    if (e is InvalidOperationException)
                    {
                        Log.WarnFormat("Titanfall 2 process not found. Waiting {0} minute(s) and trying again.", ProcessOpenWaitTimeInMinutes);
                        Thread.Sleep(ProcessOpenWaitTimeInMinutes * 60 * 1000);
                    }
                    else
                    {
                        Log.Warn("Failed to perform Titanfall 2 rich presence update. Waiting " +
                                 Program.StatusRefreshTimeInSeconds + " seconds and trying again.", e);
                        Thread.Sleep(Program.StatusRefreshTimeInSeconds * 1000);
                    }
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