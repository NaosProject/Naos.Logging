// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogInclusionKindToOriginsMaps.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System.Collections.Generic;
    using Naos.Logging.Domain;

    internal static class LogInclusionKindToOriginsMaps
    {
        public static readonly Dictionary<LogItemKind, IReadOnlyCollection<string>> StringAndObjectsFromItsLogEntryPosted
            = new Dictionary<LogItemKind, IReadOnlyCollection<string>>
            {
                {
                    LogItemKind.String, new[] { LogItemOrigin.ItsLogEntryPosted.ToString() }
                },
                {
                    LogItemKind.Object, new[] { LogItemOrigin.ItsLogEntryPosted.ToString() }
                },
                {
                    LogItemKind.Exception, new string[0]
                },
                {
                    LogItemKind.Telemetry, new string[0]
                },
            };

        public static readonly Dictionary<LogItemKind, IReadOnlyCollection<string>> ExceptionsFromAnywhere =
            new Dictionary<LogItemKind, IReadOnlyCollection<string>>
            {
                {
                    LogItemKind.String, new string[0]
                },
                {
                    LogItemKind.Object, new string[0]
                },
                {
                    LogItemKind.Exception, null
                },
                {
                    LogItemKind.Telemetry, new string[0]
                },
            };

        public static readonly Dictionary<LogItemKind, IReadOnlyCollection<string>> TelemetryFromNowhere =
            new Dictionary<LogItemKind, IReadOnlyCollection<string>>
            {
                {
                    LogItemKind.String, new string[0]
                },
                {
                    LogItemKind.Object, new string[0]
                },
                {
                    LogItemKind.Exception, new string[0]
                },
                {
                    LogItemKind.Telemetry, new string[0]
                },
            };

        public static readonly Dictionary<LogItemKind, IReadOnlyCollection<string>> TelemetryFromAnywhere =
            new Dictionary<LogItemKind, IReadOnlyCollection<string>>
            {
                {
                    LogItemKind.String, new string[0]
                },
                {
                    LogItemKind.Object, new string[0]
                },
                {
                    LogItemKind.Exception, new string[0]
                },
                {
                    LogItemKind.Telemetry, null
                },
            };

        public static readonly IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<string>> AnythingFromAnywhere
            = new Dictionary<LogItemKind, IReadOnlyCollection<string>>();
    }
}
