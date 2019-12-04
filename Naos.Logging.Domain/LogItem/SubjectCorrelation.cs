// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubjectCorrelation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using OBeautifulCode.Equality.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Specifies how a single <see cref="LogItem"/> is correlated with other
    /// <see cref="LogItem"/>s with a shared common subject.
    /// </summary>
    public class SubjectCorrelation : IHaveCorrelationId, IEquatable<SubjectCorrelation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubjectCorrelation"/> class.
        /// </summary>
        /// <param name="correlationId">An identifier used to correlate multiple log-items.</param>
        /// <param name="subject">The correlating subject.</param>
        public SubjectCorrelation(
            string correlationId,
            Subject subject)
        {
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                throw new ArgumentException(Invariant($"{nameof(correlationId)} is null or white space"));
            }

            this.CorrelationId = correlationId;
            this.Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        }

        /// <inheritdoc />
        public string CorrelationId { get; private set; }

        /// <summary>
        /// Gets the correlating subject.
        /// </summary>
        public Subject Subject { get; private set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Invariant($"{nameof(this.CorrelationId)}:{this.CorrelationId} - {nameof(this.Subject)}.{nameof(this.Subject.Summary)}={this.Subject.Summary}");
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(SubjectCorrelation first, SubjectCorrelation second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return string.Equals(first.CorrelationId, second.CorrelationId, StringComparison.OrdinalIgnoreCase) && first.Subject == second.Subject;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(SubjectCorrelation first, SubjectCorrelation second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(SubjectCorrelation other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as SubjectCorrelation);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.CorrelationId).Hash(this.Subject).Value;
    }
}
