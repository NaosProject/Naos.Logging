// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItem.cs" company="Naos Project">
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
    using static System.FormattableString;

    /// <summary>
    /// The atomic unit of logging, composed of a message and some context for that message.
    /// </summary>
    public class LogItem : IEquatable<LogItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogItem"/> class.
        /// </summary>
        /// <param name="subject">The core piece of information being logged.</param>
        /// <param name="kind">The kind of log-item.</param>
        /// <param name="context">The context within which the item was logged.</param>
        /// <param name="comment">Optional comment about the logged item.</param>
        /// <param name="correlations">Optional information about how this log-item is correlated with other/related log-items.</param>
        public LogItem(
            Subject subject,
            LogItemKind kind,
            LogItemContext context,
            string comment = null,
            IReadOnlyCollection<IHaveCorrelationId> correlations = null)
        {
            this.Subject = subject ?? throw new ArgumentNullException(nameof(subject));

            if (kind == LogItemKind.Unknown)
            {
                throw new ArgumentException(Invariant($"{nameof(kind)} == {nameof(LogItemKind)}.{nameof(LogItemKind.Unknown)}"));
            }

            if ((correlations != null) && correlations.Any(_ => _ == null))
            {
                throw new ArgumentException(Invariant($"{nameof(correlations)} contains a null element"));
            }

            this.Kind = kind;
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.Comment = comment;
            this.Correlations = correlations ?? new IHaveCorrelationId[0];
        }

        /// <summary>
        /// Gets the core piece of information being logged.
        /// </summary>
        public Subject Subject { get; private set; }

        /// <summary>
        /// Gets the kind of log-item.
        /// </summary>
        public LogItemKind Kind { get; private set; }

        /// <summary>
        /// Gets the context within which the item was logged.
        /// </summary>
        public LogItemContext Context { get; private set; }

        /// <summary>
        /// Gets the comment associated with the logged item.
        /// </summary>
        public string Comment { get; private set; }

        /// <summary>
        /// Gets information about how this log-item is correlated with other/related log-items.
        /// </summary>
        public IReadOnlyCollection<IHaveCorrelationId> Correlations { get; private set; }

        /// <summary>
        /// Clones this item, replacing origin in the context with the specified origin.
        /// </summary>
        /// <param name="origin">The origin to use in the cloned item's context.</param>
        /// <returns>
        /// A clone of this item, with the specified origin replacing the origin in the context.
        /// </returns>
        public LogItem CloneWithOrigin(
            string origin)
        {
            if (string.IsNullOrWhiteSpace(origin))
            {
                throw new ArgumentException(Invariant($"Cannot have a null or whitespace {nameof(origin)} - was: {origin}."));
            }

            var result = new LogItem(this.Subject, this.Kind, this.Context.CloneWithOrigin(origin), this.Comment);
            return result;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Subject.Summary;
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(LogItem first, LogItem second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.Subject == second.Subject &&
                   first.Kind == second.Kind && 
                   first.Context == second.Context && 
                   string.Equals(first.Comment, second.Comment, StringComparison.OrdinalIgnoreCase) &&
                   first.Correlations.SequenceEqualHandlingNulls(second.Correlations);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(LogItem first, LogItem second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(LogItem other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as LogItem);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.Subject)
            .Hash(this.Kind)
            .Hash(this.Context)
            .Hash(this.Comment)
            .HashElements(this.Correlations)
            .Value;
    }
}
