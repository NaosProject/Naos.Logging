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
        /// Messages from the <see cref="AppDomain" /> event <see cref="AppDomain.UnhandledException" />.
        /// </summary>
        AppDomainUnhandledException,

        /// <summary>
        /// Messages from the <see cref="Its.Log" /> event <see cref="Log.InternalErrors" />.
        /// </summary>
        ItsLogInternalErrors,

        /// <summary>
        /// Messages from the <see cref="Its.Log" /> event <see cref="Log.EntryPosted" /> that have a subject that is NOT an <see cref="Exception" />.
        /// </summary>
        EntryPostedInformation,

        /// <summary>
        /// Messages from the <see cref="Its.Log" /> event <see cref="Log.EntryPosted" /> that have a subject that IS an <see cref="Exception" />.
        /// </summary>
        EntryPostedException,
    }

    /// <summary>
    /// The origins of a logged item that are supported by some log writer.
    /// </summary>
    [Flags]
    public enum LogItemOrigins
    {
        /// <summary>
        /// Log none.
        /// </summary>
        None = 0,

        /// <summary>
        /// Messages from the <see cref="AppDomain" /> event <see cref="AppDomain.UnhandledException" />.
        /// </summary>
        AppDomainUnhandledException = 1,

        /// <summary>
        /// Messages from the <see cref="Its.Log" /> event <see cref="Log.InternalErrors" />.
        /// </summary>
        ItsLogInternalErrors = 2,

        /// <summary>
        /// Messages from the <see cref="Its.Log" /> event <see cref="Log.EntryPosted" /> that have a subject that is NOT an <see cref="Exception" />.
        /// </summary>
        EntryPostedInformation = 4,

        /// <summary>
        /// Messages from the <see cref="Its.Log" /> event <see cref="Log.EntryPosted" /> that have a subject that IS an <see cref="Exception" />.
        /// </summary>
        EntryPostedException = 8,

        /// <summary>
        /// All entry posted events.
        /// </summary>
        EntryPosted = EntryPostedInformation | EntryPostedException,

        /// <summary>
        /// All unexpected errors.
        /// </summary>
        UnexpectedErrors = AppDomainUnhandledException | ItsLogInternalErrors,

        /// <summary>
        /// All errors.
        /// </summary>
        AllErrors = UnexpectedErrors | EntryPostedException,

        /// <summary>
        /// Log all.
        /// </summary>
        All = UnexpectedErrors | EntryPosted,
    }
}