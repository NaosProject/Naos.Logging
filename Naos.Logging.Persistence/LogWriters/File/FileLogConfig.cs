// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLogConfig.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
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
        /// Determines whether two objects of type <see cref="FileLogConfig"/> are equal.
        /// </summary>
        /// <param name="left">The object to the left of the operator.</param>
        /// <param name="right">The object to the right of the operator.</param>
        /// <returns>True if the two items are equal; false otherwise.</returns>
        public static bool operator ==(
            FileLogConfig left,
            FileLogConfig right)
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
                         string.Equals(left.LogFilePath, right.LogFilePath, StringComparison.Ordinal) &&
                         left.CreateDirectoryStructureIfMissing == right.CreateDirectoryStructureIfMissing;

            return result;
        }

        /// <summary>
        /// Determines whether two objects of type <see cref="FileLogConfig"/> are not equal.
        /// </summary>
        /// <param name="left">The object to the left of the operator.</param>
        /// <param name="right">The object to the right of the operator.</param>
        /// <returns>True if the two items not equal; false otherwise.</returns>
        public static bool operator !=(
            FileLogConfig left,
            FileLogConfig right)
            => !(left == right);

        /// <inheritdoc />
        public bool Equals(FileLogConfig other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as FileLogConfig);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize(this.GetBaseHashCode())
                                                           .Hash(this.LogFilePath)
                                                           .Hash(this.CreateDirectoryStructureIfMissing)
                                                           .Value;
    }
}