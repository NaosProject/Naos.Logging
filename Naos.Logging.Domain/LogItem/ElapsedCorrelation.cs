// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElapsedCorrelation.cs" company="Naos">
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
    public class ElapsedCorrelation : IHaveCorrelationId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElapsedCorrelation"/> class.
        /// </summary>
        /// <param name="correlationId">An identifier used to correlate multiple log-items.</param>
        /// <param name="elapsedTime">The elapsed time during the correlation.</param>
        public ElapsedCorrelation(
            string correlationId,
            TimeSpan elapsedTime)
        {
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                throw new ArgumentException(Invariant($"{nameof(correlationId)} is null or white space"));
            }

            this.CorrelationId = correlationId;
            this.ElapsedTime = elapsedTime;
        }

        /// <inheritdoc />
        public string CorrelationId { get; private set; }

        /// <summary>
        /// Gets the elapsed time in the correlation.
        /// </summary>
        public TimeSpan ElapsedTime { get; private set; }
    }
}