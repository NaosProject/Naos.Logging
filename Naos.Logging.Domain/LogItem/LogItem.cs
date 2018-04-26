// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItem.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    /// <summary>
    /// The atomic unit of logging, composed of a message and some context for that message.
    /// </summary>
    public class LogItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogItem"/> class.
        /// </summary>
        /// <param name="context">Context it was logged under.</param>
        /// <param name="message">The logged message.</param>
        public LogItem(
            LogItemContext context,
            string message)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.Context = context;
            this.Message = message;
        }

        /// <summary>
        /// Gets the context for logged message.
        /// </summary>
        public LogItemContext Context { get; private set; }

        /// <summary>
        /// Gets the message logged.
        /// </summary>
        public string Message { get; private set; }
    }
}
