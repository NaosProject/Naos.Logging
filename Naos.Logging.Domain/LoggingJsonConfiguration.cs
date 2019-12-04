// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingJsonConfiguration.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using Naos.Logging.Domain;
    using OBeautifulCode.Serialization.Json;

    /// <summary>
    /// Base class for readers.
    /// </summary>
    public class LoggingJsonConfiguration : JsonConfigurationBase
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
