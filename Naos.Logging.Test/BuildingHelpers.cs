// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildingHelpers.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using Its.Log.Instrumentation;
    using Naos.Logging.Domain;
    using Naos.Logging.Persistence;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Compression;
    using OBeautifulCode.Compression.Recipes;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Serialization.Json;

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
            new { subject }.AsArg().Must().NotBeNull();
            return subject.DescribedSerialization.DeserializePayloadUsingSpecificFactory(new JsonSerializerFactory());
        }

        public static T DeserializeSubject<T>(this Subject subject)
        {
            new { subject }.AsArg().Must().NotBeNull();
            return subject.DescribedSerialization.DeserializePayloadUsingSpecificFactory<T>(new JsonSerializerFactory());
        }
    }
}
