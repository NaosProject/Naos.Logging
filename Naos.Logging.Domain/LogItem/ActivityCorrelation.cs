// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityCorrelation.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using static System.FormattableString;

    /// <summary>
    /// Specifies how a single <see cref="LogItem"/> is correlated with other
    /// <see cref="LogItem"/>s within a defined block of code that explicitly
    /// relates those items.
    /// </summary>
    public class ActivityCorrelatio : IHaveCorrelationId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityCorrelation"/> class.
        /// </summary>
        /// <param name="correlationId">An identifier used to correlate multiple log-items.</param>
        /// <param name="correlatingSubject">The correlating subject.</param>
        /// <param name="elapsedMillisecondsFromFirst">The number of milliseconds that have elapsed from the first log-item in the timeseries of correlated log-items.</param>
        /// <param name="correlationPosition">The position of the log-item within a timeseries of correlated log-items.</param>
        public ActivityCorrelatio(
            string correlationId,
            Subject correlatingSubject,
            long elapsedMillisecondsFromFirst,
            ActivityCorrelationPosition correlationPosition)
        {
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                throw new ArgumentException(Invariant($"{nameof(correlationId)} is null or white space"));
            }

            if (correlatingSubject == null)
            {
                throw new ArgumentNullException(nameof(correlatingSubject));
            }

            if (elapsedMillisecondsFromFirst < 0)
            {
                throw new ArgumentOutOfRangeException(Invariant($"{nameof(elapsedMillisecondsFromFirst)} < 0"));
            }

            if (correlationPosition == ActivityCorrelationPosition.Unknown)
            {
                throw new ArgumentException(Invariant($"{nameof(correlationPosition)} == {nameof(ActivityCorrelationPosition)}.{nameof(ActivityCorrelationPosition.Unknown)}"));
            }

            this.CorrelationId = correlationId;
            this.CorrelatingSubject = correlatingSubject;
            this.ElapsedMillisecondsFromFirst = elapsedMillisecondsFromFirst;
            this.CorrelationPosition = correlationPosition;
        }

        /// <inheritdoc />
        public string CorrelationId { get; private set; }

        /// <inheritdoc />
        public Subject CorrelatingSubject { get; private set; }

        /// <summary>
        /// Gets the number of milliseconds that have elapsed from the first log-item in the timeseries of correlated log-items.
        /// </summary>
        public long ElapsedMillisecondsFromFirst { get; private set; }

        /// <summary>
        /// Gets the position of the log-item within a timeseries of correlated log-items.
        /// </summary>
        public ActivityCorrelationPosition CorrelationPosition { get; private set; }
    }
}