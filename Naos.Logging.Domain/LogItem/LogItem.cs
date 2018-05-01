// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItem.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using static System.FormattableString;

    /// <summary>
    /// The atomic unit of logging, composed of a message and some context for that message.
    /// </summary>
    public class LogItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogItem"/> class.
        /// </summary>
        /// <param name="subject">The core piece of information being logged.</param>
        /// <param name="kind">The kind of log-item.</param>
        /// <param name="context">The context within which the item was logged.</param>
        /// <param name="comment">A comment about the logged item.</param>
        /// <param name="correlations">Information about how this log-item is correlated with other/related log-items.</param>
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
    }
}
