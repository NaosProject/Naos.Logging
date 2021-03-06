﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventLogReader.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Naos.Logging.Domain;
    using OBeautifulCode.Serialization;

    /// <summary>
    /// <see cref="EventLog"/> focused implementation of <see cref="LogReaderBase" />.
    /// </summary>
    public class EventLogReader : LogReaderBase
    {
        private readonly EventLogConfig eventLogConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogReader"/> class.
        /// </summary>
        /// <param name="eventLogConfig">Configuration.</param>
        /// <param name="logItemSerializerFactory">Optional serializer factory; DEFAULT will be base version.</param>
        public EventLogReader(
            EventLogConfig eventLogConfig,
            ISerializerFactory logItemSerializerFactory = null)
            : base(eventLogConfig, logItemSerializerFactory)
        {
            this.eventLogConfig = eventLogConfig ?? throw new ArgumentNullException(nameof(eventLogConfig));
        }

        /// <inheritdoc cref="LogReaderBase" />
        public override IReadOnlyCollection<LogItem> ReadAll()
        {
            var result = new List<LogItem>();
            using (var eventLog = this.eventLogConfig.NewEventLogObject())
            {
                foreach (EventLogEntry entry in eventLog.Entries)
                {
                    var logItem = this.eventLogConfig.GetEventLogRawDataLogItemSerializer().Deserialize<LogItem>(entry.Data);
                    result.Add(logItem);
                }
            }

            return result;
        }

        /// <inheritdoc cref="LogReaderBase" />
        public override IReadOnlyCollection<LogItem> ReadRange(
            DateTime startUtcInclusive,
            DateTime endUtcInclusive)
        {
            throw new NotSupportedException("Event Log does not support reading ranges of time.");
        }
    }
}
