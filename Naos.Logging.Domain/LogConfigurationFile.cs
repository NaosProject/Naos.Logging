// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogConfigurationFile.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using Its.Log.Instrumentation;

    using OBeautifulCode.Math.Recipes;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// File focused implementation of <see cref="LogConfigurationBase" />.
    /// </summary>
    public class LogConfigurationFile : LogConfigurationBase, IEquatable<LogConfigurationFile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogConfigurationFile"/> class.
        /// </summary>
        /// <param name="contextsToLog">Contexts to log.</param>
        /// <param name="logFilePath">File path to write logs to.</param>
        /// <param name="createDirectoryStructureIfMissing">Optional value indicating whether to create the directory structure if it's missing; DEFAULT is true.</param>
        public LogConfigurationFile(LogContexts contextsToLog, string logFilePath, bool createDirectoryStructureIfMissing = true)
            : base(contextsToLog)
        {
            new { logFilePath }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();

            this.LogFilePath = logFilePath;
            this.CreateDirectoryStructureIfMissing = createDirectoryStructureIfMissing;
        }

        /// <summary>
        /// Gets the file path to write logs to.
        /// </summary>
        public string LogFilePath { get; private set; }

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
        public static bool operator ==(LogConfigurationFile first, LogConfigurationFile second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.ContextsToLog == second.ContextsToLog && first.LogFilePath == second.LogFilePath && first.CreateDirectoryStructureIfMissing == second.CreateDirectoryStructureIfMissing;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(LogConfigurationFile first, LogConfigurationFile second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(LogConfigurationFile other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as LogConfigurationFile);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.ContextsToLog.ToString()).Hash(this.LogFilePath).Hash(this.CreateDirectoryStructureIfMissing).Value;
    }

    /// <summary>
    /// <see cref="EventLog"/> focused implementation of <see cref="LogProcessorFile" />.
    /// </summary>
    public class LogProcessorFile : LogProcessorBase
    {
        private readonly LogConfigurationFile fileConfiguration;

        private readonly bool didCreateDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogProcessorFile"/> class.
        /// </summary>
        /// <param name="fileConfiguration">Configuration.</param>
        public LogProcessorFile(LogConfigurationFile fileConfiguration)
            : base(fileConfiguration)
        {
            new { fileConfiguration }.Must().NotBeNull().OrThrowFirstFailure();

            this.fileConfiguration = fileConfiguration;

            var directoryPath = Path.GetDirectoryName(this.fileConfiguration.LogFilePath);
            directoryPath.Named(Invariant($"directoryFrom-{this.fileConfiguration.LogFilePath}-must-be-real-path")).Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
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
        protected override void InternalLog(LogEntry logEntry)
        {
            new { logEntry }.Must().NotBeNull().OrThrowFirstFailure();

            // TODO: Trace.Listeners.Add(new TextWriterTraceListener("Log_TextWriterOutput.log", "myListener"));
            var fileLock = new object();
            string logMessage = null;
            logMessage = logEntry.Subject?.ToLogString() ?? "Null Subject Supplied to EntryPosted in " + nameof(LogProcessing);
            if ((logEntry.Params != null) && logEntry.Params.Any())
            {
                foreach (var param in logEntry.Params)
                {
                    logMessage = logMessage + " - " + param.ToLogString();
                }
            }

            var message = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture) + ": " + logMessage.ToLogString();

            lock (fileLock)
            {
                File.AppendAllText(this.fileConfiguration.LogFilePath, message + Environment.NewLine);
            }
        }

        /// <inheritdoc cref="object" />
        public override string ToString()
        {
            var ret = Invariant($"{this.GetType().FullName}; {nameof(this.fileConfiguration.LogFilePath)}: {this.fileConfiguration.LogFilePath}, {nameof(this.fileConfiguration.CreateDirectoryStructureIfMissing)}: {this.fileConfiguration.CreateDirectoryStructureIfMissing}, {nameof(this.didCreateDirectory)}: {this.didCreateDirectory}");
            return ret;
        }
    }
}