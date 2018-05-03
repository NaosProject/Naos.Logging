// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItemPropertiesToIncludeInLogMessage.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    /// <summary>
    /// Determines the properties/aspects of an <see cref="LogItem"/>
    /// to include when building a log message.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames", Justification = "Spelling/name is correct.")]
    [Flags]
    public enum LogItemPropertiesToIncludeInLogMessage
    {
        /// <summary>
        /// Do not include anything.
        /// </summary>
        None = 0,

        /// <summary>
        /// Include the subject.
        /// </summary>
        Timestamp = 1,

        /// <summary>
        /// Include a list of the correlation type and ID as tuples.
        /// </summary>
        CorrelationTypeAndIds = 2,

        /// <summary>
        /// Origin of the entry.
        /// </summary>
        Origin = 3,

        /// <summary>
        /// Include the subject summary.
        /// </summary>
        SubjectSummary = 4,

        /// <summary>
        /// Include stack trace.
        /// </summary>
        StackTrace = 5,

        /// <summary>
        /// Include the serialization correlations.
        /// </summary>
        LogItemSerialization = 6,

        /// <summary>
        /// Default configuration.
        /// </summary>
        Default = Timestamp | CorrelationTypeAndIds | SubjectSummary | StackTrace,
    }
}
