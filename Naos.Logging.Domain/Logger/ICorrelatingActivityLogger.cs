// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICorrelatingActivityLogger.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    /// <summary>
    /// Interface for a logger associated with an activity.
    /// </summary>
    public interface ICorrelatingActivityLogger : IDisposable
    {
        /// <summary>
        /// Log a message to the activity.
        /// </summary>
        /// <param name="subjectFunc">Function to get the subject.</param>
        /// <param name="comment">Optional comment.</param>
        /// <param name="originOverride">Optional origin override; DEFAULT is the originally provided origin.</param>
        void Trace(Func<object> subjectFunc, string comment = null, string originOverride = null);

        /// <summary>
        /// Log a message to the activity.
        /// </summary>
        /// <param name="subject">Subject string.</param>
        /// <param name="comment">Optional comment.</param>
        /// <param name="originOverride">Optional origin override; DEFAULT is the originally provided origin.</param>
        void Trace(string subject, string comment = null, string originOverride = null);
    }
}