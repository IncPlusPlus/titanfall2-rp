using System;
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
        static void Main(string[] args)
        {
            
            var tf2Api = new Titanfall2Api();
            var health = tf2Api.GetPlayerHealth();
            Console.WriteLine("Health is " + health);

            DiscordRpcClient client = new DiscordRpcClient("877931149740089374");
            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };
            
            //Subscribe to events
            client.OnReady += (sender, e) => { Console.WriteLine("Received Ready from user {0}", e.User.Username); };

            client.OnPresenceUpdate += (sender, e) => { Console.WriteLine("Received Update! {0}", e.Presence); };

            //Connect to the RPC
            client.Initialize();


            var i = 20;
            while (i > 0)
            {
                setCurrentPresence(client, tf2Api);
                Thread.Sleep(10000);
                i--;
            }

            client.Dispose();
        }

        public static void setCurrentPresence(DiscordRpcClient client, Titanfall2Api tf2Api)
        {
            var (gameDetails, gameState, timestamps) = GetDetailsAndState(tf2Api);
            //Set the rich presence
            //Call this as many times as you want and anywhere in your code.
            client.SetPresence(new RichPresence()
            {
                Details = gameDetails,
                State = gameState,
                Timestamps = timestamps,
                Assets = new Assets()
                {
                    LargeImageKey = "icon-900x900",
                    LargeImageText = "Large Image Text",
                    // SmallImageKey = "image_small"
                }
            });
        }

        public static (string, string, Timestamps) GetDetailsAndState(Titanfall2Api tf2Api)
        {
            string gameDetails="";
            string gameState = "";
            Timestamps timestamps = null;

            if (tf2Api.GetGameModeName().Contains("Campaign"))
            {
                gameDetails = "Campaign (" + tf2Api.GetSinglePlayerDifficulty() + ")";
                gameState = tf2Api.GetSinglePlayerMissionName();
                timestamps = new Timestamps(StartTimestamp);
            }
            
            return (gameDetails,gameState, timestamps);
        }
    }
}