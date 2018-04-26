// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventLogWriter.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using OBeautifulCode.Enum.Recipes;

    /// <summary>
    /// <see cref="EventLog"/> focused implementation of <see cref="LogWriterBase" />.
    /// </summary>
    public class EventLogWriter : LogWriterBase
    {
        private const int MaxEventId = 65535;  // https://stackoverflow.com/questions/1755615/what-event-id-to-use-for-my-custom-event-log-entries

        private readonly EventLogConfig eventLogConfig;

        private readonly object syncEventId = new object();

        private int eventId = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogWriter"/> class.
        /// </summary>
        /// <param name="eventLogConfig">Configuration.</param>
        public EventLogWriter(
            EventLogConfig eventLogConfig)
            : base(eventLogConfig)
        {
            if (eventLogConfig == null)
            {
                throw new ArgumentNullException(nameof(eventLogConfig));
            }

            this.eventLogConfig = eventLogConfig;

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

            var origins = logItem.Context.LogItemOrigin.ToOrigins();
            using (var eventLog = this.eventLogConfig.NewEventLogObject())
            {
                var eventLogEntryType =
                    LogItemOrigins.AllErrors.HasFlagOverlap(origins) ?
                        EventLogEntryType.Error :
                        EventLogEntryType.Information;

                eventLog.WriteEntry(logItem.Message, eventLogEntryType, this.eventId, (short)logItem.Context.LogItemOrigin);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var ret = FormattableString.Invariant($"{this.GetType().FullName}; {nameof(this.eventLogConfig.OriginsToLog)}: {this.eventLogConfig.OriginsToLog}; {nameof(this.eventLogConfig.Source)}: {this.eventLogConfig.Source}; {nameof(this.eventLogConfig.ShouldCreateSourceIfMissing)}: {this.eventLogConfig.ShouldCreateSourceIfMissing}");
            return ret;
        }
    }
}