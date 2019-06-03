// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingBsonConfiguration.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Collections.Generic;
    using Naos.Logging.Domain;
    using Naos.Serialization.Bson;

    /// <summary>
    /// Implementation for the <see cref="Naos.Logging" /> domain.
    /// </summary>
    public class LoggingBsonConfiguration : BsonConfigurationBase
    {
        /// <inheritdoc />
        protected override IReadOnlyCollection<Type> TypesToAutoRegister => new[]
        {
            typeof(RawSubject),
            typeof(IHaveCorrelationId),
            typeof(LogItem),
            typeof(LogItemContext),
            typeof(Subject),
            typeof(LogWriterConfigBase),
            typeof(LogWritingSettings),
        };
    }
}