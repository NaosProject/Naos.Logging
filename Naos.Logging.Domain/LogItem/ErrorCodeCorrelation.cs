// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorCodeCorrelation.cs" company="Naos Project">
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
    public class ErrorCodeCorrelation : IHaveCorrelationId, IEquatable<ErrorCodeCorrelation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCodeCorrelation"/> class.
        /// </summary>
        /// <param name="correlationId">An identifier used to correlate multiple log-items.</param>
        /// <param name="errorCodeKey">Key used to get the error code.</param>
        /// <param name="errorCode">Error code.</param>
        public ErrorCodeCorrelation(string correlationId, string errorCodeKey, string errorCode)
        {
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                throw new ArgumentException(Invariant($"{nameof(correlationId)} is null or white space"));
            }

            if (string.IsNullOrWhiteSpace(errorCodeKey))
            {
                throw new ArgumentException(Invariant($"{nameof(errorCodeKey)} is null or white space"));
            }

            if (string.IsNullOrWhiteSpace(errorCode))
            {
                throw new ArgumentException(Invariant($"{nameof(errorCode)} is null or white space"));
            }

            this.CorrelationId = correlationId;
            this.ErrorCodeKey = errorCodeKey;
            this.ErrorCode = errorCode;
        }

        /// <inheritdoc />
        public string CorrelationId { get; private set; }

        /// <summary>
        /// Gets the error code prefix.
        /// </summary>
        public string ErrorCodeKey { get; private set; }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public string ErrorCode { get; private set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Invariant($"{nameof(this.CorrelationId)}:{this.CorrelationId} - ErrorCode {this.ErrorCodeKey}={this.ErrorCode}");
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(ErrorCodeCorrelation first, ErrorCodeCorrelation second)
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
                   first.ErrorCodeKey == second.ErrorCodeKey &&
                   first.ErrorCode == second.ErrorCode;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(ErrorCodeCorrelation first, ErrorCodeCorrelation second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(ErrorCodeCorrelation other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as ErrorCodeCorrelation);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.CorrelationId)
            .Hash(this.ErrorCodeKey)
            .Hash(this.ErrorCode)
            .Value;
    }
}