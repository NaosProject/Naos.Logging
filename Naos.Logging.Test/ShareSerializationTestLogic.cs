// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShareSerializationTestLogic.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Naos.Logging.Domain;
    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;

    using static System.FormattableString;

    public static class ShareSerializationTestLogic
    {
        private static readonly NaosBsonSerializer BsonSerializerToUse = new NaosBsonSerializer();

        private static readonly NaosJsonSerializer JsonSerializerToUse = new NaosJsonSerializer();

        private static readonly IReadOnlyCollection<IStringSerializeAndDeserialize> StringSerializers = new IStringSerializeAndDeserialize[] { BsonSerializerToUse, JsonSerializerToUse }.ToList();

        private static readonly IReadOnlyCollection<IBinarySerializeAndDeserialize> BinarySerializers = new IBinarySerializeAndDeserialize[] { BsonSerializerToUse, JsonSerializerToUse }.ToList();

        internal static readonly LogConfigurationFile FileConfiguration = new LogConfigurationFile(LogContexts.All, "C:\\Temp\\File.log");

        internal static readonly LogConfigurationEventLog EventLogConfiguration = new LogConfigurationEventLog(LogContexts.All, "Source", false, "Application", "Localhost");

        internal static readonly LogConfigurationConsole ConsoleConfiguration = new LogConfigurationConsole(LogContexts.All, LogContexts.AllErrors);

        internal static readonly LogProcessorSettings LogProcessorSettingsAll = new LogProcessorSettings(new LogConfigurationBase[] { FileConfiguration, EventLogConfiguration });

        internal static void ActAndAssertForRoundtripSerialization(object expected, Action<object> throwIfObjectsDiffer)
        {
            foreach (var stringSerializer in StringSerializers)
            {
                var actualString = stringSerializer.SerializeToString(expected);
                var actualObject = stringSerializer.Deserialize(actualString, expected.GetType());

                try
                {
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

                try
                {
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