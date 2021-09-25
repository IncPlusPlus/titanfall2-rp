using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using AutoUpdaterDotNET;
using log4net;

namespace titanfall2_rp.updater
{
    /// <summary>
    /// An updater that doesn't reference WinForms and therefore works in Wine.
    /// This is largely adapted from the AutoUpdater.NET updater code.
    /// </summary>
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
            // TODO: Try cleaning up the zip extractor and any previous downloads here (be nice to %tmp%)
            XmlTextReader xmlTextReader = new(new StringReader(_client.GetStringAsync(_appCastUri).Result))
            { XmlResolver = null };
            UpdateInfoEventArgs args = (UpdateInfoEventArgs)xmlSerializer.Deserialize(xmlTextReader)!;
            return new Version(args.CurrentVersion) > AppVersion;
        }

        protected override void AttemptUpdate()
        {
            XmlTextReader xmlTextReader =
                new(new StringReader(_client.GetStringAsync(_appCastUri).Result)) { XmlResolver = null };
            UpdateInfoEventArgs args = (UpdateInfoEventArgs)xmlSerializer.Deserialize(xmlTextReader)!;
            var tempFile = Path.Combine(Path.GetTempPath(), "titanfall2-rp-update-" + Guid.NewGuid() + ".tmp");
            Uri dlUri = new Uri(args.DownloadURL);
            var response = _client.GetAsync(dlUri).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new WebException(
                    $"Download of the update file failed with code " +
                    $"{(int)response.StatusCode}: {response.StatusCode}. " +
                    $"Response body:\n{response.Content.ReadAsStringAsync().Result}");
            }

            StreamToFile(response.Content.ReadAsStream(), tempFile).Wait();

            var contentDisposition = response.Content.Headers.ContentDisposition;

            var fileName = string.IsNullOrEmpty(contentDisposition?.FileName)
                ? Path.GetFileName(response.RequestMessage!.RequestUri!.LocalPath)
                : contentDisposition.FileName!;
            var tempPath = Path.Combine(Path.GetTempPath(), fileName);

            var extension = Path.GetExtension(tempPath);
            if (extension.Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                var installerPath = Path.Combine(Path.GetTempPath(), "WineZipExtractor.exe");
                File.WriteAllBytes(installerPath, Properties.Resources.ZipExtractor);

                var executablePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
                var extractionPath = Path.GetDirectoryName(executablePath);

                var arguments = new StringBuilder($"\"{tempFile}\" \"{extractionPath}\" \"{executablePath}\"");

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = installerPath,
                    UseShellExecute = true,
                    Arguments = arguments.ToString(),
                };

                var proc = System.Diagnostics.Process.Start(processStartInfo);

                Environment.Exit(0);
            }
            else
            {
                throw new ArgumentException($"Unable to install using a file with the extension '{extension}'");
            }
        }

        public static async Task StreamToFile(Stream s, string fileName)
        {
            // https://stackoverflow.com/a/66270371/1687436
            // await using var s = await client.GetStreamAsync(uri);
            await using var fs = new FileStream(fileName, FileMode.CreateNew);
            await s.CopyToAsync(fs);
        }
    }
}