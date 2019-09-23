// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWriting.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Its.Log.Instrumentation;
    using Naos.Compression.Domain;
    using Naos.Diagnostics.Recipes;
    using Naos.Logging.Domain;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Error.Recipes;
    using OBeautifulCode.Math.Recipes;
    using OBeautifulCode.Representation;
    using OBeautifulCode.Type;
    using static System.FormattableString;
    using CorrelationManager = System.Diagnostics.CorrelationManager;
    using Log = Naos.Logging.Domain.Log;

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

        private IReadOnlyCollection<string> errorCodeKeysField;
        private static readonly NaosJsonSerializer LogEntrySerializer = new NaosJsonSerializer(typeof(LoggingJsonConfiguration), UnregisteredTypeEncounteredStrategy.Attempt);

        private LogWriting()
        {
            /* instance class that can only be used as a singleton. */
        }

        /// <summary>
        /// Entry point to configure logging.
        /// </summary>
        /// <param name="logWritingSettings">Configuration for log writing.</param>
        /// <param name="announcer">Optional announcer to communicate setup state; DEFAULT is null.</param>
        /// <param name="configuredAndManagedLogWriters">Optional configured and externally managed log writers; DEFAULT is null.</param>
        /// <param name="multipleCallsToSetupStrategy">Optional strategy to deal with multiple calls to <see cref="Setup" />; DEFAULT is <see cref="MultipleCallsToSetupStrategy.Throw" />.</param>
        /// <param name="errorCodeKeys">Optional error code keys to use when extracting from exceptions.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Keeping this way.")]
        public void Setup(
            LogWritingSettings logWritingSettings,
            Action<string> announcer = null,
            IReadOnlyCollection<LogWriterBase> configuredAndManagedLogWriters = null,
            MultipleCallsToSetupStrategy multipleCallsToSetupStrategy = MultipleCallsToSetupStrategy.Throw,
            IReadOnlyCollection<string> errorCodeKeys = null)
        {
            if (logWritingSettings == null)
            {
                throw new ArgumentNullException(nameof(logWritingSettings));
            }

            if (multipleCallsToSetupStrategy == MultipleCallsToSetupStrategy.Invalid)
            {
                throw new ArgumentException(Invariant($"{nameof(multipleCallsToSetupStrategy)} == {nameof(MultipleCallsToSetupStrategy.Invalid)}"));
            }

            this.errorCodeKeysField = errorCodeKeys;
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
                            throw new NotSupportedException(Invariant($"Unsupported implementation of {nameof(LogWriterConfigBase)} - {config.GetType().ToStringReadable()}, try providing a pre-configured implementation of {nameof(LogWriterBase)} until the config type is supported."));
                    }

                    logWriters.Add(logWriter);
                    localAnnouncer(Invariant($"Wired up {logWriter}."));
                }

                this.activeLogWriters = logWriters;

                this.WireUpAppDomainUnhandledExceptionToActiveLogWriters(localAnnouncer);
                this.WireUpItsLogInternalErrorsToActiveLogWriters(localAnnouncer);
                this.WireUpItsLogEntryPostedToActiveLogWriters(localAnnouncer);
                this.WireUpNaosLoggerToActiveLogWriters(localAnnouncer);
            }
        }

        /// <summary>
        /// Log the log item to any configured active log writers.
        /// </summary>
        /// <param name="logItemOrigin">Origin of the log entry.</param>
        /// <param name="logEntry">Log entry to record.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void LogToActiveLogWriters(
            string logItemOrigin,
            LogEntry logEntry)
        {
            logEntry = logEntry ?? throw new ArgumentNullException(nameof(logEntry));

            LogItem logItem;

            try
            {
                logItem = this.BuildLogItem(logItemOrigin, logEntry);
            }
            catch (Exception ex)
            {
                string serializedLogEntry;
                try
                {
                    serializedLogEntry = LogEntrySerializer.SerializeToString(logEntry);
                }
                catch (Exception failToSerializeLogEntryException)
                {
                    serializedLogEntry = Invariant($"Failed to serialize log entry: {logEntry.Subject} with error: {failToSerializeLogEntryException.Message}");
                }

                var updatedSubject = new InvalidLogEntryException(serializedLogEntry, ex);
                var newLogEntry = new LogEntry(updatedSubject);

                try
                {
                    logItem = this.BuildLogItem(LogItemOrigin.NaosLoggingLogWriting.ToString(), newLogEntry);
                }
                catch (Exception failedToBuildInvalidLogEntryException)
                {
                    var rawSubject = new RawSubject(
                        new FailedToBuildInvalidLogEntryException(
                            "Failed to build invalid log entry.",
                            failedToBuildInvalidLogEntryException),
                        serializedLogEntry);

                    logItem = new LogItem(rawSubject.ToSubject(), LogItemKind.Exception, new LogItemContext(DateTime.UtcNow, LogItemOrigin.NaosLoggingLogWriting.ToString()));
                }
            }

            this.LogToActiveLogWriters(logItem);
        }

        /// <summary>
        /// Log the log item to any configured active log writers.
        /// </summary>
        /// <param name="logItem">Log item to record.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void LogToActiveLogWriters(LogItem logItem)
        {
            logItem = logItem ?? throw new ArgumentNullException(nameof(logItem));

            foreach (var logWriter in this.activeLogWriters)
            {
                try
                {
                    logWriter.Log(logItem);
                }
                catch (Exception failedToLogException)
                {
                    string message;
                    try
                    {
                        var logPayload = new Tuple<LogItem, string, Exception>(logItem, logWriter.ToString(), failedToLogException);
                        message = LogWriterBase.DefaultLogItemSerializer.SerializeToString(logPayload);
                    }
                    catch (Exception failedToSerialize)
                    {
                        message = Invariant($"Error in {this.GetType().ToStringReadable()}.{nameof(this.LogToActiveLogWriters)} - {nameof(failedToLogException)}: {failedToLogException} - {nameof(failedToSerialize)}: {failedToSerialize}");
                    }

                    LastDitchLogger.LogError(message);
                }
            }
        }

        private void WireUpItsLogInternalErrorsToActiveLogWriters(
            Action<string> announcer)
        {
            Its.Log.Instrumentation.Log.InternalErrors += (sender, args) =>
            {
                var logEntry = args.LogEntry ?? new LogEntry(Invariant($"Null {nameof(LogEntry)} Supplied to {nameof(Log)}.{nameof(Its.Log.Instrumentation.Log.InternalErrors)}"));
                this.LogToActiveLogWriters(LogItemOrigin.ItsLogInternalErrors.ToString(), logEntry);
            };

            announcer(Invariant($"Wired up {nameof(Log)}.{nameof(Its.Log.Instrumentation.Log.InternalErrors)} to the {nameof(this.activeLogWriters)}."));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "tel", Justification = "Needed by compiler.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "ex", Justification = "Needed by compiler.")]
        private void WireUpItsLogEntryPostedToActiveLogWriters(
            Action<string> announcer)
        {
            Its.Log.Instrumentation.Log.EntryPosted += (sender, args) =>
            {
                var logEntry = args.LogEntry ?? new LogEntry(Invariant($"Null {nameof(LogEntry)} Supplied to {nameof(Log)}.{nameof(Its.Log.Instrumentation.Log.EntryPosted)}"));

                this.LogToActiveLogWriters(LogItemOrigin.ItsLogEntryPosted.ToString(), logEntry);
            };

            announcer(Invariant($"Wired up {nameof(Its.Log.Instrumentation.Log)}.{nameof(Its.Log.Instrumentation.Log.EntryPosted)} to the {nameof(this.activeLogWriters)}."));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "tel", Justification = "Needed by compiler.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "ex", Justification = "Needed by compiler.")]
        private void WireUpNaosLoggerToActiveLogWriters(
            Action<string> announcer)
        {
            Log.SetCallback(this.LogToActiveLogWriters);

            announcer(Invariant($"Wired up {nameof(Log)}.{nameof(Log.Instance.SetCallback)} to the {nameof(this.activeLogWriters)}."));
        }

        private void WireUpAppDomainUnhandledExceptionToActiveLogWriters(
            Action<string> announcer)
        {
            AppDomain.CurrentDomain.UnhandledException += (o, args) =>
            {
                var logEntry = new LogEntry(Invariant($"Unhandled exception encountered"), args.ExceptionObject);

                this.LogToActiveLogWriters(LogItemOrigin.AppDomainUnhandledException.ToString(), logEntry);
            };

            announcer(Invariant($"Wired up {nameof(AppDomain)}.{nameof(AppDomain.UnhandledException)} to the {nameof(this.activeLogWriters)}."));
        }

        private static void NullAnnouncement(
            string message)
        {
            /* no-op */
        }

        private static int GetSomewhatFakePositionFromLogEntry(
            LogEntry logEntry)
        {
            int result;
            switch (logEntry.EventType)
            {
                case TraceEventType.Verbose:
                    result = 1;
                    break;
                case TraceEventType.Start:
                    result = 0;
                    break;
                case TraceEventType.Stop:
                    result = 2;
                    break;
                default:
                    result = -1;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Build a <see cref="LogItem" /> from a <see cref="LogEntry" />.
        /// </summary>
        /// <param name="logItemOrigin">Origin of the <see cref="LogEntry" />.</param>
        /// <param name="logEntry"><see cref="LogEntry" /> to convert.</param>
        /// <param name="additionalCorrelations">Additional correlations to add.</param>
        /// <returns>Correct <see cref="LogItem" />.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Acceptable coupling.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Acceptable complexity.")]
        public LogItem BuildLogItem(
            string logItemOrigin,
            LogEntry logEntry,
            IReadOnlyCollection<IHaveCorrelationId> additionalCorrelations = null)
        {
            logEntry = logEntry ?? throw new ArgumentNullException(nameof(logEntry));

            var psuedoOrderCorrelationPosition = GetSomewhatFakePositionFromLogEntry(logEntry);

            if (!string.IsNullOrWhiteSpace(logEntry.Category))
            {
                throw new ArgumentException(Invariant($"{nameof(LogEntry)} cannot have the property {nameof(LogEntry.Category)} set; found: {logEntry.Category}."));
            }

            if (psuedoOrderCorrelationPosition != 1 && (logEntry.Params?.Any() ?? false))
            {
                throw new ArgumentException(Invariant($"{nameof(LogEntry)} cannot have the property {nameof(LogEntry.Params)} set unless it's part of the {nameof(LogActivity.Trace)} scenario; found: {logEntry.Params.Select(_ => _.ToString()).ToCsv()}"));
            }

            object logItemSubjectObject;
            var correlations = new List<IHaveCorrelationId>(additionalCorrelations ?? new IHaveCorrelationId[0]);
            var correlationId = Guid.NewGuid().ToString().ToLowerInvariant();
            if (psuedoOrderCorrelationPosition == -1)
            {
                logItemSubjectObject = logEntry.Subject;
            }
            else
            {
                if ((psuedoOrderCorrelationPosition == 0) ||
                    (psuedoOrderCorrelationPosition == 2))
                {
                    logItemSubjectObject = logEntry.Subject;
                }
                else if (psuedoOrderCorrelationPosition == 1)
                {
                    if (logEntry.Params?.Any() ?? false)
                    {
                        if (logEntry.Params.Count() > 1)
                        {
                            throw new InvalidOperationException(Invariant($"{nameof(LogEntry)} cannot have the property {nameof(LogEntry.Params)} set with more than one value as part of the {nameof(LogActivity.Trace)} scenario; found: {logEntry.Params.Select(_ => _.ToString()).ToCsv()}"));
                        }
                        else
                        {
                            logItemSubjectObject = logEntry.Params.Single();
                        }
                    }
                    else
                    {
                        logItemSubjectObject = logEntry.Message;
                    }
                }
                else
                {
                    throw new NotSupportedException(Invariant($"This fake order correlation position is not supported: {psuedoOrderCorrelationPosition}"));
                }

                var activityCorrelatingSubject = new RawSubject(
                    logEntry.Subject,
                    LogHelper.BuildSummaryFromSubjectObject(logEntry.Subject));

                correlationId = activityCorrelatingSubject.GetHashCode().ToGuid().ToString().ToLowerInvariant();
                var elapsedMilliseconds = psuedoOrderCorrelationPosition == 0 ? 0 : logEntry.ElapsedMilliseconds ?? throw new InvalidOperationException(Invariant($"{nameof(logEntry)}.{nameof(LogEntry.ElapsedMilliseconds)} is null when there is an {nameof(ElapsedCorrelation)}"));
                var elapsedCorrelation = new ElapsedCorrelation(correlationId, TimeSpan.FromMilliseconds(elapsedMilliseconds));
                var correlatingSubject = activityCorrelatingSubject.ToSubject();
                correlations.Add(elapsedCorrelation);

                var subjectCorrelation = new SubjectCorrelation(correlationId, correlatingSubject);
                correlations.Add(subjectCorrelation);

                var orderCorrelation = new OrderCorrelation(correlationId, psuedoOrderCorrelationPosition);
                correlations.Add(orderCorrelation);
            }

            string stackTrace = null;
            var kind = LogHelper.DetermineKindFromSubject(logItemSubjectObject);

            if (logItemSubjectObject is Exception loggedException)
            {
                var exceptionCorrelations = Logging.Domain.CorrelationManager.BuildExceptionCorrelations(loggedException, correlationId, this.errorCodeKeysField);
                correlations.AddRange(exceptionCorrelations);

                stackTrace = loggedException.StackTrace;
            }

            switch (psuedoOrderCorrelationPosition)
            {
                case 0:
                    logItemSubjectObject = UsingBlockLogger.InitialItemOfUsingBlockSubject;
                    break;
                case 2:
                    logItemSubjectObject = UsingBlockLogger.FinalItemOfUsingBlockSubject;
                    break;
            }

            var logItemRawSubject = new RawSubject(
                logItemSubjectObject,
                LogHelper.BuildSummaryFromSubjectObject(logItemSubjectObject));

            var context = new LogItemContext(logEntry.TimeStamp, logItemOrigin, LogHelper.MachineName, LogHelper.ProcessName, LogHelper.ProcessFileVersion, logEntry.CallingMethod, logEntry.CallingType?.ToRepresentation(), stackTrace);

            var comment = (logItemSubjectObject is string logItemSubjectAsString) && (logItemSubjectAsString == logEntry.Message) ? null : logEntry.Message;

            var result = new LogItem(logItemRawSubject.ToSubject(), kind, context, comment, correlations);

            return result;
        }
    }
}