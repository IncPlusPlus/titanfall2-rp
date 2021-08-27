using System;
using System.Reflection;
using log4net;
using Process.NET;
using Process.NET.Memory;

namespace titanfall2_rp
{
    public static class ProcessNetApi
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private static ProcessSharp? _sharp;
        private const string ProcessName = "Titanfall2";

        public static bool NeedsInit()
        {
            return _sharp == null || _sharp.Native.HasExited;
        }

        public static bool Init()
        {
            Log.Debug("Initializing ProcessNetApi...");
            Log.Debug("Searching for process '" + ProcessName + "'...");
            System.Diagnostics.Process[] processSearch = System.Diagnostics.Process.GetProcessesByName(ProcessName);
            if (processSearch.Length == 0)
            {
                Log.Debug("'" + ProcessName + "'" + " isn't running!");
                return false;
            }

            Log.Debug("Found '" + ProcessName + "'" + "!");
            var proc = processSearch[0];
            _sharp = new ProcessSharp(proc, MemoryType.Remote);
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