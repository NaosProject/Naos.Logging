// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSlicedFilesLogWriter.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Globalization;
    using System.IO;

    using Spritely.Recipes;

    /// <summary>
    /// <see cref="File"/> focused implementation of <see cref="LogWriterBase" />.
    /// </summary>
    public class TimeSlicedFilesLogWriter : LogWriterBase
    {
        private readonly TimeSlicedFilesLogConfig timeSlicedFilesLogConfig;

        private readonly bool didCreateDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSlicedFilesLogWriter"/> class.
        /// </summary>
        /// <param name="timeSlicedFilesLogConfig">Configuration.</param>
        public TimeSlicedFilesLogWriter(TimeSlicedFilesLogConfig timeSlicedFilesLogConfig)
            : base(timeSlicedFilesLogConfig)
        {
            new { timeSlicedFilesLogConfig }.Must().NotBeNull().OrThrowFirstFailure();

            this.timeSlicedFilesLogConfig = timeSlicedFilesLogConfig;

            if (this.timeSlicedFilesLogConfig.CreateDirectoryStructureIfMissing && !Directory.Exists(this.timeSlicedFilesLogConfig.LogFileDirectoryPath))
            {
                Directory.CreateDirectory(this.timeSlicedFilesLogConfig.LogFileDirectoryPath ?? "won't get here but VS can't figure that out");
                this.didCreateDirectory = true;
            }
            else
            {
                this.didCreateDirectory = false;
            }
        }

        /// <inheritdoc />
        public override void LogInternal(
            LogItem logMessage)
        {
            var fileLock = new object();
            var message = FormattableString.Invariant($"TimeSliced|{logMessage.Context.LoggedTimeUtc.ToString("o", CultureInfo.InvariantCulture)}|{logMessage.Context}|{logMessage.Message}");

            lock (fileLock)
            {
                var filePath = this.timeSlicedFilesLogConfig.ComputeFilePath();
                File.AppendAllText(filePath, message + Environment.NewLine);
            }
        }

        /// <inheritdoc cref="object" />
        public override string ToString()
        {
            var ret = FormattableString.Invariant($"{this.GetType().FullName}; {nameof(this.timeSlicedFilesLogConfig.OriginsToLog)}: {this.timeSlicedFilesLogConfig.OriginsToLog}; {nameof(this.timeSlicedFilesLogConfig.LogFileDirectoryPath)}: {this.timeSlicedFilesLogConfig.LogFileDirectoryPath}; {nameof(this.timeSlicedFilesLogConfig.FileNamePrefix)}: {this.timeSlicedFilesLogConfig.FileNamePrefix}; {nameof(this.timeSlicedFilesLogConfig.TimeSlicePerFile)}: {this.timeSlicedFilesLogConfig.TimeSlicePerFile}; {nameof(this.timeSlicedFilesLogConfig.CreateDirectoryStructureIfMissing)}: {this.timeSlicedFilesLogConfig.CreateDirectoryStructureIfMissing}; {nameof(this.didCreateDirectory)}: {this.didCreateDirectory}");
            return ret;
        }
    }
}