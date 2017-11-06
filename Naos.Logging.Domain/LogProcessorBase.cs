// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogProcessorBase.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using Its.Log.Instrumentation;

    using OBeautifulCode.Enum.Recipes;

    using Spritely.Recipes;

    /// <summary>
    /// Base class for processors.
    /// </summary>
    public abstract class LogProcessorBase
    {
        private readonly LogConfigurationBase logConfigurationBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogProcessorBase"/> class.
        /// </summary>
        /// <param name="logConfigurationBase">Base configuration.</param>
        protected LogProcessorBase(LogConfigurationBase logConfigurationBase)
        {
            new { logConfigurationBase }.Must().NotBeNull().OrThrowFirstFailure();

            this.logConfigurationBase = logConfigurationBase;
        }

        /// <summary>
        /// Log a <see cref="LogEntry"/> from <see cref="Its.Log" />.
        /// </summary>
        /// <param name="context">Context it is coming from.</param>
        /// <param name="message">Message to log.</param>
        public void Log(LogContexts context, string message)
        {
            var entry = new LogEntry(message);
            this.Log(context, entry);
        }

        /// <summary>
        /// Log a <see cref="LogEntry"/> from <see cref="Its.Log" />.
        /// </summary>
        /// <param name="context">Context it is coming from.</param>
        /// <param name="comment">Comment to log.</param>
        /// <param name="subject">Subject to log.</param>
        public void Log(LogContexts context, string comment, object subject)
        {
            var entry = new LogEntry(comment, subject);
            this.Log(context, entry);
        }

        /// <summary>
        /// Log a <see cref="LogEntry"/> from <see cref="Its.Log" />.
        /// </summary>
        /// <param name="context">Context it is coming from.</param>
        /// <param name="logEntry">Entry to log.</param>
        public void Log(LogContexts context, LogEntry logEntry)
        {
            // if it is only None then cut out; can NOT do a HasFlag here, LogProcessorConsole for example can have the None flag but still need to be called directly...
            if (this.logConfigurationBase.ContextsToLog == LogContexts.None)
            {
                return;
            }

            if (this.logConfigurationBase.ContextsToLog.HasFlagOverlap(context))
            {
                var localLogEntry = logEntry ?? new LogEntry(FormattableString.Invariant($"Null {nameof(LogEntry)} Supplied to {nameof(LogProcessorBase)}.{nameof(this.Log)}"));

                var logItem = new LogItem(context, localLogEntry);
                this.InternalLog(logItem);
            }
        }

        /// <summary>
        /// Log a <see cref="LogItem" />.
        /// </summary>
        /// <param name="logItem">Item to log.</param>
        protected abstract void InternalLog(LogItem logItem);

        /// <summary>
        /// Log a <see cref="LogMessage" />.
        /// </summary>
        /// <param name="logMessage">Message to log.</param>
        public abstract void Log(LogMessage logMessage);
    }
}