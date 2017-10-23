// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogConfigurationEventLog.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Diagnostics;

    using Its.Log.Instrumentation;

    using OBeautifulCode.Math.Recipes;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// <see cref="EventLog"/> focused implementation of <see cref="LogConfigurationBase" />.
    /// </summary>
    public class LogConfigurationEventLog : LogConfigurationBase, IEquatable<LogConfigurationEventLog>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogConfigurationEventLog"/> class.
        /// </summary>
        /// <param name="contextsToLog">Contexts to log.</param>
        /// <param name="machineName">Machine name.</param>
        /// <param name="logName">Log name.</param>
        /// <param name="source">Event log source to use.</param>
        public LogConfigurationEventLog(LogContexts contextsToLog, string logName, string machineName, string source)
            : base(contextsToLog)
        {
            new { logName }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
            new { machineName }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
            new { source }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();

            this.LogName = logName;
            this.MachineName = machineName;
            this.Source = source;
        }

        /// <summary>
        /// Gets the event log source to use.
        /// </summary>
        public string LogName { get; private set; }

        /// <summary>
        /// Gets the event log source to use.
        /// </summary>
        public string MachineName { get; private set; }

        /// <summary>
        /// Gets the event log source to use.
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(LogConfigurationEventLog first, LogConfigurationEventLog second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.ContextsToLog == second.ContextsToLog && first.LogName == second.LogName && first.MachineName == second.MachineName && first.Source == second.Source;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(LogConfigurationEventLog first, LogConfigurationEventLog second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(LogConfigurationEventLog other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as LogConfigurationEventLog);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.ContextsToLog.ToString()).Hash(this.LogName).Hash(this.MachineName).Hash(this.Source).Value;
    }

    /// <summary>
    /// <see cref="EventLog"/> focused implementation of <see cref="LogProcessorBase" />.
    /// </summary>
    public class LogProcessorEventLog : LogProcessorBase
    {
        private readonly LogConfigurationEventLog eventLogConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogProcessorEventLog"/> class.
        /// </summary>
        /// <param name="eventLogConfiguration">Configuration.</param>
        public LogProcessorEventLog(LogConfigurationEventLog eventLogConfiguration)
            : base(eventLogConfiguration)
        {
            new { eventLogConfiguration }.Must().NotBeNull().OrThrowFirstFailure();

            this.eventLogConfiguration = eventLogConfiguration;
        }

        /// <inheritdoc cref="LogProcessorBase" />
        protected override void InternalLog(LogEntry logEntry)
        {
            new { logEntry }.Must().NotBeNull().OrThrowFirstFailure();

            using (var eventLog = new EventLog(this.eventLogConfiguration.LogName, this.eventLogConfiguration.MachineName, this.eventLogConfiguration.Source))
            {
                var eventLogEntryType = logEntry.Subject is Exception ? EventLogEntryType.Error : EventLogEntryType.Information;

                eventLog.WriteEntry(logEntry.ToLogString(), eventLogEntryType);
            }
        }

        /// <inheritdoc cref="object" />
        public override string ToString()
        {
            var ret = Invariant($"{this.GetType().FullName}; {nameof(this.eventLogConfiguration.Source)}: {this.eventLogConfiguration.Source}");
            return ret;
        }
    }
}