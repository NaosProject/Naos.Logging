// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogConfig.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using Its.Log.Instrumentation;

    using OBeautifulCode.Math.Recipes;

    /// <summary>
    /// <see cref="Console" /> focused implementation of <see cref="LogWriterConfigBase" />.
    /// </summary>
    public class ConsoleLogConfig : LogWriterConfigBase, IEquatable<ConsoleLogConfig>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogConfig"/> class.
        /// </summary>
        /// <param name="includeLogEntrySubjectAndParameters">Indicates whether to include <see cref="Its.Log"/> <see cref="LogEntry"/> subject and parameters when building the string message to log; DEFAULT is false</param>
        /// <param name="originsToLogConsoleOut">The log-item origins to log to <see cref="Console.Out" />.</param>
        /// <param name="originsToLogConsoleError">The log-item origins to log to <see cref="Console.Error" />.</param>
        public ConsoleLogConfig(
            bool includeLogEntrySubjectAndParameters,
            LogItemOrigins originsToLogConsoleOut,
            LogItemOrigins originsToLogConsoleError)
            : base(originsToLogConsoleOut | originsToLogConsoleError, includeLogEntrySubjectAndParameters)
        {
            this.OriginsToLogConsoleOut = originsToLogConsoleOut;
            this.OriginsToLogConsoleError = originsToLogConsoleError;
        }

        /// <summary>
        /// Gets the log-item origins to log to <see cref="Console.Out" />
        /// </summary>
        public LogItemOrigins OriginsToLogConsoleOut { get; private set; }

        /// <summary>
        /// Gets the log-item origins to log to <see cref="Console.Error" />
        /// </summary>
        public LogItemOrigins OriginsToLogConsoleError { get; private set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(
            ConsoleLogConfig first,
            ConsoleLogConfig second)
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
                         (first.IncludeLogEntrySubjectAndParameters == second.IncludeLogEntrySubjectAndParameters) &&
                         (first.OriginsToLogConsoleOut == second.OriginsToLogConsoleOut) &&
                         (first.OriginsToLogConsoleError == second.OriginsToLogConsoleError);
            return result;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(
            ConsoleLogConfig first,
            ConsoleLogConfig second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(
            ConsoleLogConfig other) => this == other;

        /// <inheritdoc />
        public override bool Equals(
            object obj) => this == (obj as ConsoleLogConfig);

        /// <inheritdoc />
        public override int GetHashCode() =>
            HashCodeHelper
                .Initialize()
                .Hash(this.OriginsToLog)
                .Hash(this.IncludeLogEntrySubjectAndParameters)
                .Hash(this.OriginsToLogConsoleOut)
                .Hash(this.OriginsToLogConsoleError)
                .Value;
    }
}