// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileTests.cs" company="Naos">
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

    using Xunit;

    using static System.FormattableString;

    public static class FileTests
    {
        [Fact]
        public static void LogConfigurationFileConstructor___Null_path___Throws()
        {
            // Arrange
            Action action = () => new FileLogConfiguration(LogContexts.All, null);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: logFilePath");
        }

        [Fact]
        public static void LogConfigurationFileConstructor___Empty_string_path___Throws()
        {
            // Arrange
            Action action = () => new FileLogConfiguration(LogContexts.All, string.Empty);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Argument cannot be null or white space.\r\nParameter name: logFilePath");
        }

        [Fact]
        public static void LogConfigurationFileConstructor___WhiteSpace_path___Throws()
        {
            // Arrange
            Action action = () => new FileLogConfiguration(LogContexts.All, '\t'.ToString());

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Argument cannot be null or white space.\r\nParameter name: logFilePath");
        }

        [Fact]
        public static void LogConfigurationFileConstructor___Valid___Works()
        {
            // Arrange
            var contextsToLog = LogContexts.EntryPostedInformation;
            var filePath = Path.GetTempFileName();

            // Act
            var actual = new FileLogConfiguration(contextsToLog, filePath);

            // Assert
            actual.Should().NotBeNull();
            actual.LogFilePath.Should().Be(filePath);
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
            var tempFileName = Path.GetTempFileName();

            try
            {
                // Arrange
                var configuration = new FileLogConfiguration(LogContexts.AllErrors, tempFileName);
                var processor = new FileLogProcessor(configuration);

                var infoCanary = A.Dummy<string>();
                var errorCanary = A.Dummy<string>();

                // Act
                processor.Log(LogContexts.EntryPostedInformation, infoCanary);
                processor.Log(LogContexts.EntryPostedException, errorCanary);

                // Assert
                var fileContents = File.ReadAllText(tempFileName);
                fileContents.Should().NotContain(infoCanary);
                fileContents.Should().Contain(errorCanary);
            }
            finally
            {
                if (File.Exists(tempFileName))
                {
                    File.Delete(tempFileName);
                }
            }
        }

        [Fact]
        public static void RoundtripSerialization___LogConfigurationFile___Works()
        {
            // Arrange
            var expected = ShareSerializationTestLogic.FileConfiguration;

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as FileLogConfiguration;
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
            var logFilePath = A.Dummy<string>();
            var createDirectoryStructureIfMissing = true;
            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new FileLogConfiguration(LogContexts.EntryPosted, logFilePath, createDirectoryStructureIfMissing),
                                                Second = new FileLogConfiguration(LogContexts.ItsLogInternalErrors, logFilePath, createDirectoryStructureIfMissing),
                                            },
                                        new
                                            {
                                                First = new FileLogConfiguration(logContexts, "C:\\Path1", createDirectoryStructureIfMissing),
                                                Second = new FileLogConfiguration(logContexts, "C:\\Path2", createDirectoryStructureIfMissing),
                                            },
                                        new
                                            {
                                                First = new FileLogConfiguration(logContexts, logFilePath, true),
                                                Second = new FileLogConfiguration(logContexts, logFilePath, false),
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
                                                First = new FileLogConfiguration(logContexts, logFilePath, createDirectoryStructureIfMissing),
                                                Second = new FileLogConfiguration(logContexts, logFilePath, createDirectoryStructureIfMissing),
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