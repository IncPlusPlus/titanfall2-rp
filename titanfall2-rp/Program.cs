using System;
using System.Text.RegularExpressions;
using System.Threading;
using DiscordRPC;
using DiscordRPC.Logging;


namespace titanfall2_rp
{
    class Program
    {
        // For some reason, discord will show 4 hours as the starting time unless I add 4 hours here.
        // Seems the 
        private static readonly DateTime StartTimestamp = DateTime.Now.AddHours(4);
        private static readonly int StatusRefreshTimeInSeconds = 5;

        static void Main(string[] args)
        {
            var tf2Api = new Titanfall2Api();

            DiscordRpcClient client = new DiscordRpcClient("877931149740089374");
            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

            //Subscribe to events
            client.OnReady += (sender, e) => { Console.WriteLine("Received Ready from user {0}", e.User.Username); };

            client.OnPresenceUpdate += (sender, e) => { Console.WriteLine("Received Update! {0}", e.Presence); };

            //Connect to the RPC
            client.Initialize();


            while (true)
            {
                try
                {
                    SetCurrentPresence(client, tf2Api);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine("Failed to perform Titanfall 2 rich presence update. Waiting " +
                                      StatusRefreshTimeInSeconds + " seconds and trying again.");
                }

                Thread.Sleep(StatusRefreshTimeInSeconds * 1000);
            }

            client.Dispose();
        }

        public static void SetCurrentPresence(DiscordRpcClient client, Titanfall2Api tf2Api)
        {
            var (gameDetails, gameState, timestamps, assets) = GetDetailsAndState(tf2Api);
            //Set the rich presence
            //Call this as many times as you want and anywhere in your code.
            client.SetPresence(new RichPresence()
            {
                Details = gameDetails,
                State = gameState,
                Timestamps = timestamps,
                Assets = assets == null ? new Assets()
                {
                    LargeImageKey = "icon-900x900",
                    LargeImageText = "Large Image Text",
                    // SmallImageKey = "image_small"
                } : assets
            });
        }

        public static (string, string, Timestamps, Assets assets) GetDetailsAndState(Titanfall2Api tf2Api)
        {
            Regex rg = new Regex("");
            string gameDetails = "";
            string gameState = "";
            Timestamps timestamps = null;
            Assets assets = null;

            if (tf2Api.GetGameModeName().Contains("Campaign"))
            {
                gameDetails = "Campaign (" + tf2Api.GetSinglePlayerDifficulty() + ")";
                gameState = tf2Api.GetFriendlyMapName();
                timestamps = new Timestamps(StartTimestamp);
            }
            else if (tf2Api.GetMultiplayerMapName().Equals("mp_lobby"))
            {
                gameDetails = "In a lobby";
                timestamps = new Timestamps(StartTimestamp);
            }
            else if (tf2Api.GetGameModeAndMapName().Contains("Attrition"))
            {
                gameDetails = tf2Api.GetGameModeName();
                gameState = tf2Api.GetFriendlyMapName();
            }
            // Could be main menu, might be some other random thing. This can be cleaned up later
            else
            {
                gameDetails = tf2Api.GetGameModeName();
                timestamps = new Timestamps(StartTimestamp);
            }

            return (gameDetails, gameState, timestamps, assets);
        }
    }
}