// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSlicedFilesLogReader.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// <see cref="timeSlicedFilesLogConfiguration"/> focused implementation of <see cref="LogReaderBase" />.
    /// </summary>
    public class TimeSlicedFilesLogReader : LogReaderBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Will use when implementing ReadRange.")]
        private readonly TimeSlicedFilesLogConfiguration timeSlicedFilesLogConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSlicedFilesLogReader"/> class.
        /// </summary>
        /// <param name="timeSlicedFilesLogConfiguration">Configuration.</param>
        public TimeSlicedFilesLogReader(TimeSlicedFilesLogConfiguration timeSlicedFilesLogConfiguration)
            : base(timeSlicedFilesLogConfiguration)
        {
            this.timeSlicedFilesLogConfiguration = timeSlicedFilesLogConfiguration;
        }

        /// <inheritdoc cref="LogReaderBase" />
        public override IReadOnlyCollection<LogMessage> ReadAll()
        {
            return this.ReadRange(DateTime.MinValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime());
        }

        /// <inheritdoc cref="LogReaderBase" />
        public override IReadOnlyCollection<LogMessage> ReadRange(DateTime startUtc, DateTime endUtc)
        {
            throw new NotImplementedException();
        }
    }
}