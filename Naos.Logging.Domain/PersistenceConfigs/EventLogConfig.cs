// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventLogConfig.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Naos.Diagnostics.Recipes;
    using OBeautifulCode.Equality.Recipes;

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
        /// Initializes a new instance of the <see cref="EventLogConfig"/> class.
        /// </summary>
        /// <param name="logInclusionKindToOriginsMap">The log-item origins to log for.</param>
        /// <param name="source">Optional event log source; DEFAULT is running process' name.</param>
        /// <param name="shouldCreateSourceIfMissing">Value indicating whether or not to create the source if missing.</param>
        /// <param name="logName">Optional log name; DEFAULT is <see cref="DefaultLogName" />.</param>
        /// <param name="machineName">Optional machine name; DEFAULT is <see cref="DefaultMachineName" />.</param>
        /// <param name="logItemPropertiesToIncludeInLogMessage"> The properties/aspects of a <see cref="LogItem"/> to include when building a log message.</param>
        public EventLogConfig(
            IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<string>> logInclusionKindToOriginsMap,
            string source = null,
            string logName = DefaultLogName,
            string machineName = DefaultMachineName,
            bool shouldCreateSourceIfMissing = false,
            LogItemPropertiesToIncludeInLogMessage logItemPropertiesToIncludeInLogMessage = LogItemPropertiesToIncludeInLogMessage.Default)
            : base(logInclusionKindToOriginsMap, logItemPropertiesToIncludeInLogMessage)
        {
            this.Source = string.IsNullOrWhiteSpace(source) ? ProcessHelpers.GetRunningProcess().GetName() : source;
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
        /// Determines whether two objects of type <see cref="EventLogConfig"/> are equal.
        /// </summary>
        /// <param name="left">The object to the left of the operator.</param>
        /// <param name="right">The object to the right of the operator.</param>
        /// <returns>True if the two items are equal; false otherwise.</returns>
        public static bool operator ==(
            EventLogConfig left,
            EventLogConfig right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            var result = left.BaseEquals(right) &&
                         string.Equals(left.Source, right.Source, StringComparison.Ordinal) &&
                         left.ShouldCreateSourceIfMissing == right.ShouldCreateSourceIfMissing &&
                         string.Equals(left.LogName, right.LogName, StringComparison.Ordinal) &&
                         string.Equals(left.MachineName, right.MachineName, StringComparison.Ordinal);

            return result;
        }

        /// <summary>
        /// Determines whether two objects of type <see cref="EventLogConfig"/> are not equal.
        /// </summary>
        /// <param name="left">The object to the left of the operator.</param>
        /// <param name="right">The object to the right of the operator.</param>
        /// <returns>True if the two items not equal; false otherwise.</returns>
        public static bool operator !=(
            EventLogConfig left,
            EventLogConfig right)
            => !(left == right);

        /// <inheritdoc />
        public bool Equals(EventLogConfig other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as EventLogConfig);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize(this.GetBaseHashCode())
                                                           .Hash(this.Source)
                                                           .Hash(this.ShouldCreateSourceIfMissing)
                                                           .Hash(this.LogName)
                                                           .Hash(this.MachineName)
                                                           .Value;
    }
}
