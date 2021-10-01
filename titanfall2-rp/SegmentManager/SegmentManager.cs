using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Common;
using log4net;
using Segment;
using Segment.Model;
using titanfall2_rp.misc;
using titanfall2_rp.updater;
using Config = Common.Config;

namespace titanfall2_rp.SegmentManager
{
    public static class SegmentManager
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private const string WriteKey = "DAXSzUgf2tu57mPFgnWSggNyD3v4BEhC";
        private const string AnonymousIdentifierFileName = "IDENTIFIER";
        private static bool _enableSegment = true;
        private static bool _initialized;
        private static Titanfall2Api? _tf2Api;

        private static readonly Segment.Model.Properties GlobalProps = new()
        {
            { "App Version", UpdateHelper.AppVersion.ToString() },
            { "$os", EnvironmentUtils.GetBasicOsName() },
            { "OS Description", System.Runtime.InteropServices.RuntimeInformation.OSDescription },
            { "Release Edition", EnvironmentUtils.GetReleaseEdition() },
            { "Computer Name", Environment.MachineName },
            { "User Name", Environment.UserName }
        };

        public static void Initialize(Titanfall2Api titanfall2Api)
        {
            if (_initialized) return;
            Logger.Handlers += LoggingHandler;
            Analytics.Initialize(WriteKey);
            _initialized = true;
            _tf2Api = titanfall2Api;
            _enableSegment = Config.IsAnalyticsAllowed;
        }

        public static void TrackEvent(TrackableEvent @event)
        {
            if (!_enableSegment) return;
            try
            {
                switch (@event)
                {
                    case TrackableEvent.Error:
                        break;
                    case TrackableEvent.Gameplay:
                        break;
                    case TrackableEvent.GameplayInfoFailure:
                        break;
                    // ReSharper disable once RedundantCaseLabel (we don't want the user to use these events)
                    case TrackableEvent.FailureWhenFiringEvent or TrackableEvent.DoubleFailure:
                    default:
                        throw new ArgumentOutOfRangeException(nameof(@event), @event, null);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed when trying to track event '{@event.ToString()}'. Firing failure event...", e);
                TrackFailure(e);
            }
        }

        private static void CreateAnonymousIdentifier()
        {
            var fileInfo = new FileInfo(Path.Combine(Constants.DataPath, AnonymousIdentifierFileName));
            File.WriteAllText(fileInfo.FullName, Guid.NewGuid().ToString());
        }

        private static string GetAnonymousIdentifier()
        {
            var fileInfo = new FileInfo(Path.Combine(Constants.DataPath, AnonymousIdentifierFileName));
            if (!fileInfo.Exists)
            {
                CreateAnonymousIdentifier();
            }

            return Guid.Parse(File.ReadAllText(fileInfo.FullName)).ToString();
        }

        static void LoggingHandler(Logger.Level level, string message, IDictionary<string, object>? args)
        {
            if (args != null)
            {
                foreach (string key in args.Keys)
                {
                    message += String.Format(" {0}: {1},", "" + key, "" + args[key]);
                }
            }

            Console.WriteLine(String.Format("[Analytics] [{0}] {1}", level, message));
        }

        private static void TrackFailure(Exception e)
        {
            var dict = new Dictionary<string, object>(GlobalProps) { { "Exception", e } };
            try
            {
                Analytics.Client.Track(_tf2Api!.GetUserId(), TrackableEvent.FailureWhenFiringEvent.ToString(),
                    dict, new Options().SetAnonymousId(GetAnonymousIdentifier()));
            }
            catch (Exception secondaryException)
            {
                Log.Error(
                    $"Failed to track the {TrackableEvent.FailureWhenFiringEvent} event. There will be one more attempt to send the failure notice. This time, without any identifiers nor API calls",
                    secondaryException);
                try
                {
                    dict.Add("SecondaryException", secondaryException);
                    Analytics.Client.Track(null, TrackableEvent.DoubleFailure.ToString(), dict,
                        new Options().SetAnonymousId(GetAnonymousIdentifier()));
                }
                catch (Exception finalException)
                {
                    Log.Error($"Failed both attempts to try and track a failure. No information was able to be sent.",
                        finalException);
                }
            }
        }

        public static void Dispose()
        {
            Analytics.Client.Flush();
            Analytics.Dispose();
        }
    }
}