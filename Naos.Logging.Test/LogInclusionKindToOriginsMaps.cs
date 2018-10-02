// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogInclusionKindToOriginsMaps.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System.Collections.Generic;
    using Naos.Logging.Domain;

    internal static class LogInclusionKindToOriginsMaps
    {
        public static readonly Dictionary<LogItemKind, IReadOnlyCollection<LogItemOrigin>> StringAndObjectsFromItsLogEntryPosted
            = new Dictionary<LogItemKind, IReadOnlyCollection<LogItemOrigin>>
            {
                {
                    LogItemKind.String, new[] { LogItemOrigin.ItsLogEntryPosted }
                },
                {
                    LogItemKind.Object, new[] { LogItemOrigin.ItsLogEntryPosted }
                },
                {
                    LogItemKind.Exception, new LogItemOrigin[0]
                },
                {
                    LogItemKind.Telemetry, new LogItemOrigin[0]
                },
            };

        public static readonly Dictionary<LogItemKind, IReadOnlyCollection<LogItemOrigin>> ExceptionsFromAnywhere =
            new Dictionary<LogItemKind, IReadOnlyCollection<LogItemOrigin>>
            {
                {
                    LogItemKind.String, new LogItemOrigin[0]
                },
                {
                    LogItemKind.Object, new LogItemOrigin[0]
                },
                {
                    LogItemKind.Exception, null
                },
                {
                    LogItemKind.Telemetry, new LogItemOrigin[0]
                },
            };

        public static readonly Dictionary<LogItemKind, IReadOnlyCollection<LogItemOrigin>> TelemetryFromNowhere =
            new Dictionary<LogItemKind, IReadOnlyCollection<LogItemOrigin>>
            {
                {
                    LogItemKind.String, new LogItemOrigin[0]
                },
                {
                    LogItemKind.Object, new LogItemOrigin[0]
                },
                {
                    LogItemKind.Exception, new LogItemOrigin[0]
                },
                {
                    LogItemKind.Telemetry, new LogItemOrigin[0]
                },
            };

        public static readonly Dictionary<LogItemKind, IReadOnlyCollection<LogItemOrigin>> TelemetryFromAnywhere =
            new Dictionary<LogItemKind, IReadOnlyCollection<LogItemOrigin>>
            {
                {
                    LogItemKind.String, new LogItemOrigin[0]
                },
                {
                    LogItemKind.Object, new LogItemOrigin[0]
                },
                {
                    LogItemKind.Exception, new LogItemOrigin[0]
                },
                {
                    LogItemKind.Telemetry, null
                },
            };

        public static readonly IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<LogItemOrigin>> AnythingFromAnywhere
            = new Dictionary<LogItemKind, IReadOnlyCollection<LogItemOrigin>>();
    }
}