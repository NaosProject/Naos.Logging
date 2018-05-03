// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleTests.cs" company="Naos">
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

    public static class ConsoleTests
    {
        [Fact]
        public static void LogConfigurationConsoleConstructor___Valid___Works()
        {
            // Arrange
            var contextsToLogConsoleOut = LogItemOrigins.ItsLogEntryPostedInformation;
            var contextsToLogConsoleError = LogItemOrigins.AllErrors;

            // Act
            var actual = new ConsoleLogConfig(contextsToLogConsoleOut, contextsToLogConsoleError);

            // Assert
            actual.Should().NotBeNull();
        }

        [Fact]
        public static void LogProcessorConsoleConstructor___Null_config___Throws()
        {
            // Arrange
            Action action = () => new ConsoleLogWriter(null);

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
            using (var consoleOut = new StringWriter())
            {
                using (var consoleError = new StringWriter())
                {
                    // Arrange
                    var infoCanary = A.Dummy<string>();
                    var errorCanary = A.Dummy<string>();
                    var logProcessor = new ConsoleLogWriter(new ConsoleLogConfig(LogItemOrigins.All, LogItemOrigins.AllErrors));
                    Console.SetOut(consoleOut);
                    Console.SetError(consoleError);

                    // Act
                    logProcessor.Log(infoCanary.ToLogEntry().ToLogItem(LogItemOrigin.ItsLogEntryPostedInformation));
                    logProcessor.Log(errorCanary.ToLogEntry().ToLogItem(LogItemOrigin.ItsLogEntryPostedException));

                    var consoleOutOutput = consoleOut.ToString();
                    var consoleErrorOutput = consoleError.ToString();

                    // Assert
                    consoleOutOutput.Should().Contain(infoCanary);
                    consoleOutOutput.Should().Contain(errorCanary);
                    consoleErrorOutput.Should().Contain(errorCanary);
                }
            }
        }

        [Fact]
        public static void RoundtripSerialization___LogConfigurationConsole___Works()
        {
            // Arrange
            var expected = ShareSerializationTestLogic.ConsoleConfig;

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as ConsoleLogConfig;
                actual.Should().NotBeNull();
                actual.Should().Be(expected);
            }

            // Act & Assert
            ShareSerializationTestLogic.ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer);
        }

        [Fact]
        public static void ConsoleConfiguration___EqualityLogic___Should_be_valid___When_different_data()
        {
            // Arrange
            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new ConsoleLogConfig(LogItemOrigins.ItsLogEntryPostedException, LogItemOrigins.ItsLogEntryPostedInformation),
                                                Second = new ConsoleLogConfig(LogItemOrigins.ItsLogEntryPostedException, LogItemOrigins.AppDomainUnhandledException),
                                            },
                                        new
                                            {
                                                First = new ConsoleLogConfig(LogItemOrigins.ItsLogEntryPostedException, LogItemOrigins.ItsLogEntryPostedInformation),
                                                Second = new ConsoleLogConfig(LogItemOrigins.AppDomainUnhandledException, LogItemOrigins.ItsLogEntryPostedInformation),
                                            },
                                        new
                                            {
                                                First = new ConsoleLogConfig(LogItemOrigins.ItsLogEntryPostedException, LogItemOrigins.ItsLogEntryPosted),
                                                Second = new ConsoleLogConfig(LogItemOrigins.ItsLogEntryPostedException, LogItemOrigins.ItsLogEntryPostedInformation),
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
        public static void ConsoleConfiguration___EqualityLogic___Should_be_valid___When_same_data()
        {
            // Arrange
            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new ConsoleLogConfig(LogItemOrigins.ItsLogEntryPosted, LogItemOrigins.AllErrors),
                                                Second = new ConsoleLogConfig(LogItemOrigins.ItsLogEntryPosted, LogItemOrigins.AllErrors),
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