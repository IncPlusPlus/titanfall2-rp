using System;
using DiscordRPC;

namespace titanfall2_rp
{
    public static class GameDetailsProvider
    {
        public static (string, string, Timestamps?, Assets? assets) GetMultiplayerDetails(Titanfall2Api tf2Api, DateTime gameOpenTimestamp)
        {
            string gameDetails = "";
            string gameState = "";
            var timestamps = new Timestamps(gameOpenTimestamp);
            var assets = new Assets() { LargeImageKey = tf2Api.GetMultiplayerMapName(), LargeImageText = tf2Api.GetFriendlyMapName(), };
            var currentGameMode = tf2Api.GetGameMode();
            switch (currentGameMode)
            {
                case GameMode.coliseum:
                    break;
                case GameMode.aitdm:
                    break;
                case GameMode.tdm:
                    break;
                case GameMode.cp:
                    break;
                case GameMode.at:
                    break;
                case GameMode.ctf:
                    break;
                case GameMode.lts:
                    break;
                case GameMode.ps:
                    break;
                case GameMode.speedball:
                    break;
                case GameMode.mfd:
                    break;
                case GameMode.ttdm:
                    break;
                case GameMode.fd_easy:
                    break;
                case GameMode.fd_normal:
                    break;
                case GameMode.fd_hard:
                    break;
                case GameMode.fd_insane:
                    break;
                case GameMode.solo:
                    break;
                default:
                    throw new ArgumentException("Unknown game mode '" + currentGameMode + "'.");
            }

            return (gameDetails, gameState, timestamps, assets);
        }
    }
}