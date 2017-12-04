// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogProcessing.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
        /// <param name="configuredAndManagedLogProcessors">Optional configured and externally managed log processors; DEFAULT is null.</param>
        /// <param name="multipleCallsToSetupStrategy">Optional strategy to deal with multiple calls to <see cref="Setup" />; DEFAULT is <see cref="MultipleCallsToSetupStrategy.Throw" />.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Keeping this way.")]
        public void Setup(LogProcessorSettings logProcessorSettings, Action<string> announcer = null, IReadOnlyCollection<LogProcessorBase> configuredAndManagedLogProcessors = null, MultipleCallsToSetupStrategy multipleCallsToSetupStrategy = MultipleCallsToSetupStrategy.Throw)
        {
            new { multipleCallsToSetupStrategy }.Must().NotBeEqualTo(MultipleCallsToSetupStrategy.Invalid).OrThrowFirstFailure();
            var localAnnouncer = announcer ?? NullAnnouncement;

            lock (this.sync)
            {
                if (this.hasBeenSetup)
                {
                    switch (multipleCallsToSetupStrategy)
                    {
                        case MultipleCallsToSetupStrategy.Throw:
                            throw new ArgumentException(Invariant($"{nameof(LogProcessing)}.{nameof(LogProcessing.Setup)} was called a second time with {nameof(multipleCallsToSetupStrategy)} - {multipleCallsToSetupStrategy}."));
                        case MultipleCallsToSetupStrategy.Ignore:
                            return;
                        case MultipleCallsToSetupStrategy.Overwrite:
                            break;
                        default:
                            throw new NotSupportedException(Invariant($"{nameof(LogProcessing)}.{nameof(LogProcessing.Setup)} was called with unspported {nameof(multipleCallsToSetupStrategy)} - {multipleCallsToSetupStrategy}."));
                    }
                }

                this.hasBeenSetup = true;

                new { logProcessorSettings }.Must().NotBeNull().OrThrowFirstFailure();

                var logProcessors = new List<LogProcessorBase>(configuredAndManagedLogProcessors ?? new LogProcessorBase[0]);
                if (logProcessors.Any())
                {
                    localAnnouncer(Invariant($"Used pre-configured loggers: {string.Join(",", logProcessors)}"));
                }

                foreach (var configuration in logProcessorSettings.Configurations)
                {
                    if (configuration is FileLogConfiguration fileConfiguration)
                    {
                        var fileLogger = new FileLogProcessor(fileConfiguration);
                        new { fileLogger }.Must().NotBeNull().OrThrowFirstFailure();
                        logProcessors.Add(fileLogger);
                        localAnnouncer(Invariant($"Wired up {fileLogger}."));
                    }
                    else if (configuration is TimeSlicedFilesLogConfiguration timeSlicedFilesConfiguration)
                    {
                        var timeSlicedFilesLogger = new TimeSlicedFilesLogProcessor(timeSlicedFilesConfiguration);
                        new { fileLogger = timeSlicedFilesLogger }.Must().NotBeNull().OrThrowFirstFailure();
                        logProcessors.Add(timeSlicedFilesLogger);
                        localAnnouncer(Invariant($"Wired up {timeSlicedFilesLogger}."));
                    }
                    else if (configuration is EventLogConfiguration eventLogConfiguration)
                    {
                        var eventLogLogger = new EventLogProcessor(eventLogConfiguration);
                        new { eventLogLogger }.Must().NotBeNull().OrThrowFirstFailure();
                        logProcessors.Add(eventLogLogger);
                        localAnnouncer(Invariant($"Wired up {eventLogLogger}."));
                    }
                    else if (configuration is ConsoleLogConfiguration consoleConfiguration)
                    {
                        var consoleLogger = new ConsoleLogProcessor(consoleConfiguration);
                        new { consoleLogger }.Must().NotBeNull().OrThrowFirstFailure();
                        logProcessors.Add(consoleLogger);
                        localAnnouncer(Invariant($"Wired up {consoleLogger}."));
                    }
                    else
                    {
                        throw new NotSupportedException(Invariant($"Unsupported implementation of {nameof(LogConfigurationBase)} - {configuration.GetType().FullName}, try providing a pre-configured implementation of {nameof(LogProcessorBase)} until the config type is supported."));
                    }
                }

                this.activeLogProcessors = logProcessors;

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
    /// Enumeration of options when <see cref="LogProcessing.Setup" /> is called more than once.
    /// </summary>
    public enum MultipleCallsToSetupStrategy
    {
        /// <summary>
        /// Invalid default state.
        /// </summary>
        Invalid,

        /// <summary>
        /// Throw an exception.
        /// </summary>
        Throw,

        /// <summary>
        /// Configure with new settings.
        /// </summary>
        Overwrite,

        /// <summary>
        /// Ignore the new settings.
        /// </summary>
        Ignore,
    }
}