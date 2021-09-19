using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
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
            return null;
        }

        protected override void AttemptUpdate()
        {
            throw new NotImplementedException();
        }
    }
}