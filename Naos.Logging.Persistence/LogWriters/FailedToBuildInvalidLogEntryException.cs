// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FailedToBuildInvalidLogEntryException.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Persistence
{
    using System;
    using System.Runtime.Serialization;

    using Its.Log.Instrumentation;

    /// <summary>
    /// Indicates that a <see cref="LogEntry"/> was created in a way that's
    /// invalid for logging within this framework.
    /// </summary>
    [Serializable]
    public class FailedToBuildInvalidLogEntryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToBuildInvalidLogEntryException"/> class.
        /// </summary>
        public FailedToBuildInvalidLogEntryException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToBuildInvalidLogEntryException"/> class.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        public FailedToBuildInvalidLogEntryException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToBuildInvalidLogEntryException"/> class.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        /// <param name="innerException">Inner exception.</param>
        public FailedToBuildInvalidLogEntryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToBuildInvalidLogEntryException"/> class.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Reading context.</param>
        protected FailedToBuildInvalidLogEntryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}