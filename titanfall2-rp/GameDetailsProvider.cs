using System;
using DiscordRPC;
using titanfall2_rp.enums;

namespace titanfall2_rp
{
    public static class GameDetailsProvider
    {
        public static (string, string, Timestamps?, Assets? assets) GetMultiplayerDetails(Titanfall2Api tf2Api,
            DateTime gameOpenTimestamp)
        {
            var mpStats = tf2Api.GetMultiPlayerGameStats();
            var gameMode = tf2Api.GetGameMode();
            var gameDetails = gameMode == GameMode.UNKNOWN_GAME_MODE
                ? $"Game mode: {tf2Api.GetGameModeCodeName()}"
                : gameMode.ToFriendlyString();
            var gameState = mpStats.GetGameState();
            var timestamps = new Timestamps(gameOpenTimestamp);
            var map = Map.FromName(tf2Api.GetMultiplayerMapName());
            var playerInTitan = tf2Api.IsPlayerInTitan();
            var assets = new Assets
            {
                LargeImageKey = map.ToString(),
                LargeImageText = map.InEnglish(),
                SmallImageKey = playerInTitan
                    ? tf2Api.GetTitan().GetAssetName()
                    : mpStats.GetCurrentFaction().GetAssetName(),
                SmallImageText = playerInTitan
                    ? tf2Api.GetTitan().ToFriendlyString()
                    : mpStats.GetCurrentFaction().ToFriendlyString(),
            };
            return (gameDetails, gameState, timestamps, assets);
        }

        public static Assets GetSinglePlayerAssets(Titanfall2Api tf2Api)
        {
            return new Assets()
            {
                LargeImageKey = GetRandomImageNameForCurrentMap(tf2Api),
                LargeImageText = tf2Api.GetSinglePlayerMapName(),
            };
        }

        /// <summary>
        /// Randomly get the asset name of one of the multiple assets. This is intended for usage with single-player
        /// on account of them having multiple preview images. However, this should still work fine for multiplayer
        /// maps (although I likely won't use it for that purpose).
        /// </summary>
        /// <param name="tf2Api">the API instance to fetch the current SP map</param>
        /// <returns>one of the asset names of of the applicable map</returns>
        private static string GetRandomImageNameForCurrentMap(Titanfall2Api tf2Api)
        {
            return Map.FromName(tf2Api.GetSinglePlayerMapName()).GetRandomPreview();
        }
    }
}