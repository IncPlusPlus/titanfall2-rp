using System;
using System.Linq;
using System.Reflection;
using log4net;
using Process.NET;
using Process.NET.Memory;

namespace titanfall2_rp
{
    public static class ProcessNetApi
    {
        private const string ProcessName = "Titanfall2";
        private const string NorthstarProcessName = "Titanfall2-unpacked";
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);

        private static ProcessSharp? _sharp;

        // The time this field is initialized to will be wrong until it's set from Init().
        // This is just a placeholder value.
        public static DateTime StartTimestamp { get; private set; } = DateTimeOffset.Now.UtcDateTime;

        public static bool NeedsInit()
        {
            return _sharp == null || _sharp.Native.HasExited;
        }

        public static bool Init()
        {
            Log.Debug("Initializing ProcessNetApi...");
            Log.Debug("Searching for process '" + ProcessName + "'...");
            var processSearch = System.Diagnostics.Process.GetProcesses()
                .Where(process => process.ProcessName is ProcessName or NorthstarProcessName).ToArray();
            if (processSearch.Length == 0)
            {
                Log.Debug("'" + ProcessName + "'" + " isn't running!");
                return false;
            }

            var proc = processSearch[0];
            Log.Debug("Found '" + proc.ProcessName + "'" + "!");
            _sharp = new ProcessSharp(proc, MemoryType.Remote);
            StartTimestamp = _sharp.Native.StartTime.ToUniversalTime();
            return true;
        }

        public static ProcessSharp GetProcess()
        {
            if (_sharp == null)
            {
                throw new NullReferenceException(
                    "The Init() method was never called before attempting to access the process.");
            }

            if (_sharp.Native.HasExited)
            {
                throw new AccessViolationException("Attempted to access a process that has already exited.");
            }

            return _sharp;
        }
    }
}