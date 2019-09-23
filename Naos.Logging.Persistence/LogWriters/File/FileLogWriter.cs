// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLogWriter.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Persistence
{
    using System;
    using System.IO;
    using Naos.Logging.Domain;
    using OBeautifulCode.Representation;
    using static System.FormattableString;

    /// <summary>
    /// <see cref="File"/> focused implementation of <see cref="LogWriterBase" />.
    /// </summary>
    public class FileLogWriter : LogWriterBase
    {
        private readonly FileLogConfig fileLogConfig;

        private readonly string thisToString;

        private readonly object fileLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogWriter"/> class.
        /// </summary>
        /// <param name="fileLogConfig">Configuration.</param>
        public FileLogWriter(
            FileLogConfig fileLogConfig)
            : base(fileLogConfig)
        {
            this.fileLogConfig = fileLogConfig ?? throw new ArgumentNullException(nameof(fileLogConfig));

            var directoryPath = Path.GetDirectoryName(this.fileLogConfig.LogFilePath);
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new ArgumentException(Invariant($"directory name from {nameof(this.fileLogConfig)}.{nameof(FileLogConfig.LogFilePath)} is null or white space"));
            }

            bool didCreateDirectory;
            if (this.fileLogConfig.CreateDirectoryStructureIfMissing && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                didCreateDirectory = true;
            }
            else
            {
                didCreateDirectory = false;
            }

            // this is to capture the directory creation info as well as prevent inconsistent syncronization usage of this.fileLogConfig...
            this.thisToString = Invariant($"{this.GetType().ToStringReadable()}; {nameof(this.fileLogConfig.LogInclusionKindToOriginsMap)}: {this.fileLogConfig.LogInclusionKindToOriginsMapFriendlyString}; {nameof(this.fileLogConfig.LogFilePath)}: {this.fileLogConfig.LogFilePath}; {nameof(this.fileLogConfig.CreateDirectoryStructureIfMissing)}: {this.fileLogConfig.CreateDirectoryStructureIfMissing}; {nameof(didCreateDirectory)}: {didCreateDirectory}");
        }

        /// <inheritdoc />
        protected override void LogInternal(
            LogItem logItem)
        {
            if (logItem == null)
            {
                throw new ArgumentNullException(nameof(logItem));
            }

            lock (this.fileLock)
            {
                var logMessage = BuildLogMessageFromLogItem(logItem, this.fileLogConfig.LogItemPropertiesToIncludeInLogMessage, true);
                File.AppendAllText(this.fileLogConfig.LogFilePath, logMessage);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.thisToString;
        }
    }
}