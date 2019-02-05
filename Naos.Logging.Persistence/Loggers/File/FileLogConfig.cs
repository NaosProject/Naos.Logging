// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLogConfig.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Naos.Logging.Domain;
    using OBeautifulCode.Math.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// <see cref="File"/> focused implementation of <see cref="LogWriterConfigBase" />.
    /// </summary>
    public class FileLogConfig : LogWriterConfigBase, IEquatable<FileLogConfig>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogConfig"/> class.
        /// </summary>
        /// <param name="logInclusionKindToOriginsMap">The log-item origins to log for.</param>
        /// <param name="logFilePath">File path to write logs to.</param>
        /// <param name="createDirectoryStructureIfMissing">Optional value indicating whether to create the directory structure if it's missing; DEFAULT is true.</param>
        /// <param name="logItemPropertiesToIncludeInLogMessage"> The properties/aspects of a <see cref="LogItem"/> to include when building a log message.</param>
        public FileLogConfig(
            IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<string>> logInclusionKindToOriginsMap,
            string logFilePath,
            bool createDirectoryStructureIfMissing = true,
            LogItemPropertiesToIncludeInLogMessage logItemPropertiesToIncludeInLogMessage = LogItemPropertiesToIncludeInLogMessage.Default)
            : base(logInclusionKindToOriginsMap, logItemPropertiesToIncludeInLogMessage)
        {
            if (string.IsNullOrWhiteSpace(logFilePath))
            {
                throw new ArgumentException(Invariant($"{nameof(logFilePath)} is null or white space"));
            }

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

            var result = (first.LogItemPropertiesToIncludeInLogMessage == second.LogItemPropertiesToIncludeInLogMessage) &&
                         (first.LogInclusionKindToOriginsMapFriendlyString == second.LogInclusionKindToOriginsMapFriendlyString) &&
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
                .Hash(this.LogItemPropertiesToIncludeInLogMessage)
                .Hash(this.LogInclusionKindToOriginsMapFriendlyString)
                .Hash(this.LogFilePath)
                .Hash(this.CreateDirectoryStructureIfMissing)
                .Value;
    }
}