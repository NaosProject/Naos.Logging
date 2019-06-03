// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSlicingExtensions.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using static System.FormattableString;

    /// <summary>
    /// Extension methods for time slicing.
    /// </summary>
    public static class TimeSlicingExtensions
    {
        /// <summary>
        /// Divide a day EQUALLY into slices using the slice size <see cref="TimeSpan" /> provided.
        /// </summary>
        /// <param name="sliceSize">Size of slice.</param>
        /// <returns>List of <see cref="TimeSpan" /> offsets.</returns>
        public static IReadOnlyCollection<TimeSpan> SliceIntoOffsetsPerDay(
            this TimeSpan sliceSize)
        {
            var oneMinute = TimeSpan.FromMinutes(1);
            if (sliceSize < oneMinute)
            {
                throw new ArgumentOutOfRangeException(Invariant($"{nameof(sliceSize)} is < {oneMinute}"));
            }

            var oneDay = TimeSpan.FromDays(1);
            if (sliceSize > oneDay)
            {
                throw new ArgumentOutOfRangeException(Invariant($"{nameof(sliceSize)} is > {oneDay}"));
            }

            var dayMinutes = TimeSpan.FromDays(1).TotalMinutes;
            var sliceMinutes = sliceSize.TotalMinutes;
            var sliceCount = dayMinutes / sliceMinutes;
            var sliceCountInteger = (int)sliceCount;
            if (Math.Abs(sliceCountInteger - sliceCount) > 0)
            {
                throw new ArgumentException(Invariant($"Must specify a time slice that can be divided into a day equally; {sliceSize} does not."));
            }

            var timeSpan = TimeSpan.Zero;
            var ret = new List<TimeSpan>();
            for (var idx = 0; idx < sliceCount; idx++)
            {
                ret.Add(timeSpan);
                timeSpan = timeSpan.Add(sliceSize);
            }

            return ret;
        }

        /// <summary>
        /// Find the current offset to use when logging.
        /// </summary>
        /// <param name="offsets">Offsets to search within.</param>
        /// <param name="now">Time to search for.</param>
        /// <returns>Correct offset.</returns>
        public static Tuple<TimeSpan, TimeSpan> FindOffsetRange(
            this IReadOnlyCollection<TimeSpan> offsets,
            DateTime now)
        {
            if (offsets == null)
            {
                throw new ArgumentNullException(nameof(offsets));
            }

            if (!offsets.Any())
            {
                throw new ArgumentException(Invariant($"{nameof(offsets)} is empty"));
            }

            if (now.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(Invariant($"{nameof(now)}.{nameof(DateTime.Kind)} != {nameof(DateTimeKind)}.{nameof(DateTimeKind.Utc)}"));
            }

            var dayStart = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
            var nowOffset = now - dayStart;
            var prior = default(TimeSpan);
            var next = default(TimeSpan);
            foreach (var offset in offsets)
            {
                // bump the offset range
                prior = next;
                next = offset;

                if (nowOffset < offset)
                {
                    // we've passed the correct offset (stored in ret)
                    break;
                }
            }

            return new Tuple<TimeSpan, TimeSpan>(prior, next);
        }
    }
}