using System;
using Ardalis.SmartEnum;

namespace titanfall2_rp.enums
{
    /// <summary>
    /// A quick and dirty way to keep a list of maps that isn't just strings so they can be strongly typed for future
    /// use.
    /// </summary>
    public sealed class Map : SmartEnum<Map, string>
    {
        public int MapPreviewVariants { get; }

        public Map(string name, string mapNameInEnglish, int mapPreviewVariants) : base(name, mapNameInEnglish)
        {
            MapPreviewVariants = mapPreviewVariants;
        }

        // ReSharper disable InconsistentNaming
        // ReSharper disable IdentifierTypo
        public static readonly Map mp_angel_city = new(nameof(mp_angel_city), "Angel City", 1);
        public static readonly Map mp_black_water_canal = new(nameof(mp_black_water_canal), "Black Water Canal", 1);
        public static readonly Map mp_coliseum = new(nameof(mp_coliseum), "The Coliseum", 1);
        public static readonly Map mp_coliseum_column = new(nameof(mp_coliseum_column), "Pillars", 1);
        public static readonly Map mp_colony02 = new(nameof(mp_colony02), "Colony", 1);
        public static readonly Map mp_complex3 = new(nameof(mp_complex3), "Complex", 1);
        public static readonly Map mp_crashsite3 = new(nameof(mp_crashsite3), "Crash Site", 1);
        public static readonly Map mp_drydock = new(nameof(mp_drydock), "Drydock", 1);
        public static readonly Map mp_eden = new(nameof(mp_eden), "Eden", 1);
        public static readonly Map mp_forwardbase_kodai = new(nameof(mp_forwardbase_kodai), "Forwardbase Kodai", 1);
        public static readonly Map mp_glitch = new(nameof(mp_glitch), "Glitch", 1);
        public static readonly Map mp_grave = new(nameof(mp_grave), "Grave", 1);
        public static readonly Map mp_homestead = new(nameof(mp_homestead), "Homestead", 1);
        public static readonly Map mp_lf_deck = new(nameof(mp_lf_deck), "Deck", 1);
        public static readonly Map mp_lf_meadow = new(nameof(mp_lf_meadow), "Meadow", 1);
        public static readonly Map mp_lf_stacks = new(nameof(mp_lf_stacks), "Stacks", 1);
        public static readonly Map mp_lf_township = new(nameof(mp_lf_township), "Township", 1);
        public static readonly Map mp_lf_traffic = new(nameof(mp_lf_traffic), "Traffic", 1);
        public static readonly Map mp_lf_uma = new(nameof(mp_lf_uma), "UMA", 1);
        public static readonly Map mp_lobby = new(nameof(mp_lobby), "mp_lobby", 1);
        public static readonly Map mp_relic02 = new(nameof(mp_relic02), "Relic", 1);
        public static readonly Map mp_rise = new(nameof(mp_rise), "Rise", 1);
        public static readonly Map mp_thaw = new(nameof(mp_thaw), "Thaw", 1);
        public static readonly Map mp_wargames = new(nameof(mp_wargames), "War Games", 1);
        // TODO: All of these SP maps need their campaign names! Oh boy! Time to replay the campaign!
        public static readonly Map sp_beacon = new(nameof(sp_beacon), "sp_beacon", 1);
        public static readonly Map sp_beacon_spoke0 = new(nameof(sp_beacon_spoke0), "sp_beacon_spoke0", 1);
        public static readonly Map sp_boomtown = new(nameof(sp_boomtown), "sp_boomtown", 1);
        public static readonly Map sp_boomtown_end = new(nameof(sp_boomtown_end), "sp_boomtown_end", 1);
        public static readonly Map sp_boomtown_start = new(nameof(sp_boomtown_start), "sp_boomtown_start", 1);
        public static readonly Map sp_crashsite = new(nameof(sp_crashsite), "sp_crashsite", 1);
        public static readonly Map sp_hub_timeshift = new(nameof(sp_hub_timeshift), "sp_hub_timeshift", 1);
        public static readonly Map sp_s2s = new(nameof(sp_s2s), "sp_s2s", 1);
        public static readonly Map sp_sewers1 = new(nameof(sp_sewers1), "sp_sewers1", 1);
        public static readonly Map sp_skyway_v1 = new(nameof(sp_skyway_v1), "sp_skyway_v1", 1);
        public static readonly Map sp_tday = new(nameof(sp_tday), "sp_tday", 1);
        public static readonly Map sp_timeshift_spoke02 = new(nameof(sp_timeshift_spoke02), "sp_timeshift_spoke02", 1);
        public static readonly Map sp_training = new(nameof(sp_training), "sp_training", 1);
        // ReSharper restore IdentifierTypo
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// If there are multiple preview images for this map (as is the case with the single-player maps, this method
        /// will randomly return one of the asset names for this specific map. See the map previews in the assets
        /// folder for more context.
        /// </summary>
        /// <returns>a random applicable map preview name for the given map</returns>
        public string GetRandomPreview()
        {
            return MapPreviewVariants.Equals(1) ? Name : $"{Name}_{new Random().Next(1, MapPreviewVariants)}";
        }

        public string InEnglish()
        {
            return Value;
        }
    }
}