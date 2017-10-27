// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugTests.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using FakeItEasy;

    using FluentAssertions;

    using Naos.Logging.Domain;
    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;

    using Xunit;

    public static class DebugTests
    {
        [Fact(Skip = "For debugging log processors.")]
        public static void EventLog_default_config___Test___Non_logging_context()
        {
            // Arrange
            var config = new LogConfigurationEventLog(LogContexts.UnexpectedErrors);
            var logger = new LogProcessorEventLog(config);

            // Act
            logger.Log(LogContexts.EntryPostedInformation, "MessageShouldntBeThere", "SubjectShouldntBeThere");

            // Assert
            /* Confirm no entry - by hand */
        }

        [Fact(Skip = "For debugging log processors.")]
        public static void EventLog_default_config___Test___Logging_context()
        {
            // Arrange
            var config = new LogConfigurationEventLog(LogContexts.UnexpectedErrors);
            var logger = new LogProcessorEventLog(config);

            // Act
            logger.Log(LogContexts.ItsLogInternalErrors, "Message", "Subject");

            // Assert
            /* Confirm entry - by hand */
        }

        [Fact(Skip = "For debugging log processors.")]
        public static void EventLog_custom_config___Test___Non_logging_context()
        {
            // Arrange
            var config = new LogConfigurationEventLog(LogContexts.UnexpectedErrors, "MySourceLawson", "MyLogLawson", "MyMachine", true);
            var logger = new LogProcessorEventLog(config);

            // Act
            logger.Log(LogContexts.EntryPostedInformation, "MessageShouldntBeThere", "SubjectShouldntBeThere");

            // Assert
            /* Confirm no entry - by hand */
        }

        [Fact(Skip = "For debugging log processors.")]
        public static void EventLog_custom_config___Test___Logging_context_information()
        {
            // Arrange
            var config = new LogConfigurationEventLog(LogContexts.EntryPosted, "MySource", "MyLog", "Laptop", true);
            var logger = new LogProcessorEventLog(config);

            // Act
            logger.Log(LogContexts.EntryPostedInformation, "Message Information", "Subject");

            // Assert
            /* Confirm entry - by hand */
        }

        [Fact(Skip = "For debugging log processors.")]
        public static void EventLog_custom_config___Test___Logging_context_error()
        {
            // Arrange
            var config = new LogConfigurationEventLog(LogContexts.UnexpectedErrors, "MySource", "MyLog", "Laptop", true);
            var logger = new LogProcessorEventLog(config);

            // Act
            logger.Log(LogContexts.ItsLogInternalErrors, "Message Error", "Subject");

            // Assert
            /* Confirm entry - by hand */
        }
    }
}