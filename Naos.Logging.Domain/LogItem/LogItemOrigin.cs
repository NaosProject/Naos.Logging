// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItemOrigin.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using Its.Log.Instrumentation;

    /// <summary>
    /// The origin of a logged item.
    /// </summary>
    public enum LogItemOrigin
    {
        /// <summary>
        /// Unknown (default)
        /// </summary>
        Unknown,

        /// <summary>
        /// Messages generated internally by <see cref="LogWriting" />.
        /// </summary>
        NaosLoggingLogWriting,

        /// <summary>
        /// Messages from the <see cref="AppDomain" /> event <see cref="AppDomain.UnhandledException" />.
        /// </summary>
        AppDomainUnhandledException,

        /// <summary>
        /// Messages from the <see cref="Its.Log" /> event <see cref="Log.InternalErrors" />.
        /// </summary>
        ItsLogInternalErrors,

        /// <summary>
        /// Messages from the <see cref="Its.Log" /> event <see cref="Log.EntryPosted" /> that have a subject that is some information.
        /// </summary>
        ItsLogEntryPosted,

        /// <summary>
        /// Messages from Hangfire (a 3rd party open source message platform).
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Hangfire", Justification = "Spelling/name is correct.")]
        Hangfire,
    }
}