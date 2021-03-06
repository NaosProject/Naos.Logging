// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryLogWriter.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Naos.Logging.Domain;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Type.Recipes;

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
        /// <param name="logItemSerializerFactory">Optional serializer factory; DEFAULT will be base version.</param>
        public InMemoryLogWriter(
            InMemoryLogConfig memoryConfig,
            ISerializerFactory logItemSerializerFactory = null)
            : base(memoryConfig, logItemSerializerFactory)
        {
            this.memoryLogConfig = memoryConfig ?? throw new ArgumentNullException(nameof(memoryConfig));
        }

        /// <inheritdoc  />
        protected override void LogInternal(
            LogItem logItem)
        {
            if (logItem == null)
            {
                throw new ArgumentNullException(nameof(logItem));
            }

            lock (this.syncLoggedMessages)
            {
                this.loggedMessages.Enqueue(logItem);

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

        /// <inheritdoc />
        public override string ToString()
        {
            var ret = FormattableString.Invariant($"{this.GetType().ToStringReadable()}; {nameof(this.memoryLogConfig.LogInclusionKindToOriginsMap)}: {this.memoryLogConfig.LogInclusionKindToOriginsMapFriendlyString}; {nameof(this.memoryLogConfig.MaxLoggedItemCount)}: {this.memoryLogConfig.MaxLoggedItemCount}");
            return ret;
        }
    }
}
