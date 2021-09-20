using System;
using System.Reflection;
using Common;
using log4net;

namespace ZipExtractor
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private const string LogFileName = "titanfall2-rp-ZipExtractor.log";
        private const string LoggerConfigFileName = "log4net.config";

        static void Main(string[] args)
        {
            Log4NetConfig.ConfigureLogger(LogFileName, LoggerConfigFileName);
            Console.WriteLine("Hello World!");
        }
    }
}