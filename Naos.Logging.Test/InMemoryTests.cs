// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryTests.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Linq;

    using FakeItEasy;

    using FluentAssertions;

    using Naos.Logging.Domain;

    using Xunit;

    using static System.FormattableString;

    public static class InMemoryTests
    {
        [Fact]
        public static void LogConfigurationInMemoryConstructor___Less_than_NegativeOne_MaxLoggedItemCount___Throws()
        {
            // Arrange
            Action action = () => new InMemoryLogConfig(LogItemOrigins.All, -2);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
            exception.Message.Should().Be("Value must be greater than or equal to -1.\r\nParameter name: maxLoggedItemCount\r\nActual value was -2.");
        }

        [Fact]
        public static void LogProcessorInMemoryConstructor___Null_config___Throws()
        {
            // Arrange
            Action action = () => new InMemoryLogWriter(null);

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
            // Arrange
            var configuration = new InMemoryLogConfig(LogItemOrigins.All);
            var processor = new InMemoryLogWriter(configuration);

            var infoCanary = A.Dummy<string>();
            var errorCanary = A.Dummy<string>();

            // Act
            processor.Log(LogItemOrigins.EntryPostedInformation, infoCanary);
            processor.Log(LogItemOrigins.EntryPostedException, errorCanary);

            // Assert
            processor.LoggedItems.Count.Should().Be(2);
            processor.LoggedItems.Single(_ => _.Context == LogItemOrigins.EntryPostedInformation).Message.Should().Contain("Message = " + infoCanary);
            processor.LoggedItems.Single(_ => _.Context == LogItemOrigins.EntryPostedInformation).Message.Should().Contain("Subject = " + infoCanary);
            processor.LoggedItems.Single(_ => _.Context == LogItemOrigins.EntryPostedException).Message.Should().Contain("Message = " + errorCanary);
            processor.LoggedItems.Single(_ => _.Context == LogItemOrigins.EntryPostedException).Message.Should().Contain("Subject = " + errorCanary);
        }

        [Fact]
        public static void LogProcessor___Default_max_elements___Honored()
        {
            // Arrange
            var configuration = new InMemoryLogConfig(LogItemOrigins.All);
            var processor = new InMemoryLogWriter(configuration);
            var logCallCount = 1000;

            // Act
            Enumerable.Range(0, logCallCount).ToList().ForEach(_ => processor.Log(LogItemOrigins.EntryPostedInformation, "Hello"));

            // Assert
            processor.LoggedItems.Count.Should().Be(logCallCount);
        }

        [Fact]
        public static void LogProcessor___0_max_elements___Honored()
        {
            // Arrange
            var maxLoggedItemCount = 0;
            var configuration = new InMemoryLogConfig(LogItemOrigins.All, maxLoggedItemCount);
            var processor = new InMemoryLogWriter(configuration);
            var logCallCount = 1000;

            // Act
            Enumerable.Range(0, logCallCount).ToList().ForEach(_ => processor.Log(LogItemOrigins.EntryPostedInformation, "Hello"));

            // Assert
            processor.LoggedItems.Count.Should().Be(maxLoggedItemCount);
        }

        [Fact]
        public static void LogProcessor___1_max_elements___Honored()
        {
            // Arrange
            var maxLoggedItemCount = 1;
            var configuration = new InMemoryLogConfig(LogItemOrigins.All, maxLoggedItemCount);
            var processor = new InMemoryLogWriter(configuration);
            var logCallCount = 1000;

            // Act
            Enumerable.Range(0, logCallCount).ToList().ForEach(_ => processor.Log(LogItemOrigins.EntryPostedInformation, "Hello"));

            // Assert
            processor.LoggedItems.Count.Should().Be(maxLoggedItemCount);
        }

        [Fact]
        public static void LogProcessor___2_max_elements___Honored()
        {
            // Arrange
            var maxLoggedItemCount = 2;
            var configuration = new InMemoryLogConfig(LogItemOrigins.All, maxLoggedItemCount);
            var processor = new InMemoryLogWriter(configuration);
            var logCallCount = 1000;

            // Act
            Enumerable.Range(0, logCallCount).ToList().ForEach(_ => processor.Log(LogItemOrigins.EntryPostedInformation, "Hello"));

            // Assert
            processor.LoggedItems.Count.Should().Be(maxLoggedItemCount);
        }

        [Fact]
        public static void LogProcessor___Purge___Works()
        {
            // Arrange
            var configuration = new InMemoryLogConfig(LogItemOrigins.All);
            var processor = new InMemoryLogWriter(configuration);
            var logCallCount = 10;
            Enumerable.Range(0, logCallCount).ToList().ForEach(_ => processor.Log(LogItemOrigins.EntryPostedInformation, "Hello"));

            // Act
            processor.PurgeAllLoggedItems();

            // Assert
            processor.LoggedItems.Count.Should().Be(0);
        }

        [Fact]
        public static void RoundtripSerialization___LogConfigurationInMemory___Works()
        {
            // Arrange
            var expected = new InMemoryLogConfig(LogItemOrigins.All);

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as InMemoryLogConfig;
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
            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new InMemoryLogConfig(LogItemOrigins.EntryPosted, 1),
                                                Second = new InMemoryLogConfig(LogItemOrigins.ItsLogInternalErrors, 1),
                                            },
                                        new
                                            {
                                                First = new InMemoryLogConfig(LogItemOrigins.EntryPosted, 1),
                                                Second = new InMemoryLogConfig(LogItemOrigins.EntryPosted, 0),
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
            var logContexts = LogItemOrigins.All;

            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new InMemoryLogConfig(logContexts, 1),
                                                Second = new InMemoryLogConfig(logContexts, 1),
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