﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILog.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for a client facing logger.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Write a message.
        /// </summary>
        /// <param name="subjectFunc">Function to get the subject.</param>
        /// <param name="comment">Optional defaultCommentOverride.</param>
        /// <param name="origin">Optional origin override.</param>
        /// <param name="additionalCorrelations">Optional additional correlations.</param>
        void Write(Func<object> subjectFunc, string comment = null, string origin = null, IReadOnlyCollection<IHaveCorrelationId> additionalCorrelations = null);

        /// <summary>
        /// With into a logged activity.
        /// </summary>
        /// <param name="correlatingSubjectFunc">Optional function to get the correlating subject.</param>
        /// <param name="defaultCommentOverride">Optional defaultCommentOverride.</param>
        /// <param name="defaultOriginOverride">Optional origin override.</param>
        /// <param name="correlationId">Optional correlation ID that will be used for each of the block correlations; DEFAULT is a different one for each.</param>
        /// <param name="additionalCorrelations">Optional additional correlations.</param>
        /// <returns>A configured <see cref="ICorrelatingActivityLogger" />.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "With", Justification = "Spelling/name is correct.")]
        ILogDisposable With(Func<object> correlatingSubjectFunc = null, string defaultCommentOverride = null, string defaultOriginOverride = null, string correlationId = null, IReadOnlyCollection<IHaveCorrelationId> additionalCorrelations = null);

        /// <summary>
        /// Set the <see cref="LogItemHandler" /> to send logged messages to.
        /// </summary>
        /// <param name="callback">Call back to send messages to.</param>
        void SetCallback(LogItemHandler callback);
    }

    /// <summary>
    /// Interface for a logger that can be disposable to perform any wrap up tasks.
    /// </summary>
    public interface ILogDisposable : ILog, IDisposable
    {
    }
}
