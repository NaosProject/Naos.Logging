// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogReaderBase.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;

    using Spritely.Recipes;

    /// <summary>
    /// Base class for readers.
    /// </summary>
    public abstract class LogReaderBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Keeping for now to maintain contract of providing it.")]
        private readonly LogConfigurationBase logConfigurationBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogReaderBase"/> class.
        /// </summary>
        /// <param name="logConfigurationBase">Base configuration.</param>
        protected LogReaderBase(LogConfigurationBase logConfigurationBase)
        {
            new { logConfigurationBase }.Must().NotBeNull().OrThrowFirstFailure();

            this.logConfigurationBase = logConfigurationBase;
        }

        /// <summary>
        /// Reads the log.
        /// </summary>
        /// <returns>Collection of <see cref="LogMessage" />'s.</returns>
        public abstract IReadOnlyCollection<LogMessage> ReadAll();

        /// <summary>
        /// Reads the log for the specified range.
        /// </summary>
        /// <param name="startUtc">Start time to range to read in UTC.</param>
        /// <param name="endUtc">End time of range to read in UTC.</param>
        /// <returns>Collection of <see cref="LogMessage" />'s.</returns>
        public abstract IReadOnlyCollection<LogMessage> ReadRange(DateTime startUtc, DateTime endUtc);
    }

    /// <summary>
    /// Model object to hold a read message.
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="context">Context it was logged under.</param>
        /// <param name="message">Message logged.</param>
        /// <param name="loggedDateTimeUtc"><see cref="DateTime" /> in UTC that is was logged.</param>
        public LogMessage(LogContexts context, string message, DateTime loggedDateTimeUtc)
        {
            this.Context = context;
            this.Message = message;
            this.LoggedDateTimeUtc = loggedDateTimeUtc;
        }

        /// <summary>
        /// Gets the context it was logged under.
        /// </summary>
        public LogContexts Context { get; private set; }

        /// <summary>
        /// Gets the message logged.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the <see cref="DateTime" /> in UTC that is was logged.
        /// </summary>
        public DateTime LoggedDateTimeUtc { get; private set; }
    }
}