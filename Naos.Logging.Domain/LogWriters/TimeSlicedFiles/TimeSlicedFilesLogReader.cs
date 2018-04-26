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
    /// <see cref="_timeSlicedFilesLogConfig"/> focused implementation of <see cref="LogReaderBase" />.
    /// </summary>
    public class TimeSlicedFilesLogReader : LogReaderBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Will use when implementing ReadRange.")]
        private readonly TimeSlicedFilesLogConfig _timeSlicedFilesLogConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSlicedFilesLogReader"/> class.
        /// </summary>
        /// <param name="timeSlicedFilesLogConfig">Configuration.</param>
        public TimeSlicedFilesLogReader(TimeSlicedFilesLogConfig timeSlicedFilesLogConfig)
            : base(timeSlicedFilesLogConfig)
        {
            this._timeSlicedFilesLogConfig = timeSlicedFilesLogConfig;
        }

        /// <inheritdoc cref="LogReaderBase" />
        public override IReadOnlyCollection<LogItem> ReadAll()
        {
            return this.ReadRange(DateTime.MinValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime());
        }

        /// <inheritdoc cref="LogReaderBase" />
        public override IReadOnlyCollection<LogItem> ReadRange(DateTime startUtc, DateTime endUtc)
        {
            throw new NotImplementedException();
        }
    }
}