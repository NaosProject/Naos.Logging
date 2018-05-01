// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogProcessorTests.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Linq;

    using FluentAssertions;

    using Naos.Logging.Domain;

    using Xunit;

    using static System.FormattableString;

    public static class LogProcessorTests
    {
        [Fact]
        public static void Constructor___Valid___Works()
        {
            // Arrange
            var configurations = new LogWriterConfigBase[] { ShareSerializationTestLogic.FileConfig, ShareSerializationTestLogic.EventLogConfig, ShareSerializationTestLogic.ConsoleConfig }.ToList();

            // Act
            var actual = new LogWritingSettings(configurations);

            // Assert
            actual.Should().NotBeNull();
            actual.Configs.Should().BeEquivalentTo(configurations);
        }

        [Fact]
        public static void Constructor___Null_configuration___Empty_set()
        {
            // Arrange
            LogWriterConfigBase[] configurations = null;

            // Act
            var actual = new LogWritingSettings(configurations);

            // Assert
            actual.Should().NotBeNull();
            actual.Configs.Should().NotBeNull();
            actual.Configs.Should().BeEmpty();
        }

        [Fact]
        public static void RoundtripSerialization___LogProcessorSettings___Works()
        {
            // Arrange
            var expected = ShareSerializationTestLogic.LogWritingSettingsAll;

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as LogWritingSettings;
                actual.Should().NotBeNull();
                actual.Configs.Should().BeEquivalentTo(expected.Configs);
            }

            // Act & Assert
            ShareSerializationTestLogic.ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer);
        }
    }
}