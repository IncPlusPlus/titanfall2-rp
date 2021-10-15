using System;
using Process.NET.Memory;
using Process.NET.Native.Types;

namespace UniversalMem
{
    public class LinuxIMemoryImpl : ProcessMemory
    {
        public LinuxIMemoryImpl(SafeMemoryHandle handle) : base(handle)
        {
        }

        public override byte[] Read(IntPtr intPtr, int length)
        {
            throw new NotImplementedException();
        }

        public override T Read<T>(IntPtr intPtr)
        {
            throw new NotImplementedException();
        }

        public override int Write(IntPtr intPtr, byte[] bytesToWrite)
        {
            throw new NotImplementedException();
        }

        public override void Write<T>(IntPtr intPtr, T value)
        {
            throw new NotImplementedException();
        }
    }
}