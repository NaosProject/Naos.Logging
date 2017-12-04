// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventLogConfiguration.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Diagnostics;

    using OBeautifulCode.Math.Recipes;

    using Spritely.Recipes;

    /// <summary>
    /// <see cref="EventLog"/> focused implementation of <see cref="LogConfigurationBase" />.
    /// </summary>
    public class EventLogConfiguration : LogConfigurationBase, IEquatable<EventLogConfiguration>
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
        /// Initializes a new instance of the <see cref="EventLogConfiguration"/> class.
        /// </summary>
        /// <param name="contextsToLog">Contexts to log.</param>
        /// <param name="source">Optional event log source; DEFAULT is <see cref="Process.GetCurrentProcess"/> <see cref="Process.ProcessName" />.</param>
        /// <param name="shouldCreateSourceIfMissing">Value indicating whether or not to create the source if missing.</param>
        /// <param name="logName">Optional log name; DEFAULT is <see cref="DefaultLogName" />.</param>
        /// <param name="machineName">Optional machine name; DEFAULT is <see cref="DefaultMachineName" />.</param>
        public EventLogConfiguration(LogContexts contextsToLog, string source = null, string logName = DefaultLogName, string machineName = DefaultMachineName, bool shouldCreateSourceIfMissing = false)
            : base(contextsToLog)
        {
            this.Source = string.IsNullOrWhiteSpace(source) ? Process.GetCurrentProcess().ProcessName : source;
            this.ShouldCreateSourceIfMissing = shouldCreateSourceIfMissing;
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
        public bool ShouldCreateSourceIfMissing { get; private set; }

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
        public static bool operator ==(EventLogConfiguration first, EventLogConfiguration second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.ContextsToLog == second.ContextsToLog && first.Source == second.Source && first.ShouldCreateSourceIfMissing == second.ShouldCreateSourceIfMissing && first.LogName == second.LogName && first.MachineName == second.MachineName;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(EventLogConfiguration first, EventLogConfiguration second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(EventLogConfiguration other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as EventLogConfiguration);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.ContextsToLog.ToString()).Hash(this.Source).Hash(this.ShouldCreateSourceIfMissing).Hash(this.LogName).Hash(this.MachineName).Value;
    }

    /// <summary>
    /// Extensions on <see cref="EventLogConfiguration" />.
    /// </summary>
    public static class EventLogConfigurationExtensions
    {
        /// <summary>
        /// Create the <see cref="EventLog" /> <see cref="EventLog.Source" /> from configuration if it does not exist (requires considerable set of permissions).
        /// </summary>
        /// <param name="eventLogConfiguration">Configuration to use for creating source.</param>
        public static void CreateEventLogSourceIfMissing(this EventLogConfiguration eventLogConfiguration)
        {
            new { eventLogConfiguration }.Must().NotBeNull().OrThrowFirstFailure();

            new { CreateSourceIfMissing = eventLogConfiguration.ShouldCreateSourceIfMissing }.Must().BeTrue().OrThrowFirstFailure();

            if (!EventLog.SourceExists(eventLogConfiguration.Source))
            {
                var sourceData = new EventSourceCreationData(eventLogConfiguration.Source, eventLogConfiguration.LogName);
                EventLog.CreateEventSource(sourceData);
            }
        }

        /// <summary>
        /// Builds a new <see cref="EventLog" /> object from <see cref="EventLogConfiguration" />.
        /// </summary>
        /// <param name="eventLogConfiguration">Configuration to use.</param>
        /// <returns>Event log object.</returns>
        public static EventLog NewEventLogObject(this EventLogConfiguration eventLogConfiguration)
        {
            new { eventLogConfiguration }.Must().NotBeNull().OrThrowFirstFailure();

            return new EventLog(eventLogConfiguration.LogName, eventLogConfiguration.MachineName, eventLogConfiguration.Source);
        }
    }
}