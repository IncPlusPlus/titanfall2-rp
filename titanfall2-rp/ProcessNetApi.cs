﻿using System;
using Process.NET;
using Process.NET.Memory;

namespace titanfall2_rp
{
    public class ProcessNetApi
    {
        private static ProcessSharp _sharp;
        private static readonly String ProcessName = "Titanfall2";

        private ProcessNetApi()
        {
        }

        public static bool NeedsInit()
        {
            return _sharp == null || _sharp.Native.HasExited;
        }

        public static bool Init()
        {
            Console.WriteLine("Initializing ProcessNetApi...");
            Console.WriteLine("Searching for process '" + ProcessName + "'...");
            System.Diagnostics.Process[] processSearch = System.Diagnostics.Process.GetProcessesByName(ProcessName);
            if (processSearch.Length == 0)
            {
                Console.WriteLine("'" + ProcessName + "'" + " isn't running!");
                return false;
            }
            else
            {
                Console.WriteLine("Found '" + ProcessName + "'" + "!");
                var proc = processSearch[0];
                _sharp = new ProcessSharp(proc, MemoryType.Remote);
                return true;
            }
        }

        public static ProcessSharp GetProcess()
        {
            if (_sharp == null)
            {
                throw new NullReferenceException(
                    "The Init() method was never called before attempting to access the process.");
            }
            else if (_sharp.Native.HasExited)
            {
                throw new AccessViolationException("Attempted to access a process that has already exited.");
            }
            else
            {
                return _sharp;
            }
        }
    }
}