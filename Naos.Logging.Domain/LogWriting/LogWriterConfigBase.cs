// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWriterConfigBase.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using OBeautifulCode.Math.Recipes;
    using OBeautifulCode.Validation.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Base class for all log writer configuration.
    /// </summary>
    [Bindable(BindableSupport.Default)]
    public abstract class LogWriterConfigBase
    {
        private static readonly LogInclusionConfigComparer LogInclusionConfigComparer = new LogInclusionConfigComparer();

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
        /// Compare two inclusion map configs for equality.
        /// </summary>
        /// <param name="first">First to compare.</param>
        /// <param name="second">Second to compare.</param>
        /// <returns>A value indicating whether or not they are equal.</returns>
        protected static bool LogInclusionKindToOriginsMapsAreEqual(
            IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<LogItemOrigin>> first,
            IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<LogItemOrigin>> second)
        {
            var result = first.OrderBy(_ => _.Key).SequenceEqual(second.OrderBy(_ => _.Key), LogInclusionConfigComparer);
            return result;
        }
    }

    /// <summary>
    /// Comparer for use in comparing values from the log inclusion config.
    /// </summary>
    internal class LogInclusionConfigComparer : IEqualityComparer<KeyValuePair<LogItemKind, IReadOnlyCollection<LogItemOrigin>>>
    {
        /// <inheritdoc />
        public bool Equals(KeyValuePair<LogItemKind, IReadOnlyCollection<LogItemOrigin>> x, KeyValuePair<LogItemKind, IReadOnlyCollection<LogItemOrigin>> y)
        {
            // null implies all and empty collection implies none so nulls must be checked separately, cannot coalesce...
            var result = x.Key == y.Key &&
                         !(x.Value != null && y.Value == null) &&
                         !(x.Value == null && y.Value != null) &&
                         ((x.Value == null && y.Value == null) || x.Value.OrderBy(_ => _).SequenceEqual(y.Value.OrderBy(_ => _)));

            return result;
        }

        /// <inheritdoc />
        public int GetHashCode(KeyValuePair<LogItemKind, IReadOnlyCollection<LogItemOrigin>> obj)
        {
            var result =
                HashCodeHelper.Initialize()
                   .Hash(obj.Key)
                   .HashElements(obj.Value == null ? obj.Value : obj.Value.OrderBy(_ => _).ToList())
                   .Value;

            return result;
        }
    }
}