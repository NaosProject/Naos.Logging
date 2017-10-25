// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogContexts.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using Its.Log.Instrumentation;

    /// <summary>
    /// Enumeration of the various contexts that can be logged from.
    /// </summary>
    [Flags]
    public enum LogContexts
    {
        /// <summary>
        /// Log none.
        /// </summary>
        None = 1,

        /// <summary>
        /// Messages from the <see cref="AppDomain" /> event <see cref="AppDomain.UnhandledException" />.
        /// </summary>
        AppDomainUnhandledException = 2,

        /// <summary>
        /// Messages from the <see cref="Its.Log" /> event <see cref="Log.InternalErrors" />.
        /// </summary>
        ItsLogInternalErrors = 4,

        /// <summary>
        /// Messages from the <see cref="Its.Log" /> event <see cref="Log.EntryPosted" /> that have a subject that is NOT an <see cref="Exception" />.
        /// </summary>
        EntryPostedInformation = 8,

        /// <summary>
        /// Messages from the <see cref="Its.Log" /> event <see cref="Log.EntryPosted" /> that have a subject that IS an <see cref="Exception" />.
        /// </summary>
        EntryPostedException = 16,

        /// <summary>
        /// All entry posted events.
        /// </summary>
        EntryPosted = LogContexts.EntryPostedInformation | LogContexts.EntryPostedException,

        /// <summary>
        /// All unexpected errors.
        /// </summary>
        UnexpectedErrors = LogContexts.AppDomainUnhandledException | LogContexts.ItsLogInternalErrors,

        /// <summary>
        /// All errors.
        /// </summary>
        AllErrors = LogContexts.UnexpectedErrors | LogContexts.EntryPostedException,

        /// <summary>
        /// Log all.
        /// </summary>
        All = LogContexts.UnexpectedErrors | LogContexts.EntryPosted,
    }
}