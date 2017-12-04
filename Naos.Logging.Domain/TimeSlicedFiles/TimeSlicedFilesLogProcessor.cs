// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSlicedFilesLogProcessor.cs" company="Naos">
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
    /// <see cref="File"/> focused implementation of <see cref="LogProcessorBase" />.
    /// </summary>
    public class TimeSlicedFilesLogProcessor : LogProcessorBase
    {
        private readonly TimeSlicedFilesLogConfiguration timeSlicedFilesConfiguration;

        private readonly bool didCreateDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSlicedFilesLogProcessor"/> class.
        /// </summary>
        /// <param name="timeSlicedFilesConfiguration">Configuration.</param>
        public TimeSlicedFilesLogProcessor(TimeSlicedFilesLogConfiguration timeSlicedFilesConfiguration)
            : base(timeSlicedFilesConfiguration)
        {
            new { fileConfiguration = timeSlicedFilesConfiguration }.Must().NotBeNull().OrThrowFirstFailure();

            this.timeSlicedFilesConfiguration = timeSlicedFilesConfiguration;

            if (this.timeSlicedFilesConfiguration.CreateDirectoryStructureIfMissing && !Directory.Exists(this.timeSlicedFilesConfiguration.LogFileDirectoryPath))
            {
                Directory.CreateDirectory(this.timeSlicedFilesConfiguration.LogFileDirectoryPath ?? "won't get here but VS can't figure that out");
                this.didCreateDirectory = true;
            }
            else
            {
                this.didCreateDirectory = false;
            }
        }

        /// <inheritdoc cref="LogProcessorBase" />
        public override void Log(LogMessage logMessage)
        {
            // if it is has the None flag then cut out.
            if (this.timeSlicedFilesConfiguration.ContextsToLog.HasFlag(LogContexts.None))
            {
                return;
            }

            new { logMessage }.Must().NotBeNull().OrThrowFirstFailure();

            var fileLock = new object();
            var message = FormattableString.Invariant($"TimeSliced|{logMessage.LoggedDateTimeUtc.ToString("o", CultureInfo.InvariantCulture)}|{logMessage.Context}|{logMessage.Message}");

            lock (fileLock)
            {
                var filePath = this.timeSlicedFilesConfiguration.ComputeFilePath();
                File.AppendAllText(filePath, message + Environment.NewLine);
            }
        }

        /// <inheritdoc cref="LogProcessorBase" />
        protected override void InternalLog(LogItem logItem)
        {
            // if it is has the None flag then cut out.
            if (this.timeSlicedFilesConfiguration.ContextsToLog.HasFlag(LogContexts.None))
            {
                return;
            }

            new { logItem }.Must().NotBeNull().OrThrowFirstFailure();

            var logMessage = new LogMessage(logItem.Context, logItem.BuildLogMessage(true), logItem.LoggedTimeUtc);

            this.Log(logMessage);
        }

        /// <inheritdoc cref="object" />
        public override string ToString()
        {
            var ret = FormattableString.Invariant($"{this.GetType().FullName}; {nameof(this.timeSlicedFilesConfiguration.ContextsToLog)}: {this.timeSlicedFilesConfiguration.ContextsToLog}; {nameof(this.timeSlicedFilesConfiguration.LogFileDirectoryPath)}: {this.timeSlicedFilesConfiguration.LogFileDirectoryPath}; {nameof(this.timeSlicedFilesConfiguration.FileNamePrefix)}: {this.timeSlicedFilesConfiguration.FileNamePrefix}; {nameof(this.timeSlicedFilesConfiguration.TimeSlicePerFile)}: {this.timeSlicedFilesConfiguration.TimeSlicePerFile}; {nameof(this.timeSlicedFilesConfiguration.CreateDirectoryStructureIfMissing)}: {this.timeSlicedFilesConfiguration.CreateDirectoryStructureIfMissing}; {nameof(this.didCreateDirectory)}: {this.didCreateDirectory}");
            return ret;
        }
    }
}