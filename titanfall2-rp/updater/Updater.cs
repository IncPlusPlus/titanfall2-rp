using System;
using System.Reflection;
using System.Runtime.InteropServices;
using log4net;
using titanfall2_rp.misc;

namespace titanfall2_rp.updater
{
    public abstract class UpdateHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        public static readonly Version AppVersion = Assembly.GetEntryAssembly()!.GetName().Version!;
        private static UpdateHelper? _updater;

        public static UpdateHelper Updater => _updater ?? GetUpdaterForOs();

        private static UpdateHelper GetUpdaterForOs()
        {
            Log.DebugFormat("Grabbing an updater for OS '{0}'.", RuntimeInformation.OSDescription);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (EnvironmentUtils.IsRunningInWine())
                {
                    _updater = new WineUpdater();
                }
                else
                {
                    _updater = new WindowsUpdater();
                }

            }
            else
            {
                _updater = new StubUpdater();
            }
            return _updater;
        }

        public void Update()
        {
            Log.Info("Checking for update...");
            bool checkUpdatesSuccess = false;
            try
            {
                var updateAvailable = CheckForUpdates();
                checkUpdatesSuccess = true;
                if (updateAvailable == null)
                {
                    Log.Info("It's unknown if an update is available, attempting to update now...");
                    AttemptUpdate();
                }
                else if ((bool)updateAvailable)
                {
                    Log.Info("There's an update available! Starting the update process...");
                    AttemptUpdate();
                }
                else
                {
                    Log.Info("You are on the most up-to-date version!");
                }
            }
            catch (Exception e)
            {
                Log.Error(checkUpdatesSuccess ? "Failed to perform update." : "Failed to check for updates.", e);
            }
            Log.Info("Update check complete!");
        }

        /// <summary>
        /// Checks for available updates
        /// </summary>
        /// <returns>false if there's no update, true if there is one, null if unknown</returns>
        protected abstract bool? CheckForUpdates();

        protected abstract void AttemptUpdate();
    }
}