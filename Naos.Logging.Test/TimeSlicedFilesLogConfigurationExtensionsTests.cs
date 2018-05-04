// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSlicedFilesLogConfigurationExtensionsTests.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using Naos.Logging.Domain;

    using Xunit;

    public static class TimeSlicedFilesLogConfigurationExtensionsTests
    {
        [Fact]
        public static void SliceIntoOffsetsPerDay___LessThanMinuteSliceSize___Throws()
        {
            // Arrange
            Action action = () => TimeSpan.FromMinutes(1).Subtract(TimeSpan.FromMilliseconds(1)).SliceIntoOffsetsPerDay();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
            exception.Message.Should().Be("Specified argument was out of the range of valid values.\r\nParameter name: sliceSize is < 00:01:00");
        }

        [Fact]
        public static void SliceIntoOffsetsPerDay___GreaterThanDaySliceSize___Throws()
        {
            // Arrange
            Action action = () => TimeSpan.FromDays(1).Add(TimeSpan.FromMilliseconds(1)).SliceIntoOffsetsPerDay();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
            exception.Message.Should().Be("Specified argument was out of the range of valid values.\r\nParameter name: sliceSize is > 1.00:00:00");
        }

        [Fact]
        public static void SliceIntoOffsetsPerDay___UnequalDivision___Throws()
        {
            // Arrange
            Action action = () => TimeSpan.FromHours(17).SliceIntoOffsetsPerDay();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();

            exception.Message.Should().Be("Must specify a time slice that can be divided into a day equally; 17:00:00 does not.");
        }

        [Fact]
        public static void SliceIntoOffsetsPerDay___OneMinute___Works()
        {
            // Arrange
            var sliceSize = TimeSpan.FromMinutes(1);
            var minutesInDay = 24 * 60;
            var expectedMaxOffset = TimeSpan.FromDays(1).Subtract(TimeSpan.FromMinutes(1));

            // Act
            var slices = sliceSize.SliceIntoOffsetsPerDay();

            // Assert
            slices.Count.Should().Be(minutesInDay);
            slices.Min().Should().Be(TimeSpan.Zero);
            slices.Max().Should().Be(expectedMaxOffset);
        }

        [Fact]
        public static void SliceIntoOffsetsPerDay___OneDay___Works()
        {
            // Arrange
            var sliceSize = TimeSpan.FromDays(1);

            // Act
            var slices = sliceSize.SliceIntoOffsetsPerDay();

            // Assert
            slices.Count.Should().Be(1);
            slices.Single().Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public static void ComputeFilePath___Works()
        {
            // Arrange
            var rootPath = Path.GetTempPath();
            var config = new TimeSlicedFilesLogConfig(LogItemOrigins.All, rootPath, "Application", TimeSpan.FromMinutes(90));
            var timeOne = new DateTime(2000, 10, 10, 04, 20, 42, DateTimeKind.Utc);
            var expectedOne = Path.Combine(rootPath, "Application--2000-10-10--0300Z-0430Z.log");
            var timeTwo = new DateTime(2010, 10, 10, 16, 20, 42, DateTimeKind.Utc);
            var expectedTwo = Path.Combine(rootPath, "Application--2010-10-10--1500Z-1630Z.log");

            // Act
            var pathOne = config.ComputeFilePath(timeOne);
            var pathTwo = config.ComputeFilePath(timeTwo);

            // Assert
            pathOne.Should().Be(expectedOne);
            pathTwo.Should().Be(expectedTwo);
        }
    }
}