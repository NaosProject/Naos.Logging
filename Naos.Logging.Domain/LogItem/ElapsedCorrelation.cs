// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElapsedCorrelation.cs" company="Naos Project">
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
    /// <see cref="LogItem"/>s within a defined block of code that explicitly
    /// relates those items.
    /// </summary>
    public class ElapsedCorrelation : IHaveCorrelationId, IEquatable<ElapsedCorrelation>
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

        /// <inheritdoc />
        public override string ToString()
        {
            return Invariant($"{nameof(this.CorrelationId)}:{this.CorrelationId} - {nameof(this.ElapsedTime)}={this.ElapsedTime}");
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(ElapsedCorrelation first, ElapsedCorrelation second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.CorrelationId == second.CorrelationId &&
                   first.ElapsedTime == second.ElapsedTime;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(ElapsedCorrelation first, ElapsedCorrelation second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(ElapsedCorrelation other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as ElapsedCorrelation);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.CorrelationId)
            .Hash(this.ElapsedTime)
            .Value;
    }
}