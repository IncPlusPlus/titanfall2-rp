using System.Reflection;
using DiscordRPC.Logging;
using log4net;

namespace titanfall2_rp
{
    public class Log4NetDiscordLogger : ILogger
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);

        public void Trace(string message, params object[] args)
        {
            if (Level <= LogLevel.Trace)
            {
                Log.DebugFormat(message, args);
            }
        }

        public void Info(string message, params object[] args)
        {
            if (Level <= LogLevel.Info)
            {
                Log.InfoFormat(message, args);
            }
        }

        public void Warning(string message, params object[] args)
        {
            if (Level <= LogLevel.Warning)
            {
                Log.WarnFormat(message, args);
            }
        }

        public void Error(string message, params object[] args)
        {
            if (Level <= LogLevel.Error)
            {
                Log.ErrorFormat(message, args);
            }
        }

        public LogLevel Level { get; set; }
    }
}