// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingJsonSerializationConfiguration.cs" company="Naos Project">
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
    public class LoggingJsonSerializationConfiguration : JsonSerializationConfigurationBase
    {
        /// <inheritdoc />
        public override UnregisteredTypeEncounteredStrategy UnregisteredTypeEncounteredStrategy => UnregisteredTypeEncounteredStrategy.Attempt;

        /// <inheritdoc />
        protected override IReadOnlyCollection<TypeToRegisterForJson> TypesToRegisterForJson => new[]
        {
            typeof(RawSubject).ToTypeToRegisterForJson(),
            typeof(IHaveCorrelationId).ToTypeToRegisterForJson(),
            typeof(LogItem).ToTypeToRegisterForJson(),
            typeof(LogItemContext).ToTypeToRegisterForJson(),
            typeof(Subject).ToTypeToRegisterForJson(),
            typeof(LogWriterConfigBase).ToTypeToRegisterForJson(),
            typeof(LogWritingSettings).ToTypeToRegisterForJson(),
        };
    }
}
