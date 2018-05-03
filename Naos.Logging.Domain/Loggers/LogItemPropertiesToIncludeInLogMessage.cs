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
        Origin = 4,

        /// <summary>
        /// Include the subject summary.
        /// </summary>
        SubjectSummary = 8,

        /// <summary>
        /// Include the comment.
        /// </summary>
        Comment = 16,

        /// <summary>
        /// Include stack trace.
        /// </summary>
        StackTrace = 32,

        /// <summary>
        /// Include the serialization correlations.
        /// </summary>
        LogItemSerialization = 64,

        /// <summary>
        /// Include the default items.
        /// </summary>
        Default = Timestamp | CorrelationTypeAndIds | SubjectSummary | Comment | Origin | StackTrace,

        /// <summary>
        /// Include all items.
        /// </summary>
        All = Timestamp | CorrelationTypeAndIds | Origin | SubjectSummary | Comment | StackTrace | LogItemSerialization,
    }
}
