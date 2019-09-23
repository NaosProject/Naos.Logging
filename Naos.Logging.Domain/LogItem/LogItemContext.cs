// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItemContext.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using OBeautifulCode.Math.Recipes;
    using OBeautifulCode.Representation;

    using static System.FormattableString;

    /// <summary>
    /// Stores some context for a <see cref="LogItem"/>
    /// (e.g. it's origin, time logged, the name of process within which the log-item was generated).
    /// </summary>
    public class LogItemContext : IEquatable<LogItemContext>
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
            string origin,
            string machineName = null,
            string processName = null,
            string processFileVersion = null,
            string callingMethod = null,
            TypeRepresentation callingType = null,
            string stackTrace = null)
        {
            if (timestampUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(Invariant($"{nameof(timestampUtc)}.{nameof(DateTime.Kind)} != {nameof(DateTimeKind)}.{nameof(DateTimeKind.Utc)}"));
            }

            if (string.IsNullOrWhiteSpace(origin))
            {
                throw new ArgumentException(Invariant($"{nameof(origin)} cannot be null or whitespace."));
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
        public string Origin { get; private set; }

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
        public TypeRepresentation CallingType { get; private set; }

        /// <summary>
        /// Gets the stack trace.
        /// </summary>
        public string StackTrace { get; private set; }

        /// <summary>
        /// Clones this context, replacing origin with the specified origin.
        /// </summary>
        /// <param name="origin">The origin to use in the cloned context.</param>
        /// <returns>
        /// A clone of this context, with the specified origin replacing the origin.
        /// </returns>
        public LogItemContext CloneWithOrigin(
            string origin)
        {
            if (string.IsNullOrWhiteSpace(origin))
            {
                throw new ArgumentException(Invariant($"Cannot have a null or whitespace {nameof(origin)} - was: {origin}."));
            }

            var result = new LogItemContext(this.TimestampUtc, origin, this.MachineName, this.ProcessName, this.ProcessFileVersion, this.CallingMethod, this.CallingType, this.StackTrace);
            return result;
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(LogItemContext first, LogItemContext second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.TimestampUtc == second.TimestampUtc &&
                   string.Equals(first.Origin, second.Origin, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(first.MachineName, second.MachineName, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(first.ProcessName, second.ProcessName, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(first.ProcessFileVersion, second.ProcessFileVersion, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(first.CallingMethod, second.CallingMethod, StringComparison.OrdinalIgnoreCase) &&
                   first.CallingType == second.CallingType &&
                   string.Equals(first.StackTrace, second.StackTrace, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are unequal.</returns>
        public static bool operator !=(LogItemContext first, LogItemContext second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(LogItemContext other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as LogItemContext);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.TimestampUtc)
            .Hash(this.Origin)
            .Hash(this.MachineName)
            .Hash(this.ProcessName)
            .Hash(this.ProcessFileVersion)
            .Hash(this.CallingMethod)
            .Hash(this.CallingType)
            .Hash(this.StackTrace)
            .Value;
    }
}