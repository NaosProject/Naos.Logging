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
            var contextsToLogConsoleOut = LogContexts.EntryPostedInformation;
            var contextsToLogConsoleError = LogContexts.AllErrors;

            // Act
            var actual = new ConsoleLogConfiguration(contextsToLogConsoleOut, contextsToLogConsoleError);

            // Assert
            actual.Should().NotBeNull();
        }

        [Fact]
        public static void LogProcessorConsoleConstructor___Null_config___Throws()
        {
            // Arrange
            Action action = () => new ConsoleLogProcessor(null);

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
            using (var consoleOut = new StringWriter())
            {
                using (var consoleError = new StringWriter())
                {
                    // Arrange
                    var infoCanary = A.Dummy<string>();
                    var errorCanary = A.Dummy<string>();
                    var logProcessor = new ConsoleLogProcessor(new ConsoleLogConfiguration(LogContexts.All, LogContexts.AllErrors));
                    Console.SetOut(consoleOut);
                    Console.SetError(consoleError);

                    // Act
                    logProcessor.Log(LogContexts.EntryPostedInformation, infoCanary);
                    logProcessor.Log(LogContexts.EntryPostedException, errorCanary);

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
            var expected = ShareSerializationTestLogic.ConsoleConfiguration;

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as ConsoleLogConfiguration;
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
                                                First = new ConsoleLogConfiguration(LogContexts.EntryPostedException, LogContexts.EntryPostedInformation),
                                                Second = new ConsoleLogConfiguration(LogContexts.EntryPostedException, LogContexts.AppDomainUnhandledException),
                                            },
                                        new
                                            {
                                                First = new ConsoleLogConfiguration(LogContexts.EntryPostedException, LogContexts.EntryPostedInformation),
                                                Second = new ConsoleLogConfiguration(LogContexts.AppDomainUnhandledException, LogContexts.EntryPostedInformation),
                                            },
                                        new
                                            {
                                                First = new ConsoleLogConfiguration(LogContexts.EntryPostedException, LogContexts.EntryPosted),
                                                Second = new ConsoleLogConfiguration(LogContexts.EntryPostedException, LogContexts.EntryPostedInformation),
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
                                                First = new ConsoleLogConfiguration(LogContexts.EntryPosted, LogContexts.AllErrors),
                                                Second = new ConsoleLogConfiguration(LogContexts.EntryPosted, LogContexts.AllErrors),
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