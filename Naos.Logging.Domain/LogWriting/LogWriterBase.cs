// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWriterBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Naos.Logging.Domain;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Enum.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Serialization.Json;
    using OBeautifulCode.Type;
    using static System.FormattableString;
    using SerializationFormat = OBeautifulCode.Serialization.SerializationFormat;

    /// <summary>
    /// Base class for all log writers.
    /// </summary>
    public abstract class LogWriterBase
    {
        /// <summary>
        /// Default serializer description to use for converting a <see cref="LogItem" /> into a string.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is immuatable.")]
        public static readonly SerializationDescription DefaultLogItemSerializationDescription = new SerializationDescription(SerializationKind.Json, SerializationFormat.String, typeof(LoggingJsonConfiguration).ToRepresentation());

        /// <summary>
        /// Default serializer to use for converting a <see cref="LogItem" /> into a string.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is immuatable.")]
        public static readonly ISerializeAndDeserialize DefaultLogItemSerializer = JsonSerializerFactory.Instance.BuildSerializer(DefaultLogItemSerializationDescription, unregisteredTypeEncounteredStrategy: UnregisteredTypeEncounteredStrategy.Attempt);

        private readonly LogWriterConfigBase logWriterConfigBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogWriterBase"/> class.
        /// </summary>
        /// <param name="logWriterConfigBase">The base log writer configuration.</param>
        protected LogWriterBase(
            LogWriterConfigBase logWriterConfigBase)
        {
            this.logWriterConfigBase = logWriterConfigBase ?? throw new ArgumentNullException(nameof(logWriterConfigBase));
        }

        /// <summary>
        /// Logs a <see cref="LogItem" />.
        /// </summary>
        /// <param name="logItem">The item to log.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catching to avoid failures externally.")]
        public void Log(
            LogItem logItem)
        {
            if (logItem == null)
            {
                throw new ArgumentNullException(nameof(logItem));
            }

            if (this.logWriterConfigBase.ShouldLog(logItem.Kind, logItem.Context.Origin))
            {
                try
                {
                    this.LogInternal(logItem);
                }
                catch (Exception failedToLogException)
                {
                    var logPayload = new Tuple<LogItem, LogWriterConfigBase, Exception>(logItem, this.logWriterConfigBase, failedToLogException);
                    var logPayloadJson = DefaultLogItemSerializer.SerializeToString(logPayload);
                    LastDitchLogger.LogError(logPayloadJson);
                }
            }
        }

        /// <summary>
        /// Implementation-specific method for logging a <see cref="LogItem" />.
        /// </summary>
        /// <param name="logItem">The item to log.</param>
        protected abstract void LogInternal(
            LogItem logItem);

        /// <summary>
        /// Builds a log message from a <see cref="LogItem" />.
        /// </summary>
        /// <param name="logItem">The log item.</param>
        /// <param name="logItemPropertiesToIncludeInLogMessage"> The properties/aspects of a <see cref="LogItem"/> to include when building a log message.</param>
        /// <param name="appendTrailingNewLine">Optional value indicating whether or not to append a trailing new line; DEFAULT is false.</param>
        /// <returns>Log message.</returns>
        public static string BuildLogMessageFromLogItem(
            LogItem logItem,
            LogItemPropertiesToIncludeInLogMessage logItemPropertiesToIncludeInLogMessage,
            bool appendTrailingNewLine = false)
        {
            if (logItem == null)
            {
                throw new ArgumentNullException(nameof(logItem));
            }

            if (logItemPropertiesToIncludeInLogMessage == LogItemPropertiesToIncludeInLogMessage.None)
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();
            var itemsToLog = logItemPropertiesToIncludeInLogMessage.GetIndividualFlags<LogItemPropertiesToIncludeInLogMessage>().Where(_ => _.GetIndividualFlags().Count == 1).OrderBy(_ => (int)_).ToList();

            if (itemsToLog.Contains(LogItemPropertiesToIncludeInLogMessage.LogItemSerialization))
            {
                var serializedLogItem = DefaultLogItemSerializer.SerializeToString(logItem);
                stringBuilder.AppendLine();
                stringBuilder.Append(serializedLogItem);
                stringBuilder.Append(",");
            }
            else
            {
                const string indentationPadding = "      ";
                const string firstLineDelimiter = "|";
                if (itemsToLog.Contains(LogItemPropertiesToIncludeInLogMessage.Timestamp))
                {
                    stringBuilder.Append(logItem.Context.TimestampUtc.ToString("u", CultureInfo.InvariantCulture));
                    stringBuilder.Append(firstLineDelimiter);
                }

                if (itemsToLog.Contains(LogItemPropertiesToIncludeInLogMessage.SubjectSummary))
                {
                    stringBuilder.Append("Summary: ");
                    stringBuilder.Append(logItem.Subject.Summary);
                    stringBuilder.Append(firstLineDelimiter);
                }

                if (itemsToLog.Contains(LogItemPropertiesToIncludeInLogMessage.Comment))
                {
                    stringBuilder.Append("Comment: ");
                    stringBuilder.Append(logItem.Comment ?? LogHelper.NullSubjectSummary);
                    stringBuilder.Append(firstLineDelimiter);
                }

                // close out first line
                stringBuilder.AppendLine();

                if (itemsToLog.Contains(LogItemPropertiesToIncludeInLogMessage.Origin))
                {
                    stringBuilder.Append(indentationPadding);
                    stringBuilder.Append("Origin: ");
                    stringBuilder.AppendLine(logItem.Context.Origin);
                }

                if (itemsToLog.Contains(LogItemPropertiesToIncludeInLogMessage.Kind))
                {
                    stringBuilder.Append(indentationPadding);
                    stringBuilder.Append("Kind: ");
                    stringBuilder.AppendLine(logItem.Kind.ToString());
                }

                if (itemsToLog.Contains(LogItemPropertiesToIncludeInLogMessage.CorrelationTypeAndIds))
                {
                    stringBuilder.Append(indentationPadding);
                    stringBuilder.AppendLine("Correlations:");
                    foreach (var correlation in logItem.Correlations)
                    {
                        stringBuilder.Append(indentationPadding);
                        stringBuilder.Append(indentationPadding);
                        stringBuilder.AppendLine(correlation.ToString());
                    }
                }

                if (itemsToLog.Contains(LogItemPropertiesToIncludeInLogMessage.StackTrace))
                {
                    stringBuilder.Append(indentationPadding);
                    stringBuilder.AppendLine("StackTrace:");
                    stringBuilder.Append(indentationPadding);
                    stringBuilder.Append(indentationPadding);
                    stringBuilder.Append(logItem.Context.StackTrace ?? LogHelper.NullSubjectSummary);
                    stringBuilder.AppendLine();
                }
            }

            if (appendTrailingNewLine)
            {
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
     }

    /// <summary>
    /// Last ditch non throwing logger that will try several options.
    /// </summary>
    public static class LastDitchLogger
    {
        /// <summary>
        /// Log the message to several locations as an error.
        /// </summary>
        /// <param name="message">Message to log.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catching to avoid failures externally.")]
        public static void LogError(string message)
        {
            try
            {
                var serializedDateTimeUtcNow = ObcDateTimeStringSerializer.SerializeToString(DateTime.UtcNow);
                const ushort eventLogCharacterLimit = 31914;

                Action<string> logToFile = localMessage =>
                {
                    var preppedForFilePathTime = serializedDateTimeUtcNow.Replace(":", "-").Replace(".", "_").Replace("T", "--");
                    var fileName = preppedForFilePathTime + "--LastDitchLoggedError.log";
                    File.WriteAllText(fileName, localMessage);
                };

                Action<string> logToEventLog = localMessage =>
                {
                    var preppedMessage = localMessage == null ? "Null Message Provided." :
                        localMessage.Length <= eventLogCharacterLimit ? localMessage :
                        localMessage.Substring(0, eventLogCharacterLimit);
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry(preppedMessage, EventLogEntryType.Error, 101, 1);
                    }
                };

                Action<string> logToConsoleOut = localMessage =>
                {
                    var preppedMessage = serializedDateTimeUtcNow + "--" + localMessage;
                    Console.Out.WriteLine(preppedMessage);
                };

                Action<string> logToConsoleError = localMessage =>
                {
                    var preppedMessage = serializedDateTimeUtcNow + "--" + localMessage;
                    Console.Error.WriteLine(preppedMessage);
                };

                Action<string> logToTraceOut = localMessage =>
                {
                    var preppedMessage = serializedDateTimeUtcNow + "--" + localMessage;
                    Trace.WriteLine(preppedMessage);
                };

                Action<string> logToTraceError = localMessage =>
                {
                    var preppedMessage = serializedDateTimeUtcNow + "--" + localMessage;
                    Trace.TraceError(preppedMessage);
                };

                IReadOnlyCollection<Action<string>> actionsToRun = new[]
                {
                    logToFile,
                    logToEventLog,
                    logToConsoleOut,
                    logToConsoleError,
                    logToTraceOut,
                    logToTraceError,
                };

                void RunActionWithExceptionSwallowing(Action<string> actionToRunWithoutAllowingExceptions, string input)
                {
                    try
                    {
                        actionToRunWithoutAllowingExceptions(input);
                    }
                    catch (Exception)
                    {
                        /* no-op */
                    }
                }

                foreach (var actionToRun in actionsToRun)
                {
                    RunActionWithExceptionSwallowing(actionToRun, message);
                }
            }
            catch (Exception)
            {
                 /* no-op */
            }
        }
    }
}
