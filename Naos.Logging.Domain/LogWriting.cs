// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWriting.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Its.Log.Instrumentation;
    using Naos.Compression.Domain;
    using Naos.Diagnostics.Recipes;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Domain.Extensions;
    using Naos.Serialization.Json;
    using Naos.Telemetry.Domain;

    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Enum.Recipes;
    using OBeautifulCode.Math.Recipes;
    using OBeautifulCode.TypeRepresentation;
    using static System.FormattableString;

    /// <summary>
    /// Log writing manager.
    /// </summary>
    public class LogWriting
    {
        /// <summary>
        /// <see cref="SerializationDescription" /> to use for serializing the subject object.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Want a field here.")]
        public static readonly SerializationDescription SubjectSerializationDescription = new SerializationDescription(SerializationFormat.Json, SerializationRepresentation.String, SerializationKind.Compact);

        private static readonly HashSet<LogItemOrigin> ErrorOrigins = new HashSet<LogItemOrigin>(LogItemOrigins.AllErrors.GetIndividualFlags<LogItemOrigins>().Select(_ => (LogItemOrigin)Enum.Parse(typeof(LogItemOrigin), _.ToString())));

        private readonly string machineName;

        private readonly string processName;

        private readonly string processFileVersion;

        /// <summary>
        /// Gets the singleton entry point to the code.
        /// </summary>
        public static LogWriting Instance { get; } = new LogWriting();

        private readonly object sync = new object();

        private bool hasBeenSetup = false;

        private IReadOnlyCollection<LogWriterBase> activeLogWriters;

        private LogWriting()
        {
            this.machineName = MachineName.GetMachineName();
            this.processName = ProcessHelpers.GetRunningProcess().GetName();
            this.processFileVersion = ProcessHelpers.GetRunningProcess().GetFileVersion();
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
                    serializedLogEntry = new NaosJsonSerializer().SerializeToString(logEntry);
                }
                catch (Exception failToSerializeLogEntryException)
                {
                    serializedLogEntry = Invariant($"Failed to serialize log entry: {failToSerializeLogEntryException.Message}");
                }

                var updatedSubject = new InvalidLogEntryException(serializedLogEntry, ex);
                var newLogEntry = new LogEntry(updatedSubject);

                try
                {
                    logItem = this.BuildLogItem(LogItemOrigin.ItsLogEntryPostedMalformedLogEntry, newLogEntry);
                }
                catch (Exception failedToBuildInvalidLogEntryException)
                {
                    var rawSubject = new RawSubject
                    {
                        OriginalSubject = Invariant($"Failed to build invalid log entry: {failedToBuildInvalidLogEntryException.Message}"),
                        Summary = serializedLogEntry,
                    };

                    logItem = new LogItem(rawSubject.ToSubject(), LogItemKind.Error, new LogItemContext(DateTime.UtcNow, LogItemOrigin.ItsLogEntryPostedFailedToBuildInvalidLogEntry));
                }
            }

            foreach (var logWriter in this.activeLogWriters)
            {
                try
                {
                    logWriter.Log(logItem);
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "tel", Justification = "Needed by compiler.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "ex", Justification = "Needed by compiler.")]
        private void WireUpItsLogEntryPostedToActiveLogWriters(
            Action<string> announcer)
        {
            Log.EntryPosted += (sender, args) =>
            {
                var logEntry = args.LogEntry ?? new LogEntry(Invariant($"Null {nameof(LogEntry)} Supplied to {nameof(Log)}.{nameof(Log.EntryPosted)}"));

                LogItemOrigin logItemOrigin;
                switch (logEntry.Subject)
                {
                    case Exception ex:
                        logItemOrigin = LogItemOrigin.ItsLogEntryPostedException;
                        break;
                    case IAmTelemetryItem tel:
                        logItemOrigin = LogItemOrigin.ItsLogEntryPostedTelemetry;
                        break;
                    default:
                        logItemOrigin = LogItemOrigin.ItsLogEntryPostedInformation;
                        break;
                }

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

        private static ActivityCorrelationPosition GetActivityCorrelationPositionFromLogEntry(
            LogEntry logEntry)
        {
            ActivityCorrelationPosition result;
            switch (logEntry.EventType)
            {
                case TraceEventType.Verbose:
                    result = ActivityCorrelationPosition.Middle;
                    break;
                case TraceEventType.Start:
                    result = ActivityCorrelationPosition.First;
                    break;
                case TraceEventType.Stop:
                    result = ActivityCorrelationPosition.Last;
                    break;
                default:
                    result = ActivityCorrelationPosition.Unknown;
                    break;
            }

            return result;
        }

        private static string BuildSummaryFromSubjectObject(
            object subjectObject,
            ActivityCorrelationPosition activityCorrelationPosition)
        {
            string result;
            if (subjectObject is Exception ex)
            {
                result = Invariant($"{ex.GetType().Name}: {ex.Message}");
            }
            else
            {
                result = Formatter.Format(subjectObject);
            }

            if (activityCorrelationPosition != ActivityCorrelationPosition.Unknown)
            {
                result = Invariant($"{activityCorrelationPosition}: {result}");
            }

            return result;
        }

        /// <summary>
        /// Build a <see cref="LogItem" /> from a <see cref="LogEntry" />.
        /// </summary>
        /// <param name="logItemOrigin"><see cref="LogItemOrigin" /> of the <see cref="LogEntry" />.</param>
        /// <param name="logEntry"><see cref="LogEntry" /> to convert.</param>
        /// <returns>Correct <see cref="LogItem" />.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Acceptable complexity.")]
        public LogItem BuildLogItem(
            LogItemOrigin logItemOrigin,
            LogEntry logEntry)
        {
            logEntry = logEntry ?? throw new ArgumentNullException(nameof(logEntry));

            var activityCorrelationPosition = GetActivityCorrelationPositionFromLogEntry(logEntry);

            if (!string.IsNullOrWhiteSpace(logEntry.Category))
            {
                throw new ArgumentException(Invariant($"{nameof(LogEntry)} cannot have the property {nameof(LogEntry.Category)} set; found: {logEntry.Category}."));
            }

            if (activityCorrelationPosition != ActivityCorrelationPosition.Middle && (logEntry.Params?.Any() ?? false))
            {
                throw new ArgumentException(Invariant($"{nameof(LogEntry)} cannot have the property {nameof(LogEntry.Params)} set unless it's part of the {nameof(LogActivity.Trace)} scenario; found: {logEntry.Params.Select(_ => _.ToString()).ToCsv()}"));
            }

            object logItemSubjectObject;
            var correlations = new List<IHaveCorrelationId>(2);

            if (activityCorrelationPosition == ActivityCorrelationPosition.Unknown)
            {
                logItemSubjectObject = logEntry.Subject;
            }
            else
            {
                if ((activityCorrelationPosition == ActivityCorrelationPosition.First) ||
                    (activityCorrelationPosition == ActivityCorrelationPosition.Last))
                {
                    logItemSubjectObject = logEntry.Subject;
                }
                else if (activityCorrelationPosition == ActivityCorrelationPosition.Middle)
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
                    throw new NotSupportedException(Invariant($"This {nameof(ActivityCorrelationPosition)} is not supported: {activityCorrelationPosition}"));
                }

                var activityCorrelatingSubject = new RawSubject
                {
                    OriginalSubject = logEntry.Subject,
                    Summary = BuildSummaryFromSubjectObject(logEntry.Subject, activityCorrelationPosition),
                };

                var correlatingId = activityCorrelatingSubject.GetHashCode().ToGuid().ToString();
                var elapsedMilliseconds = activityCorrelationPosition == ActivityCorrelationPosition.First ? 0 : logEntry.ElapsedMilliseconds ?? throw new InvalidOperationException(Invariant($"{nameof(logEntry)}.{nameof(LogEntry.ElapsedMilliseconds)} is null when there is an {nameof(ActivityCorrelation)}"));
                var activityCorrelation = new ActivityCorrelation(correlatingId, activityCorrelatingSubject.ToSubject(), elapsedMilliseconds, activityCorrelationPosition);
                correlations.Add(activityCorrelation);
            }

            string stackTrace = null;
            if (logItemSubjectObject is Exception loggedException)
            {
                var correlatingException = loggedException.FindFirstExceptionInChainWithExceptionId();
                if (correlatingException == null)
                {
                    loggedException.GenerateAndWriteExceptionIdIntoExceptionData();
                    correlatingException = loggedException;
                }

                var correlationId = correlatingException.GetExceptionIdFromExceptionData().ToString();

                var exceptionCorrelatingSubject = new RawSubject
                {
                    OriginalSubject = correlatingException,
                    Summary = BuildSummaryFromSubjectObject(correlatingException, activityCorrelationPosition),
                };

                var exceptionCorrelation = new ExceptionCorrelation(correlationId, exceptionCorrelatingSubject.ToSubject());
                correlations.Add(exceptionCorrelation);

                stackTrace = loggedException.StackTrace;
            }

            var logItemRawSubject = new RawSubject
            {
                OriginalSubject = logItemSubjectObject,
                Summary = BuildSummaryFromSubjectObject(logItemSubjectObject, activityCorrelationPosition),
            };

            var context = new LogItemContext(logEntry.TimeStamp, logItemOrigin, this.machineName, this.processName, this.processFileVersion, logEntry.CallingMethod, logEntry.CallingType?.ToTypeDescription(), stackTrace);

            var kind = ErrorOrigins.Contains(context.Origin) ? LogItemKind.Error : LogItemKind.Info;

            var comment = (logItemSubjectObject is string logItemSubjectAsString) && (logItemSubjectAsString == logEntry.Message) ? null : logEntry.Message;

            var result = new LogItem(logItemRawSubject.ToSubject(), kind, context, comment, correlations);

            return result;
        }

        private class RawSubject
        {
            public object OriginalSubject { get; set; }

            public string Summary { get; set; }

            public Subject ToSubject()
            {
                var describedSerialization = this.OriginalSubject.ToDescribedSerializationUsingSpecificFactory(
                    SubjectSerializationDescription,
                    JsonSerializerFactory.Instance,
                    CompressorFactory.Instance);
                var result = new Subject(describedSerialization, this.Summary);
                return result;
            }
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