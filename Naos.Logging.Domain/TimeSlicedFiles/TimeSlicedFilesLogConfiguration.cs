// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSlicedFilesLogConfiguration.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using OBeautifulCode.Math.Recipes;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// <see cref="File"/> focused implementation of <see cref="LogConfigurationBase" />.
    /// </summary>
    public class TimeSlicedFilesLogConfiguration : LogConfigurationBase, IEquatable<TimeSlicedFilesLogConfiguration>
    {
        /// <summary>
        /// File extension of files written without the leading period/dot "."
        /// </summary>
        public const string FileExtensionWithoutDot = "log";

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSlicedFilesLogConfiguration"/> class.
        /// </summary>
        /// <param name="contextsToLog">Contexts to log.</param>
        /// <param name="logFileDirectoryPath">Directory path to write log files to.</param>
        /// <param name="fileNamePrefix">File name to use for each log file as a prefix and a unique time slice hash will be used as the suffix.</param>
        /// <param name="timeSlicePerFile">Amount of time to store in each file.</param>
        /// <param name="createDirectoryStructureIfMissing">Optional value indicating whether to create the directory structure if it's missing; DEFAULT is true.</param>
        public TimeSlicedFilesLogConfiguration(LogContexts contextsToLog, string logFileDirectoryPath, string fileNamePrefix, TimeSpan timeSlicePerFile, bool createDirectoryStructureIfMissing = true)
            : base(contextsToLog)
        {
            new { logFileDirectoryPath }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
            new { fileNamePrefix }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
            new { timeSlicePerFile }.Must().NotBeEqualTo(default(TimeSpan)).OrThrowFirstFailure();

            this.LogFileDirectoryPath = logFileDirectoryPath;
            this.FileNamePrefix = fileNamePrefix;
            this.CreateDirectoryStructureIfMissing = createDirectoryStructureIfMissing;
            this.TimeSlicePerFile = timeSlicePerFile;

            this.SliceOffsets = timeSlicePerFile.SliceIntoOffsetsPerDay();
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
        /// Gets the offsets of the day using the provided slice size.
        /// </summary>
        public IReadOnlyCollection<TimeSpan> SliceOffsets { get; private set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(TimeSlicedFilesLogConfiguration first, TimeSlicedFilesLogConfiguration second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.ContextsToLog == second.ContextsToLog && first.LogFileDirectoryPath == second.LogFileDirectoryPath && first.FileNamePrefix == second.FileNamePrefix && first.TimeSlicePerFile == second.TimeSlicePerFile && first.CreateDirectoryStructureIfMissing == second.CreateDirectoryStructureIfMissing;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(TimeSlicedFilesLogConfiguration first, TimeSlicedFilesLogConfiguration second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(TimeSlicedFilesLogConfiguration other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as TimeSlicedFilesLogConfiguration);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.ContextsToLog.ToString()).Hash(this.LogFileDirectoryPath).Hash(this.FileNamePrefix).Hash(this.TimeSlicePerFile).Hash(this.CreateDirectoryStructureIfMissing).Value;

        /// <summary>
        /// Compute the file path to log to right now using <see cref="DateTime" />.<see cref="DateTime.UtcNow" />.
        /// </summary>
        /// <param name="nowUtc">Optionally override "now".</param>
        /// <returns>Correct file path to log to.</returns>
        public string ComputeFilePath(DateTime nowUtc = default(DateTime))
        {
            var now = nowUtc == default(DateTime) ? DateTime.UtcNow : nowUtc;
            var date = now.ToString("yyyy-dd-MM", CultureInfo.InvariantCulture);
            var offsets = this.SliceOffsets.FindOffsetRange(now);

            var file = Invariant($"{this.FileNamePrefix}--{date}--{offsets.Item1.ToString("hhmm", CultureInfo.InvariantCulture)}Z-{offsets.Item2.ToString("hhmm", CultureInfo.InvariantCulture)}Z.{FileExtensionWithoutDot}");
            var path = Path.Combine(this.LogFileDirectoryPath, file);
            return path;
        }
    }
}