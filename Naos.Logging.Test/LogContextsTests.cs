// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogContextsTests.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Linq;

    using FluentAssertions;

    using Naos.Logging.Domain;

    using OBeautifulCode.Enum.Recipes;

    using Xunit;

    using static System.FormattableString;

    public static class LogContextsTests
    {
        [Fact]
        public static void Constructor___Valid___Works()
        {
            // Arrange
            var examine = LogContexts.AllErrors;
            var expected = new[] { LogContexts.ItsLogInternalErrors, LogContexts.EntryPostedException, LogContexts.AppDomainUnhandledException };

            // Act
            var all = examine.GetIndividualFlags();
            var hasFlagNone = examine.HasFlag(LogContexts.None);

            // Assert
            all.Should().BeEquivalentTo(expected);
            hasFlagNone.Should().BeFalse();
        }
    }
}