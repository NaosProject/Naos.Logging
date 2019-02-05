// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILog.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    /// <summary>
    /// Interface for a client facing logger.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Write a message.
        /// </summary>
        /// <param name="subjectFunc">Function to get the subject.</param>
        /// <param name="comment">Optional comment.</param>
        /// <param name="originOverride">Optional origin override.</param>
        void Write(Func<object> subjectFunc, string comment = null, string originOverride = null);

        /// <summary>
        /// Write a message.
        /// </summary>
        /// <param name="subject">Subject object.</param>
        /// <param name="comment">Optional comment.</param>
        /// <param name="originOverride">Optional origin override.</param>
        void Write(string subject, string comment = null, string originOverride = null);

        /// <summary>
        /// Enter into a logged activity.
        /// </summary>
        /// <param name="correlatingSubjectFunc">Function to get the correlating subject.</param>
        /// <param name="comment">Optional comment.</param>
        /// <param name="originOverride">Optional origin override.</param>
        /// <returns>A configured <see cref="ICorrelatingActivityLogger" />.</returns>
        ICorrelatingActivityLogger Enter(Func<object> correlatingSubjectFunc, string comment = null, string originOverride = null);

        /// <summary>
        /// Set the <see cref="LoggerCallback" /> to send logged messages to.
        /// </summary>
        /// <param name="callback">Call back to send messages to.</param>
        void SetCallback(LoggerCallback callback);
    }
}