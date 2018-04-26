// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEntryPropertiesToIncludeInLogMessage.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using Its.Log.Instrumentation;

    /// <summary>
    /// Determines the properties/aspects of an <see cref="Its.Log"/> <see cref="LogEntry"/>
    /// to include when building a log message.  If <see cref="LogEntryPropertiesToIncludeInLogMessage.Default"/>
    /// then the entry's default string representation is used.
    /// </summary>
    [Flags]
    public enum LogEntryPropertiesToIncludeInLogMessage
    {
        /// <summary>
        /// Use the log entry's default string representation.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Include the subject.
        /// </summary>
        Subject = 1,

        /// <summary>
        /// Include parameters.
        /// </summary>
        Parameters = 2,

        /// <summary>
        /// Include subject and parameters.
        /// </summary>
        SubjectAndParameters = Subject | Parameters,
    }
}
