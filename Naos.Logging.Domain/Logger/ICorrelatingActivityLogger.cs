// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICorrelatingActivityLogger.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
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
        void Write(Func<object> subjectFunc, string comment = null, string originOverride = null);
    }
}
