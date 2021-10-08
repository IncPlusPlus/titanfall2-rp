using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DiscordRPC;
using titanfall2_rp.enums;

namespace titanfall2_rp
{
    public static class GameDetailsProvider
    {
        /// <summary>
        /// This string was created by listing all of the files in `assets/map previews/square` with `ls -1`
        /// and then manually chopping off the file extensions. Then I put it in a verbatim string and used the context
        /// actions menu to turn it into a regular string. In verbatim string form, it had both carriage returns
        /// and newlines. In the regular string form, it's just newlines.
        /// KEEP THIS CONSISTENT WITH THE ASSET NAMES!!!
        /// </summary>
        [SuppressMessage("ReSharper", "StringLiteralTypo")] private static readonly string[] AllMapAssetNames = "mp_angel_city\nmp_black_water_canal\nmp_coliseum\nmp_coliseum_column\nmp_colony02\nmp_complex3\nmp_crashsite3\nmp_drydock\nmp_eden\nmp_forwardbase_kodai\nmp_glitch\nmp_grave\nmp_homestead\nmp_lf_deck\nmp_lf_meadow\nmp_lf_stacks\nmp_lf_township\nmp_lf_traffic\nmp_lf_uma\nmp_relic02\nmp_rise\nmp_thaw\nmp_wargames\nsp_beacon_1\nsp_beacon_2\nsp_beacon_3\nsp_beacon_4\nsp_beacon_5\nsp_beacon_spoke0_1\nsp_beacon_spoke0_2\nsp_beacon_spoke0_3\nsp_boomtown_1\nsp_boomtown_2\nsp_boomtown_3\nsp_boomtown_end_1\nsp_boomtown_end_2\nsp_boomtown_end_3\nsp_boomtown_end_4\nsp_boomtown_start_1\nsp_boomtown_start_2\nsp_crashsite_1\nsp_crashsite_2\nsp_hub_timeshift_1\nsp_hub_timeshift_2\nsp_hub_timeshift_3\nsp_s2s_1\nsp_s2s_2\nsp_s2s_3\nsp_s2s_4\nsp_s2s_5\nsp_sewers1_1\nsp_sewers1_2\nsp_sewers1_3\nsp_sewers1_4\nsp_sewers1_5\nsp_sewers1_6\nsp_sewers1_7\nsp_sewers1_8\nsp_skyway_v1_1\nsp_skyway_v1_2\nsp_skyway_v1_3\nsp_skyway_v1_4\nsp_skyway_v1_5\nsp_skyway_v1_6\nsp_tday_1\nsp_tday_2\nsp_tday_3\nsp_tday_4\nsp_tday_5\nsp_timeshift_spoke02_1\nsp_timeshift_spoke02_2\nsp_timeshift_spoke02_3\nsp_timeshift_spoke02_4\nsp_training_1\nsp_training_2".Split("\n");

        public static (string, string, Timestamps?, Assets? assets) GetMultiplayerDetails(Titanfall2Api tf2Api, DateTime gameOpenTimestamp)
        {
            var mpStats = tf2Api.GetMultiPlayerGameStats();
            string gameDetails = tf2Api.GetGameMode().ToFriendlyString();
            string gameState = $"Score: {mpStats.GetTeam1Score()} - {mpStats.GetTeam2Score()}";
            var timestamps = new Timestamps(gameOpenTimestamp);
            var playerInTitan = tf2Api.IsPlayerInTitan();
            var assets = new Assets
            {
                LargeImageKey = tf2Api.GetMultiplayerMapName(),
                LargeImageText = tf2Api.GetFriendlyMapName(),
                SmallImageKey = playerInTitan ? tf2Api.GetTitan().GetAssetName() : tf2Api.GetMultiPlayerGameStats().GetCurrentFaction().GetAssetName(),
                SmallImageText = playerInTitan ? tf2Api.GetTitan().ToFriendlyString() : tf2Api.GetMultiPlayerGameStats().GetCurrentFaction().ToFriendlyString(),
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
            string currentMapName = tf2Api.GetSinglePlayerMapName();
            var candidates = AllMapAssetNames.Where(assetName => assetName.StartsWith(currentMapName));
            return candidates.RandomElement();
        }

        // https://stackoverflow.com/a/7259419/1687436
        private static T RandomElement<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.RandomElementUsing<T>(new Random());
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private static T RandomElementUsing<T>(this IEnumerable<T> enumerable, Random rand)
        {
            var index = rand.Next(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }
    }
}