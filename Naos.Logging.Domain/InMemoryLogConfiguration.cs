// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryLogConfiguration.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OBeautifulCode.Math.Recipes;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// In memory implementation of <see cref="LogConfigurationBase" />.
    /// </summary>
    public class InMemoryLogConfiguration : LogConfigurationBase, IEquatable<InMemoryLogConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryLogConfiguration"/> class.
        /// </summary>
        /// <param name="contextsToLog">Contexts to log.</param>
        /// <param name="maxLoggedItemCount">Optional maximum number of elements to keep internally before removing the oldest items; DEFAULT is -1 which is infinite.</param>
        public InMemoryLogConfiguration(LogContexts contextsToLog, int maxLoggedItemCount = -1)
            : base(contextsToLog)
        {
            new { maxLoggedItemCount }.Must().BeGreaterThanOrEqualTo(-1).OrThrowFirstFailure();

            this.MaxLoggedItemCount = maxLoggedItemCount;
        }

        /// <summary>
        /// Gets the maximum number of elements to keep internally before removing the oldest items.
        /// </summary>
        public int MaxLoggedItemCount { get; private set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(InMemoryLogConfiguration first, InMemoryLogConfiguration second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.ContextsToLog == second.ContextsToLog && first.MaxLoggedItemCount == second.MaxLoggedItemCount;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(InMemoryLogConfiguration first, InMemoryLogConfiguration second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(InMemoryLogConfiguration other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as InMemoryLogConfiguration);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.ContextsToLog.ToString()).Hash(this.MaxLoggedItemCount).Value;
    }

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
            var ret = Invariant($"{this.GetType().FullName}; {nameof(this.memoryConfiguration.ContextsToLog)}: {this.memoryConfiguration.ContextsToLog}; {nameof(this.memoryConfiguration.MaxLoggedItemCount)}: {this.memoryConfiguration.MaxLoggedItemCount}");
            return ret;
        }
    }
}