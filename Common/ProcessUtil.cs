﻿using System;
using System.IO;
using System.Reflection;
using log4net;

namespace Common
{
    public static class ProcessUtil
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);

        /// <summary>
        /// Launches Titanfall 2 using the information it can get from the Config file
        /// </summary>
        /// <returns>true if successful, false if the operation failed</returns>
        public static bool LaunchTitanfall2()
        {
            Log.Info("Attempting to launch Titanfall 2.");
            if (Config.IsInstalledThroughSteam)
            {
                Log.Debug("Launching Titanfall 2 via Steam.");
                return LaunchExeOrProtocol("steam://rungameid/1237970");
            }
            else
            {
                var exePath = Config.Titanfall2ExecutablePath;
                if (exePath.Length == 0)
                {
                    Log.Error("Tried to launch Titanfall 2 but the executable path was never set in the settings!");
                    return false;
                }
                else if (!new FileInfo(exePath).Exists)
                {
                    Log.ErrorFormat(
                        "Tried to launch Titanfall exe at '{0}' but it doesn't exist at the path specified!", exePath);
                    return false;
                }
                else
                {
                    return LaunchExeOrProtocol(exePath);
                }
            }
        }

        public static bool LaunchExeOrProtocol(string fullExePathOrUri, string arguments = "")
        {
            Log.DebugFormat("Launching '{0}'...", fullExePathOrUri);
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = fullExePathOrUri,
                // Set to false when you need env vars. Set to true to execute steam:// protocol links
                UseShellExecute = true,
                Arguments = arguments,
            };
            var process = new System.Diagnostics.Process();
            process.StartInfo = startInfo;
            try
            {
                process.Start();
                Log.DebugFormat("Successfully launched '{0}'.", fullExePathOrUri);
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"Failed to launch exe or protocol specified by '{fullExePathOrUri}'.", e);
                return false;
            }
        }

        public static bool ShowFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                return LaunchExeOrProtocol("explorer.exe", $"/select,\"{fileInfo.FullName}\"");
            }

            Log.ErrorFormat("Tried to show file '{0}' but it doesn't exist!", fileInfo.FullName);
            return false;
        }

        public static bool EditFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                return LaunchExeOrProtocol("notepad.exe", $"\"{fileInfo.FullName}\"");
            }

            Log.ErrorFormat("Tried to open file '{0}' but it doesn't exist!", fileInfo.FullName);
            return false;
        }
    }
}