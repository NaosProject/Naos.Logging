// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShareSerializationTestLogic.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Naos.Logging.Domain;
    using Naos.Logging.Persistence;
    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;

    using static System.FormattableString;

    public static class ShareSerializationTestLogic
    {
        private static readonly NaosBsonSerializer BsonSerializerToUse = new NaosBsonSerializer(typeof(LoggingBsonConfiguration), UnregisteredTypeEncounteredStrategy.Attempt);

        private static readonly NaosJsonSerializer JsonSerializerToUse = new NaosJsonSerializer(typeof(LoggingJsonConfiguration), UnregisteredTypeEncounteredStrategy.Attempt);

        private static readonly IReadOnlyCollection<IStringSerializeAndDeserialize> StringSerializers = new IStringSerializeAndDeserialize[] { BsonSerializerToUse, JsonSerializerToUse }.ToList();

        private static readonly IReadOnlyCollection<IBinarySerializeAndDeserialize> BinarySerializers = new IBinarySerializeAndDeserialize[] { BsonSerializerToUse, JsonSerializerToUse }.ToList();

        internal static readonly FileLogConfig FileConfig = new FileLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, "C:\\Temp\\File.log");

        internal static readonly TimeSlicedFilesLogConfig TimeSlicedFilesLogConfig = new TimeSlicedFilesLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, "C:\\Temp\\", "LogFile", TimeSpan.FromHours(1));

        internal static readonly EventLogConfig EventLogConfig = new EventLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, "Source", "Application", "Localhost", false);

        internal static readonly ConsoleLogConfig ConsoleConfig = new ConsoleLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, LogInclusionKindToOriginsMaps.AnythingFromAnywhere, LogInclusionKindToOriginsMaps.ExceptionsFromAnywhere);

        internal static readonly LogWritingSettings LogWritingSettingsAll = new LogWritingSettings(new LogWriterConfigBase[] { FileConfig, EventLogConfig });

        internal static void ActAndAssertForRoundtripSerialization(object expected, Action<object> throwIfObjectsDiffer)
        {
            var expectedHashCode = expected.GetHashCode();
            foreach (var stringSerializer in StringSerializers)
            {
                var actualString = stringSerializer.SerializeToString(expected);
                var actualObject = stringSerializer.Deserialize(actualString, expected.GetType());
                var actualHashCode = actualObject.GetHashCode();

                try
                {
                    actualHashCode.Should().Be(expectedHashCode);

                    throwIfObjectsDiffer(actualObject);
                }
                catch (Exception ex)
                {
                    throw new NaosSerializationException(Invariant($"Failure with {nameof(stringSerializer)} - {stringSerializer.GetType()}"), ex);
                }
            }

            foreach (var binarySerializer in BinarySerializers)
            {
                var actualBytes = binarySerializer.SerializeToBytes(expected);
                var actualObject = binarySerializer.Deserialize(actualBytes, expected.GetType());
                var actualHashCode = actualObject.GetHashCode();

                try
                {
                    actualHashCode.Should().Be(expectedHashCode);
                    throwIfObjectsDiffer(actualObject);
                }
                catch (Exception ex)
                {
                    throw new NaosSerializationException(Invariant($"Failure with {nameof(binarySerializer)} - {binarySerializer.GetType()}"), ex);
                }
            }
        }
    }
}