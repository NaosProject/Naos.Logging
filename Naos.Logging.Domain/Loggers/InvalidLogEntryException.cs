// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidLogEntryException.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Runtime.Serialization;

    using Its.Log.Instrumentation;

    /// <summary>
    /// Indicates that a <see cref="LogEntry"/> was created in a way that's
    /// invalid for logging within this framework.
    /// </summary>
    [Serializable]
    public class InvalidLogEntryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidLogEntryException"/> class.
        /// </summary>
        public InvalidLogEntryException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidLogEntryException"/> class.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        public InvalidLogEntryException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidLogEntryException"/> class.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        /// <param name="innerException">Inner exception.</param>
        public InvalidLogEntryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidLogEntryException"/> class.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Reading context.</param>
        protected InvalidLogEntryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}