// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogConfig.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using OBeautifulCode.Equality.Recipes;

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
            IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<string>> logInclusionKindToOriginsMap,
            IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<string>> logInclusionKindToOriginsMapForConsoleOut,
            IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<string>> logInclusionKindToOriginsMapForConsoleError,
            LogItemPropertiesToIncludeInLogMessage logItemPropertiesToIncludeInLogMessage = LogItemPropertiesToIncludeInLogMessage.Default)
            : base(logInclusionKindToOriginsMap, logItemPropertiesToIncludeInLogMessage)
        {
            this.LogInclusionKindToOriginsMapForConsoleOut = logInclusionKindToOriginsMapForConsoleOut;
            this.LogInclusionKindToOriginsMapForConsoleError = logInclusionKindToOriginsMapForConsoleError;

            this.LogInclusionKindToOriginsMapForConsoleOutFriendlyString = GenerateFriendlyStringFromLogInclusionKindToOriginsMap(logInclusionKindToOriginsMapForConsoleOut);
            this.LogInclusionKindToOriginsMapForConsoleErrorFriendlyString = GenerateFriendlyStringFromLogInclusionKindToOriginsMap(logInclusionKindToOriginsMapForConsoleError);
        }

        /// <summary>
        /// Gets the log-item origins to log to <see cref="Console.Out" />.
        /// </summary>
        public IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<string>> LogInclusionKindToOriginsMapForConsoleOut { get; private set; }

        /// <summary>
        /// Gets a string friendly version of the <see cref="LogInclusionKindToOriginsMapForConsoleOut" />.
        /// </summary>
        public string LogInclusionKindToOriginsMapForConsoleOutFriendlyString { get; private set; }

        /// <summary>
        /// Gets the log-item origins to log to <see cref="Console.Error" />.
        /// </summary>
        public IReadOnlyDictionary<LogItemKind, IReadOnlyCollection<string>> LogInclusionKindToOriginsMapForConsoleError { get; private set; }

        /// <summary>
        /// Gets a string friendly version of the <see cref="LogInclusionKindToOriginsMapForConsoleError" />.
        /// </summary>
        public string LogInclusionKindToOriginsMapForConsoleErrorFriendlyString { get; private set; }

        /// <summary>
        /// Determines whether two objects of type <see cref="ConsoleLogConfig"/> are equal.
        /// </summary>
        /// <param name="left">The object to the left of the operator.</param>
        /// <param name="right">The object to the right of the operator.</param>
        /// <returns>True if the two items are equal; false otherwise.</returns>
        public static bool operator ==(
            ConsoleLogConfig left,
            ConsoleLogConfig right)
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
                         left.LogInclusionKindToOriginsMapForConsoleOut.IsEqualTo(right.LogInclusionKindToOriginsMapForConsoleOut) &&
                         left.LogInclusionKindToOriginsMapForConsoleError.IsEqualTo(right.LogInclusionKindToOriginsMapForConsoleError);

            return result;
        }

        /// <summary>
        /// Determines whether two objects of type <see cref="ConsoleLogConfig"/> are not equal.
        /// </summary>
        /// <param name="left">The object to the left of the operator.</param>
        /// <param name="right">The object to the right of the operator.</param>
        /// <returns>True if the two items not equal; false otherwise.</returns>
        public static bool operator !=(
            ConsoleLogConfig left,
            ConsoleLogConfig right)
            => !(left == right);

        /// <inheritdoc />
        public bool Equals(ConsoleLogConfig other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as ConsoleLogConfig);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize(this.GetBaseHashCode())
                                                           .Hash(this.LogInclusionKindToOriginsMapForConsoleOut)
                                                           .Hash(this.LogInclusionKindToOriginsMapForConsoleError)
                                                           .Value;

        /// <summary>
        /// Decide whether to include the item in <see cref="Console.Out" />.
        /// </summary>
        /// <param name="logItemKind">Kind of item.</param>
        /// <param name="logItemOrigin">Origin of item.</param>
        /// <returns>A value indicating whether to log.</returns>
        public bool ShouldLogConsole(LogItemKind logItemKind, string logItemOrigin)
        {
            return ShouldLog(this.LogInclusionKindToOriginsMapForConsoleOut, logItemKind, logItemOrigin);
        }

        /// <summary>
        /// Decide whether to include the item in <see cref="Console.Error" />.
        /// </summary>
        /// <param name="logItemKind">Kind of item.</param>
        /// <param name="logItemOrigin">Origin of item.</param>
        /// <returns>A value indicating whether to log.</returns>
        public bool ShouldLogError(LogItemKind logItemKind, string logItemOrigin)
        {
            return ShouldLog(this.LogInclusionKindToOriginsMapForConsoleError, logItemKind, logItemOrigin);
        }
    }
}
