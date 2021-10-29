using System;
using DiscordRPC;
using titanfall2_rp.enums;

namespace titanfall2_rp
{
    public static class GameDetailsProvider
    {
        public static (string, string, Timestamps?, Assets? assets) GetMultiplayerDetails(Titanfall2Api tf2Api, DateTime gameOpenTimestamp)
        {
            var mpStats = tf2Api.GetMultiPlayerGameStats();
            string gameDetails = tf2Api.GetGameMode().ToFriendlyString();
            string gameState = $"Score: {mpStats.GetMyTeamScore()} - {mpStats.GetEnemyTeamScore()}";
            var timestamps = new Timestamps(gameOpenTimestamp);
            var map = Map.FromName(tf2Api.GetMultiplayerMapName());
            var (smallKey, smallText) = GetSmallImageDetails(tf2Api);
            var assets = new Assets
            {
                LargeImageKey = map.ToString(),
                LargeImageText = map.InEnglish(),
                SmallImageKey = smallKey,
                SmallImageText = smallText
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

        /// <summary>
        /// Determines what the small image file name and display string should be
        /// </summary>
        /// <param name="tf2Api">an active API instance</param>
        /// <returns>A tuple containing the image key followed by the image text</returns>
        private static (string, string) GetSmallImageDetails(Titanfall2Api tf2Api)
        {
            var playerInTitan = tf2Api.IsPlayerInTitan();
            if (playerInTitan)
            {
                var titan = tf2Api.GetTitan();
                return (titan.GetAssetName(), titan.ToFriendlyString());
            }

            var gameMode = tf2Api.GetGameMode();
            if (gameMode.IsFrontierDefense())
            {
                return (gameMode.ToString(), gameMode.GetFrontierDefenseDifficultyString());
            }

            var faction = tf2Api.GetMultiPlayerGameStats().GetCurrentFaction();
            return (faction.GetAssetName(), faction.ToFriendlyString());
        }
    }
}