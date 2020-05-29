// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogReaderBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using Naos.Logging.Domain;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Serialization.Json;

    /// <summary>
    /// Base class for readers.
    /// </summary>
    public abstract class LogReaderBase
    {
        private readonly LogWriterConfigBase logWriterConfigBase;

        private readonly ISerializerFactory serializerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogReaderBase"/> class.
        /// </summary>
        /// <param name="logConfigBase">Base configuration.</param>
        /// <param name="logItemSerializerFactory">Optional serialization factory; DEFAULT is JSON.</param>
        protected LogReaderBase(
            LogWriterConfigBase logConfigBase,
            ISerializerFactory logItemSerializerFactory = null)
        {
            this.logWriterConfigBase = logConfigBase ?? throw new ArgumentNullException(nameof(logConfigBase));
            this.serializerFactory = logItemSerializerFactory ?? new JsonSerializerFactory();
        }

        /// <summary>
        /// Builds the serializer for a log item.
        /// </summary>
        /// <returns>ISerializer.</returns>
        public ISerializer BuildSerializer()
        {
            var result = this.serializerFactory.BuildSerializer(this.logWriterConfigBase.LogItemSerializerRepresentation);
            return result;
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
