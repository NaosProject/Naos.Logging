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
        private readonly EventLogConfig _eventLogConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogReader"/> class.
        /// </summary>
        /// <param name="eventLogConfig">Configuration.</param>
        public EventLogReader(EventLogConfig eventLogConfig)
            : base(eventLogConfig)
        {
            this._eventLogConfig = eventLogConfig;
        }

        /// <inheritdoc cref="LogReaderBase" />
        public override IReadOnlyCollection<LogItem> ReadAll()
        {
            var ret = new List<LogItem>();
            using (var eventLog = this._eventLogConfig.NewEventLogObject())
            {
                foreach (EventLogEntry entry in eventLog.Entries)
                {
                    var logMessage = new LogItem((LogItemOrigins)entry.CategoryNumber, entry.Message, entry.TimeWritten.ToUniversalTime());
                    ret.Add(logMessage);
                }
            }

            return ret;
        }

        /// <inheritdoc cref="LogReaderBase" />
        public override IReadOnlyCollection<LogItem> ReadRange(DateTime startUtc, DateTime endUtc)
        {
            throw new NotSupportedException("Event Log does not support reading ranges of time.");
        }
    }
}