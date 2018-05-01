// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionCorrelation.cs" company="Naos">
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
    public class ExceptionCorrelation : IHaveCorrelatingSubject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionCorrelation"/> class.
        /// </summary>
        /// <param name="correlationId">An identifier used to correlate multiple log-items.</param>
        /// <param name="correlatingSubject">The correlating subject.</param>
        public ExceptionCorrelation(
            string correlationId,
            Subject correlatingSubject)
        {
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                throw new ArgumentException(Invariant($"{nameof(correlationId)} is null or white space"));
            }

            if (correlatingSubject == null)
            {
                throw new ArgumentNullException(nameof(correlatingSubject));
            }

            this.CorrelationId = correlationId;
            this.CorrelatingSubject = correlatingSubject;
        }

        /// <inheritdoc />
        public string CorrelationId { get; private set; }

        /// <inheritdoc />
        public Subject CorrelatingSubject { get; private set; }
    }
}