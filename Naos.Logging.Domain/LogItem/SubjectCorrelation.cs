// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubjectCorrelation.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using static System.FormattableString;

    /// <summary>
    /// Specifies how a single <see cref="LogItem"/> is correlated with other
    /// <see cref="LogItem"/>s with a shared common subject.
    /// </summary>
    public class SubjectCorrelation : IHaveCorrelationId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubjectCorrelation"/> class.
        /// </summary>
        /// <param name="correlationId">An identifier used to correlate multiple log-items.</param>
        /// <param name="correlatingSubject">The correlating subject.</param>
        public SubjectCorrelation(
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

        /// <summary>
        /// Gets the correlating subject.
        /// </summary>
        public Subject CorrelatingSubject { get; private set; }
    }
}