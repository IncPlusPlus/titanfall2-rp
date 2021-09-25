using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Common;
using log4net;

namespace ZipExtractor
{
    /// <summary>
    /// A zip extractor program largely copied from AutoUpdater.NET
    /// </summary>
    static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private const string LogFileName = "titanfall2-rp-ZipExtractor.log";
        private const string LoggerConfigFileName = "log4net-tf2-ZipExtractor.config";
        private const int MaxRetries = 2;

        static void Main(string[] args)
        {
            Log4NetConfig.ConfigureLogger(LogFileName, LoggerConfigFileName);
            Log.Info("Starting ZipExtractor...");
            var stringBuilder = new StringBuilder("ZipExtractor started with the following commandline args:\n");
            for (var index = 0; index < args.Length; index++)
            {
                var arg = args[index];
                stringBuilder.AppendLine($"[{index}] {arg}");
            }

            Log.DebugFormat("ZipExtractor started with the following commandline args: {0}", stringBuilder);

            try
            {
                AttemptUnzip(args);
            }
            catch (Exception e)
            {
                Log.Fatal("Failed to perform update. Exception to follow.", e);
                Environment.Exit(1);
            }
        }

        private static void AttemptUnzip(string[] args)
        {
            if (args.Length >= 4)
            {
                string executablePath = args[3];

                // Extract all the files.
                var backgroundWorker = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true,
                };

                backgroundWorker.DoWork += (_, eventArgs) =>
                {
                    foreach (var process in
                        Process.GetProcessesByName(Path.GetFileNameWithoutExtension(executablePath)))
                    {
                        try
                        {
                            if (process.MainModule is { FileName: { } } &&
                                process.MainModule.FileName.Equals(executablePath))
                            {
                                Log.Info("Waiting for application process to exit...");

                                backgroundWorker.ReportProgress(0, "Waiting for application to exit...");
                                process.WaitForExit();
                            }
                        }
                        catch (Exception exception)
                        {
                            Debug.WriteLine(exception.Message);
                        }
                    }

                    Log.Debug("BackgroundWorker started successfully.");

                    var path = args[2];

                    // Ensures that the last character on the extraction path
                    // is the directory separator char.
                    // Without this, a malicious zip file could try to traverse outside of the expected
                    // extraction path.
                    if (!path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                    {
                        path += Path.DirectorySeparatorChar;
                    }

                    var archive = ZipFile.OpenRead(args[1]);

                    var entries = archive.Entries;

                    Log.DebugFormat("Found total of {0} files and folders inside the zip file.", entries.Count);

                    try
                    {
                        int progress = 0;
                        for (var index = 0; index < entries.Count; index++)
                        {
                            if (backgroundWorker.CancellationPending)
                            {
                                eventArgs.Cancel = true;
                                break;
                            }

                            var entry = entries[index];

                            string currentFile = $"Extracting {entry.FullName}";
                            backgroundWorker.ReportProgress(progress, currentFile);
                            int retries = 0;
                            bool notCopied = true;
                            while (notCopied)
                            {
                                string filePath = String.Empty;
                                try
                                {
                                    filePath = Path.Combine(path, entry.FullName);
                                    if (!entry.IsDirectory())
                                    {
                                        var parentDirectory = Path.GetDirectoryName(filePath);
                                        if (!Directory.Exists(parentDirectory))
                                        {
                                            Directory.CreateDirectory(parentDirectory);
                                        }

                                        entry.ExtractToFile(filePath, true);
                                    }

                                    notCopied = false;
                                }
                                catch (IOException exception)
                                {
                                    const int errorSharingViolation = 0x20;
                                    const int errorLockViolation = 0x21;
                                    var errorCode = Marshal.GetHRForException(exception) & 0x0000FFFF;
                                    if (errorCode == errorSharingViolation || errorCode == errorLockViolation)
                                    {
                                        retries++;
                                        if (retries > MaxRetries)
                                        {
                                            throw;
                                        }

                                        List<Process> lockingProcesses = new();
                                        if (Environment.OSVersion.Version.Major >= 6 && retries >= 2)
                                        {
                                            try
                                            {
                                                lockingProcesses = FileUtil.WhoIsLocking(filePath);
                                            }
                                            catch (Exception)
                                            {
                                                // ignored
                                            }
                                        }

                                        if (lockingProcesses == null)
                                        {
                                            Thread.Sleep(5000);
                                        }
                                        else
                                        {
                                            foreach (var lockingProcess in lockingProcesses)
                                            {
                                                Log.ErrorFormat(
                                                    "{0} is still open and it is using \"{1}\". Please close the process manually and try again.",
                                                    lockingProcess.ProcessName, filePath);
                                            }

                                            throw;
                                        }
                                    }
                                    else
                                    {
                                        throw;
                                    }
                                }
                            }

                            progress = (index + 1) * 100 / entries.Count;
                            backgroundWorker.ReportProgress(progress, currentFile);

                            Log.Info($"{currentFile} [{progress}%]");
                        }
                    }
                    finally
                    {
                        archive.Dispose();
                    }
                };

                backgroundWorker.ProgressChanged += (_, eventArgs) =>
                {
                    Log.InfoFormat("Extraction {0}% complete. {1}", eventArgs.ProgressPercentage, eventArgs.UserState?.ToString());
                };

                backgroundWorker.RunWorkerCompleted += (_, eventArgs) =>
                {
                    try
                    {
                        if (eventArgs.Error != null)
                        {
                            throw eventArgs.Error;
                        }

                        if (!eventArgs.Cancelled)
                        {
                            try
                            {
                                var processStartInfo = new ProcessStartInfo(executablePath);
                                if (args.Length > 4)
                                {
                                    processStartInfo.Arguments = args[4];
                                }

                                var newProcess = Process.Start(processStartInfo);

                                Log.Info("Successfully launched the updated application.");
                            }
                            catch (Win32Exception exception)
                            {
                                if (exception.NativeErrorCode != 1223)
                                {
                                    throw;
                                }
                            }
                        }
                    }
                    finally
                    {
                        Environment.Exit(0);
                    }
                };

                backgroundWorker.RunWorkerAsync();
            }
            else
            {
                Log.FatalFormat("Expected args[] to contain >4 items but got only {0}", args.Length);
            }
        }
    }

    public static class ExtensionMethods
    {
        public static bool IsDirectory(this ZipArchiveEntry entry)
        {
            return string.IsNullOrEmpty(entry.Name) && (entry.FullName.EndsWith("/") || entry.FullName.EndsWith(@"\"));
        }
    }
}