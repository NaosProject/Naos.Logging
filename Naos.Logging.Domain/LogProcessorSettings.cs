// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogProcessorSettings.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using Its.Log.Instrumentation;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Settings to use when setup log processing.
    /// </summary>
    public class LogProcessorSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogProcessorSettings"/> class.
        /// </summary>
        /// <param name="configurations">Configurations to setup.</param>
        public LogProcessorSettings(IReadOnlyCollection<LogConfigurationBase> configurations)
        {
            this.Configurations = configurations ?? new List<LogConfigurationBase>();
        }

        /// <summary>
        /// Gets the configurations to use.
        /// </summary>
        public IReadOnlyCollection<LogConfigurationBase> Configurations { get; private set; }
    }

    /// <summary>
    /// Base class for all log configuration.
    /// </summary>
    [Bindable(BindableSupport.Default)]
    public abstract class LogConfigurationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogConfigurationBase"/> class.
        /// </summary>
        /// <param name="contextsToLog">Contexts to log.</param>
        protected LogConfigurationBase(LogContexts contextsToLog)
        {
            this.ContextsToLog = contextsToLog;
        }

        /// <summary>
        /// Gets the contexts to log from.
        /// </summary>
        public LogContexts ContextsToLog { get; private set; }
    }

    /// <summary>
    /// In memory implementation of <see cref="LogConfigurationBase" />.
    /// </summary>
    public class LogConfigurationMemory : LogConfigurationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogConfigurationMemory"/> class.
        /// </summary>
        /// <param name="contextsToLog">Contexts to log.</param>
        public LogConfigurationMemory(LogContexts contextsToLog)
            : base(contextsToLog)
        {
        }
    }

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