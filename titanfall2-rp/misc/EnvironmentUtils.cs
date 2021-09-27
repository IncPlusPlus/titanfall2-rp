using System;
using System.IO;

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
    }
}