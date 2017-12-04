// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryLogProcessor.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Spritely.Recipes;

    /// <summary>
    /// In memory implementation of <see cref="LogProcessorBase" />.
    /// </summary>
    public class InMemoryLogProcessor : LogProcessorBase
    {
        private readonly Queue<LogMessage> loggedMessages = new Queue<LogMessage>();

        private readonly object syncLoggedMessages = new object();

        private readonly InMemoryLogConfiguration memoryConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryLogProcessor"/> class.
        /// </summary>
        /// <param name="memoryConfiguration">Configuration.</param>
        public InMemoryLogProcessor(InMemoryLogConfiguration memoryConfiguration)
            : base(memoryConfiguration)
        {
            new { memoryConfiguration }.Must().NotBeNull().OrThrowFirstFailure();

            this.memoryConfiguration = memoryConfiguration;
        }

        /// <inheritdoc cref="LogProcessorBase" />
        public override void Log(LogMessage logMessage)
        {
            new { logMessage }.Must().NotBeNull().OrThrowFirstFailure();

            lock (this.syncLoggedMessages)
            {
                this.loggedMessages.Enqueue(logMessage);

                if (this.memoryConfiguration.MaxLoggedItemCount == -1)
                {
                    // no pruning just grow infinitely.
                    return;
                }

                if (this.loggedMessages.Count > this.memoryConfiguration.MaxLoggedItemCount)
                {
                    this.loggedMessages.Dequeue();
                }
            }
        }

        /// <inheritdoc cref="LogProcessorBase" />
        protected override void InternalLog(LogItem logItem)
        {
            new { logItem }.Must().NotBeNull().OrThrowFirstFailure();

            if (this.memoryConfiguration.MaxLoggedItemCount == 0)
            {
                // this is basically using it as a null LogProcessor.
                return;
            }

            var logMessage = new LogMessage(logItem.Context, logItem.BuildLogMessage(), logItem.LoggedTimeUtc);

            this.Log(logMessage);
        }

        /// <summary>
        /// Purge all <see cref="LoggedItems" />.
        /// </summary>
        public void PurgeAllLoggedItems()
        {
            lock (this.syncLoggedMessages)
            {
                this.loggedMessages.Clear();
            }
        }

        /// <summary>
        /// Gets the items tracked from logging.
        /// </summary>
        public IReadOnlyList<LogMessage> LoggedItems
        {
            get
            {
                lock (this.syncLoggedMessages)
                {
                    return this.loggedMessages.ToList();
                }
            }
        }

        /// <inheritdoc cref="object" />
        public override string ToString()
        {
            var ret = FormattableString.Invariant($"{this.GetType().FullName}; {nameof(this.memoryConfiguration.ContextsToLog)}: {this.memoryConfiguration.ContextsToLog}; {nameof(this.memoryConfiguration.MaxLoggedItemCount)}: {this.memoryConfiguration.MaxLoggedItemCount}");
            return ret;
        }
    }
}