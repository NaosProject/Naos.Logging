// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderCorrelation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using OBeautifulCode.Math.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Specifies how a single <see cref="LogItem"/> is correlated with other
    /// <see cref="LogItem"/>s within a sequence of items.
    /// </summary>
    public class OrderCorrelation : IHaveCorrelationId, IEquatable<OrderCorrelation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderCorrelation"/> class.
        /// </summary>
        /// <param name="correlationId">An identifier used to correlate multiple log-items.</param>
        /// <param name="position">Relative position in the order (range is int min to int max; default start is 0).</param>
        public OrderCorrelation(
            string correlationId,
            int position)
        {
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                throw new ArgumentException(Invariant($"{nameof(correlationId)} is null or white space"));
            }

            this.CorrelationId = correlationId;
            this.Position = position;
        }

        /// <inheritdoc />
        public string CorrelationId { get; private set; }

        /// <summary>
        /// Gets the position in a correlation.
        /// </summary>
        public int Position { get; private set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Invariant($"{nameof(this.CorrelationId)}:{this.CorrelationId} - {nameof(this.Position)}={this.Position}");
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(OrderCorrelation first, OrderCorrelation second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return string.Equals(first.CorrelationId, second.CorrelationId, StringComparison.OrdinalIgnoreCase) &&
                   first.Position == second.Position;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(OrderCorrelation first, OrderCorrelation second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(OrderCorrelation other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as OrderCorrelation);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.CorrelationId).Hash(this.Position).Value;
    }
}