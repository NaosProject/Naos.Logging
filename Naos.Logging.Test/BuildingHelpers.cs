// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildingHelpers.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;

    using Its.Log.Instrumentation;

    using Naos.Logging.Domain;

    public static class BuildingHelpers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Not sure why this wants all defaults.")]
        public static LogEntry ToLogEntry(this object subject, string comment = null)
        {
            if (subject is LogEntry subjectAsLogEntry)
            {
                return subjectAsLogEntry;
            }

            return new LogEntry(comment, subject);
        }

        public static LogItem ToLogItem(this LogEntry logEntry, LogItemOrigin logItemOrigin)
        {
            return LogWriting.Instance.BuildLogItem(logItemOrigin, logEntry);
        }
    }
}