// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerationTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
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
        [Fact]
        public static void LogItemOrigin_and_LogItemOrigins_should_have_the_same_set_of_named_members_excluding_LogItemOrigin_Unknown_and_LogItemOrigins_None()
        {
            // Arrange
            var logItemOriginsMembers = EnumExtensions.GetEnumValues<LogItemOrigins>().Where(_ => ((Enum)_).GetIndividualFlags().Count == 1).Select(_ => _.ToString()).Where(_ => _ != nameof(LogItemOrigins.None)).ToList();
            var logItemOriginMembers = EnumExtensions.GetEnumValues<LogItemOrigin>().Where(_ => ((Enum)_).GetIndividualFlags().Count == 1 ? true : throw new InvalidOperationException()).Select(_ => _.ToString()).Where(_ => _ != nameof(LogItemOrigin.Unknown)).ToList();

            // Act
            var actual = logItemOriginsMembers.SymmetricDifference(logItemOriginMembers);

            // Assert
            actual.Should().BeEmpty();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification = "Name/spelling is correct.")]
        [Fact]
        public static void LogItemOrigins_All_should_include_all_flags_other_than_none_and_have_all_bits_on()
        {
            // Arrange & Act
            var all = LogItemOrigins.All;
            var allIndividual = all.GetIndividualFlags().Where(_ => _.GetIndividualFlags().Count == 1).ToList();
            var enumIndividual = new ArrayList(Enum.GetValues(typeof(LogItemOrigins))).Cast<LogItemOrigins>().Where(_ => _ != LogItemOrigins.None).Where(_ => _.GetIndividualFlags().Count == 1).ToList();

            var expectedNumberFromAll = Enumerable.Range(0, allIndividual.Count).Select(_ => Math.Pow(2, _)).Sum();
            var expectedNumberFromEnum = Enumerable.Range(0, enumIndividual.Count).Select(_ => Math.Pow(2, _)).Sum();

            // Assert
            allIndividual.Should().Equal(enumIndividual);
            expectedNumberFromAll.Should().Be(expectedNumberFromEnum);
            ((double)all).Should().Be(expectedNumberFromAll);
        }

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
