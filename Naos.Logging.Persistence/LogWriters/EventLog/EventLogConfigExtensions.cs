// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventLogConfigExtensions.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Persistence
{
    using System;
    using System.Diagnostics;

    using static System.FormattableString;

    /// <summary>
    /// Extensions on <see cref="EventLogConfig" />.
    /// </summary>
    public static class EventLogConfigExtensions
    {
        /// <summary>
        /// Create the <see cref="EventLog" /> <see cref="EventLog.Source" /> from configuration if it does not exist (requires considerable set of permissions).
        /// </summary>
        /// <param name="eventLogConfig">Configuration to use for creating source.</param>
        public static void CreateEventLogSourceIfMissing(
            this EventLogConfig eventLogConfig)
        {
            if (eventLogConfig == null)
            {
                throw new ArgumentNullException(nameof(eventLogConfig));
            }

            if (!eventLogConfig.ShouldCreateSourceIfMissing)
            {
                throw new ArgumentException(Invariant($"{nameof(eventLogConfig)}.{nameof(EventLogConfig.ShouldCreateSourceIfMissing)} == false"));
            }

            if (!EventLog.SourceExists(eventLogConfig.Source))
            {
                var sourceData = new EventSourceCreationData(eventLogConfig.Source, eventLogConfig.LogName);
                EventLog.CreateEventSource(sourceData);
            }
        }

        /// <summary>
        /// Builds a new <see cref="EventLog" /> object from <see cref="EventLogConfig" />.
        /// </summary>
        /// <param name="eventLogConfig">Configuration to use.</param>
        /// <returns>Event log object.</returns>
        public static EventLog NewEventLogObject(
            this EventLogConfig eventLogConfig)
        {
            if (eventLogConfig == null)
            {
                throw new ArgumentNullException(nameof(eventLogConfig));
            }

            return new EventLog(eventLogConfig.LogName, eventLogConfig.MachineName, eventLogConfig.Source);
        }
    }
}
