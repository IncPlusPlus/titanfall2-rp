using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Process.NET;
using static titanfall2_rp.ghapi;

namespace titanfall2_rp
{
    public class ProcessApi
    {
        // https://stackoverflow.com/a/61842098/1687436
        public static IntPtr GetModuleBaseAddress(System.Diagnostics.Process proc, string modName)
        {
            IntPtr addr = IntPtr.Zero;

            foreach (ProcessModule m in proc.Modules)
            {
                if (m.ModuleName == modName)
                {
                    addr = m.BaseAddress;
                    break;
                }
            }
            return addr;
            // return GetProcessModule(proc, modName).BaseAddress;
        }

        public static ProcessModule GetProcessModule(System.Diagnostics.Process proc, string modName)
        {
            foreach (ProcessModule m in proc.Modules)
            {
                if (m.ModuleName == modName)
                {
                    return m;
                }
            }
            throw new ArgumentException("Couldn't find module '" + modName+"'.");
        }

        /// <summary>
        /// Follows the provided chain of offsets to get the precise address that is being pointed to.
        /// </summary>
        /// <param name="sharp"></param>
        /// <param name="baseAddress">the base address of the pointer</param>
        /// <param name="offsets">the offset chain that points to the desired address</param>
        /// <returns>the address that the pointer was pointing to; 0x0 if the pointer couldn't be resolved</returns>
        public static IntPtr ResolvePointerAddress(ProcessSharp sharp, IntPtr baseAddress, IEnumerable<int> offsets)
        {
            var buffer = sharp.Memory.Read<IntPtr>(baseAddress);
            return offsets.Aggregate(buffer, (current, i) => sharp.Memory.Read<IntPtr>(current + i));
        }

        // https://stackoverflow.com/a/61830014/1687436
        public static IntPtr FindDMAAddy(IntPtr hProc, IntPtr ptr, int[] offsets)
        {
            var buffer = new byte[IntPtr.Size];
            foreach (int i in offsets)
            {
                ReadProcessMemory(hProc, ptr, buffer, buffer.Length, out var read);

                ptr = (IntPtr.Size == 4)
                    ? IntPtr.Add(new IntPtr(BitConverter.ToInt32(buffer, 0)), i)
                    : ptr = IntPtr.Add(new IntPtr(BitConverter.ToInt64(buffer, 0)), i);
            }
            return ptr;
        }
    }
}