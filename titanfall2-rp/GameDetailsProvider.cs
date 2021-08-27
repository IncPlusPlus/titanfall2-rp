using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DiscordRPC;

namespace titanfall2_rp
{
    public static class GameDetailsProvider
    {
        /// <summary>
        /// This string was created by listing all of the files in `assets/map previews/square` with `ls -1`
        /// and then manually chopping off the file extensions. KEEP THIS CONSISTENT WITH THE ASSET NAMES!!!
        /// </summary>
        [SuppressMessage("ReSharper", "StringLiteralTypo")] private static readonly string[] AllMapAssetNames = @"mp_angel_city
mp_black_water_canal
mp_coliseum
mp_coliseum_column
mp_colony02
mp_complex3
mp_crashsite3
mp_drydock
mp_eden
mp_forwardbase_kodai
mp_glitch
mp_grave
mp_homestead
mp_lf_deck
mp_lf_meadow
mp_lf_stacks
mp_lf_township
mp_lf_traffic
mp_lf_uma
mp_relic02
mp_rise
mp_thaw
mp_wargames
sp_beacon_1
sp_beacon_2
sp_beacon_3
sp_beacon_4
sp_beacon_5
sp_beacon_spoke0_1
sp_beacon_spoke0_2
sp_beacon_spoke0_3
sp_boomtown_1
sp_boomtown_2
sp_boomtown_3
sp_boomtown_end_1
sp_boomtown_end_2
sp_boomtown_end_3
sp_boomtown_end_4
sp_boomtown_start_1
sp_boomtown_start_2
sp_crashsite_1
sp_crashsite_2
sp_hub_timeshift_1
sp_hub_timeshift_2
sp_hub_timeshift_3
sp_s2s_1
sp_s2s_2
sp_s2s_3
sp_s2s_4
sp_s2s_5
sp_sewers1_1
sp_sewers1_2
sp_sewers1_3
sp_sewers1_4
sp_sewers1_5
sp_sewers1_6
sp_sewers1_7
sp_sewers1_8
sp_skyway_v1_1
sp_skyway_v1_2
sp_skyway_v1_3
sp_skyway_v1_4
sp_skyway_v1_5
sp_skyway_v1_6
sp_tday_1
sp_tday_2
sp_tday_3
sp_tday_4
sp_tday_5
sp_timeshift_spoke02_1
sp_timeshift_spoke02_2
sp_timeshift_spoke02_3
sp_timeshift_spoke02_4
sp_training_1
sp_training_2".Split("\n");

        public static (string, string, Timestamps?, Assets? assets) GetMultiplayerDetails(Titanfall2Api tf2Api, DateTime gameOpenTimestamp)
        {
            var currentGameMode = tf2Api.GetGameMode();
            string gameDetails = tf2Api.GetGameMode().ToFriendlyString();
            string gameState = "";
            var timestamps = new Timestamps(gameOpenTimestamp);
            var assets = new Assets() { LargeImageKey = tf2Api.GetMultiplayerMapName(), LargeImageText = tf2Api.GetFriendlyMapName(), };
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

        public static Assets GetSinglePlayerAssets(Titanfall2Api tf2Api)
        {
            return new Assets()
            {
                LargeImageKey = GetRandomImageNameForCurrentMap(tf2Api),
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