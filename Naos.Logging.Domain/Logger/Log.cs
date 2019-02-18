// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Helper to deal with correlations of <see cref="Exception" />'s.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Gets the singleton logger being used.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Keeping for preferred IL.")]
        public static ILog Instance { get; private set; } = new Logger();

        /// <summary>
        /// Write a message.
        /// </summary>
        /// <param name="subjectFunc">Function to get the subject.</param>
        /// <param name="comment">Optional defaultCommentOverride.</param>
        /// <param name="origin">Optional origin override.</param>
        /// <param name="additionalCorrelations">Optional additional correlations.</param>
        public static void Write(Func<object> subjectFunc, string comment = null, string origin = null, IReadOnlyCollection<IHaveCorrelationId> additionalCorrelations = null)
        {
            Instance.Write(subjectFunc, comment, origin, additionalCorrelations);
        }

        /// <summary>
        /// With into a logged activity.
        /// </summary>
        /// <param name="correlatingSubjectFunc">Optional function to get the correlating subject.</param>
        /// <param name="defaultCommentOverride">Optional defaultCommentOverride.</param>
        /// <param name="defaultOriginOverride">Optional origin override.</param>
        /// <param name="correlationId">Optional correlation ID that will be used for each of the block correlations; DEFAULT is a different one for each.</param>
        /// <param name="additionalCorrelations">Optional additional correlations.</param>
        /// <returns>A configured <see cref="ICorrelatingActivityLogger" />.</returns>
        public static ILogDisposable With(Func<object> correlatingSubjectFunc = null, string defaultCommentOverride = null, string defaultOriginOverride = null, string correlationId = null, IReadOnlyCollection<IHaveCorrelationId> additionalCorrelations = null)
        {
            return Instance.With(correlatingSubjectFunc, defaultCommentOverride, defaultOriginOverride, correlationId, additionalCorrelations);
        }

        /// <summary>
        /// Set the <see cref="LogItemHandler" /> to send logged messages to.
        /// </summary>
        /// <param name="callback">Call back to send messages to.</param>
        public static void SetCallback(LogItemHandler callback)
        {
            Instance.SetCallback(callback);
        }
    }
}