// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventLogConfigExtensions.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System.Diagnostics;

    using Spritely.Recipes;

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
            new { eventLogConfig }.Must().NotBeNull().OrThrowFirstFailure();

            new { CreateSourceIfMissing = eventLogConfig.ShouldCreateSourceIfMissing }.Must().BeTrue().OrThrowFirstFailure();

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
            new { eventLogConfig }.Must().NotBeNull().OrThrowFirstFailure();

            return new EventLog(eventLogConfig.LogName, eventLogConfig.MachineName, eventLogConfig.Source);
        }
    }
}
