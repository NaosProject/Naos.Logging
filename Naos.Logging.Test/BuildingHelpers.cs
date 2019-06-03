// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildingHelpers.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using Its.Log.Instrumentation;

    using Naos.Compression.Domain;
    using Naos.Logging.Domain;
    using Naos.Logging.Persistence;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;
    using OBeautifulCode.Validation.Recipes;

    public static class BuildingHelpers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1026:DefaultParametersShouldNotBeUsed",
            Justification = "Not sure why this wants all defaults.")]
        public static LogEntry ToLogEntry(this object subject, string comment = null)
        {
            if (subject is LogEntry subjectAsLogEntry)
            {
                return subjectAsLogEntry;
            }

            return new LogEntry(comment, subject);
        }

        /// <summary>
        /// Build a <see cref="LogItem" /> from a <see cref="LogEntry" />.
        /// </summary>
        /// <param name="logEntry">Log entry to use.</param>
        /// <param name="logItemOrigin">Origin of log entry.</param>
        /// <returns>Constructed <see cref="LogItem" />.</returns>
        public static LogItem ToLogItem(this LogEntry logEntry, LogItemOrigin logItemOrigin)
        {
            return logEntry.ToLogItem(logItemOrigin.ToString());
        }

        /// <summary>
        /// Build a <see cref="LogItem" /> from a <see cref="LogEntry" />.
        /// </summary>
        /// <param name="logEntry">Log entry to use.</param>
        /// <param name="logItemOrigin">Origin of log entry.</param>
        /// <returns>Constructed <see cref="LogItem" />.</returns>
        public static LogItem ToLogItem(this LogEntry logEntry, string logItemOrigin)
        {
            if (logEntry == null)
            {
                throw new ArgumentNullException(nameof(logEntry));
            }

            return LogWriting.Instance.BuildLogItem(logItemOrigin, logEntry);
        }

        public static object DeserializeSubject(this Subject subject)
        {
            new { subject }.Must().NotBeNull();
            return subject.DescribedSerialization.DeserializePayloadUsingSpecificFactory(JsonSerializerFactory.Instance, CompressorFactory.Instance);
        }

        public static T DeserializeSubject<T>(this Subject subject)
        {
            new { subject }.Must().NotBeNull();
            return subject.DescribedSerialization.DeserializePayloadUsingSpecificFactory<T>(JsonSerializerFactory.Instance, CompressorFactory.Instance, unregisteredTypeEncounteredStrategy: UnregisteredTypeEncounteredStrategy.Attempt);
        }
    }
}