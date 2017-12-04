// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryLogConfiguration.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using OBeautifulCode.Math.Recipes;

    using Spritely.Recipes;

    /// <summary>
    /// In memory implementation of <see cref="LogConfigurationBase" />.
    /// </summary>
    public class InMemoryLogConfiguration : LogConfigurationBase, IEquatable<InMemoryLogConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryLogConfiguration"/> class.
        /// </summary>
        /// <param name="contextsToLog">Contexts to log.</param>
        /// <param name="maxLoggedItemCount">Optional maximum number of elements to keep internally before removing the oldest items; DEFAULT is -1 which is infinite.</param>
        public InMemoryLogConfiguration(LogContexts contextsToLog, int maxLoggedItemCount = -1)
            : base(contextsToLog)
        {
            new { maxLoggedItemCount }.Must().BeGreaterThanOrEqualTo(-1).OrThrowFirstFailure();

            this.MaxLoggedItemCount = maxLoggedItemCount;
        }

        /// <summary>
        /// Gets the maximum number of elements to keep internally before removing the oldest items.
        /// </summary>
        public int MaxLoggedItemCount { get; private set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(InMemoryLogConfiguration first, InMemoryLogConfiguration second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.ContextsToLog == second.ContextsToLog && first.MaxLoggedItemCount == second.MaxLoggedItemCount;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(InMemoryLogConfiguration first, InMemoryLogConfiguration second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(InMemoryLogConfiguration other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as InMemoryLogConfiguration);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.ContextsToLog.ToString()).Hash(this.MaxLoggedItemCount).Value;
    }
}