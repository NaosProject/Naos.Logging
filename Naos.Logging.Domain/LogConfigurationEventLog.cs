// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogConfigurationEventLog.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using OBeautifulCode.Enum.Recipes;
    using OBeautifulCode.Math.Recipes;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// <see cref="EventLog"/> focused implementation of <see cref="LogConfigurationBase" />.
    /// </summary>
    public class LogConfigurationEventLog : LogConfigurationBase, IEquatable<LogConfigurationEventLog>
    {
        /// <summary>
        /// Default <see cref="EventLog.Log" /> to use; "Application".
        /// </summary>
        public const string DefaultLogName = "Application";

        /// <summary>
        /// Default <see cref="EventLog.MachineName" /> to use; "." (current machine interpreted by <see cref="EventLog" />).
        /// </summary>
        public const string DefaultMachineName = ".";

        /// <summary>
        /// Initializes a new instance of the <see cref="LogConfigurationEventLog"/> class.
        /// </summary>
        /// <param name="contextsToLog">Contexts to log.</param>
        /// <param name="source">Optional event log source; DEFAULT is <see cref="Process.GetCurrentProcess"/> <see cref="Process.ProcessName" />.</param>
        /// <param name="shouldCreateSource">Value indicating whether or not to create the source if missing.</param>
        /// <param name="logName">Optional log name; DEFAULT is <see cref="DefaultLogName" />.</param>
        /// <param name="machineName">Optional machine name; DEFAULT is <see cref="DefaultMachineName" />.</param>
        public LogConfigurationEventLog(LogContexts contextsToLog, string source = null, bool shouldCreateSource = false, string logName = DefaultLogName, string machineName = DefaultMachineName)
            : base(contextsToLog)
        {
            this.Source = string.IsNullOrWhiteSpace(source) ? Process.GetCurrentProcess().ProcessName : source;
            this.ShouldCreateSource = shouldCreateSource;
            this.LogName = string.IsNullOrWhiteSpace(logName) ? DefaultLogName : logName;
            this.MachineName = string.IsNullOrWhiteSpace(machineName) ? DefaultMachineName : machineName;
        }

        /// <summary>
        /// Gets the event log source to use.
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not to create the source if missing.
        /// </summary>
        public bool ShouldCreateSource { get; private set; }

        /// <summary>
        /// Gets the event log source to use.
        /// </summary>
        public string LogName { get; private set; }

        /// <summary>
        /// Gets the event log source to use.
        /// </summary>
        public string MachineName { get; private set; }

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

            return first.ContextsToLog == second.ContextsToLog && first.Source == second.Source && first.ShouldCreateSource == second.ShouldCreateSource && first.LogName == second.LogName && first.MachineName == second.MachineName;
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
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.ContextsToLog.ToString()).Hash(this.Source).Hash(this.ShouldCreateSource).Hash(this.LogName).Hash(this.MachineName).Value;
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

            if (this.eventLogConfiguration.ShouldCreateSource)
            {
                this.eventLogConfiguration.CreateEventLogSourceIfMissing();
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

            using (var eventLog = new EventLog(this.eventLogConfiguration.LogName, this.eventLogConfiguration.MachineName, this.eventLogConfiguration.Source))
            {
                var eventLogEntryType =
                    LogContexts.AllErrors.HasFlagOverlap(logItem.Context) ?
                        EventLogEntryType.Error :
                        EventLogEntryType.Information;

                var logMessage = logItem.BuildLogMessage();
                eventLog.WriteEntry(logMessage, eventLogEntryType);
            }
        }

        /// <inheritdoc cref="object" />
        public override string ToString()
        {
            var ret = Invariant($"{this.GetType().FullName}; {nameof(this.eventLogConfiguration.Source)}: {this.eventLogConfiguration.Source}; {nameof(this.eventLogConfiguration.ShouldCreateSource)}: {this.eventLogConfiguration.ShouldCreateSource}");
            return ret;
        }
    }

    /// <summary>
    /// Extensions on <see cref="LogConfigurationEventLog" />.
    /// </summary>
    public static class EventLogConfigurationExtensions
    {
        /// <summary>
        /// Create the <see cref="EventLog" /> <see cref="EventLog.Source" /> from configuration if it does not exist (requires considerable set of permissions).
        /// </summary>
        /// <param name="eventLogConfiguration">Configuration to use for creating source.</param>
        public static void CreateEventLogSourceIfMissing(this LogConfigurationEventLog eventLogConfiguration)
        {
            new { eventLogConfiguration }.Must().NotBeNull().OrThrowFirstFailure();

            new { CreateSourceIfMissing = eventLogConfiguration.ShouldCreateSource }.Must().BeTrue().OrThrowFirstFailure();

            if (!EventLog.SourceExists(eventLogConfiguration.Source))
            {
                var sourceData = new EventSourceCreationData(eventLogConfiguration.Source, eventLogConfiguration.LogName);
                EventLog.CreateEventSource(sourceData);
            }
        }
    }
}