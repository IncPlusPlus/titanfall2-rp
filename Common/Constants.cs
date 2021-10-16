using System;
using System.IO;
using System.Reflection;

namespace Common
{
    public static class Constants
    {
        public static readonly string DataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "titanfall2-rp"
        );

        /// <summary>
        /// True when this is being run off of a local build (i.e. with `dotnet build/release` without a version set)
        /// or from an IDE. This is a multi-purpose variable and can be used to avoid checking end-user stuff that
        /// would never be true in an IDE like whether the current version is up-to-date or whether the currently running
        /// assembly is named "titanfall2-rp.exe" (it appears as "titanfall2-rp.dll" when running a local build). 
        /// </summary>
        public static bool IsLocalBuild =>
            // Obviously if a debugger is attached, this isn't being run by an end-user
            System.Diagnostics.Debugger.IsAttached ||
            // This is the default version for this project when there is no RELEASE_VERSION env var
            Assembly.GetEntryAssembly()!.GetName().Version!.Equals(new Version(0,
                0, 1,0));
    }
}