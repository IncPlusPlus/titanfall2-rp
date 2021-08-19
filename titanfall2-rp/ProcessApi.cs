using System;
using System.Diagnostics;
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
        }
        
        // https://stackoverflow.com/a/61842098/1687436
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