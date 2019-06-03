// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWriterConfigBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Math.Recipes;
    using OBeautifulCode.Validation.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Base class for all log writer configuration.
    /// </summary>
    public abstract class LogWriterConfigBase : IEquatable<LogWriterConfigBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogWriterConfigBase"/> class.
        /// </summary>
        /// <param name="logInclusionKindToOriginsMap">The log items to log.</param>
        /// <param name="logItemPropertiesToIncludeInLogMessage">The properties/aspects of an <see cref="LogItem"/> to include when building a log message.</param>
        protected LogWriterConfigBase(
            IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<string>> logInclusionKindToOriginsMap,
            LogItemPropertiesToIncludeInLogMessage logItemPropertiesToIncludeInLogMessage)
        {
            new { logInclusionKindToOriginsMap }.Must().NotBeNull();
            this.LogInclusionKindToOriginsMap = logInclusionKindToOriginsMap;

            this.LogItemPropertiesToIncludeInLogMessage = logItemPropertiesToIncludeInLogMessage;
            this.LogInclusionKindToOriginsMapFriendlyString = GenerateFriendlyStringFromLogInclusionKindToOriginsMap(this.LogInclusionKindToOriginsMap);
        }

        /// <summary>
        /// Gets a map of <see cref="LogItemKind" /> to a list of origins (null list is all, empty list is none).
        /// </summary>
        public IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<string>> LogInclusionKindToOriginsMap { get; private set; }

        /// <summary>
        /// Gets the properties/aspects of an <see cref="LogItem"/> to include when building a log message.
        /// </summary>
        public LogItemPropertiesToIncludeInLogMessage LogItemPropertiesToIncludeInLogMessage { get; private set; }

        /// <summary>
        /// Generates a string friendly version of <see cref="LogInclusionKindToOriginsMap" />.
        /// </summary>
        /// <param name="logInclusionKindToOriginsMap">Config to generate from.</param>
        /// <returns>Friendly string representation.</returns>
        protected static string GenerateFriendlyStringFromLogInclusionKindToOriginsMap(
            IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<string>> logInclusionKindToOriginsMap)
        {
            var keyEqualsValueList = logInclusionKindToOriginsMap.Select(_ =>
            {
                if (_.Value == null)
                {
                    return Invariant($"{_.Key}=NULL");
                }
                else
                {
                    return Invariant($"{_.Key}={string.Join("|", _.Value)}");
                }
            }).ToList();
            var result = string.Join(",", keyEqualsValueList);
            return result;
        }

        /// <summary>
        /// Check to see if the config specifies logging under the specific scenario.
        /// </summary>
        /// <param name="logItemKind"><see cref="LogItemKind" /> to check.</param>
        /// <param name="logItemOrigin"><see cref="LogItemOrigin" /> to check.</param>
        /// <returns>A value indicating whether or not to log.</returns>
        public virtual bool ShouldLog(LogItemKind logItemKind, string logItemOrigin)
        {
            return ShouldLog(this.LogInclusionKindToOriginsMap, logItemKind, logItemOrigin);
        }

        /// <summary>
        /// Check to see if the config specifies logging under the specific scenario.
        /// </summary>
        /// <param name="logInclusionKindToOriginsMap">Map of <see cref="LogItemKind" /> to a list of origins (null list is all, empty list is none).</param>
        /// <param name="logItemKind"><see cref="LogItemKind" /> to check.</param>
        /// <param name="logItemOrigin"><see cref="LogItemOrigin" /> to check.</param>
        /// <returns>A value indicating whether or not to log.</returns>
        protected static bool ShouldLog(
            IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<string>> logInclusionKindToOriginsMap,
            LogItemKind logItemKind,
            string logItemOrigin)
        {
            new { logInclusionKindToOriginsMap }.Must().NotBeNull();

            var hasKey = logInclusionKindToOriginsMap.TryGetValue(logItemKind, out IReadOnlyCollection<string> origins);
            var result = !hasKey || (origins?.Contains(logItemOrigin) ?? true);
            return result;
        }

        /// <summary>
        /// Gets a string friendly version of the <see cref="LogInclusionKindToOriginsMap" />.
        /// </summary>
        public string LogInclusionKindToOriginsMapFriendlyString { get; private set; }

        /// <summary>
        /// Determines whether two objects of type <see cref="LogWriterConfigBase"/> are equal.
        /// </summary>
        /// <param name="left">The object to the left of the operator.</param>
        /// <param name="right">The object to the right of the operator.</param>
        /// <returns>True if the two items are equal; false otherwise.</returns>
        public static bool operator ==(
            LogWriterConfigBase left,
            LogWriterConfigBase right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            var result = left.Equals((object)right);

            return result;
        }

        /// <summary>
        /// Determines whether two objects of type <see cref="LogWriterConfigBase"/> are not equal.
        /// </summary>
        /// <param name="left">The object to the left of the operator.</param>
        /// <param name="right">The object to the right of the operator.</param>
        /// <returns>True if the two items not equal; false otherwise.</returns>
        public static bool operator !=(
            LogWriterConfigBase left,
            LogWriterConfigBase right)
            => !(left == right);

        /// <inheritdoc />
        public bool Equals(
            LogWriterConfigBase other)
            => this == other;
        /// <inheritdoc />

        public abstract override bool Equals(object obj);

        /// <inheritdoc />
        public abstract override int GetHashCode();

        /// <summary>
        /// Determines if this <see cref="LogWriterConfigBase "/> is equal to another based on properties on the base class.
        /// </summary>
        /// <param name="other">The other Form.</param>
        /// <returns>
        /// true if the two Forms are equal based on properties on the base class; otherwise false.
        /// </returns>
        protected virtual bool BaseEquals(
            LogWriterConfigBase other)
        {
            new { other }.Must().NotBeNull();

            var result = this.LogInclusionKindToOriginsMap.DictionaryEqualHavingEnumerableValues(other.LogInclusionKindToOriginsMap, enumerableEqualityComparerStrategy: EnumerableEqualityComparerStrategy.NoSymmetricDifference) &&
                         this.LogItemPropertiesToIncludeInLogMessage == other.LogItemPropertiesToIncludeInLogMessage;

            return result;
        }

        /// <summary>
        /// Gets a <see cref="HashCodeHelper"/> pre-computed by hashing the base class's properties.
        /// </summary>
        /// <returns>
        /// A <see cref="HashCodeHelper"/> pre-computed by hashing the base class's properties.
        /// </returns>
        protected virtual int GetBaseHashCode()
        {
            var result = HashCodeHelper.Initialize()
                .HashDictionaryHavingEnumerableValuesForSymmetricDifferenceValueEquality(this.LogInclusionKindToOriginsMap)
                .Hash(this.LogItemPropertiesToIncludeInLogMessage)
                .Value;

            return result;
        }
    }
}