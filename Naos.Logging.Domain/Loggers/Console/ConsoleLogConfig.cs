// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogConfig.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OBeautifulCode.Math.Recipes;

    /// <summary>
    /// <see cref="Console" /> focused implementation of <see cref="LogWriterConfigBase" />.
    /// </summary>
    public class ConsoleLogConfig : LogWriterConfigBase, IEquatable<ConsoleLogConfig>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogConfig"/> class.
        /// </summary>
        /// <param name="logInclusionKindToOriginsMap">The items to log to this logger.</param>
        /// <param name="logInclusionKindToOriginsMapForConsoleOut">The items to log to <see cref="Console.Out" />.</param>
        /// <param name="logInclusionKindToOriginsMapForConsoleError">The items to log to <see cref="Console.Error" />.</param>
        /// <param name="logItemPropertiesToIncludeInLogMessage"> The properties/aspects of a <see cref="LogItem"/> to include when building a log message.</param>
        public ConsoleLogConfig(
            IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<LogItemOrigin>> logInclusionKindToOriginsMap,
            IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<LogItemOrigin>> logInclusionKindToOriginsMapForConsoleOut,
            IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<LogItemOrigin>> logInclusionKindToOriginsMapForConsoleError,
            LogItemPropertiesToIncludeInLogMessage logItemPropertiesToIncludeInLogMessage = LogItemPropertiesToIncludeInLogMessage.Default)
            : base(logInclusionKindToOriginsMap, logItemPropertiesToIncludeInLogMessage)
        {
            this.LogInclusionKindToOriginsMapForConsoleOut = logInclusionKindToOriginsMapForConsoleOut;
            this.LogInclusionKindToOriginsMapForConsoleError = logInclusionKindToOriginsMapForConsoleError;

            this.LogInclusionKindToOriginsMapForConsoleOutFriendlyString = GenerateFriendlyStringFromLogInclusionKindToOriginsMap(logInclusionKindToOriginsMapForConsoleOut);
            this.LogInclusionKindToOriginsMapForConsoleErrorFriendlyString = GenerateFriendlyStringFromLogInclusionKindToOriginsMap(logInclusionKindToOriginsMapForConsoleError);
        }

        /// <summary>
        /// Gets the log-item origins to log to <see cref="Console.Out" />
        /// </summary>
        public IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<LogItemOrigin>> LogInclusionKindToOriginsMapForConsoleOut { get; private set; }

        /// <summary>
        /// Gets a string friendly version of the <see cref="LogInclusionKindToOriginsMapForConsoleOut" />.
        /// </summary>
        public string LogInclusionKindToOriginsMapForConsoleOutFriendlyString { get; private set; }

        /// <summary>
        /// Gets the log-item origins to log to <see cref="Console.Error" />
        /// </summary>
        public IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<LogItemOrigin>> LogInclusionKindToOriginsMapForConsoleError { get; private set; }

        /// <summary>
        /// Gets a string friendly version of the <see cref="LogInclusionKindToOriginsMapForConsoleError" />.
        /// </summary>
        public string LogInclusionKindToOriginsMapForConsoleErrorFriendlyString { get; private set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(
            ConsoleLogConfig first,
            ConsoleLogConfig second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            var result = (first.LogItemPropertiesToIncludeInLogMessage == second.LogItemPropertiesToIncludeInLogMessage) &&
                         (first.LogInclusionKindToOriginsMapFriendlyString == second.LogInclusionKindToOriginsMapFriendlyString) &&
                         (first.LogInclusionKindToOriginsMapForConsoleOutFriendlyString == second.LogInclusionKindToOriginsMapForConsoleOutFriendlyString) &&
                         (first.LogInclusionKindToOriginsMapForConsoleErrorFriendlyString == second.LogInclusionKindToOriginsMapForConsoleErrorFriendlyString);

            return result;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(
            ConsoleLogConfig first,
            ConsoleLogConfig second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(
            ConsoleLogConfig other) => this == other;

        /// <inheritdoc />
        public override bool Equals(
            object obj) => this == (obj as ConsoleLogConfig);

        /// <inheritdoc />
        public override int GetHashCode() =>
            HashCodeHelper
                .Initialize()
                .Hash(this.LogItemPropertiesToIncludeInLogMessage)
                .Hash(this.LogInclusionKindToOriginsMapFriendlyString)
                .Hash(this.LogInclusionKindToOriginsMapForConsoleOutFriendlyString)
                .Hash(this.LogInclusionKindToOriginsMapForConsoleErrorFriendlyString)
                .Value;

        /// <summary>
        /// Decide whether to include the item in <see cref="Console.Out" />.
        /// </summary>
        /// <param name="logItemKind">Kind of item.</param>
        /// <param name="logItemOrigin">Origin of item.</param>
        /// <returns>A value indicating whether to log.</returns>
        public bool ShouldLogConsole(LogItemKind logItemKind, LogItemOrigin logItemOrigin)
        {
            return ShouldLog(this.LogInclusionKindToOriginsMapForConsoleOut, logItemKind, logItemOrigin);
        }

        /// <summary>
        /// Decide whether to include the item in <see cref="Console.Error" />.
        /// </summary>
        /// <param name="logItemKind">Kind of item.</param>
        /// <param name="logItemOrigin">Origin of item.</param>
        /// <returns>A value indicating whether to log.</returns>
        public bool ShouldLogError(LogItemKind logItemKind, LogItemOrigin logItemOrigin)
        {
            return ShouldLog(this.LogInclusionKindToOriginsMapForConsoleError, logItemKind, logItemOrigin);
        }
    }
}