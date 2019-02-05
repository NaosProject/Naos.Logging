// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    /// <summary>
    /// Helper to deal with correlations of <see cref="Exception" />'s.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Gets the singleton logger being used.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Keeping for preferred IL.")]
        public static ILog Instance { get; private set; } = new Logger();

        /// <summary>
        /// Write a message.
        /// </summary>
        /// <param name="subjectFunc">Function to get the subject.</param>
        /// <param name="comment">Optional comment.</param>
        /// <param name="originOverride">Optional origin override.</param>
        public static void Write(Func<object> subjectFunc, string comment = null, string originOverride = null)
        {
            Instance.Write(subjectFunc, comment, originOverride);
        }

        /// <summary>
        /// Write a message.
        /// </summary>
        /// <param name="subject">Subject object.</param>
        /// <param name="comment">Optional comment.</param>
        /// <param name="originOverride">Optional origin override.</param>
        public static void Write(string subject, string comment = null, string originOverride = null)
        {
            Instance.Write(subject, comment, originOverride);
        }

        /// <summary>
        /// Enter into a logged activity.
        /// </summary>
        /// <param name="correlatingSubjectFunc">Function to get the correlating subject.</param>
        /// <param name="comment">Optional comment.</param>
        /// <param name="originOverride">Optional origin override.</param>
        /// <returns>A configured <see cref="ICorrelatingActivityLogger" />.</returns>
        public static ICorrelatingActivityLogger Enter(Func<object> correlatingSubjectFunc, string comment = null, string originOverride = null)
        {
            return Instance.Enter(correlatingSubjectFunc, comment, originOverride);
        }

        /// <summary>
        /// Set the <see cref="LoggerCallback" /> to send logged messages to.
        /// </summary>
        /// <param name="callback">Call back to send messages to.</param>
        public static void SetCallback(LoggerCallback callback)
        {
            Instance.SetCallback(callback);
        }
    }
}