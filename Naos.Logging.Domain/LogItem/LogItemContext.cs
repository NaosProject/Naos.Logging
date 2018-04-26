// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItemContext.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using Spritely.Recipes;

    /// <summary>
    /// Stores some context for a <see cref="LogItem"/>
    /// (e.g. it's origin, name of process within which the log-item was generated).
    /// </summary>
    public class LogItemContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogItemContext"/> class.
        /// </summary>
        /// <param name="loggedTimeUtc">The date/time, in UTC, when the log-item was logged.</param>
        /// <param name="logItemOrigin">The origin of the log-item.</param>
        /// <param name="machineName">Optional name of the machine that generated the log-item.</param>
        /// <param name="processName">Optional name of the process that generated the log-item.</param>
        /// <param name="processFileVersion">Optional file version of the process that generated the log-item.</param>
        public LogItemContext(
            DateTime loggedTimeUtc,
            LogItemOrigin logItemOrigin,
            string machineName = null,
            string processName = null,
            string processFileVersion = null)
        {
            var loggedTimeUtcDateTimeKind = loggedTimeUtc.Kind;
            new { loggedTimeUtcDateTimeKind }.Must().BeEqualTo(DateTimeKind.Utc).OrThrowFirstFailure();
            new { logItemOrigin }.Must().NotBeEqualTo(LogItemOrigin.Unknown).OrThrowFirstFailure();

            this.LoggedTimeUtc = loggedTimeUtc;
            this.LogItemOrigin = logItemOrigin;
            this.MachineName = machineName;
            this.ProcessName = processName;
            this.ProcessFileVersion = processFileVersion;
        }

        /// <summary>
        /// Gets the date/time, in UTC, when the log-item was logged.
        /// </summary>
        public DateTime LoggedTimeUtc { get; private set; }

        /// <summary>
        /// Gets the origin of the log-item.
        /// </summary>
        public LogItemOrigin LogItemOrigin { get; private set; }

        /// <summary>
        /// Gets the name of the machine that generated the log-item.
        /// </summary>
        public string MachineName { get; private set; }

        /// <summary>
        /// Gets the name of the process that generated the log-item.
        /// </summary>
        public string ProcessName { get; private set; }

        /// <summary>
        /// Gets the file version of the process that generated the log-item
        /// </summary>
        public string ProcessFileVersion { get; private set; }
    }
}