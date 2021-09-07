using System;

namespace titanfall2_rp.enums
{
    /// <summary>
    /// Represents the user's current faction. Interestingly, the order of the factions is actually different from
    /// the order shown in the in-game menu.
    /// </summary>
    public enum Faction
    {
        MarauderCorps = 3,
        ApexPredators = 0,
        VinsonDynamics = 2,
        AngelCityElite = 4,
        The64 = 1,
        AresDivision = 5,
        // ReSharper disable once IdentifierTypo
        MarvinsFinestHour = 6,
    }

    internal static class FactionMethods
    {
        /// <summary>
        /// Get the Faction represented by the integer (technically it's a byte) faction value
        /// </summary>
        /// <param name="factionValue">the numeric value of the currently set faction</param>
        /// <returns>the faction in enum form</returns>
        public static Faction GetFaction(int factionValue)
        {
            return (Faction)factionValue;
        }

        /// <summary>
        /// Get the user-friendly name of the given Faction
        /// </summary>
        /// <param name="faction">a Faction enum</param>
        /// <returns>the name of the Faction</returns>
        /// <exception cref="ArgumentOutOfRangeException">thrown if you've done some sort of satanic ritual</exception>
        public static string ToFriendlyString(this Faction faction)
        {
            return faction switch
            {
                Faction.MarauderCorps => "Marauder Corps",
                Faction.ApexPredators => "Apex Predators",
                Faction.VinsonDynamics => "Vinson Dynamics",
                Faction.AngelCityElite => "Angel City Elite",
                Faction.The64 => "The 6-4",
                Faction.AresDivision => "ARES Division",
                Faction.MarvinsFinestHour => "Marvin's Finest Hour",
                _ => throw new ArgumentOutOfRangeException(nameof(faction), faction, null)
            };
        }

        /// <summary>
        /// Gets the Discord asset name for the image of the given Faction
        /// </summary>
        /// <param name="faction">a Faction to find the artwork of</param>
        /// <returns>the string representing the asset name that's been uploaded to Discord</returns>
        /// <exception cref="ArgumentOutOfRangeException">thrown if you've done something you shouldn't have</exception>
        public static string GetAssetName(this Faction faction)
        {
            return faction switch
            {
                Faction.MarauderCorps => "marauder_corps",
                Faction.ApexPredators => "apex_predators",
                Faction.VinsonDynamics => "vinson_dynamics",
                Faction.AngelCityElite => "angel_city_elite",
                Faction.The64 => "the_6-4",
                Faction.AresDivision => "ares_division",
                Faction.MarvinsFinestHour => "marvin_s_finest_hour",
                _ => throw new ArgumentOutOfRangeException(nameof(faction), faction, null)
            };
        }
    }
}