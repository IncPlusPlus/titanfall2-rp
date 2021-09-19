using System;
using System.Reflection;
using AutoUpdaterDotNET;
using log4net;
using titanfall2_rp.misc;

namespace titanfall2_rp.updater
{
    public class WindowsUpdater: UpdateHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);

        protected override bool? CheckForUpdates()
        {
            return null;
        }

        protected override void AttemptUpdate()
        {
            if (EnvironmentUtils.AppDirectoryWritable())
            {
                // Don't give a UAC prompt if I don't have to (they're annoying)
                AutoUpdater.RunUpdateAsAdmin = false;
            }
            AutoUpdater.Synchronous = true;
            AutoUpdater.ApplicationExitEvent += AutoUpdater_ApplicationExitEvent;
            AutoUpdater.Start(AppCastURL);
        }
        
        private static void AutoUpdater_ApplicationExitEvent()
        {
            Log.Debug("Closing application in preparation for update...");
            Environment.Exit(0);
        }
    }
}