// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdCorrelation.cs" company="Naos Project">
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

        /// <inheritdoc />
        public override string ToString()
        {
            return Invariant($"{nameof(this.CorrelationId)}:{this.CorrelationId}");
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(IdCorrelation first, IdCorrelation second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return string.Equals(first.CorrelationId, second.CorrelationId, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(IdCorrelation first, IdCorrelation second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(IdCorrelation other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as IdCorrelation);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.CorrelationId).Value;
    }
}