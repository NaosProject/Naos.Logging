// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWriterBase.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Linq;

    using Its.Log.Instrumentation;

    using Naos.Diagnostics.Domain;

    using OBeautifulCode.Enum.Recipes;

    using Spritely.Recipes;

    /// <summary>
    /// Base class for all log writers.
    /// </summary>
    public abstract class LogWriterBase
    {
        private readonly LogWriterConfigBase logWriterConfigBase;

        private readonly string machineName;

        private readonly string processName;

        private readonly string processFileVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogWriterBase"/> class.
        /// </summary>
        /// <param name="logWriterConfigBase">The base log writer configuration.</param>
        protected LogWriterBase(
            LogWriterConfigBase logWriterConfigBase)
        {
            new { logWriterConfigBase }.Must().NotBeNull().OrThrowFirstFailure();

            this.logWriterConfigBase = logWriterConfigBase;
            this.machineName = MachineName.GetMachineName();
            this.processName = ProcessHelpers.GetRunningProcess().Name();
            this.processFileVersion = ProcessHelpers.GetRunningProcess().FileVersion();
        }

        /// <summary>
        /// Logs a <see cref="LogItem" />.
        /// </summary>
        /// <param name="logItem">The item to log.</param>
        public void Log(
            LogItem logItem)
        {
            new { logItem }.Must().NotBeNull().OrThrowFirstFailure();

            var origins = logItem.Context.LogItemOrigin.ToOrigins();
            if ((this.logWriterConfigBase.OriginsToLog != LogItemOrigins.None) && this.logWriterConfigBase.OriginsToLog.HasFlagOverlap(origins))
            {
                this.LogInternal(logItem);
            }
        }

        /// <summary>
        /// Create an <see cref="Its.Log" /> <see cref="LogEntry"/>
        /// from a string and log it.
        /// </summary>
        /// <param name="logItemOrigin">The origin of the logged item.</param>
        /// <param name="message">Message to log.</param>
        public void Log(
            LogItemOrigin logItemOrigin,
            string message)
        {
            var entry = new LogEntry(message);
            this.Log(logItemOrigin, entry);
        }

        /// <summary>
        /// Create an <see cref="Its.Log" /> <see cref="LogEntry"/>
        /// from a comment and subject object and log it.
        /// </summary>
        /// <param name="logItemOrigin">The origin of the logged item.</param>
        /// <param name="comment">Comment to log.</param>
        /// <param name="subject">Subject to log.</param>
        public void Log(
            LogItemOrigin logItemOrigin,
            string comment,
            object subject)
        {
            var entry = new LogEntry(comment, subject);
            this.Log(logItemOrigin, entry);
        }

        /// <summary>
        /// Log a <see cref="LogEntry"/> from <see cref="Its.Log" />.
        /// </summary>
        /// <param name="logItemOrigin">The origin of the logged item.</param>
        /// <param name="logEntry"><see cref="Its.Log" /> entry to log.</param>
        public void Log(
            LogItemOrigin logItemOrigin,
            LogEntry logEntry)
        {
            logEntry = logEntry ?? new LogEntry(FormattableString.Invariant($"Null {nameof(LogEntry)} Supplied to {nameof(LogWriterBase)}.{nameof(this.Log)}"));

            var logItemContext = new LogItemContext(logEntry.TimeStamp, logItemOrigin, this.machineName, this.processName, this.processFileVersion);
            var logItem = this.BuildLogItemFromLogEntry(logEntry, logItemContext);

            this.Log(logItem);
        }

        /// <summary>
        /// Implementation-specific method for logging a <see cref="LogItem" />.
        /// </summary>
        /// <param name="logItem">The item to log.</param>
        protected abstract void LogInternal(
            LogItem logItem);

        /// <summary>
        /// Builds a <see cref="LogItem" /> from a <see cref="LogEntry"/>.
        /// </summary>
        /// <param name="logEntry"><see cref="Its.Log" /> entry to log.</param>
        /// <param name="logItemContext">Some context for the logged item.</param>
        /// <returns>
        /// The log-item that results from an <see cref="Its.Log"/>
        /// <see cref="LogEntry"/> and some context about the logged item.
        /// </returns>
        protected virtual LogItem BuildLogItemFromLogEntry(
            LogEntry logEntry,
            LogItemContext logItemContext)
        {
            var logMessage = BuildLogMessageFromLogEntry(logEntry, this.logWriterConfigBase.LogEntryPropertiesToIncludeInLogMessage);
            var result = new LogItem(logItemContext, logMessage);
            return result;
        }

        /// <summary>
        /// Builds a log message from a <see cref="LogItem" /> from a <see cref="LogEntry"/>.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <param name="logEntryPropertiesToIncludeInLogMessage"> The properties/aspects of an <see cref="Its.Log"/> <see cref="LogEntry"/> to include when building a log message.</param>
        /// <returns>Log message.</returns>
        protected static string BuildLogMessageFromLogEntry(
            LogEntry logEntry,
            LogEntryPropertiesToIncludeInLogMessage logEntryPropertiesToIncludeInLogMessage)
        {
            string result;
            if (logEntryPropertiesToIncludeInLogMessage == LogEntryPropertiesToIncludeInLogMessage.Default)
            {
                result = logEntry.ToLogString();
            }
            else
            {
                result = string.Empty;

                if (logEntryPropertiesToIncludeInLogMessage.HasFlag(LogEntryPropertiesToIncludeInLogMessage.Subject))
                {
                    result += logEntry.Subject?.ToLogString() ?? "Null Subject Supplied to EntryPosted in " + nameof(LogWriting);
                }

                if (logEntryPropertiesToIncludeInLogMessage.HasFlag(LogEntryPropertiesToIncludeInLogMessage.Parameters))
                {
                    if (logEntry.Params != null && logEntry.Params.Any())
                    {
                        foreach (var param in logEntry.Params)
                        {
                            result += " - " + param.ToLogString();
                        }
                    }
                }

                result = result.ToLogString();
            }

            return result;
        }
    }
}