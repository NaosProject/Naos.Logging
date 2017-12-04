// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventLogProcessor.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using OBeautifulCode.Enum.Recipes;

    using Spritely.Recipes;

    /// <summary>
    /// <see cref="EventLog"/> focused implementation of <see cref="LogProcessorBase" />.
    /// </summary>
    public class EventLogProcessor : LogProcessorBase
    {
        private const int MaxEventId = 65535;  // https://stackoverflow.com/questions/1755615/what-event-id-to-use-for-my-custom-event-log-entries

        private readonly EventLogConfiguration eventLogConfiguration;

        private readonly object syncEventId = new object();

        private int eventId = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogProcessor"/> class.
        /// </summary>
        /// <param name="eventLogConfiguration">Configuration.</param>
        public EventLogProcessor(EventLogConfiguration eventLogConfiguration)
            : base(eventLogConfiguration)
        {
            new { eventLogConfiguration }.Must().NotBeNull().OrThrowFirstFailure();

            this.eventLogConfiguration = eventLogConfiguration;

            if (this.eventLogConfiguration.ShouldCreateSourceIfMissing)
            {
                this.eventLogConfiguration.CreateEventLogSourceIfMissing();
            }
        }

        /// <inheritdoc cref="LogProcessorBase" />
        public override void Log(LogMessage logMessage)
        {
            // if it is has the None flag then cut out.
            if (this.eventLogConfiguration.ContextsToLog.HasFlag(LogContexts.None))
            {
                return;
            }

            new { logMessage }.Must().NotBeNull().OrThrowFirstFailure();

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

            using (var eventLog = this.eventLogConfiguration.NewEventLogObject())
            {
                var eventLogEntryType =
                    LogContexts.AllErrors.HasFlagOverlap(logMessage.Context) ?
                        EventLogEntryType.Error :
                        EventLogEntryType.Information;

                eventLog.WriteEntry(logMessage.Message, eventLogEntryType, this.eventId, (short)logMessage.Context);
            }
        }

        /// <inheritdoc cref="LogProcessorBase" />
        protected override void InternalLog(LogItem logItem)
        {
            // if it is has the None flag then cut out.
            if (this.eventLogConfiguration.ContextsToLog.HasFlag(LogContexts.None))
            {
                return;
            }

            new { logItem }.Must().NotBeNull().OrThrowFirstFailure();

            var logMessage = new LogMessage(logItem.Context, logItem.BuildLogMessage(), logItem.LoggedTimeUtc);

            this.Log(logMessage);
        }

        /// <inheritdoc cref="object" />
        public override string ToString()
        {
            var ret = FormattableString.Invariant($"{this.GetType().FullName}; {nameof(this.eventLogConfiguration.ContextsToLog)}: {this.eventLogConfiguration.ContextsToLog}; {nameof(this.eventLogConfiguration.Source)}: {this.eventLogConfiguration.Source}; {nameof(this.eventLogConfiguration.ShouldCreateSourceIfMissing)}: {this.eventLogConfiguration.ShouldCreateSourceIfMissing}");
            return ret;
        }
    }
}