// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItem.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using Naos.Serialization.Domain;

    using static System.FormattableString;

    /// <summary>
    /// The atomic unit of logging, composed of a message and some context for that message.
    /// </summary>
    public class LogItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogItem"/> class.
        /// </summary>
        /// <param name="subject">A described serializtion of the object that was logged.</param>
        /// <param name="kind">The kind of log-item.</param>
        /// <param name="message">A friendly message representation of the subject.</param>
        /// <param name="context">The context within which the item was logged.</param>
        /// <param name="correlation">Information about how this log-item is correlated with other/related log-items.</param>
        public LogItem(
            DescribedSerialization subject,
            LogItemKind kind,
            string message,
            LogItemContext context,
            LogItemCorrelation correlation = null)
        {
            if (subject == null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            if (kind == LogItemKind.Unknown)
            {
                throw new ArgumentException(Invariant($"{nameof(kind)} == {nameof(LogItemKind)}.{nameof(LogItemKind.Unknown)}"));
            }

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.Subject = subject;
            this.Kind = kind;
            this.Message = message;
            this.Context = context;
            this.Correlation = correlation;
        }

        /// <summary>
        /// Gets a described serializtion of the object that was logged.
        /// </summary>
        public DescribedSerialization Subject { get; private set; }

        /// <summary>
        /// Gets the kind of log-item.
        /// </summary>
        public LogItemKind Kind { get; private set; }

        /// <summary>
        /// Gets a friendly message representation of the subject.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the context within which the item was logged.
        /// </summary>
        public LogItemContext Context { get; private set; }

        /// <summary>
        /// Gets information about how this log-item is correlated with other/related log-items.
        /// </summary>
        public LogItemCorrelation Correlation { get; private set; }

        /// <summary>
        /// Clones this log-item, replacing the message with the specified message.
        /// </summary>
        /// <param name="message">The message to replace the existing message.</param>
        /// <returns>
        /// A clone of this log-item, with the original message replaced with the specified message.
        /// </returns>
        public LogItem CloneWithMessage(
            string message)
        {
            var result = new LogItem(this.Subject, this.Kind, message, this.Context, this.Correlation);
            return result;
        }

        /// <summary>
        /// Clones this log-item, replacing the context with the specified context.
        /// </summary>
        /// <param name="context">The context to replace the existing context.</param>
        /// <returns>
        /// A clone of this log-item, with the original context replaced with the specified context.
        /// </returns>
        public LogItem CloneWithContext(
            LogItemContext context)
        {
            var result = new LogItem(this.Subject, this.Kind, this.Message, context, this.Correlation);
            return result;
        }
    }
}
