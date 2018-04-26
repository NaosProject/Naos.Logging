// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLogConfig.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.IO;

    using Its.Log.Instrumentation;

    using OBeautifulCode.Math.Recipes;

    using Spritely.Recipes;

    /// <summary>
    /// <see cref="File"/> focused implementation of <see cref="LogWriterConfigBase" />.
    /// </summary>
    public class FileLogConfig : LogWriterConfigBase, IEquatable<FileLogConfig>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogConfig"/> class.
        /// </summary>
        /// <param name="originsToLog">The log-item origins to log for.</param>
        /// <param name="logEntryPropertiesToIncludeInLogMessage"> The properties/aspects of an <see cref="Its.Log"/> <see cref="LogEntry"/> to include when building a log message.</param>
        /// <param name="logFilePath">File path to write logs to.</param>
        /// <param name="createDirectoryStructureIfMissing">Optional value indicating whether to create the directory structure if it's missing; DEFAULT is true.</param>
        public FileLogConfig(
            LogItemOrigins originsToLog,
            LogEntryPropertiesToIncludeInLogMessage logEntryPropertiesToIncludeInLogMessage,
            string logFilePath,
            bool createDirectoryStructureIfMissing = true)
            : base(originsToLog, logEntryPropertiesToIncludeInLogMessage)
        {
            new { logFilePath }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();

            this.LogFilePath = logFilePath;
            this.CreateDirectoryStructureIfMissing = createDirectoryStructureIfMissing;
        }

        /// <summary>
        /// Gets the file path to write logs to.
        /// </summary>
        public string LogFilePath { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to create the directory structure if it's missing.
        /// </summary>
        public bool CreateDirectoryStructureIfMissing { get; private set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(FileLogConfig first, FileLogConfig second)
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
                         (first.LogFilePath == second.LogFilePath) &&
                         (first.CreateDirectoryStructureIfMissing == second.CreateDirectoryStructureIfMissing);
            return result;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(
            FileLogConfig first,
            FileLogConfig second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(
            FileLogConfig other) => this == other;

        /// <inheritdoc />
        public override bool Equals(
            object obj) => this == (obj as FileLogConfig);

        /// <inheritdoc />
        public override int GetHashCode() =>
            HashCodeHelper
                .Initialize()
                .Hash(this.OriginsToLog)
                .Hash(this.LogEntryPropertiesToIncludeInLogMessage)
                .Hash(this.LogFilePath)
                .Hash(this.CreateDirectoryStructureIfMissing)
                .Value;
    }
}