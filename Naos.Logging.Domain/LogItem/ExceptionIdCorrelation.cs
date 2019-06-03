// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionIdCorrelation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using OBeautifulCode.Math.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Specifies how a <see cref="LogItem"/> is correlated with other
    /// <see cref="LogItem"/>s with a shared <see cref="Exception"/> in
    /// the chain of <see cref="Exception.InnerException"/>.
    /// </summary>
    public class ExceptionIdCorrelation : IHaveCorrelationId, IEquatable<ExceptionIdCorrelation>
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

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(ExceptionIdCorrelation first, ExceptionIdCorrelation second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return string.Equals(first.CorrelationId, second.CorrelationId, StringComparison.OrdinalIgnoreCase) && string.Equals(first.ExceptionId, second.ExceptionId, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(ExceptionIdCorrelation first, ExceptionIdCorrelation second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(ExceptionIdCorrelation other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as ExceptionIdCorrelation);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.CorrelationId).Hash(this.ExceptionId).Value;
    }
}