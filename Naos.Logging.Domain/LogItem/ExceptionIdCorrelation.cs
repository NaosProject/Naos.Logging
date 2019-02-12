// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionIdCorrelation.cs" company="Naos">
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
    public class ExceptionIdCorrelation : IHaveCorrelationId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionIdCorrelation"/> class.
        /// </summary>
        /// <param name="correlationId">An identifier used to correlate multiple log-items.</param>
        /// <param name="exceptionId">The exception ID.</param>
        public ExceptionIdCorrelation(string correlationId, string exceptionId)
        {
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                throw new ArgumentException(Invariant($"{nameof(correlationId)} is null or white space"));
            }

            if (string.IsNullOrWhiteSpace(exceptionId))
            {
                throw new ArgumentException(Invariant($"{nameof(exceptionId)} is null or white space"));
            }

            this.CorrelationId = correlationId;
            this.ExceptionId = exceptionId;
        }

        /// <inheritdoc />
        public string CorrelationId { get; private set; }

        /// <summary>
        /// Gets the exception ID.
        /// </summary>
        public string ExceptionId { get; private set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Invariant($"{nameof(this.CorrelationId)}:{this.CorrelationId} - {nameof(this.ExceptionId)}={this.ExceptionId}");
        }
    }
}