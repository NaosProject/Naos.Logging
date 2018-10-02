﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventLogConfig.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Naos.Diagnostics.Recipes;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Want a field here.")]
        public static readonly IBinarySerializeAndDeserialize EventLogRawDataLogItemSerializer = new NaosJsonSerializer();

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
            IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<LogItemOrigin>> logInclusionKindToOriginsMap,
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

            var result = (first.LogItemPropertiesToIncludeInLogMessage == second.LogItemPropertiesToIncludeInLogMessage) &&
                         (first.LogInclusionKindToOriginsMapFriendlyString == second.LogInclusionKindToOriginsMapFriendlyString) &&
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
                .Hash(this.LogItemPropertiesToIncludeInLogMessage)
                .Hash(this.LogInclusionKindToOriginsMapFriendlyString)
                .Hash(this.Source)
                .Hash(this.ShouldCreateSourceIfMissing)
                .Hash(this.LogName)
                .Hash(this.MachineName)
                .Value;
    }
}