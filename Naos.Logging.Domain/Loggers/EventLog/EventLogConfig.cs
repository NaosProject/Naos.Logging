// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventLogConfig.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Diagnostics;

    using Its.Log.Instrumentation;

    using Naos.Diagnostics.Domain;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;

    using OBeautifulCode.Math.Recipes;

    /// <summary>
    /// <see cref="EventLog"/> focused implementation of <see cref="LogWriterConfigBase" />.
    /// </summary>
    public class EventLogConfig : LogWriterConfigBase, IEquatable<EventLogConfig>
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
        /// Serializes and deserializes a log-item.
        /// </summary>
        public static readonly IBinarySerializeAndDeserialize LogItemSerializer = new NaosJsonSerializer();

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogConfig"/> class.
        /// </summary>
        /// <param name="originsToLog">The log-item origins to log for.</param>
        /// <param name="logEntryPropertiesToIncludeInLogMessage"> The properties/aspects of an <see cref="Its.Log"/> <see cref="LogEntry"/> to include when building a log message.</param>
        /// <param name="source">Optional event log source; DEFAULT is running process' name.</param>
        /// <param name="shouldCreateSourceIfMissing">Value indicating whether or not to create the source if missing.</param>
        /// <param name="logName">Optional log name; DEFAULT is <see cref="DefaultLogName" />.</param>
        /// <param name="machineName">Optional machine name; DEFAULT is <see cref="DefaultMachineName" />.</param>
        public EventLogConfig(
            LogItemOrigins originsToLog,
            LogEntryPropertiesToIncludeInLogMessage logEntryPropertiesToIncludeInLogMessage,
            string source = null,
            string logName = DefaultLogName,
            string machineName = DefaultMachineName,
            bool shouldCreateSourceIfMissing = false)
            : base(originsToLog, logEntryPropertiesToIncludeInLogMessage)
        {
            this.Source = string.IsNullOrWhiteSpace(source) ? ProcessHelpers.GetRunningProcess().Name() : source;
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
        public static bool operator ==(
            EventLogConfig first,
            EventLogConfig second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            var result = (first.OriginsToLog == second.OriginsToLog) &&
                         (first.LogEntryPropertiesToIncludeInLogMessage == second.LogEntryPropertiesToIncludeInLogMessage) &&
                         (first.Source == second.Source) &&
                         (first.ShouldCreateSourceIfMissing == second.ShouldCreateSourceIfMissing) &&
                         (first.LogName == second.LogName) &&
                         (first.MachineName == second.MachineName);
            return result;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(
            EventLogConfig first,
            EventLogConfig second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(
            EventLogConfig other) => this == other;

        /// <inheritdoc />
        public override bool Equals(
            object obj) => this == (obj as EventLogConfig);

        /// <inheritdoc />
        public override int GetHashCode() =>
            HashCodeHelper
                .Initialize()
                .Hash(this.OriginsToLog)
                .Hash(this.LogEntryPropertiesToIncludeInLogMessage)
                .Hash(this.Source)
                .Hash(this.ShouldCreateSourceIfMissing)
                .Hash(this.LogName)
                .Hash(this.MachineName)
                .Value;
    }
}