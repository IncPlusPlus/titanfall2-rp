using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using AutoUpdaterDotNET;
using log4net;

namespace titanfall2_rp.updater
{
    public class WineUpdater : UpdateHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private readonly HttpClient _client;
        private readonly Uri _appCastUri;
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpdateInfoEventArgs));


        public WineUpdater()
        {
            _client = new HttpClient();
            _appCastUri = new Uri(AppCastURL);
        }

        protected override bool? CheckForUpdates()
        {
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(_client.GetStringAsync(_appCastUri).Result)) { XmlResolver = null };
            UpdateInfoEventArgs args = (UpdateInfoEventArgs)xmlSerializer.Deserialize(xmlTextReader)!;
            return new Version(args.CurrentVersion) > AppVersion;
        }

        protected override void AttemptUpdate()
        {
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(_client.GetStringAsync(_appCastUri).Result)) { XmlResolver = null };
            UpdateInfoEventArgs args = (UpdateInfoEventArgs)xmlSerializer.Deserialize(xmlTextReader)!;
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
            Uri dlUri = new Uri(args.DownloadURL);
            _client.DownloadFileTaskAsync(dlUri, tempFile).RunSynchronously();
        }
    }

    public static class HttpClientUtils
    {
        public static async Task DownloadFileTaskAsync(this HttpClient client, Uri uri, string fileName)
        {
            // https://stackoverflow.com/a/66270371/1687436
            await using var s = await client.GetStreamAsync(uri);
            await using var fs = new FileStream(fileName, FileMode.CreateNew);
            await s.CopyToAsync(fs);
        }
    }
}