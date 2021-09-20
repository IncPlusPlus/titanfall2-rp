using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using log4net;
using log4net.Config;

namespace Common
{
    public static class Log4NetConfig
    {
        /// <summary>
        /// In case the user doesn't have the log4net config file next to the exe. Make one for them using this string
        /// </summary>
        private const string DefaultLog4NetConfig = @"<log4net>
	<root>
		<!--Set this to ALL to see all log levels-->
		<level value=""INFO"" />
		<appender-ref ref=""console"" />
		<appender-ref ref=""file"" />
	</root>

	<!--File Appender-->
	<appender name=""file"" type=""log4net.Appender.RollingFileAppender"">
		<file value=""titanfall2-rp.log"" />
		<appendToFile value=""true"" />
		<rollingStyle value=""Size"" />
		<maxSizeRollBackups value=""5"" />
		<maximumFileSize value=""25MB"" />
		<staticLogFileName value=""true"" />
		<layout type=""log4net.Layout.PatternLayout"">
			<conversionPattern value=""%date [%thread] %level %logger - %message%newline"" />
		</layout>
	</appender>

	<!--Console appender-->
	<appender name=""console"" type=""log4net.Appender.ManagedColoredConsoleAppender"">
		<mapping>
			<level value=""INFO"" />
			<forecolor value=""Green"" />
		</mapping>
		<mapping>
			<level value=""WARN"" />
			<forecolor value=""Yellow"" />
		</mapping>
		<mapping>
			<level value=""ERROR"" />
			<forecolor value=""Red"" />
		</mapping>
		<mapping>
			<level value=""DEBUG"" />
			<forecolor value=""Blue"" />
		</mapping>
		<layout type=""log4net.Layout.PatternLayout"">
			<conversionpattern value=""%date [%thread] %-5level - %message%newline"" />
		</layout>
	</appender>
</log4net>";

        public static XmlElement GetLoggerConfigAsXml(string logFileName)
        {
            XmlDocument doc = new();
            doc.LoadXml(DefaultLog4NetConfig);
            var fileNode = doc.SelectSingleNode("/log4net/appender[1]/file");
            fileNode!.Attributes!["value"]!.Value = logFileName;
            return doc.DocumentElement!;
        }

        public static string GetLoggerConfigAsString(string logFileName)
        {
            return XDocument.Parse(GetLoggerConfigAsXml(logFileName).OuterXml).ToString();
        }

        public static void ConfigureLogger(string logFileName, string loggerConfigFileName)
        {
            ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
            // Load logging configuration
            // Thanks to https://jakubwajs.wordpress.com/2019/11/28/logging-with-log4net-in-net-core-3-0-console-app/
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            // Start with the default logger before trying any fancy config file stuff
            XmlConfigurator.Configure(logRepository, GetLoggerConfigAsXml(logFileName));
            var loggerConfigFile = new FileInfo(loggerConfigFileName);
            if (!loggerConfigFile.Exists)
            {
                log.WarnFormat("Couldn't find '{0}'! Creating it (this only needs to happen once)...", loggerConfigFileName);
                try
                {
                    File.WriteAllText(loggerConfigFileName, GetLoggerConfigAsString(logFileName));
                }
                catch (Exception e)
                {
                    log.Error("Unable to save logging config", e);
                    log.Error("Using built-in default logging configuration.");
                    return;
                }
            }
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
    }
}