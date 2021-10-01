using System;
using System.IO;
using System.Reflection;

namespace titanfall2_rp.misc
{
    public static class EnvironmentUtils
    {
        /// <summary>
        /// This method is somewhat deceiving. This will return true as long as it's not on windows. This should be
        /// used in tandem with an IfOnWindows() conditional. That'll probably return true when in wine so you'd want
        /// another method to make sure you're truly in a Windows environment. That's what this method is useful for.
        /// </summary>
        /// <returns>whether this program is running inside WINE</returns>
        public static bool IsRunningInWine()
        {
            return System.Diagnostics.Process.GetProcessesByName("winlogon").Length == 0;
        }

        public static bool AppDirectoryWritable()
        {
            try
            {
                File.Create(AppContext.BaseDirectory + "temp.txt").Close();
                File.Delete(AppContext.BaseDirectory + "temp.txt");
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// There is a Windows-only build of this software as well as a console app targeted for use with Wine.
        /// This determines which of those versions is in use based on the <![CDATA[<AssemblyName/>]]> tag in the
        /// .csproj file. For the Wine release, this is titanfall2-rp-Wine. For Windows, it's titanfall2-rp.
        /// </summary>
        /// <returns>Windows, Wine, or UNKNOWN</returns>
        public static string GetReleaseEdition()
        {
            return (Assembly.GetEntryAssembly()?.GetName().Name ?? "") switch
            {
                "titanfall2-rp" => "Windows",
                "titanfall2-rp-Wine" => "Wine",
                _ => "UNKNOWN"
            };
        }

        /// <summary>
        /// https://stackoverflow.com/a/38795621/1687436
        /// </summary>
        /// <returns>the operating system name in its most simple form</returns>
        public static string GetBasicOsName()
        {
            string? windir = Environment.GetEnvironmentVariable("windir");
            if (!string.IsNullOrEmpty(windir) && windir.Contains(@"\") && Directory.Exists(windir))
            {
                return "Windows";
            }
            else if (File.Exists(@"/proc/sys/kernel/ostype"))
            {
                string osType = File.ReadAllText(@"/proc/sys/kernel/ostype");
                return osType.StartsWith("Linux", StringComparison.OrdinalIgnoreCase) ? "Linux" : osType;
            }
            else if (File.Exists(@"/System/Library/CoreServices/SystemVersion.plist"))
            {
                // Note: iOS gets here too
                return "Mac OS X";
            }
            else
            {
                return "UNKNOWN";
            }
        }
    }
}