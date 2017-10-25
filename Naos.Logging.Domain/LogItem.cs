// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItem.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System.Linq;

    using Its.Log.Instrumentation;

    using Spritely.Recipes;

    /// <summary>
    /// Item to log.
    /// </summary>
    public class LogItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogItem"/> class.
        /// </summary>
        /// <param name="context">Context log item came from.</param>
        /// <param name="logEntry"><see cref="LogEntry" /> to process.</param>
        public LogItem(LogContexts context, LogEntry logEntry)
        {
            new { context }.Must().NotBeEqualTo(LogContexts.None).OrThrowFirstFailure();
            new { logEntry }.Must().NotBeNull().OrThrowFirstFailure();

            this.Context = context;
            this.LogEntry = logEntry;
        }

        /// <summary>
        /// Gets the context log item came from.
        /// </summary>
        public LogContexts Context { get; private set; }

        /// <summary>
        /// Gets the <see cref="LogEntry" /> to process.
        /// </summary>
        public LogEntry LogEntry { get; private set; }

        /// <summary>
        /// Builds a log message from the entry.
        /// </summary>
        /// <param name="includeSubjectAndParameters">Optional value indicating to include additional log strings of subject and parameters; DEFAULT is false.</param>
        /// <returns>Log message.</returns>
        public string BuildLogMessage(bool includeSubjectAndParameters = false)
        {
            if (includeSubjectAndParameters)
            {
                string logMessage = null;
                logMessage = this.LogEntry.Subject?.ToLogString() ?? "Null Subject Supplied to EntryPosted in " + nameof(LogProcessing);
                if (this.LogEntry.Params != null && this.LogEntry.Params.Any())
                {
                    foreach (var param in this.LogEntry.Params)
                    {
                        logMessage = logMessage + " - " + param.ToLogString();
                    }
                }

                return logMessage.ToLogString();
            }
            else
            {
                return this.LogEntry.ToLogString();
            }
        }
    }
}