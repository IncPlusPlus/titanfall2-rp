using System;
using System.IO;
using System.Reflection;
using AutoUpdaterDotNET;
using log4net;
using titanfall2_rp.misc;

namespace titanfall2_rp.updater
{
    public class WindowsUpdater : UpdateHelper
    {
        private const string AppCastUrl =
            "https://github.com/IncPlusPlus/titanfall2-rp/releases/latest/download/updater-helper-file.xml";
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

            AutoUpdater.ApplicationExitEvent += AutoUpdater_ApplicationExitEvent;
            try
            {
                AutoUpdater.Start(AppCastUrl);
            }
            catch (Exception e)
            {
                if (e is not FileNotFoundException fileNotFoundException) throw;

                if (fileNotFoundException.FileName?.StartsWith("System.Windows.Forms") ?? false)
                {
                    throw new ApplicationException(
                        $"'{fileNotFoundException.Source}' tried to use Windows Forms but failed to load the assembly '{fileNotFoundException.FileName}' at runtime. Was this exe compiled on Linux?");
                }

                throw;
            }
        }

        private static void AutoUpdater_ApplicationExitEvent()
        {
            Log.Debug("Closing application in preparation for update...");
            Environment.Exit(0);
        }
    }
}