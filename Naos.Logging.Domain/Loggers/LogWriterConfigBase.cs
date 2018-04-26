// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWriterConfigBase.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System.ComponentModel;

    using Its.Log.Instrumentation;

    /// <summary>
    /// Base class for all log writer configuration.
    /// </summary>
    [Bindable(BindableSupport.Default)]
    public abstract class LogWriterConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogWriterConfigBase"/> class.
        /// </summary>
        /// <param name="originsToLog">The log-item origins to log for.</param>
        /// <param name="logEntryPropertiesToIncludeInLogMessage">The properties/aspects of an <see cref="Its.Log"/> <see cref="LogEntry"/> to include when building a log message.</param>
        protected LogWriterConfigBase(
            LogItemOrigins originsToLog,
            LogEntryPropertiesToIncludeInLogMessage logEntryPropertiesToIncludeInLogMessage)
        {
            this.OriginsToLog = originsToLog;
            this.LogEntryPropertiesToIncludeInLogMessage = logEntryPropertiesToIncludeInLogMessage;
        }

        /// <summary>
        /// Gets the log-item origins to log for.
        /// </summary>
        public LogItemOrigins OriginsToLog { get; private set; }

        /// <summary>
        /// Gets the properties/aspects of an <see cref="Its.Log"/>
        /// <see cref="LogEntry"/> to include when building a log message.
        /// </summary>
        public LogEntryPropertiesToIncludeInLogMessage LogEntryPropertiesToIncludeInLogMessage { get; private set; }
    }
}