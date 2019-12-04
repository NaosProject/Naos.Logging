// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleTests.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using FakeItEasy;

    using FluentAssertions;

    using Naos.Logging.Domain;
    using Naos.Logging.Persistence;
    using Xunit;

    using static System.FormattableString;

    public static class ConsoleTests
    {
        [Fact]
        public static void LogConfigurationConsoleConstructor___Valid___Works()
        {
            // Arrange & Act
            var actual = new ConsoleLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, LogInclusionKindToOriginsMaps.StringAndObjectsFromItsLogEntryPosted, LogInclusionKindToOriginsMaps.ExceptionsFromAnywhere);

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
            using (var consoleOut = new StringWriter(CultureInfo.CurrentCulture))
            {
                using (var consoleError = new StringWriter(CultureInfo.CurrentCulture))
                {
                    // Arrange
                    var infoCanary = A.Dummy<string>();
                    var errorCanary = new ArgumentException(A.Dummy<string>());
                    var logProcessor = new ConsoleLogWriter(new ConsoleLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, LogInclusionKindToOriginsMaps.StringAndObjectsFromItsLogEntryPosted, LogInclusionKindToOriginsMaps.ExceptionsFromAnywhere));
                    Console.SetOut(consoleOut);
                    Console.SetError(consoleError);

                    // Act
                    logProcessor.Log(infoCanary.ToLogEntry().ToLogItem(LogItemOrigin.ItsLogEntryPosted));
                    logProcessor.Log(errorCanary.ToLogEntry().ToLogItem(LogItemOrigin.ItsLogEntryPosted));

                    var consoleOutOutput = consoleOut.ToString();
                    var consoleErrorOutput = consoleError.ToString();

                    // Assert
                    consoleOutOutput.Should().Contain(infoCanary);
                    consoleOutOutput.Should().NotContain(errorCanary.Message);
                    consoleErrorOutput.Should().Contain(errorCanary.Message);
                    consoleErrorOutput.Should().NotContain(infoCanary);
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
            var logInclusionKindToOriginsMapForConsoleOut = LogInclusionKindToOriginsMaps.StringAndObjectsFromItsLogEntryPosted;
            var logInclusionKindToOriginsMapForConsoleError = LogInclusionKindToOriginsMaps.ExceptionsFromAnywhere;
            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new ConsoleLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, logInclusionKindToOriginsMapForConsoleOut, LogInclusionKindToOriginsMaps.TelemetryFromAnywhere),
                                                Second = new ConsoleLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, logInclusionKindToOriginsMapForConsoleOut, LogInclusionKindToOriginsMaps.TelemetryFromNowhere),
                                            },
                                        new
                                            {
                                                First = new ConsoleLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, LogInclusionKindToOriginsMaps.TelemetryFromAnywhere, logInclusionKindToOriginsMapForConsoleOut),
                                                Second = new ConsoleLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, LogInclusionKindToOriginsMaps.TelemetryFromNowhere, logInclusionKindToOriginsMapForConsoleOut),
                                            },
                                        new
                                            {
                                                First = new ConsoleLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, logInclusionKindToOriginsMapForConsoleOut, LogInclusionKindToOriginsMaps.TelemetryFromAnywhere),
                                                Second = new ConsoleLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, LogInclusionKindToOriginsMaps.TelemetryFromNowhere, logInclusionKindToOriginsMapForConsoleError),
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
                                                First = new ConsoleLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, LogInclusionKindToOriginsMaps.TelemetryFromNowhere, LogInclusionKindToOriginsMaps.ExceptionsFromAnywhere),
                                                Second = new ConsoleLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, LogInclusionKindToOriginsMaps.TelemetryFromNowhere, LogInclusionKindToOriginsMaps.ExceptionsFromAnywhere),
                                            },
                                        new
                                            {
                                                First = new ConsoleLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, LogInclusionKindToOriginsMaps.ExceptionsFromAnywhere, LogInclusionKindToOriginsMaps.TelemetryFromAnywhere),
                                                Second = new ConsoleLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, LogInclusionKindToOriginsMaps.ExceptionsFromAnywhere, LogInclusionKindToOriginsMaps.TelemetryFromAnywhere),
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
