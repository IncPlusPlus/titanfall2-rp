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
using DiscordRPC.Message;

namespace titanfall2_rp.SegmentManager
{
    public static class SegmentManager
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private const string WriteKey = "DAXSzUgf2tu57mPFgnWSggNyD3v4BEhC";
        private const string AnonymousIdentifierFileName = "IDENTIFIER";
        private static bool _enableSegment = true;
        private static bool _initialized;
        private static bool _hasIdentifiedSelf;
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

        public static void TrackEvent(TrackableEvent @event, Exception? exception = null,
            PresenceMessage? presence = null)
        {
            if (!_enableSegment) return;
            try
            {
                switch (@event)
                {
                    case TrackableEvent.Gameplay:
                        TrackGameplay(@event, presence ?? throw new ArgumentNullException(nameof(presence)));
                        break;
                    case TrackableEvent.Error:
                    case TrackableEvent.GameplayInfoFailure:
                        // For now, Error and GameplayInfoFailure will be treated the same way
                        TrackErrorOrFailure(@event, exception ?? throw new ArgumentNullException(nameof(exception)));
                        break;
                    case TrackableEvent.GameOpened:
                    case TrackableEvent.GameClosed:
                        // GameOpened and GameClosed both do the same thing
                        Analytics.Client.Track(_tf2Api?.GetUserId(), @event.ToString(),
                            new Options().SetAnonymousId(GetAnonymousIdentifier()));
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

        private static void TrackErrorOrFailure(TrackableEvent @event, Exception e)
        {
            Analytics.Client.Track(_tf2Api?.GetUserId(), @event.ToString(),
                new Dictionary<string, object>(GlobalProps) { { "Exception", e } },
                new Options().SetAnonymousId(GetAnonymousIdentifier()));
        }

        private static void TrackGameplay(TrackableEvent @event, PresenceMessage presenceMessage)
        {
            IdentifySelf();
            var presence = presenceMessage.Presence;
            Analytics.Client.Track(_tf2Api?.GetUserId(), @event.ToString(),
                new Dictionary<string, object?>(GlobalProps)
                {
                    {"state",presence.State},
                    {"details",presence.Details},
                    {"start", presence.Timestamps.Start},
                    {"end", presence.Timestamps.End},
                    {"large_image",presence.Assets.LargeImageKey},
                    {"large_text",presence.Assets.LargeImageText},
                    {"small_image",presence.Assets.SmallImageKey},
                    {"small_text",presence.Assets.SmallImageText},
                    {"gamemode_and_map_name",_tf2Api?.GetGameModeAndMapName()}
                },
                new Options().SetAnonymousId(GetAnonymousIdentifier()));
        }

        private static void IdentifySelf()
        {
            if (_hasIdentifiedSelf) return;
            Analytics.Client.Identify(_tf2Api!.GetUserId(), new Traits()
            {
                {"name",Environment.UserName},
                {"Origin Name", _tf2Api.GetOriginName()}
            }, new Options().SetAnonymousId(GetAnonymousIdentifier()));
            _hasIdentifiedSelf = true;
        }

        private static void CreateAnonymousIdentifier()
        {
            var fileInfo = new FileInfo(Path.Combine(Constants.DataPath, AnonymousIdentifierFileName));
            if (!Directory.Exists(fileInfo.DirectoryName))
            {
                Directory.CreateDirectory(fileInfo.DirectoryName!);
            }

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

            Log.DebugFormat("[Analytics] [{0}] {1}", level, message);
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