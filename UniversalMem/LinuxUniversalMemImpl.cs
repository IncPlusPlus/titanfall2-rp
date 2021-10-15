using System;
using Process.NET.Memory;

namespace UniversalMem
{
    public class LinuxUniversalMemImpl : UniversalMem
    {
        public LinuxUniversalMemImpl(System.Diagnostics.Process native, MemoryType type) : base(native, type)
        {
        }

        public LinuxUniversalMemImpl(string processName, MemoryType type) : base(processName, type)
        {
        }

        public LinuxUniversalMemImpl(int processId, MemoryType type) : base(processId, type)
        {
        }

        public override event EventHandler OnDispose
        {
            add => throw new NotImplementedException();
            remove => throw new NotImplementedException();
        }

        public override event EventHandler ProcessExited
        {
            add => throw new NotImplementedException();
            remove => throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
            GC.SuppressFinalize(this);
        }
    }
}