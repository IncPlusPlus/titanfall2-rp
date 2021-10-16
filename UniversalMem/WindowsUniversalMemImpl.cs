using System;
using Process.NET;
using Process.NET.Memory;
using Process.NET.Modules;
using Process.NET.Native.Types;
using Process.NET.Threads;
using Process.NET.Windows;

namespace UniversalMem
{
    public class WindowsUniversalMemImpl : UniversalMem
    {
        private readonly ProcessSharp _sharpInstance;

        public WindowsUniversalMemImpl(System.Diagnostics.Process native, MemoryType type) : base(native, type)
        {
            _sharpInstance = new ProcessSharp(native, type);
        }

        public override event EventHandler OnDispose;
        public override event EventHandler ProcessExited;

        public override System.Diagnostics.Process Native
        {
            get => _sharpInstance.Native;
            set => _sharpInstance.Native = value;
        }

        public override SafeMemoryHandle Handle
        {
            get => _sharpInstance.Handle;
            set => _sharpInstance.Handle = value;
        }

        public override IMemory Memory
        {
            get => _sharpInstance.Memory;
            set => _sharpInstance.Memory = value;
        }

        public override IThreadFactory ThreadFactory
        {
            get => _sharpInstance.ThreadFactory;
            set => _sharpInstance.ThreadFactory = value;
        }

        public override IModuleFactory ModuleFactory
        {
            get => _sharpInstance.ModuleFactory;
            set => _sharpInstance.ModuleFactory = value;
        }

        public override IMemoryFactory MemoryFactory
        {
            get => _sharpInstance.MemoryFactory;
            set => _sharpInstance.MemoryFactory = value;
        }

        public override IWindowFactory WindowFactory
        {
            get => _sharpInstance.WindowFactory;
            set => _sharpInstance.WindowFactory = value;
        }

        public override IProcessModule this[string moduleName] => _sharpInstance[moduleName];
        public override IPointer this[IntPtr intPtr] => _sharpInstance[intPtr];

        public override void Dispose()
        {
            _sharpInstance.Dispose();
            GC.SuppressFinalize(this);
        }

        private void ConfigureEventHandlers()
        {
            _sharpInstance.ProcessExited += (EventHandler)((s, e) => { ProcessExited?.Invoke(s, e); });
            _sharpInstance.OnDispose += (EventHandler)((s, e) => { OnDispose?.Invoke(s, e); });
        }
    }
}