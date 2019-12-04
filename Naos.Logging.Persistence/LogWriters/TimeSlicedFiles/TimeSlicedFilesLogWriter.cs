// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSlicedFilesLogWriter.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Persistence
{
    using System;
    using System.IO;
    using Naos.Logging.Domain;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// <see cref="File"/> focused implementation of <see cref="LogWriterBase" />.
    /// </summary>
    public class TimeSlicedFilesLogWriter : LogWriterBase
    {
        private readonly TimeSlicedFilesLogConfig timeSlicedFilesLogConfig;

        private readonly bool didCreateDirectory;

        private readonly object fileLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSlicedFilesLogWriter"/> class.
        /// </summary>
        /// <param name="timeSlicedFilesLogConfig">Configuration.</param>
        public TimeSlicedFilesLogWriter(
            TimeSlicedFilesLogConfig timeSlicedFilesLogConfig)
            : base(timeSlicedFilesLogConfig)
        {
            this.timeSlicedFilesLogConfig = timeSlicedFilesLogConfig ?? throw new ArgumentNullException(nameof(timeSlicedFilesLogConfig));

            var directoryPath = this.timeSlicedFilesLogConfig.LogFileDirectoryPath;
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new ArgumentException(Invariant($"directory path from {nameof(this.timeSlicedFilesLogConfig)}.{nameof(TimeSlicedFilesLogConfig.LogFileDirectoryPath)} is null or white space"));
            }

            if (this.timeSlicedFilesLogConfig.CreateDirectoryStructureIfMissing && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                this.didCreateDirectory = true;
            }
            else
            {
                this.didCreateDirectory = false;
            }
        }

        /// <inheritdoc />
        protected override void LogInternal(
            LogItem logItem)
        {
            if (logItem == null)
            {
                throw new ArgumentNullException(nameof(logItem));
            }

            var logMessage = BuildLogMessageFromLogItem(logItem, this.timeSlicedFilesLogConfig.LogItemPropertiesToIncludeInLogMessage, true);

            lock (this.fileLock)
            {
                var filePath = this.timeSlicedFilesLogConfig.ComputeFilePath();
                File.AppendAllText(filePath, logMessage);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var ret = Invariant($"{this.GetType().ToStringReadable()}; {nameof(this.timeSlicedFilesLogConfig.LogInclusionKindToOriginsMap)}: {this.timeSlicedFilesLogConfig.LogInclusionKindToOriginsMapFriendlyString}; {nameof(this.timeSlicedFilesLogConfig.LogFileDirectoryPath)}: {this.timeSlicedFilesLogConfig.LogFileDirectoryPath}; {nameof(this.timeSlicedFilesLogConfig.FileNamePrefix)}: {this.timeSlicedFilesLogConfig.FileNamePrefix}; {nameof(this.timeSlicedFilesLogConfig.TimeSlicePerFile)}: {this.timeSlicedFilesLogConfig.TimeSlicePerFile}; {nameof(this.timeSlicedFilesLogConfig.CreateDirectoryStructureIfMissing)}: {this.timeSlicedFilesLogConfig.CreateDirectoryStructureIfMissing}; {nameof(this.didCreateDirectory)}: {this.didCreateDirectory}");
            return ret;
        }
    }
}
