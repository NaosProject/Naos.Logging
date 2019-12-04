// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSlicedFilesLogConfig.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Naos.Logging.Domain;
    using OBeautifulCode.Equality.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// <see cref="File"/> focused implementation of <see cref="LogWriterConfigBase" />.
    /// </summary>
    public class TimeSlicedFilesLogConfig : LogWriterConfigBase, IEquatable<TimeSlicedFilesLogConfig>
    {
        private readonly IReadOnlyCollection<TimeSpan> sliceOffsets;

        /// <summary>
        /// File extension of files written without the leading period/dot.
        /// </summary>
        public const string FileExtensionWithoutDot = "log";

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSlicedFilesLogConfig"/> class.
        /// </summary>
        /// <param name="logInclusionKindToOriginsMap">The log-item origins to log for.</param>
        /// <param name="logFileDirectoryPath">Directory path to write log files to.</param>
        /// <param name="fileNamePrefix">File name to use for each log file as a prefix and a unique time slice hash will be used as the suffix.</param>
        /// <param name="timeSlicePerFile">Amount of time to store in each file.</param>
        /// <param name="createDirectoryStructureIfMissing">Optional value indicating whether to create the directory structure if it's missing; DEFAULT is true.</param>
        /// <param name="logItemPropertiesToIncludeInLogMessage"> The properties/aspects of a <see cref="LogItem"/> to include when building a log message.</param>
        public TimeSlicedFilesLogConfig(
            IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<string>> logInclusionKindToOriginsMap,
            string logFileDirectoryPath,
            string fileNamePrefix,
            TimeSpan timeSlicePerFile,
            bool createDirectoryStructureIfMissing = true,
            LogItemPropertiesToIncludeInLogMessage logItemPropertiesToIncludeInLogMessage = LogItemPropertiesToIncludeInLogMessage.Default)
            : base(logInclusionKindToOriginsMap, logItemPropertiesToIncludeInLogMessage)
        {
            if (string.IsNullOrWhiteSpace(logFileDirectoryPath))
            {
                throw new ArgumentException(Invariant($"{nameof(logFileDirectoryPath)} is null or white space"));
            }

            if (string.IsNullOrWhiteSpace(fileNamePrefix))
            {
                throw new ArgumentException(Invariant($"{nameof(fileNamePrefix)} is null or white space"));
            }

            if (timeSlicePerFile == default(TimeSpan))
            {
                throw new ArgumentException(Invariant($"{nameof(timeSlicePerFile)} is equal to the default {nameof(TimeSpan)}"));
            }

            this.LogFileDirectoryPath = logFileDirectoryPath;
            this.FileNamePrefix = fileNamePrefix;
            this.CreateDirectoryStructureIfMissing = createDirectoryStructureIfMissing;
            this.TimeSlicePerFile = timeSlicePerFile;

            this.sliceOffsets = timeSlicePerFile.SliceIntoOffsetsPerDay();
        }

        /// <summary>
        /// Gets the amount of time to store in each file.
        /// </summary>
        public TimeSpan TimeSlicePerFile { get; private set; }

        /// <summary>
        /// Gets the file name to use for each log file as a prefix and a unique time slice hash will be used as the suffix.
        /// </summary>
        public string FileNamePrefix { get; private set; }

        /// <summary>
        /// Gets the directory path to write log files to.
        /// </summary>
        public string LogFileDirectoryPath { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to create the directory structure if it's missing.
        /// </summary>
        public bool CreateDirectoryStructureIfMissing { get; private set; }

        /// <summary>
        /// Compute the file path to log to right now using <see cref="DateTime" />.<see cref="DateTime.UtcNow" />.
        /// </summary>
        /// <param name="nowUtc">Optionally override "now".</param>
        /// <returns>Correct file path to log to.</returns>
        public string ComputeFilePath(
            DateTime nowUtc = default(DateTime))
        {
            var now = nowUtc == default(DateTime) ? DateTime.UtcNow : nowUtc;
            var date = now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var offsets = this.sliceOffsets.FindOffsetRange(now);

            var file = Invariant($"{this.FileNamePrefix}--{date}--{offsets.Item1.ToString("hhmm", CultureInfo.InvariantCulture)}Z-{offsets.Item2.ToString("hhmm", CultureInfo.InvariantCulture)}Z.{FileExtensionWithoutDot}");
            var path = Path.Combine(this.LogFileDirectoryPath, file);
            return path;
        }

        /// <summary>
        /// Determines whether two objects of type <see cref="TimeSlicedFilesLogConfig"/> are equal.
        /// </summary>
        /// <param name="left">The object to the left of the operator.</param>
        /// <param name="right">The object to the right of the operator.</param>
        /// <returns>True if the two items are equal; false otherwise.</returns>
        public static bool operator ==(
            TimeSlicedFilesLogConfig left,
            TimeSlicedFilesLogConfig right)
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
                         left.TimeSlicePerFile == right.TimeSlicePerFile &&
                         string.Equals(left.FileNamePrefix, right.FileNamePrefix, StringComparison.Ordinal) &&
                         string.Equals(left.LogFileDirectoryPath, right.LogFileDirectoryPath, StringComparison.Ordinal) &&
                         left.CreateDirectoryStructureIfMissing == right.CreateDirectoryStructureIfMissing;

            return result;
        }

        /// <summary>
        /// Determines whether two objects of type <see cref="TimeSlicedFilesLogConfig"/> are not equal.
        /// </summary>
        /// <param name="left">The object to the left of the operator.</param>
        /// <param name="right">The object to the right of the operator.</param>
        /// <returns>True if the two items not equal; false otherwise.</returns>
        public static bool operator !=(
            TimeSlicedFilesLogConfig left,
            TimeSlicedFilesLogConfig right)
            => !(left == right);

        /// <inheritdoc />
        public bool Equals(TimeSlicedFilesLogConfig other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as TimeSlicedFilesLogConfig);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize(this.GetBaseHashCode())
                                                           .Hash(this.TimeSlicePerFile)
                                                           .Hash(this.FileNamePrefix)
                                                           .Hash(this.LogFileDirectoryPath)
                                                           .Hash(this.CreateDirectoryStructureIfMissing)
                                                           .Value;
    }
}
