// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryLogConfig.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using Its.Log.Instrumentation;

    using OBeautifulCode.Math.Recipes;

    using Spritely.Recipes;

    /// <summary>
    /// In memory implementation of <see cref="LogWriterConfigBase" />.
    /// </summary>
    public class InMemoryLogConfig : LogWriterConfigBase, IEquatable<InMemoryLogConfig>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryLogConfig"/> class.
        /// </summary>
        /// <param name="originsToLog">The log-item origins to log for.</param>
        /// <param name="logEntryPropertiesToIncludeInLogMessage"> The properties/aspects of an <see cref="Its.Log"/> <see cref="LogEntry"/> to include when building a log message.</param>
        /// <param name="maxLoggedItemCount">Optional maximum number of elements to keep internally before removing the oldest items; DEFAULT is -1 which is infinite.</param>
        public InMemoryLogConfig(
            LogItemOrigins originsToLog,
            LogEntryPropertiesToIncludeInLogMessage logEntryPropertiesToIncludeInLogMessage,
            int maxLoggedItemCount = -1)
            : base(originsToLog, logEntryPropertiesToIncludeInLogMessage)
        {
            new { maxLoggedItemCount }.Must().BeGreaterThanOrEqualTo(-1).OrThrowFirstFailure();

            this.MaxLoggedItemCount = maxLoggedItemCount;
        }

        /// <summary>
        /// Gets the maximum number of elements to keep internally before removing the oldest items.
        /// </summary>
        public int MaxLoggedItemCount { get; private set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(
            InMemoryLogConfig first,
            InMemoryLogConfig second)
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
                         (first.MaxLoggedItemCount == second.MaxLoggedItemCount);
            return result;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(
            InMemoryLogConfig first,
            InMemoryLogConfig second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(
            InMemoryLogConfig other) => this == other;

        /// <inheritdoc />
        public override bool Equals(
            object obj) => this == (obj as InMemoryLogConfig);

        /// <inheritdoc />
        public override int GetHashCode() =>
            HashCodeHelper
                .Initialize()
                .Hash(this.OriginsToLog)
                .Hash(this.LogEntryPropertiesToIncludeInLogMessage)
                .Hash(this.MaxLoggedItemCount)
                .Value;
    }
}