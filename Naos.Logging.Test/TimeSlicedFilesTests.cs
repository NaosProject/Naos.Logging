// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSlicedFilesTests.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using FakeItEasy;

    using FluentAssertions;

    using Naos.Compression.Domain;
    using Naos.Logging.Domain;
    using Naos.Logging.Persistence;
    using Naos.Serialization.Domain.Extensions;
    using Naos.Serialization.Json;

    using Xunit;

    using static System.FormattableString;

    public static class TimeSlicedFilesTests
    {
        [Fact]
        public static void LogConfigurationTimeSliced___Can___Deserialize()
        {
            // Arrange
            var input = @"
                 {
                     ""timeSlicePerFile"": ""00:01:00"",
                     ""fileNamePrefix"": ""Log"",
                     ""logFileDirectoryPath"": ""D:\\Logs"",
                     ""createDirectoryStructureIfMissing"": true,
                     ""logInclusionKindToOriginsMap"": {
                          ""String"": null,
                          ""Object"": null,
                          ""Exception"": null,
                     },
                     ""logItemPropertiesToIncludeInLogMessage"": ""default""
                 }";

            var serializer = new NaosJsonSerializer();

            // Act
            var result = serializer.Deserialize<LogWriterConfigBase>(input);

            // Assert
            var typed = (TimeSlicedFilesLogConfig)result;
            typed.LogFileDirectoryPath.Should().Be("D:\\Logs");
            typed.TimeSlicePerFile.Should().Be(TimeSpan.FromMinutes(1));
            typed.FileNamePrefix.Should().Be("Log");
            typed.CreateDirectoryStructureIfMissing.Should().BeTrue();
            typed.LogInclusionKindToOriginsMapFriendlyString.Should().Be("String=NULL,Object=NULL,Exception=NULL");
        }

        [Fact]
        public static void LogConfigurationFileConstructor___Null_prefix___Throws()
        {
            // Arrange
            Action action = () => new TimeSlicedFilesLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, Path.GetTempPath(), null, TimeSpan.FromMinutes(10));

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("fileNamePrefix is null or white space");
        }

        [Fact]
        public static void LogConfigurationFileConstructor___Empty_string_prefix___Throws()
        {
            // Arrange
            Action action = () => new TimeSlicedFilesLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, Path.GetTempPath(), string.Empty, TimeSpan.FromMinutes(10));

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("fileNamePrefix is null or white space");
        }

        [Fact]
        public static void LogConfigurationFileConstructor___WhiteSpace_prefix___Throws()
        {
            // Arrange
            Action action = () => new TimeSlicedFilesLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, Path.GetTempPath(), '\t'.ToString(), TimeSpan.FromMinutes(10));

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("fileNamePrefix is null or white space");
        }

        [Fact]
        public static void LogConfigurationFileConstructor___Valid___Works()
        {
            // Arrange
            var contextsToLog = LogInclusionKindToOriginsMaps.AnythingFromAnywhere;
            var directoryPath = Path.GetTempPath();
            var filePrefix = Guid.NewGuid().ToString();
            var timeSlice = TimeSpan.FromHours(1);

            // Act
            var actual = new TimeSlicedFilesLogConfig(contextsToLog, directoryPath, filePrefix, timeSlice);

            // Assert
            actual.Should().NotBeNull();
            actual.LogFileDirectoryPath.Should().Be(directoryPath);
            actual.FileNamePrefix.Should().Be(filePrefix);
            actual.TimeSlicePerFile.Should().Be(timeSlice);
            actual.CreateDirectoryStructureIfMissing.Should().BeTrue();
        }

        [Fact]
        public static void LogProcessorFileConstructor___Null_config___Throws()
        {
            // Arrange
            Action action = () => new FileLogWriter(null);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("Value cannot be null.\r\nParameter name: logWriterConfigBase");
        }

        [Fact]
        public static void LogProcessor___Valid___Works()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            try
            {
                // Arrange
                var configuration = new TimeSlicedFilesLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, tempDirectory, "TestFile", TimeSpan.FromMinutes(1));
                var processor = new TimeSlicedFilesLogWriter(configuration);

                var infoCanary = A.Dummy<string>();
                var errorCanary = new ArgumentException(A.Dummy<string>());

                // Act
                processor.Log(infoCanary.ToLogEntry().ToLogItem(LogItemOrigin.ItsLogEntryPosted));
                processor.Log(errorCanary.ToLogEntry().ToLogItem(LogItemOrigin.ItsLogEntryPosted));

                // Assert
                var files = Directory.GetFiles(tempDirectory);
                files.Should().NotBeEmpty();
            }
            finally
            {
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory, true);
                }
            }
        }

        [Fact]
        public static void RoundtripSerialization___LogConfigurationFile___Works()
        {
            // Arrange
            var expected = ShareSerializationTestLogic.TimeSlicedFilesLogConfig;

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as TimeSlicedFilesLogConfig;
                actual.Should().NotBeNull();
                actual.Should().Be(expected);
            }

            // Act & Assert
            ShareSerializationTestLogic.ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer);
        }

        [Fact]
        public static void FileConfiguration___EqualityLogic___Should_be_valid___When_different_data()
        {
            // Arrange
            var logContexts = LogInclusionKindToOriginsMaps.AnythingFromAnywhere;
            var filePrefix = A.Dummy<string>();
            var directoryPath = A.Dummy<string>();
            var timeSlice = TimeSpan.FromMinutes(10);
            var createDirectoryStructureIfMissing = true;
            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new TimeSlicedFilesLogConfig(LogInclusionKindToOriginsMaps.StringAndObjectsFromItsLogEntryPosted, directoryPath, filePrefix, timeSlice, createDirectoryStructureIfMissing),
                                                Second = new TimeSlicedFilesLogConfig(LogInclusionKindToOriginsMaps.ExceptionsFromAnywhere, directoryPath, filePrefix, timeSlice, createDirectoryStructureIfMissing),
                                            },
                                        new
                                            {
                                                First = new TimeSlicedFilesLogConfig(logContexts, "C:\\Path1", filePrefix, timeSlice, createDirectoryStructureIfMissing),
                                                Second = new TimeSlicedFilesLogConfig(logContexts, "C:\\Path2", filePrefix, timeSlice, createDirectoryStructureIfMissing),
                                            },
                                        new
                                            {
                                                First = new TimeSlicedFilesLogConfig(logContexts, directoryPath, "Application", timeSlice, createDirectoryStructureIfMissing),
                                                Second = new TimeSlicedFilesLogConfig(logContexts, directoryPath, "Database", timeSlice, createDirectoryStructureIfMissing),
                                            },
                                        new
                                            {
                                                First = new TimeSlicedFilesLogConfig(logContexts, directoryPath, filePrefix, TimeSpan.FromMinutes(10), createDirectoryStructureIfMissing),
                                                Second = new TimeSlicedFilesLogConfig(logContexts, directoryPath, filePrefix, TimeSpan.FromMinutes(30), createDirectoryStructureIfMissing),
                                            },
                                        new
                                            {
                                                First = new TimeSlicedFilesLogConfig(logContexts, directoryPath, filePrefix, timeSlice, true),
                                                Second = new TimeSlicedFilesLogConfig(logContexts, directoryPath, filePrefix, timeSlice, false),
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
            var logContexts = LogInclusionKindToOriginsMaps.AnythingFromAnywhere;
            var filePrefix = A.Dummy<string>();
            var directoryPath = A.Dummy<string>();
            var timeSlice = TimeSpan.FromMinutes(10);
            var createDirectoryStructureIfMissing = true;

            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new TimeSlicedFilesLogConfig(logContexts, directoryPath, filePrefix, timeSlice, createDirectoryStructureIfMissing),
                                                Second = new TimeSlicedFilesLogConfig(logContexts, directoryPath, filePrefix, timeSlice, createDirectoryStructureIfMissing),
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
        public static void Writer_reader___Roundtrip___Test()
        {
            var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString().ToUpperInvariant());

            try
            {
                // Arrange
                var config = new TimeSlicedFilesLogConfig(
                    LogInclusionKindToOriginsMaps.AnythingFromAnywhere,
                    directory,
                    "TestingTimeSliced",
                    TimeSpan.FromMinutes(1),
                    true,
                    LogItemPropertiesToIncludeInLogMessage.LogItemSerialization);

                var writer = new TimeSlicedFilesLogWriter(config);
                var reader = new TimeSlicedFilesLogReader(config);

                var origin = LogItemOrigin.ItsLogEntryPosted;
                var subjectOne = "Hello";
                var subjectTwo = "Goodbye";

                // Act
                writer.Log(subjectOne.ToLogEntry().ToLogItem(origin));
                writer.Log(subjectTwo.ToLogEntry().ToLogItem(origin));
                var all = reader.ReadAll();

                // Assert
                all.Count.Should().Be(2);
                all.First().Subject.DescribedSerialization.DeserializePayloadUsingSpecificFactory(JsonSerializerFactory.Instance, CompressorFactory.Instance).Should().Be(subjectOne);
                all.Last().Subject.DescribedSerialization.DeserializePayloadUsingSpecificFactory(JsonSerializerFactory.Instance, CompressorFactory.Instance).Should().Be(subjectTwo);
            }
            finally
            {
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                }
            }
        }
    }
}