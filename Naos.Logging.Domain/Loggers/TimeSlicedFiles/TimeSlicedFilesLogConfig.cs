﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSlicedFilesLogConfig.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using OBeautifulCode.Math.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// <see cref="File"/> focused implementation of <see cref="LogWriterConfigBase" />.
    /// </summary>
    public class TimeSlicedFilesLogConfig : LogWriterConfigBase, IEquatable<TimeSlicedFilesLogConfig>
    {
        private readonly IReadOnlyCollection<TimeSpan> sliceOffsets;

        /// <summary>
        /// File extension of files written without the leading period/dot "."
        /// </summary>
        public const string FileExtensionWithoutDot = "log";

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSlicedFilesLogConfig"/> class.
        /// </summary>
        /// <param name="originsToLog">The log-item origins to log for.</param>
        /// <param name="logFileDirectoryPath">Directory path to write log files to.</param>
        /// <param name="fileNamePrefix">File name to use for each log file as a prefix and a unique time slice hash will be used as the suffix.</param>
        /// <param name="timeSlicePerFile">Amount of time to store in each file.</param>
        /// <param name="createDirectoryStructureIfMissing">Optional value indicating whether to create the directory structure if it's missing; DEFAULT is true.</param>
        /// <param name="logItemPropertiesToIncludeInLogMessage"> The properties/aspects of a <see cref="LogItem"/> to include when building a log message.</param>
        public TimeSlicedFilesLogConfig(
            LogItemOrigins originsToLog,
            string logFileDirectoryPath,
            string fileNamePrefix,
            TimeSpan timeSlicePerFile,
            bool createDirectoryStructureIfMissing = true,
            LogItemPropertiesToIncludeInLogMessage logItemPropertiesToIncludeInLogMessage = LogItemPropertiesToIncludeInLogMessage.Default)
            : base(originsToLog, logItemPropertiesToIncludeInLogMessage)
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
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(
            TimeSlicedFilesLogConfig first,
            TimeSlicedFilesLogConfig second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            var result =
                (first.OriginsToLog == second.OriginsToLog) &&
                (first.LogItemPropertiesToIncludeInLogMessage == second.LogItemPropertiesToIncludeInLogMessage) &&
                (first.LogFileDirectoryPath == second.LogFileDirectoryPath) &&
                (first.FileNamePrefix == second.FileNamePrefix) &&
                (first.TimeSlicePerFile == second.TimeSlicePerFile) &&
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
            TimeSlicedFilesLogConfig first,
            TimeSlicedFilesLogConfig second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(
            TimeSlicedFilesLogConfig other) => this == other;

        /// <inheritdoc />
        public override bool Equals(
            object obj) => this == (obj as TimeSlicedFilesLogConfig);

        /// <inheritdoc />
        public override int GetHashCode() =>
            HashCodeHelper
                .Initialize()
                .Hash(this.OriginsToLog)
                .Hash(this.LogItemPropertiesToIncludeInLogMessage)
                .Hash(this.LogFileDirectoryPath)
                .Hash(this.FileNamePrefix)
                .Hash(this.TimeSlicePerFile)
                .Hash(this.CreateDirectoryStructureIfMissing)
                .Value;

        /// <summary>
        /// Gets the offsets of the slices in the day.
        /// </summary>
        /// <returns>Offset collection.</returns>
        public IReadOnlyCollection<TimeSpan> GetSliceOffsets()
        {
            return this.sliceOffsets;
        }
    }
}