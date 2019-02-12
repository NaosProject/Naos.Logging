// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSlicedFilesLogReader.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Naos.Logging.Domain;
    using OBeautifulCode.Enum.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// <see cref="TimeSlicedFilesLogConfig"/> focused implementation of <see cref="LogReaderBase" />.
    /// </summary>
    public class TimeSlicedFilesLogReader : LogReaderBase
    {
        private readonly TimeSlicedFilesLogConfig timeSlicedFilesLogConfig;

        private readonly object fileLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSlicedFilesLogReader"/> class.
        /// </summary>
        /// <param name="timeSlicedFilesLogConfig">Configuration.</param>
        public TimeSlicedFilesLogReader(
            TimeSlicedFilesLogConfig timeSlicedFilesLogConfig)
            : base(timeSlicedFilesLogConfig)
        {
            this.timeSlicedFilesLogConfig = timeSlicedFilesLogConfig ?? throw new ArgumentNullException(nameof(timeSlicedFilesLogConfig));

            var logItemPropertiesLogged = this.timeSlicedFilesLogConfig.LogItemPropertiesToIncludeInLogMessage.GetIndividualFlags();
            if (logItemPropertiesLogged.Count > 1 || !logItemPropertiesLogged.Contains(LogItemPropertiesToIncludeInLogMessage.LogItemSerialization))
            {
                throw new ArgumentException(Invariant($"{nameof(TimeSlicedFilesLogReader)} is only compatible when the {nameof(TimeSlicedFilesLogConfig)} used to write the logs was configured with {nameof(timeSlicedFilesLogConfig.LogItemPropertiesToIncludeInLogMessage)} set to ONLY {LogItemPropertiesToIncludeInLogMessage.LogItemSerialization}."));
            }
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<LogItem> ReadAll()
        {
            return this.ReadRange(DateTime.MinValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime());
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<LogItem> ReadRange(
            DateTime startUtcInclusive,
            DateTime endUtcInclusive)
        {
            lock (this.fileLock)
            {
                var stringBuiler = new StringBuilder();
                var minFile = this.timeSlicedFilesLogConfig.ComputeFilePath(startUtcInclusive);
                var maxFile = this.timeSlicedFilesLogConfig.ComputeFilePath(endUtcInclusive);
                var allFiles = Directory.GetFiles(this.timeSlicedFilesLogConfig.LogFileDirectoryPath);
                var result = allFiles.OrderBy(_ => _)
                    .Where(
                        _ => string.Compare(minFile, _, StringComparison.OrdinalIgnoreCase) <= 0
                             && string.Compare(maxFile, _, StringComparison.OrdinalIgnoreCase) >= 0).SelectMany(
                        filePath =>
                            {
                                try
                                {
                                    var fileContents = File.ReadAllText(filePath);
                                    stringBuiler.Clear();
                                    stringBuiler.Append("[");
                                    stringBuiler.Append(fileContents);
                                    stringBuiler.Append("]");

                                    var deserializedLogItems = LogWriterBase.DefaultLogItemSerializer.Deserialize<IReadOnlyCollection<LogItem>>(stringBuiler.ToString());
                                    var filtered = deserializedLogItems.Where(
                                        _ => _.Context.TimestampUtc >= startUtcInclusive && _.Context.TimestampUtc <= endUtcInclusive);
                                    return filtered;
                                }
                                catch (Exception ex)
                                {
                                    throw new InvalidOperationException(Invariant($"Failed to read log file: {filePath}"), ex);
                                }
                            }).ToList();

                return result;
            }
        }
    }
}