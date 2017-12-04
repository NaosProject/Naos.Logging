// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLogProcessor.cs" company="Naos">
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
    public class FileLogProcessor : LogProcessorBase
    {
        private readonly FileLogConfiguration fileConfiguration;

        private readonly bool didCreateDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogProcessor"/> class.
        /// </summary>
        /// <param name="fileConfiguration">Configuration.</param>
        public FileLogProcessor(FileLogConfiguration fileConfiguration)
            : base(fileConfiguration)
        {
            new { fileConfiguration }.Must().NotBeNull().OrThrowFirstFailure();

            this.fileConfiguration = fileConfiguration;

            var directoryPath = Path.GetDirectoryName(this.fileConfiguration.LogFilePath);
            directoryPath.Named(FormattableString.Invariant($"directoryFrom-{this.fileConfiguration.LogFilePath}-must-be-real-path")).Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
            if (this.fileConfiguration.CreateDirectoryStructureIfMissing && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath ?? "won't get here but VS can't figure that out");
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
            if (this.fileConfiguration.ContextsToLog.HasFlag(LogContexts.None))
            {
                return;
            }

            new { logMessage }.Must().NotBeNull().OrThrowFirstFailure();

            // TODO: Trace.Listeners.Add(new TextWriterTraceListener("Log_TextWriterOutput.log", "myListener"));
            var fileLock = new object();
            var message = FormattableString.Invariant($"{logMessage.LoggedDateTimeUtc.ToString("o", CultureInfo.InvariantCulture)}|{logMessage.Context}|{logMessage.Message}");

            lock (fileLock)
            {
                File.AppendAllText(this.fileConfiguration.LogFilePath, message + Environment.NewLine);
            }
        }

        /// <inheritdoc cref="LogProcessorBase" />
        protected override void InternalLog(LogItem logItem)
        {
            // if it is has the None flag then cut out.
            if (this.fileConfiguration.ContextsToLog.HasFlag(LogContexts.None))
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
            var ret = FormattableString.Invariant($"{this.GetType().FullName}; {nameof(this.fileConfiguration.ContextsToLog)}: {this.fileConfiguration.ContextsToLog}; {nameof(this.fileConfiguration.LogFilePath)}: {this.fileConfiguration.LogFilePath}; {nameof(this.fileConfiguration.CreateDirectoryStructureIfMissing)}: {this.fileConfiguration.CreateDirectoryStructureIfMissing}; {nameof(this.didCreateDirectory)}: {this.didCreateDirectory}");
            return ret;
        }
    }
}