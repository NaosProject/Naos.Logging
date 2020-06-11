// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingBsonSerializationConfiguration.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Collections.Generic;
    using Naos.Logging.Domain;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Serialization.Bson;

    /// <summary>
    /// Implementation for the <see cref="Naos.Logging" /> domain.
    /// </summary>
    public class LoggingBsonSerializationConfiguration : BsonSerializationConfigurationBase
    {
        /// <inheritdoc />
        protected override IReadOnlyCollection<string> TypeToRegisterNamespacePrefixFilters => new[]
                                                                                               {
                                                                                                   typeof(RawSubject).Namespace,
                                                                                               };
        
        /// <inheritdoc />
        public override UnregisteredTypeEncounteredStrategy UnregisteredTypeEncounteredStrategy => UnregisteredTypeEncounteredStrategy.Attempt;

        /// <inheritdoc />
        protected override IReadOnlyCollection<TypeToRegisterForBson> TypesToRegisterForBson => new[]
        {
            typeof(RawSubject).ToTypeToRegisterForBson(),
            typeof(IHaveCorrelationId).ToTypeToRegisterForBson(),
            typeof(LogItem).ToTypeToRegisterForBson(),
            typeof(LogItemContext).ToTypeToRegisterForBson(),
            typeof(Subject).ToTypeToRegisterForBson(),
            typeof(LogWriterConfigBase).ToTypeToRegisterForBson(),
            typeof(LogWritingSettings).ToTypeToRegisterForBson(),
        };
    }
}
