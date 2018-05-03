// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLogWriter.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.IO;

    using static System.FormattableString;

    /// <summary>
    /// <see cref="File"/> focused implementation of <see cref="LogWriterBase" />.
    /// </summary>
    public class FileLogWriter : LogWriterBase
    {
        private readonly FileLogConfig fileLogConfig;

        private readonly string thisToString;

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
            this.thisToString = FormattableString.Invariant($"{this.GetType().FullName}; {nameof(this.fileLogConfig.OriginsToLog)}: {this.fileLogConfig.OriginsToLog}; {nameof(this.fileLogConfig.LogFilePath)}: {this.fileLogConfig.LogFilePath}; {nameof(this.fileLogConfig.CreateDirectoryStructureIfMissing)}: {this.fileLogConfig.CreateDirectoryStructureIfMissing}; {nameof(didCreateDirectory)}: {didCreateDirectory}");
        }

        /// <inheritdoc />
        protected override void LogInternal(
            LogItem logItem)
        {
            if (logItem == null)
            {
                throw new ArgumentNullException(nameof(logItem));
            }

            // TODO: Trace.Listeners.Add(new TextWriterTraceListener("Log_TextWriterOutput.log", "myListener"));
            var fileLock = new object();

            lock (fileLock)
            {
                var logMessage = BuildLogMessageFromLogEntry(logItem, this.fileLogConfig.LogItemPropertiesToIncludeInLogMessage, true);
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