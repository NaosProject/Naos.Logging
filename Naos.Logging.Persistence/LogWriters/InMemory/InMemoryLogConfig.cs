// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryLogConfig.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Persistence
{
    using System;
    using System.Collections.Generic;
    using Naos.Logging.Domain;
    using OBeautifulCode.Math.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// In memory implementation of <see cref="LogWriterConfigBase" />.
    /// </summary>
    public class InMemoryLogConfig : LogWriterConfigBase, IEquatable<InMemoryLogConfig>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryLogConfig"/> class.
        /// </summary>
        /// <param name="logInclusionKindToOriginsMap">The log-item origins to log for.</param>
        /// <param name="maxLoggedItemCount">Optional maximum number of elements to keep internally before removing the oldest items; DEFAULT is -1 which is infinite.</param>
        /// <param name="logItemPropertiesToIncludeInLogMessage"> The properties/aspects of a <see cref="LogItem"/> to include when building a log message.</param>
        public InMemoryLogConfig(
            IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<string>> logInclusionKindToOriginsMap,
            int maxLoggedItemCount = -1,
            LogItemPropertiesToIncludeInLogMessage logItemPropertiesToIncludeInLogMessage = LogItemPropertiesToIncludeInLogMessage.Default)
            : base(logInclusionKindToOriginsMap, logItemPropertiesToIncludeInLogMessage)
        {
            if (maxLoggedItemCount < -1)
            {
                throw new ArgumentOutOfRangeException(Invariant($"{nameof(maxLoggedItemCount)} is <= -1; value is {maxLoggedItemCount}"));
            }

            this.MaxLoggedItemCount = maxLoggedItemCount;
        }

        /// <summary>
        /// Gets the maximum number of elements to keep internally before removing the oldest items.
        /// </summary>
        public int MaxLoggedItemCount { get; private set; }

        /// <summary>
        /// Determines whether two objects of type <see cref="InMemoryLogConfig"/> are equal.
        /// </summary>
        /// <param name="left">The object to the left of the operator.</param>
        /// <param name="right">The object to the right of the operator.</param>
        /// <returns>True if the two items are equal; false otherwise.</returns>
        public static bool operator ==(
            InMemoryLogConfig left,
            InMemoryLogConfig right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            var result = left.BaseEquals(right) &&
                         left.MaxLoggedItemCount == right.MaxLoggedItemCount;

            return result;
        }

        /// <summary>
        /// Determines whether two objects of type <see cref="InMemoryLogConfig"/> are not equal.
        /// </summary>
        /// <param name="left">The object to the left of the operator.</param>
        /// <param name="right">The object to the right of the operator.</param>
        /// <returns>True if the two items not equal; false otherwise.</returns>
        public static bool operator !=(
            InMemoryLogConfig left,
            InMemoryLogConfig right)
            => !(left == right);

        /// <inheritdoc />
        public bool Equals(InMemoryLogConfig other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as InMemoryLogConfig);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize(this.GetBaseHashCode())
                                                           .Hash(this.MaxLoggedItemCount)
                                                           .Value;
    }
}