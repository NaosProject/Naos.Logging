// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerationTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Collections;
    using System.Linq;

    using FluentAssertions;

    using Naos.Logging.Domain;

    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Enum.Recipes;

    using Xunit;

    public static class EnumerationTest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification = "Name/spelling is correct.")]
        [Fact]
        public static void LogItemPropertiesToIncludeInLogMessage_All_should_include_all_flags_other_than_none_and_have_all_bits_on()
        {
            // Arrange & Act
            var all = LogItemPropertiesToIncludeInLogMessage.All;
            var allIndividual = all.GetIndividualFlags().Where(_ => _.GetIndividualFlags().Count == 1).ToList();
            var enumIndividual = new ArrayList(Enum.GetValues(typeof(LogItemPropertiesToIncludeInLogMessage))).Cast<LogItemPropertiesToIncludeInLogMessage>().Where(_ => _ != LogItemPropertiesToIncludeInLogMessage.None).Where(_ => _.GetIndividualFlags().Count == 1).ToList();

            var expectedNumberFromAll = Enumerable.Range(0, allIndividual.Count).Select(_ => Math.Pow(2, _)).Sum();
            var expectedNumberFromEnum = Enumerable.Range(0, enumIndividual.Count).Select(_ => Math.Pow(2, _)).Sum();

            // Assert
            allIndividual.Should().Equal(enumIndividual);
            expectedNumberFromAll.Should().Be(expectedNumberFromEnum);
            ((double)all).Should().Be(expectedNumberFromAll);
        }

        [Fact]
        public static void LogItemOrigin_values_can_cast_to_short_to_use_in_WindowsEventLog()
        {
            // Arrange
            var all = EnumExtensions.GetEnumValues<LogItemOrigin>();

            // Act
            var actual = all.Select(_ => (short)_).ToList();
            var roundtrip = actual.Select(_ => (LogItemOrigin)_).ToList();

            // Assert
            actual.Count.Should().Be(all.Count);
            roundtrip.Count.Should().Be(all.Count);
        }
    }
}
