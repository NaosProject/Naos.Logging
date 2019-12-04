// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventLogWriter.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Persistence
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Naos.Logging.Domain;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// <see cref="EventLog"/> focused implementation of <see cref="LogWriterBase" />.
    /// </summary>
    public class EventLogWriter : LogWriterBase
    {
        private const int MaxEventId = 65535;  // https://stackoverflow.com/questions/1755615/what-event-id-to-use-for-my-custom-event-log-entries

        private readonly EventLogConfig eventLogConfig;

        private readonly object syncEventId = new object();

        // ReSharper disable once RedundantDefaultMemberInitializer - prefer explicit assignment here.
        private int eventId = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogWriter"/> class.
        /// </summary>
        /// <param name="eventLogConfig">Configuration.</param>
        public EventLogWriter(
            EventLogConfig eventLogConfig)
            : base(eventLogConfig)
        {
            this.eventLogConfig = eventLogConfig ?? throw new ArgumentNullException(nameof(eventLogConfig));

            if (this.eventLogConfig.ShouldCreateSourceIfMissing)
            {
                this.eventLogConfig.CreateEventLogSourceIfMissing();
            }
        }

        /// <inheritdoc />
        protected override void LogInternal(
            LogItem logItem)
        {
            if (logItem == null)
            {
                throw new ArgumentNullException(nameof(logItem));
            }

            Interlocked.Increment(ref this.eventId);
            if (this.eventId >= MaxEventId)
            {
                lock (this.syncEventId)
                {
                    if (this.eventId >= MaxEventId)
                    {
                        this.eventId = 1;
                    }
                }
            }

            using (var eventLog = this.eventLogConfig.NewEventLogObject())
            {
                var eventLogEntryType = logItem.Kind == LogItemKind.Exception
                    ? EventLogEntryType.Error
                    : EventLogEntryType.Information;

                var bytes = EventLogConfig.EventLogRawDataLogItemSerializer.SerializeToBytes(logItem);
                var logMessage = BuildLogMessageFromLogItem(logItem, this.eventLogConfig.LogItemPropertiesToIncludeInLogMessage);
                var wasEnumerationOrigin = Enum.TryParse<LogItemOrigin>(logItem.Context.Origin, true, out LogItemOrigin enumerationOrigin);
                if (!wasEnumerationOrigin)
                {
                    enumerationOrigin = LogItemOrigin.Unknown;
                }

                eventLog.WriteEntry(logMessage, eventLogEntryType, this.eventId, (short)enumerationOrigin, bytes);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var ret = Invariant($"{this.GetType().ToStringReadable()}; {nameof(this.eventLogConfig.LogInclusionKindToOriginsMap)}: {this.eventLogConfig.LogInclusionKindToOriginsMapFriendlyString}; {nameof(this.eventLogConfig.Source)}: {this.eventLogConfig.Source}; {nameof(this.eventLogConfig.ShouldCreateSourceIfMissing)}: {this.eventLogConfig.ShouldCreateSourceIfMissing}");
            return ret;
        }
    }
}
