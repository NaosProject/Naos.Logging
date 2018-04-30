// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLogWriter.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Globalization;
    using System.IO;

    using static System.FormattableString;

    /// <summary>
    /// <see cref="File"/> focused implementation of <see cref="LogWriterBase" />.
    /// </summary>
    public class FileLogWriter : LogWriterBase
    {
        private readonly FileLogConfig fileLogConfig;

        private readonly bool didCreateDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogWriter"/> class.
        /// </summary>
        /// <param name="fileLogConfig">Configuration.</param>
        public FileLogWriter(
            FileLogConfig fileLogConfig)
            : base(fileLogConfig)
        {
            if (fileLogConfig == null)
            {
                throw new ArgumentNullException(nameof(fileLogConfig));
            }

            this.fileLogConfig = fileLogConfig;

            var directoryPath = Path.GetDirectoryName(this.fileLogConfig.LogFilePath);
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new ArgumentException(Invariant($"directory name from {nameof(this.fileLogConfig)}.{nameof(FileLogConfig.LogFilePath)} is null or white space"));
            }

            if (this.fileLogConfig.CreateDirectoryStructureIfMissing && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath ?? "won't get here but VS can't figure that out");
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

            // TODO: Trace.Listeners.Add(new TextWriterTraceListener("Log_TextWriterOutput.log", "myListener"));
            var fileLock = new object();
            var message = FormattableString.Invariant($"{logItem.Context.TimestampUtc.ToString("o", CultureInfo.InvariantCulture)}|{logItem.Context}|{logItem.Message}");

            lock (fileLock)
            {
                File.AppendAllText(this.fileLogConfig.LogFilePath, message + Environment.NewLine);
            }
        }

        /// <inheritdoc cref="object" />
        public override string ToString()
        {
            var ret = FormattableString.Invariant($"{this.GetType().FullName}; {nameof(this.fileLogConfig.OriginsToLog)}: {this.fileLogConfig.OriginsToLog}; {nameof(this.fileLogConfig.LogFilePath)}: {this.fileLogConfig.LogFilePath}; {nameof(this.fileLogConfig.CreateDirectoryStructureIfMissing)}: {this.fileLogConfig.CreateDirectoryStructureIfMissing}; {nameof(this.didCreateDirectory)}: {this.didCreateDirectory}");
            return ret;
        }
    }
}