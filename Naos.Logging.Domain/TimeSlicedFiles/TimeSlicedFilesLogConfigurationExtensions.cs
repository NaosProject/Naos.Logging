// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSlicedFilesLogConfigurationExtensions.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;

    using Spritely.Recipes;

    /// <summary>
    /// Extensions for use with <see cref="TimeSlicedFilesLogConfiguration" />.
    /// </summary>
    public static class TimeSlicedFilesLogConfigurationExtensions
    {
        /// <summary>
        /// Divide a day EQUALLY into slices using the slice size <see cref="TimeSpan" /> provided.
        /// </summary>
        /// <param name="sliceSize">Size of slice.</param>
        /// <returns>List of <see cref="TimeSpan" /> offsets.</returns>
        public static IReadOnlyCollection<TimeSpan> SliceIntoOffsetsPerDay(this TimeSpan sliceSize)
        {
            new { sliceSize }.Must().BeGreaterThanOrEqualTo(TimeSpan.FromMinutes(1)).OrThrowFirstFailure();
            new { sliceSize }.Must().BeLessThanOrEqualTo(TimeSpan.FromDays(1)).OrThrowFirstFailure();

            var dayMinutes = TimeSpan.FromDays(1).TotalMinutes;
            var sliceMinutes = sliceSize.TotalMinutes;
            var sliceCount = dayMinutes / sliceMinutes;
            var sliceCountInteger = (int)sliceCount;
            if (Math.Abs(sliceCountInteger - sliceCount) > 0)
            {
                throw new ArgumentException(FormattableString.Invariant($"Must specify a time slice that can be divided into a day equally; {sliceSize} does not."));
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
        public static Tuple<TimeSpan, TimeSpan> FindOffsetRange(this IReadOnlyCollection<TimeSpan> offsets, DateTime now)
        {
            new { offsets }.Must().NotBeNull().And().NotBeEmptyEnumerable<TimeSpan>().OrThrowFirstFailure();
            now.Kind.Named(FormattableString.Invariant($"{nameof(now)}-Must-Be-Utc-Kind")).Must().BeEqualTo(DateTimeKind.Utc).OrThrowFirstFailure();

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