// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItemPropertiesToIncludeInLogMessage.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
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
        /// Kind of the entry.
        /// </summary>
        Kind = 8,

        /// <summary>
        /// Include the subject summary.
        /// </summary>
        SubjectSummary = 16,

        /// <summary>
        /// Include the comment.
        /// </summary>
        Comment = 32,

        /// <summary>
        /// Include stack trace.
        /// </summary>
        StackTrace = 64,

        /// <summary>
        /// Include the serialization correlations.
        /// </summary>
        LogItemSerialization = 128,

        /// <summary>
        /// Include the default items.
        /// </summary>
        Default = Timestamp | CorrelationTypeAndIds | SubjectSummary | Comment | Origin | Kind | StackTrace,

        /// <summary>
        /// Include all items.
        /// </summary>
        All = Timestamp | CorrelationTypeAndIds | Origin | Kind | SubjectSummary | Comment | StackTrace | LogItemSerialization,
    }
}
