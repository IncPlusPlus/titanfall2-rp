using System.Reflection;
using System.Runtime.InteropServices;
using log4net;

namespace titanfall2_rp.updater
{
    /// <summary>
    /// An empty updater class that does nothing because you're not an a supported OS.
    /// </summary>
    public class StubUpdater : UpdateHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);

        protected override bool? CheckForUpdates()
        {
            Log.DebugFormat("Tried to check for update but OS platform '{0}' isn't able to perform auto updates.",
            RuntimeInformation.OSDescription);
            return null;
        }

        protected override void AttemptUpdate()
        { }
    }
}