// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWriting.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Its.Log.Instrumentation;

    using static System.FormattableString;

    /// <summary>
    /// Log writing manager.
    /// </summary>
    public class LogWriting
    {
        /// <summary>
        /// Gets the singleton entry point to the code.
        /// </summary>
        public static LogWriting Instance { get; } = new LogWriting();

        private readonly object sync = new object();

        private bool hasBeenSetup = false;

        private IReadOnlyCollection<LogWriterBase> activeLogWriters;

        private LogWriting()
        {
            /* no-op to make sure this can only be accessed via instance property */
        }

        /// <summary>
        /// Entry point to configure logging.
        /// </summary>
        /// <param name="logWritingSettings">Configuration for log writing.</param>
        /// <param name="announcer">Optional announcer to communicate setup state; DEFAULT is null.</param>
        /// <param name="configuredAndManagedLogWriters">Optional configured and externally managed log writers; DEFAULT is null.</param>
        /// <param name="multipleCallsToSetupStrategy">Optional strategy to deal with multiple calls to <see cref="Setup" />; DEFAULT is <see cref="MultipleCallsToSetupStrategy.Throw" />.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Keeping this way.")]
        public void Setup(
            LogWritingSettings logWritingSettings,
            Action<string> announcer = null,
            IReadOnlyCollection<LogWriterBase> configuredAndManagedLogWriters = null,
            MultipleCallsToSetupStrategy multipleCallsToSetupStrategy = MultipleCallsToSetupStrategy.Throw)
        {
            if (logWritingSettings == null)
            {
                throw new ArgumentNullException(nameof(logWritingSettings));
            }

            if (multipleCallsToSetupStrategy == MultipleCallsToSetupStrategy.Invalid)
            {
                throw new ArgumentException(Invariant($"{nameof(multipleCallsToSetupStrategy)} == {nameof(MultipleCallsToSetupStrategy.Invalid)}"));
            }

            var localAnnouncer = announcer ?? NullAnnouncement;

            lock (this.sync)
            {
                if (this.hasBeenSetup)
                {
                    switch (multipleCallsToSetupStrategy)
                    {
                        case MultipleCallsToSetupStrategy.Throw:
                            throw new ArgumentException(Invariant($"{nameof(LogWriting)}.{nameof(LogWriting.Setup)} was called a second time with {nameof(multipleCallsToSetupStrategy)} - {multipleCallsToSetupStrategy}."));
                        case MultipleCallsToSetupStrategy.Ignore:
                            return;
                        case MultipleCallsToSetupStrategy.Overwrite:
                            break;
                        default:
                            throw new NotSupportedException(Invariant($"{nameof(LogWriting)}.{nameof(LogWriting.Setup)} was called with unspported {nameof(multipleCallsToSetupStrategy)} - {multipleCallsToSetupStrategy}."));
                    }
                }

                this.hasBeenSetup = true;

                var logWriters = new List<LogWriterBase>(configuredAndManagedLogWriters ?? new LogWriterBase[0]);
                if (logWriters.Any())
                {
                    localAnnouncer(Invariant($"Used pre-configured loggers: {string.Join(",", logWriters)}"));
                }

                foreach (var config in logWritingSettings.Configs)
                {
                    LogWriterBase logWriter;
                    switch (config)
                    {
                        case FileLogConfig fileLogConfig:
                            logWriter = new FileLogWriter(fileLogConfig);
                            break;
                        case TimeSlicedFilesLogConfig timeSlicedFilesLogConfig:
                            logWriter = new TimeSlicedFilesLogWriter(timeSlicedFilesLogConfig);
                            break;
                        case EventLogConfig eventLogConfig:
                            logWriter = new EventLogWriter(eventLogConfig);
                            break;
                        case ConsoleLogConfig consoleLogConfig:
                            logWriter = new ConsoleLogWriter(consoleLogConfig);
                            break;
                        default:
                            throw new NotSupportedException(Invariant($"Unsupported implementation of {nameof(LogWriterConfigBase)} - {config.GetType().FullName}, try providing a pre-configured implementation of {nameof(LogWriterBase)} until the config type is supported."));
                    }

                    logWriters.Add(logWriter);
                    localAnnouncer(Invariant($"Wired up {logWriter}."));
                }

                this.activeLogWriters = logWriters;

                this.WireUpAppDomainUnhandledExceptionToActiveLogWriters(localAnnouncer);
                this.WireUpItsLogInternalErrorsToActiveLogWriters(localAnnouncer);
                this.WireUpItsLogEntryPostedToActiveLogWriters(localAnnouncer);
            }
        }

        private void LogToActiveLogWriters(
            LogItemOrigin logItemOrigin,
            LogEntry logEntry)
        {
            foreach (var logWriter in this.activeLogWriters)
            {
                try
                {
                    logWriter.Log(logItemOrigin, logEntry);
                }
                catch (Exception)
                {
                    /* no-op */
                }
            }
        }

        private void WireUpItsLogInternalErrorsToActiveLogWriters(
            Action<string> announcer)
        {
            Log.InternalErrors += (sender, args) =>
            {
                var logEntry = args.LogEntry ?? new LogEntry(Invariant($"Null {nameof(LogEntry)} Supplied to {nameof(Log)}.{nameof(Log.InternalErrors)}"));
                this.LogToActiveLogWriters(LogItemOrigin.ItsLogInternalErrors, logEntry);
            };

            announcer(Invariant($"Wired up {nameof(Log)}.{nameof(Log.InternalErrors)} to the {nameof(this.activeLogWriters)}."));
        }

        private void WireUpItsLogEntryPostedToActiveLogWriters(
            Action<string> announcer)
        {
            Log.EntryPosted += (sender, args) =>
            {
                var logEntry = args.LogEntry ?? new LogEntry(Invariant($"Null {nameof(LogEntry)} Supplied to {nameof(Log)}.{nameof(Log.EntryPosted)}"));

                var logItemOrigin = logEntry.Subject is Exception ? LogItemOrigin.ItsLogEntryPostedException : LogItemOrigin.ItsLogEntryPostedInformation;

                this.LogToActiveLogWriters(logItemOrigin, logEntry);
            };

            announcer(Invariant($"Wired up {nameof(Log)}.{nameof(Log.EntryPosted)} to the {nameof(this.activeLogWriters)}."));
        }

        private void WireUpAppDomainUnhandledExceptionToActiveLogWriters(
            Action<string> announcer)
        {
            AppDomain.CurrentDomain.UnhandledException += (o, args) =>
            {
                var logEntry = new LogEntry(Invariant($"Unhandled exception encountered"), args.ExceptionObject);

                this.LogToActiveLogWriters(LogItemOrigin.AppDomainUnhandledException, logEntry);
            };

            announcer(Invariant($"Wired up {nameof(AppDomain)}.{nameof(AppDomain.UnhandledException)} to the {nameof(this.activeLogWriters)}."));
        }

        private static void NullAnnouncement(
            string message)
        {
            /* no-op */
        }
    }

    /// <summary>
    /// Enumeration of options when <see cref="LogWriting.Setup" /> is called more than once.
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