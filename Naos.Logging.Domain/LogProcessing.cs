// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogProcessing.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;

    using Its.Log.Instrumentation;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Logging setup logic manager.
    /// </summary>
    public class LogProcessing
    {
        /// <summary>
        /// Gets the singleton entry point to the code.
        /// </summary>
        public static LogProcessing Instance { get; } = new LogProcessing();

        private readonly object sync = new object();

        private bool hasBeenSetup = false;

        private IReadOnlyCollection<LogProcessorBase> activeLogProcessors;

        private LogProcessing()
        {
            /* no-op to make sure this can only be accessed via instance property */
        }

        /// <summary>
        /// Entry point to configure logging.
        /// </summary>
        /// <param name="logProcessorSettings">Configuration for log processing.</param>
        /// <param name="announcer">Optional announcer to communicate setup state; DEFAULT is null.</param>
        public void Setup(LogProcessorSettings logProcessorSettings, Action<string> announcer = null)
        {
            var localAnnouncer = announcer ?? NullAnnouncement;

            lock (this.sync)
            {
                new { isSetup = this.hasBeenSetup }.Must().BeFalse().OrThrowFirstFailure();

                this.hasBeenSetup = true;

                new { logProcessorSettings }.Must().NotBeNull().OrThrowFirstFailure();

                var loggers = new List<LogProcessorBase>();
                foreach (var configuration in logProcessorSettings.Configurations)
                {
                    var fileConfiguration = configuration as LogConfigurationFile;
                    if (fileConfiguration != null)
                    {
                        var fileLogger = new LogProcessorFile(fileConfiguration);
                        new { fileLogger }.Must().NotBeNull().OrThrowFirstFailure();
                        loggers.Add(fileLogger);
                        localAnnouncer(Invariant($"Wired up {fileLogger.GetType().FullName} ; {fileLogger}."));
                    }

                    var eventLogConfiguration = configuration as LogConfigurationEventLog;
                    if (eventLogConfiguration != null)
                    {
                        var eventLogLogger = new LogProcessorEventLog(eventLogConfiguration);
                        new { eventLogLogger }.Must().NotBeNull().OrThrowFirstFailure();
                        loggers.Add(eventLogLogger);
                        localAnnouncer(Invariant($"Wired up {eventLogLogger.GetType().FullName} : {eventLogLogger}."));
                    }
                }

                this.activeLogProcessors = loggers;

                this.WireUpAppDomainUnhandledExceptionToToActiveLogProcessors(localAnnouncer);
                this.WireUpItsLogInternalErrorsToActiveLogProcessors(localAnnouncer);
                this.WireUpItsLogEntryPostedToActiveLogProcessors(localAnnouncer);
            }
        }

        private void LogOnActiveLogProcessors(LogContexts context, LogEntry logEntry)
        {
            foreach (var logProcessor in this.activeLogProcessors)
            {
                try
                {
                    logProcessor.Log(context, logEntry);
                }
                catch (Exception)
                {
                    /* no-op */
                }
            }
        }

        private void WireUpItsLogInternalErrorsToActiveLogProcessors(Action<string> announcer)
        {
            Log.InternalErrors += (sender, args) =>
                {
                    var logEntry = args.LogEntry ?? new LogEntry(Invariant($"Null {nameof(LogEntry)} Supplied to {nameof(Log)}.{nameof(Log.InternalErrors)}"));
                    this.LogOnActiveLogProcessors(LogContexts.ItsLogInternalErrors, logEntry);
                };

            announcer(Invariant($"Wired up {nameof(Log)}.{nameof(Log.InternalErrors)} to the {nameof(this.activeLogProcessors)}."));
        }

        private void WireUpItsLogEntryPostedToActiveLogProcessors(Action<string> announcer)
        {
            Log.EntryPosted += (sender, args) =>
                {
                    var logEntry = args.LogEntry ?? new LogEntry(Invariant($"Null {nameof(LogEntry)} Supplied to {nameof(Log)}.{nameof(Log.EntryPosted)}"));

                    var context = logEntry.Subject is Exception ? LogContexts.EntryPostedException : LogContexts.EntryPostedInformation;

                    this.LogOnActiveLogProcessors(context, logEntry);
                };

            announcer(Invariant($"Wired up {nameof(Log)}.{nameof(Log.EntryPosted)} to the {nameof(this.activeLogProcessors)}."));
        }

        private void WireUpAppDomainUnhandledExceptionToToActiveLogProcessors(Action<string> announcer)
        {
            AppDomain.CurrentDomain.UnhandledException += (o, args) =>
                {
                    var logEntry = new LogEntry(Invariant($"Unhandled exception encountered"), args.ExceptionObject);

                    this.LogOnActiveLogProcessors(LogContexts.AppDomainUnhandledException, logEntry);
                };

            announcer(Invariant($"Wired up {nameof(AppDomain)}.{nameof(AppDomain.UnhandledException)} to the {nameof(this.activeLogProcessors)}."));
        }

        private static void NullAnnouncement(string message)
        {
            /* no-op */
        }
    }

    /// <summary>
    /// Base class for processors.
    /// </summary>
    public abstract class LogProcessorBase
    {
        private readonly LogConfigurationBase logConfigurationBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogProcessorBase"/> class.
        /// </summary>
        /// <param name="logConfigurationBase">Base configuration.</param>
        protected LogProcessorBase(LogConfigurationBase logConfigurationBase)
        {
            new { logConfigurationBase }.Must().NotBeNull().OrThrowFirstFailure();

            this.logConfigurationBase = logConfigurationBase;
        }

        /// <summary>
        /// Log a <see cref="LogEntry"/> from <see cref="Its.Log" />.
        /// </summary>
        /// <param name="context">Context it is coming from.</param>
        /// <param name="logEntry">Entry to log.</param>
        public void Log(LogContexts context, LogEntry logEntry)
        {
            if (this.logConfigurationBase.ContextsToLog.HasFlag(LogContexts.None))
            {
                return;
            }

            if (this.logConfigurationBase.ContextsToLog.HasFlag(context))
            {
                var localLogEntry = logEntry ?? new LogEntry(Invariant($"Null {nameof(LogEntry)} Supplied to {nameof(LogProcessorBase)}.{nameof(this.Log)}"));

                this.InternalLog(localLogEntry);
            }
        }

        /// <summary>
        /// Log a <see cref="LogEntry"/> from <see cref="Its.Log" />.
        /// </summary>
        /// <param name="logEntry">Entry to log.</param>
        protected abstract void InternalLog(LogEntry logEntry);
    }
}