// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItemOriginTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Linq;

    using FluentAssertions;

    using Naos.Logging.Domain;

    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Enum.Recipes;

    using Xunit;

    public static class LogItemOriginTest
    {
        [Fact]
        public static void LogItemOrigin_and_LogItemOrigins_should_have_the_same_set_of_named_members_excluding_LogItemOrigin_Unknown_and_LogItemOrigins_None()
        {
            // Arrange
            var logItemOriginsMembers = EnumExtensions.GetEnumValues<LogItemOrigins>().Where(_ => ((Enum)_).GetIndividualFlags().Count == 1).Select(_ => _.ToString()).Where(_ => _ != nameof(LogItemOrigins.None)).ToList();
            var logItemOriginMembers = EnumExtensions.GetEnumValues<LogItemOrigin>().Where(_ => ((Enum)_).GetIndividualFlags().Count == 1 ? true : throw new Exception()).Select(_ => _.ToString()).Where(_ => _ != nameof(LogItemOrigin.Unknown)).ToList();

            // Act
            var actual = logItemOriginsMembers.SymmetricDifference(logItemOriginMembers);

            // Assert
            actual.Should().BeEmpty();
        }
    }
}
