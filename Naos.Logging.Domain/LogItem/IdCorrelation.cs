// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdCorrelation.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using static System.FormattableString;

    /// <summary>
    /// Specifies how a <see cref="LogItem"/> is correlated with other
    /// <see cref="LogItem"/>s with a shared <see cref="Exception"/> in
    /// the chain of <see cref="Exception.InnerException"/>.
    /// </summary>
    public class IdCorrelation : IHaveCorrelationId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdCorrelation"/> class.
        /// </summary>
        /// <param name="correlationId">An identifier used to correlate multiple log-items.</param>
        public IdCorrelation(string correlationId)
        {
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                throw new ArgumentException(Invariant($"{nameof(correlationId)} is null or white space"));
            }

            this.CorrelationId = correlationId;
        }

        /// <inheritdoc />
        public string CorrelationId { get; private set; }
    }
}