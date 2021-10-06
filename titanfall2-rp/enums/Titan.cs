namespace titanfall2_rp.enums
{
    public enum Titan
    {
        Ion,
        Scorch,
        Northstar,
        // ReSharper disable once IdentifierTypo
        Ronin,
        Tone,
        Legion,
        Monarch,
    }

    internal static class TitanMethods
    {
        public static Titan GetTitan(int titanValue)
        {
            return (Titan)titanValue;
        }

        public static string ToFriendlyString(this Titan titan)
        {
            return titan.ToString();
        }

        public static string GetAssetName(this Titan titan)
        {
            return titan.ToString().ToLower();
        }
    }
}