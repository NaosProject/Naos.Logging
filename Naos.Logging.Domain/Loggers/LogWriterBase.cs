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

    using OBeautifulCode.Enum.Recipes;

    /// <summary>
    /// Base class for all log writers.
    /// </summary>
    public abstract class LogWriterBase
    {
        private readonly LogWriterConfigBase logWriterConfigBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogWriterBase"/> class.
        /// </summary>
        /// <param name="logWriterConfigBase">The base log writer configuration.</param>
        protected LogWriterBase(
            LogWriterConfigBase logWriterConfigBase)
        {
            if (logWriterConfigBase == null)
            {
                throw new ArgumentNullException(nameof(logWriterConfigBase));
            }

            this.logWriterConfigBase = logWriterConfigBase;
        }

        /// <summary>
        /// Logs a <see cref="LogItem" />.
        /// </summary>
        /// <param name="logItem">The item to log.</param>
        public void Log(
            LogItem logItem)
        {
            if (logItem == null)
            {
                throw new ArgumentNullException(nameof(logItem));
            }

            var origins = logItem.Context.Origin.ToOrigins();
            if ((this.logWriterConfigBase.OriginsToLog != LogItemOrigins.None) && this.logWriterConfigBase.OriginsToLog.HasFlagOverlap(origins))
            {
                this.LogInternal(logItem);
            }
        }

        /// <summary>
        /// Implementation-specific method for logging a <see cref="LogItem" />.
        /// </summary>
        /// <param name="logItem">The item to log.</param>
        protected abstract void LogInternal(
            LogItem logItem);

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