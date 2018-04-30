// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItemContext.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using OBeautifulCode.TypeRepresentation;

    using static System.FormattableString;

    /// <summary>
    /// Stores some context for a <see cref="LogItem"/>
    /// (e.g. it's origin, time logged, the name of process within which the log-item was generated).
    /// </summary>
    public class LogItemContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogItemContext"/> class.
        /// </summary>
        /// <param name="timestampUtc">The date/time, in UTC, when the log-item was logged.</param>
        /// <param name="origin">The origin of the log-item.</param>
        /// <param name="machineName">Optional name of the machine that generated the log-item.</param>
        /// <param name="processName">Optional name of the process that generated the log-item.</param>
        /// <param name="processFileVersion">Optional file version of the process that generated the log-item.</param>
        /// <param name="callingMethod">Optional calling method.</param>
        /// <param name="callingType">Optional description of the calling type.</param>
        /// <param name="stackTrace">Optional stack trace.</param>
        public LogItemContext(
            DateTime timestampUtc,
            LogItemOrigin origin,
            string machineName = null,
            string processName = null,
            string processFileVersion = null,
            string callingMethod = null,
            TypeDescription callingType = null,
            string stackTrace = null)
        {
            if (timestampUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(Invariant($"{nameof(timestampUtc)}.{nameof(DateTime.Kind)} != {nameof(DateTimeKind)}.{nameof(DateTimeKind.Utc)}"));
            }

            if (origin == LogItemOrigin.Unknown)
            {
                throw new ArgumentException(Invariant($"{nameof(origin)} == {nameof(Origin)}.{nameof(LogItemOrigin.Unknown)}"));
            }

            this.TimestampUtc = timestampUtc;
            this.Origin = origin;
            this.MachineName = machineName;
            this.ProcessName = processName;
            this.ProcessFileVersion = processFileVersion;
            this.CallingMethod = callingMethod;
            this.CallingType = callingType;
            this.StackTrace = stackTrace;
        }

        /// <summary>
        /// Gets the date/time, in UTC, when the log-item was logged.
        /// </summary>
        public DateTime TimestampUtc { get; private set; }

        /// <summary>
        /// Gets the origin of the log-item.
        /// </summary>
        public LogItemOrigin Origin { get; private set; }

        /// <summary>
        /// Gets the name of the machine that generated the log-item.
        /// </summary>
        public string MachineName { get; private set; }

        /// <summary>
        /// Gets the name of the process that generated the log-item.
        /// </summary>
        public string ProcessName { get; private set; }

        /// <summary>
        /// Gets the file version of the process that generated the log-item.
        /// </summary>
        public string ProcessFileVersion { get; private set; }

        /// <summary>
        /// Gets the calling method.
        /// </summary>
        public string CallingMethod { get; private set; }

        /// <summary>
        /// Gets a description of the calling type.
        /// </summary>
        public TypeDescription CallingType { get; private set; }

        /// <summary>
        /// Gets the stack trace.
        /// </summary>
        public string StackTrace { get; private set; }
    }
}