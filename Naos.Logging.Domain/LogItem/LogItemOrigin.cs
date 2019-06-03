// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItemOrigin.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

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
        /// Messages from <see cref="Logger" />.
        /// </summary>
        NaosLoggingLogger,

        /// <summary>
        /// Messages generated internally by LogWriting in Persistence.
        /// </summary>
        NaosLoggingLogWriting,

        /// <summary>
        /// Messages from the <see cref="AppDomain" /> event <see cref="AppDomain.UnhandledException" />.
        /// </summary>
        AppDomainUnhandledException,

        /// <summary>
        /// Messages from the Its.Log.Instrumentation.Log.InternalErrors.
        /// </summary>
        ItsLogInternalErrors,

        /// <summary>
        /// Messages from the Its.Log.Instrumentation.Log.EntryPosted that have a subject that is some information.
        /// </summary>
        ItsLogEntryPosted,
    }
}