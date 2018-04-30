// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItemCorrelation.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using static System.FormattableString;

    /// <summary>
    /// Specifies how a <see cref="LogItem"/> is correlated with other/related <see cref="LogItem"/>s.
    /// </summary>
    public class LogItemCorrelation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogItemCorrelation"/> class.
        /// </summary>
        /// <param name="correlationId">An identifier used to correlate multiple log-items.</param>
        /// <param name="elapsedMillisecondsFromFirst">The number of milliseconds that have elapsed from the first log-item in the set of correlated log-items.</param>
        /// <param name="correlationPosition">The position of the log-item within a set of correlated log-items.</param>
        public LogItemCorrelation(
            string correlationId,
            long elapsedMillisecondsFromFirst,
            LogItemCorrelationPosition correlationPosition)
        {
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                throw new ArgumentException(Invariant($"{nameof(correlationId)} is null or white space"));
            }

            if (elapsedMillisecondsFromFirst < 0)
            {
                throw new ArgumentOutOfRangeException(Invariant($"{nameof(elapsedMillisecondsFromFirst)} < 0"));
            }

            if (correlationPosition == LogItemCorrelationPosition.Unknown)
            {
                throw new ArgumentException(Invariant($"{nameof(correlationPosition)} == {nameof(LogItemCorrelationPosition)}.{nameof(LogItemCorrelationPosition.Unknown)}"));
            }

            this.CorrelationId = correlationId;
            this.ElapsedMillisecondsFromFirst = elapsedMillisecondsFromFirst;
            this.CorrelationPosition = correlationPosition;
        }

        /// <summary>
        /// Gets an identifier used to correlate multiple log-items.
        /// </summary>
        public string CorrelationId { get; private set; }

        /// <summary>
        /// Gets the number of milliseconds that have elapsed from the first log-item in the timeseries of correlated log-items.
        /// </summary>
        public long ElapsedMillisecondsFromFirst { get; private set; }

        /// <summary>
        /// Gets the position of the log-item within a timeseries of correlated log-items.
        /// </summary>
        public LogItemCorrelationPosition CorrelationPosition { get; private set; }
    }
}