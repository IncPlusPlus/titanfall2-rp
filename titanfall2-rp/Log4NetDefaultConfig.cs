using System.Xml;

namespace titanfall2_rp
{
    public static class Log4NetDefaultConfig
    {
        /// <summary>
        /// In case the user doesn't have the log4net config file next to the exe. Make one for them using this string
        /// </summary>
        public static readonly string DefaultLog4NetConfig = @"<log4net>
	<root>
		<!--Set this to ALL to see all log levels-->
		<level value=""INFO"" />
		<appender-ref ref=""console"" />
		<appender-ref ref=""file"" />
	</root>

	<!--File Appender-->
	<appender name=""file"" type=""log4net.Appender.RollingFileAppender"">
		<file value=""titanfall-rp.log"" />
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

        public static XmlElement GetLoggerConfigAsXml()
        {
            XmlDocument doc = new();
            doc.LoadXml(DefaultLog4NetConfig);
            return doc.DocumentElement!;
        }
    }
}