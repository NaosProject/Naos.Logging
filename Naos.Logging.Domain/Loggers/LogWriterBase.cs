// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWriterBase.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;

    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Enum.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Base class for all log writers.
    /// </summary>
    public abstract class LogWriterBase
    {
        /// <summary>
        /// Default serializer description to use for converting a <see cref="LogItem" /> into a string.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is immuatable.")]
        public static readonly SerializationDescription DefaultLogItemSerializationDescription = new SerializationDescription(SerializationFormat.Json, SerializationRepresentation.String);

        /// <summary>
        /// Default serializer to use for converting a <see cref="LogItem" /> into a string.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is immuatable.")]
        public static readonly ISerializeAndDeserialize DefaultLogItemSerializer = JsonSerializerFactory.Instance.BuildSerializer(DefaultLogItemSerializationDescription);

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
        public void Log(
            LogItem logItem)
        {
            if (logItem == null)
            {
                throw new ArgumentNullException(nameof(logItem));
            }

            var origins = logItem.Context.Origin.ToOrigins();
            if ((this.logWriterConfigBase.OriginsToLog != LogItemOrigins.None) && this.logWriterConfigBase.OriginsToLog.HasFlagOverlap(origins))
            {
                this.LogInternal(logItem);
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
        protected static string BuildLogMessageFromLogItem(
            LogItem logItem,
            LogItemPropertiesToIncludeInLogMessage logItemPropertiesToIncludeInLogMessage,
            bool appendTrailingNewLine = false)
        {
            if (logItem == null)
            {
                throw new ArgumentNullException(nameof(logItem));
            }

            var stringBuiler = new StringBuilder();
            var itemsToLog = logItemPropertiesToIncludeInLogMessage.GetIndividualFlags<LogItemPropertiesToIncludeInLogMessage>().Where(_ => _.GetIndividualFlags().Count == 1).OrderBy(_ => (int)_).ToList();

            foreach (var itemToLog in itemsToLog)
            {
                switch (itemToLog)
                {
                    case LogItemPropertiesToIncludeInLogMessage.None:
                        /* no - op */
                        break;
                    case LogItemPropertiesToIncludeInLogMessage.Timestamp:
                        stringBuiler.Append(logItem.Context.TimestampUtc.ToString("u", CultureInfo.InvariantCulture));
                        stringBuiler.Append("|");
                        break;
                    case LogItemPropertiesToIncludeInLogMessage.CorrelationTypeAndIds:
                        var correlationEntries = logItem.Correlations.Select(_ => Invariant($"{_.GetType().Name}={_.CorrelationId}")).ToList();
                        stringBuiler.Append(correlationEntries.Any() ? correlationEntries.ToCsv() : "No Correlations");
                        stringBuiler.Append("|");
                        break;
                    case LogItemPropertiesToIncludeInLogMessage.Origin:
                        stringBuiler.Append(logItem.Context.Origin);
                        stringBuiler.Append("|");
                        break;
                    case LogItemPropertiesToIncludeInLogMessage.SubjectSummary:
                        stringBuiler.Append(logItem.Subject.Summary);
                        stringBuiler.Append("|");
                        break;
                    case LogItemPropertiesToIncludeInLogMessage.Comment:
                        stringBuiler.Append(string.IsNullOrWhiteSpace(logItem.Comment) ? "No Comment" : logItem.Comment);
                        stringBuiler.Append("|");
                        break;
                    case LogItemPropertiesToIncludeInLogMessage.StackTrace:
                        stringBuiler.AppendLine();
                        stringBuiler.AppendLine(logItem.Context.StackTrace);
                        break;
                    case LogItemPropertiesToIncludeInLogMessage.LogItemSerialization:
                        var serializedLogItem = DefaultLogItemSerializer.SerializeToString(logItem);
                        stringBuiler.AppendLine();
                        stringBuiler.Append(serializedLogItem);
                        stringBuiler.Append(",");
                        break;
                    default:
                        throw new NotSupportedException(Invariant($"Unsupported {nameof(LogItemPropertiesToIncludeInLogMessage)}: {itemToLog}."));
                }
            }

            if (appendTrailingNewLine)
            {
                stringBuiler.AppendLine();
            }

            return stringBuiler.ToString();
        }
     }
}