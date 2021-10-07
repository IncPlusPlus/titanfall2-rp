using System;
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
        /// <remarks>This is a Windows-only function</remarks>
        public static void LaunchTitanfall2()
        {
            Log.Info("Attempting to launch Titanfall 2.");
            if (Config.IsInstalledThroughSteam)
            {
                Log.Debug("Launching Titanfall 2 via Steam.");
                LaunchExeOrProtocol("steam://rungameid/1237970");
            }
            else
            {
                try
                {
                    var exePath = Config.Titanfall2ExecutablePath;
                    if (exePath.Length == 0)
                    {
                        throw new ArgumentException(
                            "Tried to launch Titanfall 2 but the executable path was never set in the settings!");
                    }

                    if (!new FileInfo(exePath).Exists)
                    {
                        throw new FileNotFoundException(
                            $"Tried to launch Titanfall exe at '{exePath}' but it doesn't exist at the path specified!",
                            exePath);
                    }

                    LaunchExeOrProtocol(exePath);
                }
                catch (Exception e)
                {
                    // Make sure this gets logged before tossing the error up the stack
                    Log.Error("Failed to launch TF|2", e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Executes a .exe file or launches a protocol:// path.
        /// </summary>
        /// <param name="fullExePathOrUri">either a steam:// URI or a path to an EXE file</param>
        /// <param name="arguments">any arguments to be provided to the executable</param>
        /// <remarks>This is a Windows-only function</remarks>
        public static void LaunchExeOrProtocol(string fullExePathOrUri, string arguments = "")
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
            }
            catch (Exception e)
            {
                Log.Error($"Failed to launch exe or protocol specified by '{fullExePathOrUri}'.", e);
                throw;
            }
        }

        /// <summary>
        /// Shows a file in explorer. Obviously this will only work on Windows.
        /// </summary>
        /// <param name="filePath">the path to the file</param>
        public static void ShowFile(string filePath)
        {
            EditOrShowFile(filePath, false);
        }

        /// <summary>
        /// Opens a file in notepad.exe. Obviously, this will only work on Windows.
        /// </summary>
        /// <param name="filePath">the path to the file</param>
        public static void EditFile(string filePath)
        {
            EditOrShowFile(filePath, true);
        }

        private static void EditOrShowFile(string filePath, bool edit)
        {
            var verb = edit ? "edit" : "show";
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                Log.ErrorFormat("Tried to open file '{0}' but it doesn't exist!", fileInfo.FullName);
                throw new FileNotFoundException($"Tried to {verb} file '{fileInfo.FullName}' but it doesn't exist!",
                    fileInfo.Name);
            }

            LaunchExeOrProtocol("notepad.exe", $"{(edit ? "" : "/select,")}\"{fileInfo.FullName}\"");
        }
    }
}