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

    using FakeItEasy;

    using FluentAssertions;

    using Naos.Logging.Domain;
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
                    ""contextsToLog"": ""all"",
                    ""timeSlicePerFile"": ""00:01:00"",
                    ""fileNamePrefix"": ""Log"",
                    ""logFileDirectoryPath"": ""D:\\Logs"",
                    ""createDirectoryStructureIfMissing"": true
                }";

            var serializer = new NaosJsonSerializer();

            // Act
            var result = serializer.Deserialize<LogConfigurationBase>(input);

            // Assert
            var typed = (TimeSlicedFilesLogConfiguration)result;
            typed.LogFileDirectoryPath.Should().Be("D:\\Logs");
            typed.TimeSlicePerFile.Should().Be(TimeSpan.FromMinutes(1));
            typed.FileNamePrefix.Should().Be("Log");
            typed.CreateDirectoryStructureIfMissing.Should().BeTrue();
        }

        [Fact]
        public static void LogConfigurationFileConstructor___Null_prefix___Throws()
        {
            // Arrange
            Action action = () => new TimeSlicedFilesLogConfiguration(LogContexts.All, Path.GetTempPath(), null, TimeSpan.FromMinutes(10));

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: fileNamePrefix");
        }

        [Fact]
        public static void LogConfigurationFileConstructor___Empty_string_prefix___Throws()
        {
            // Arrange
            Action action = () => new TimeSlicedFilesLogConfiguration(LogContexts.All, Path.GetTempPath(), string.Empty, TimeSpan.FromMinutes(10));

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Argument cannot be null or white space.\r\nParameter name: fileNamePrefix");
        }

        [Fact]
        public static void LogConfigurationFileConstructor___WhiteSpace_prefix___Throws()
        {
            // Arrange
            Action action = () => new TimeSlicedFilesLogConfiguration(LogContexts.All, Path.GetTempPath(), '\t'.ToString(), TimeSpan.FromMinutes(10));

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Argument cannot be null or white space.\r\nParameter name: fileNamePrefix");
        }

        [Fact]
        public static void LogConfigurationFileConstructor___Valid___Works()
        {
            // Arrange
            var contextsToLog = LogContexts.EntryPostedInformation;
            var directoryPath = Path.GetTempPath();
            var filePrefix = Guid.NewGuid().ToString();
            var timeSlice = TimeSpan.FromHours(1);

            // Act
            var actual = new TimeSlicedFilesLogConfiguration(contextsToLog, directoryPath, filePrefix, timeSlice);

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
            Action action = () => new FileLogProcessor(null);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: logConfigurationBase");
        }

        [Fact]
        public static void LogProcessor___Valid___Works()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            try
            {
                // Arrange
                var configuration = new TimeSlicedFilesLogConfiguration(LogContexts.AllErrors, tempDirectory, "TestFile", TimeSpan.FromMinutes(1));
                var processor = new TimeSlicedFilesLogProcessor(configuration);

                var infoCanary = A.Dummy<string>();
                var errorCanary = A.Dummy<string>();

                // Act
                processor.Log(LogContexts.EntryPostedInformation, infoCanary);
                processor.Log(LogContexts.EntryPostedException, errorCanary);

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
            var expected = ShareSerializationTestLogic.TimeSlicedFilesLogConfiguration;

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as TimeSlicedFilesLogConfiguration;
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
            var logContexts = LogContexts.EntryPosted;
            var filePrefix = A.Dummy<string>();
            var directoryPath = A.Dummy<string>();
            var timeSlice = TimeSpan.FromMinutes(10);
            var createDirectoryStructureIfMissing = true;
            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new TimeSlicedFilesLogConfiguration(LogContexts.EntryPosted, directoryPath, filePrefix, timeSlice, createDirectoryStructureIfMissing),
                                                Second = new TimeSlicedFilesLogConfiguration(LogContexts.ItsLogInternalErrors, directoryPath, filePrefix, timeSlice, createDirectoryStructureIfMissing),
                                            },
                                        new
                                            {
                                                First = new TimeSlicedFilesLogConfiguration(logContexts, "C:\\Path1", filePrefix, timeSlice, createDirectoryStructureIfMissing),
                                                Second = new TimeSlicedFilesLogConfiguration(logContexts, "C:\\Path2", filePrefix, timeSlice, createDirectoryStructureIfMissing),
                                            },
                                        new
                                            {
                                                First = new TimeSlicedFilesLogConfiguration(logContexts, directoryPath, "Application", timeSlice, createDirectoryStructureIfMissing),
                                                Second = new TimeSlicedFilesLogConfiguration(logContexts, directoryPath, "Database", timeSlice, createDirectoryStructureIfMissing),
                                            },
                                        new
                                            {
                                                First = new TimeSlicedFilesLogConfiguration(logContexts, directoryPath, filePrefix, TimeSpan.FromMinutes(10), createDirectoryStructureIfMissing),
                                                Second = new TimeSlicedFilesLogConfiguration(logContexts, directoryPath, filePrefix, TimeSpan.FromMinutes(30), createDirectoryStructureIfMissing),
                                            },
                                        new
                                            {
                                                First = new TimeSlicedFilesLogConfiguration(logContexts, directoryPath, filePrefix, timeSlice, true),
                                                Second = new TimeSlicedFilesLogConfiguration(logContexts, directoryPath, filePrefix, timeSlice, false),
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
            var logContexts = LogContexts.EntryPosted;
            var filePrefix = A.Dummy<string>();
            var directoryPath = A.Dummy<string>();
            var timeSlice = TimeSpan.FromMinutes(10);
            var createDirectoryStructureIfMissing = true;

            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new TimeSlicedFilesLogConfiguration(logContexts, directoryPath, filePrefix, timeSlice, createDirectoryStructureIfMissing),
                                                Second = new TimeSlicedFilesLogConfiguration(logContexts, directoryPath, filePrefix, timeSlice, createDirectoryStructureIfMissing),
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
    }
}