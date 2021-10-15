using System;
using System.Runtime.InteropServices;
using Process.NET;
using Process.NET.Memory;
using Process.NET.Modules;
using Process.NET.Native.Types;
using Process.NET.Threads;
using Process.NET.Utilities;
using Process.NET.Windows;

namespace UniversalMem
{
    public abstract class UniversalMem : IProcess
    {
        private static UniversalMem ProcessImplementation { get; set; }

        protected UniversalMem(System.Diagnostics.Process native, MemoryType type)
        {
        }

        protected UniversalMem(string processName, MemoryType type) : this(ProcessHelper.FromName(processName), type)
        {
        }

        protected UniversalMem(int processId, MemoryType type) : this(ProcessHelper.FromProcessId(processId), type)
        {
        }

        public static UniversalMem From(System.Diagnostics.Process native, MemoryType type)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ProcessImplementation = new WindowsUniversalMemImpl(native, type);
            }
            else if (RuntimeInformation.IsOSPlatform((OSPlatform.Linux)))
            {
                ProcessImplementation = new LinuxUniversalMemImpl(native, type);
            }
            else
            {
                throw new NotImplementedException(
                    $"UniversalMem isn't implemented on platform {RuntimeInformation.OSDescription}");
            }

            return ProcessImplementation;
        }

        public static UniversalMem From(string processName, MemoryType type)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ProcessImplementation = new WindowsUniversalMemImpl(processName, type);
            }
            else if (RuntimeInformation.IsOSPlatform((OSPlatform.Linux)))
            {
                ProcessImplementation = new LinuxUniversalMemImpl(processName, type);
            }
            else
            {
                throw new NotImplementedException(
                    $"UniversalMem isn't implemented on platform {RuntimeInformation.OSDescription}");
            }

            return ProcessImplementation;
        }

        public static UniversalMem From(int processId, MemoryType type)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ProcessImplementation = new WindowsUniversalMemImpl(processId, type);
            }
            else if (RuntimeInformation.IsOSPlatform((OSPlatform.Linux)))
            {
                ProcessImplementation = new LinuxUniversalMemImpl(processId, type);
            }
            else
            {
                throw new NotImplementedException(
                    $"UniversalMem isn't implemented on platform {RuntimeInformation.OSDescription}");
            }

            return ProcessImplementation;
        }

        public abstract event EventHandler OnDispose;
        public abstract event EventHandler ProcessExited;

        public virtual System.Diagnostics.Process Native { get; set; }

        /// <inheritdoc />
        public virtual SafeMemoryHandle Handle
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual IMemory Memory
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual IThreadFactory ThreadFactory
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual IModuleFactory ModuleFactory
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual IMemoryFactory MemoryFactory
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual IWindowFactory WindowFactory
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual IProcessModule this[string moduleName] => throw new NotImplementedException();

        /// <inheritdoc />
        public virtual IPointer this[IntPtr intPtr] => throw new NotImplementedException();

        /// <inheritdoc />
        public abstract void Dispose();
    }
}