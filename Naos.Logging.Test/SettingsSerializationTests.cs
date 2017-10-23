// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsSerializationTests.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FakeItEasy;

    using FluentAssertions;

    using Naos.Logging.Domain;
    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;

    using Xunit;

    using static System.FormattableString;

    public static class SettingsSerializationTests
    {
        private static readonly NaosBsonSerializer BsonSerializerToUse = new NaosBsonSerializer();

        private static readonly NaosJsonSerializer JsonSerializerToUse = new NaosJsonSerializer();

        private static readonly IReadOnlyCollection<IStringSerializeAndDeserialize> StringSerializers = new IStringSerializeAndDeserialize[] { BsonSerializerToUse, JsonSerializerToUse }.ToList();

        private static readonly IReadOnlyCollection<IBinarySerializeAndDeserialize> BinarySerializers = new IBinarySerializeAndDeserialize[] { BsonSerializerToUse, JsonSerializerToUse }.ToList();

        private static readonly LogConfigurationFile FileConfiguration = new LogConfigurationFile(LogContexts.All, "C:\\Temp\\File.log");

        private static readonly LogConfigurationEventLog EventLogConfiguration = new LogConfigurationEventLog(LogContexts.All, "Application", "Localhost", "Source");

        private static readonly LogProcessorSettings LogProcessorSettingsAll = new LogProcessorSettings(new LogConfigurationBase[] { FileConfiguration, EventLogConfiguration });

        [Fact]
        public static void RoundtripSerialization___LogProcessorSettings___Works()
        {
            // Arrange
            var expected = LogProcessorSettingsAll;

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as LogProcessorSettings;
                actual.Should().NotBeNull();
                actual.Configurations.Should().BeEquivalentTo(expected.Configurations);
            }

            // Act & Assert
            ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer);
        }

        [Fact]
        public static void RoundtripSerialization___LogConfigurationFile___Works()
        {
            // Arrange
            var expected = FileConfiguration;

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as LogConfigurationFile;
                actual.Should().NotBeNull();
                actual.Should().Be(expected);
            }

            // Act & Assert
            ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer);
        }

        [Fact]
        public static void RoundtripSerialization___LogConfigurationEventLog___Works()
        {
            // Arrange
            var expected = EventLogConfiguration;

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as LogConfigurationEventLog;
                actual.Should().NotBeNull();
                actual.Should().Be(expected);
            }

            // Act & Assert
            ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer);
        }

        [Fact]
        public static void FileConfiguration___EqualityLogic___Should_be_valid___When_different_data()
        {
            // Arrange
            var logContexts = LogContexts.EntryPosted;
            var logFilePath = A.Dummy<string>();
            var createDirectoryStructureIfMissing = true;
            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new LogConfigurationFile(LogContexts.EntryPosted, logFilePath, createDirectoryStructureIfMissing),
                                                Second = new LogConfigurationFile(LogContexts.ItsLogInternalErrors, logFilePath, createDirectoryStructureIfMissing),
                                            },
                                        new
                                            {
                                                First = new LogConfigurationFile(logContexts, "C:\\Path1", createDirectoryStructureIfMissing),
                                                Second = new LogConfigurationFile(logContexts, "C:\\Path2", createDirectoryStructureIfMissing),
                                            },
                                        new
                                            {
                                                First = new LogConfigurationFile(logContexts, logFilePath, true),
                                                Second = new LogConfigurationFile(logContexts, logFilePath, false),
                                            },
                                    }.ToList();

            // Act & Assert
            notEqualTests.ForEach(
                _ =>
                    {
                        if (_.First != null && _.Second != null)
                        {
                            (_.First.GetHashCode() == _.Second.GetHashCode()).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                            _.First.Equals(_.Second).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                            _.First.Equals((object)_.Second).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                        }

                        (_.First == _.Second).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                        (_.First != _.Second).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                    });
        }

        [Fact]
        public static void FileConfiguration___EqualityLogic___Should_be_valid___When_same_data()
        {
            // Arrange
            var logContexts = LogContexts.All;
            var logFilePath = A.Dummy<string>();
            var createDirectoryStructureIfMissing = true;

            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new LogConfigurationFile(logContexts, logFilePath, createDirectoryStructureIfMissing),
                                                Second = new LogConfigurationFile(logContexts, logFilePath, createDirectoryStructureIfMissing),
                                            },
                                    }.ToList();

            // Act & Assert
            notEqualTests.ForEach(
                _ =>
                    {
                        if (_.First != null && _.Second != null)
                        {
                            _.First.Equals(_.Second).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                            _.First.Equals((object)_.Second).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                            (_.First.GetHashCode() == _.Second.GetHashCode()).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                        }

                        (_.First == _.Second).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                        (_.First != _.Second).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                    });
        }

        [Fact]
        public static void EventLogConfiguration___EqualityLogic___Should_be_valid___When_different_data()
        {
            // Arrange
            var logContexts = LogContexts.EntryPosted;
            var logName = A.Dummy<string>();
            var machineName = A.Dummy<string>();
            var source = A.Dummy<string>();

            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new LogConfigurationEventLog(LogContexts.EntryPosted, logName, machineName, source),
                                                Second = new LogConfigurationEventLog(LogContexts.ItsLogInternalErrors, logName, machineName, source),
                                            },
                                        new
                                            {
                                                First = new LogConfigurationEventLog(logContexts, "Log1", machineName, source),
                                                Second = new LogConfigurationEventLog(logContexts, "Log2", machineName, source),
                                            },
                                        new
                                            {
                                                First = new LogConfigurationEventLog(logContexts, logName, "Machine1", source),
                                                Second = new LogConfigurationEventLog(logContexts, logName, "Machine2", source),
                                            },
                                        new
                                            {
                                                First = new LogConfigurationEventLog(logContexts, logName, machineName, "Source1"),
                                                Second = new LogConfigurationEventLog(logContexts, logName, machineName, "Source2"),
                                            },
                                    }.ToList();

            // Act & Assert
            notEqualTests.ForEach(
                _ =>
                    {
                        if (_.First != null && _.Second != null)
                        {
                            (_.First.GetHashCode() == _.Second.GetHashCode()).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                            _.First.Equals(_.Second).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                            _.First.Equals((object)_.Second).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                        }

                        (_.First == _.Second).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                        (_.First != _.Second).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                    });
        }

        [Fact]
        public static void EventLogConfiguration___EqualityLogic___Should_be_valid___When_same_data()
        {
            // Arrange
            var logContexts = LogContexts.All;
            var logName = A.Dummy<string>();
            var machineName = A.Dummy<string>();
            var source = A.Dummy<string>();

            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new LogConfigurationEventLog(logContexts, logName, machineName, source),
                                                Second = new LogConfigurationEventLog(logContexts, logName, machineName, source),
                                            },
                                    }.ToList();

            // Act & Assert
            notEqualTests.ForEach(
                _ =>
                    {
                        if (_.First != null && _.Second != null)
                        {
                            _.First.Equals(_.Second).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                            _.First.Equals((object)_.Second).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                            (_.First.GetHashCode() == _.Second.GetHashCode()).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                        }

                        (_.First == _.Second).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                        (_.First != _.Second).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                    });
        }

        private static void ActAndAssertForRoundtripSerialization(object expected, Action<object> throwIfObjectsDiffer)
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