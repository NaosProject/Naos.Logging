// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventLogReader.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// <see cref="EventLog"/> focused implementation of <see cref="LogReaderBase" />.
    /// </summary>
    public class EventLogReader : LogReaderBase
    {
        private readonly EventLogConfiguration eventLogConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogReader"/> class.
        /// </summary>
        /// <param name="eventLogConfiguration">Configuration.</param>
        public EventLogReader(EventLogConfiguration eventLogConfiguration)
            : base(eventLogConfiguration)
        {
            this.eventLogConfiguration = eventLogConfiguration;
        }

        /// <inheritdoc cref="LogReaderBase" />
        public override IReadOnlyCollection<LogMessage> ReadAll()
        {
            var ret = new List<LogMessage>();
            using (var eventLog = this.eventLogConfiguration.NewEventLogObject())
            {
                foreach (EventLogEntry entry in eventLog.Entries)
                {
                    var logMessage = new LogMessage((LogContexts)entry.CategoryNumber, entry.Message, entry.TimeWritten.ToUniversalTime());
                    ret.Add(logMessage);
                }
            }

            return ret;
        }

        /// <inheritdoc cref="LogReaderBase" />
        public override IReadOnlyCollection<LogMessage> ReadRange(DateTime startUtc, DateTime endUtc)
        {
            throw new NotSupportedException("Event Log does not support reading ranges of time.");
        }
    }
}