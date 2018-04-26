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
    /// In memory implementation of <see cref="LogWriterBase" />.
    /// </summary>
    public class InMemoryLogWriter : LogWriterBase
    {
        private readonly Queue<LogItem> loggedMessages = new Queue<LogItem>();

        private readonly object syncLoggedMessages = new object();

        private readonly InMemoryLogConfig memoryLogConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryLogWriter"/> class.
        /// </summary>
        /// <param name="memoryConfig">Configuration.</param>
        public InMemoryLogWriter(
            InMemoryLogConfig memoryConfig)
            : base(memoryConfig)
        {
            new { memoryConfig }.Must().NotBeNull().OrThrowFirstFailure();

            this.memoryLogConfig = memoryConfig;
        }

        /// <inheritdoc  />
        public override void LogInternal(LogItem logMessage)
        {
            lock (this.syncLoggedMessages)
            {
                this.loggedMessages.Enqueue(logMessage);

                if (this.memoryLogConfig.MaxLoggedItemCount == -1)
                {
                    // no pruning just grow infinitely.
                    return;
                }

                if (this.loggedMessages.Count > this.memoryLogConfig.MaxLoggedItemCount)
                {
                    this.loggedMessages.Dequeue();
                }
            }
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
        public IReadOnlyList<LogItem> LoggedItems
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
            var ret = FormattableString.Invariant($"{this.GetType().FullName}; {nameof(this.memoryLogConfig.OriginsToLog)}: {this.memoryLogConfig.OriginsToLog}; {nameof(this.memoryLogConfig.MaxLoggedItemCount)}: {this.memoryLogConfig.MaxLoggedItemCount}");
            return ret;
        }
    }
}