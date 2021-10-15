using System;
using Process.NET.Memory;

namespace UniversalMem
{
    public sealed class LinuxUniversalMemImpl : UniversalMem
    {
        public LinuxUniversalMemImpl(System.Diagnostics.Process native, MemoryType type) : base(native, type)
        {
            // THe handle isn't implemented yet. This is purely to obey the contract with the pdn API
            Memory = new LinuxIMemoryImpl(null);
        }

        public LinuxUniversalMemImpl(string processName, MemoryType type) : base(processName, type)
        {
            Memory = new LinuxIMemoryImpl(Handle);
        }

        public LinuxUniversalMemImpl(int processId, MemoryType type) : base(processId, type)
        {
            Memory = new LinuxIMemoryImpl(Handle);
        }

        public override IMemory Memory
        {
            get; set;
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
        }
    }
}