// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogConfiguration.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using OBeautifulCode.Math.Recipes;

    /// <summary>
    /// <see cref="Console" /> focused implementation of <see cref="LogConfigurationBase" />.
    /// </summary>
    public class ConsoleLogConfiguration : LogConfigurationBase, IEquatable<ConsoleLogConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogConfiguration"/> class.
        /// </summary>
        /// <param name="contextsToLogConsoleOut"><see cref="LogContexts" /> to write to <see cref="Console.Out" />.</param>
        /// <param name="contextsToLogConsoleError"><see cref="LogContexts" /> to write to <see cref="Console.Error" />.</param>
        public ConsoleLogConfiguration(LogContexts contextsToLogConsoleOut, LogContexts contextsToLogConsoleError)
            : base(contextsToLogConsoleOut | contextsToLogConsoleError)
        {
            this.ContextsToLogConsoleOut = contextsToLogConsoleOut;
            this.ContextsToLogConsoleError = contextsToLogConsoleError;
        }

        /// <summary>
        /// Gets the <see cref="LogContexts" /> to write to <see cref="Console.Out" />.
        /// </summary>
        public LogContexts ContextsToLogConsoleOut { get; private set; }

        /// <summary>
        /// Gets the <see cref="LogContexts" /> to write to <see cref="Console.Error" />.
        /// </summary>
        public LogContexts ContextsToLogConsoleError { get; private set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(ConsoleLogConfiguration first, ConsoleLogConfiguration second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.ContextsToLog == second.ContextsToLog && first.ContextsToLogConsoleOut == second.ContextsToLogConsoleOut && first.ContextsToLogConsoleError == second.ContextsToLogConsoleError;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(ConsoleLogConfiguration first, ConsoleLogConfiguration second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(ConsoleLogConfiguration other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as ConsoleLogConfiguration);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.ContextsToLog.ToString()).Hash(this.ContextsToLogConsoleOut.ToString()).Hash(this.ContextsToLogConsoleError.ToString()).Value;
    }
}