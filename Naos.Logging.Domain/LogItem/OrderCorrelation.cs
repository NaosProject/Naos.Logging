﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderCorrelation.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using static System.FormattableString;

    /// <summary>
    /// Specifies how a single <see cref="LogItem"/> is correlated with other
    /// <see cref="LogItem"/>s within a sequence of items.
    /// </summary>
    public class OrderCorrelation : IHaveCorrelationId
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
    }

    public enum ActivityCorrelationPosition
    {
        Unknown,
        Middle,
        First,
        Last
    }
}