// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogReaderBase.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;

    using Spritely.Recipes;

    /// <summary>
    /// Base class for readers.
    /// </summary>
    public abstract class LogReaderBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Keeping for now to maintain contract of providing it.")]
        private readonly LogWriterConfigBase logConfigurationBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogReaderBase"/> class.
        /// </summary>
        /// <param name="logConfigurationBase">Base configuration.</param>
        protected LogReaderBase(
            LogWriterConfigBase logConfigurationBase)
        {
            new { logConfigurationBase }.Must().NotBeNull().OrThrowFirstFailure();

            this.logConfigurationBase = logConfigurationBase;
        }

        /// <summary>
        /// Reads the log.
        /// </summary>
        /// <returns>Collection of <see cref="LogItem" />'s.</returns>
        public abstract IReadOnlyCollection<LogItem> ReadAll();

        /// <summary>
        /// Reads the log for the specified range.
        /// </summary>
        /// <param name="startUtcInclusive">Start time of the range to read in UTC, inclusive of this time.</param>
        /// <param name="endUtcInclusive">End time of the range to read in UTC, inclusive of this time.</param>
        /// <returns>Collection of <see cref="LogItem" />'s.</returns>
        public abstract IReadOnlyCollection<LogItem> ReadRange(
            DateTime startUtcInclusive,
            DateTime endUtcInclusive);
    }
}