// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLogConfiguration.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.IO;

    using OBeautifulCode.Math.Recipes;

    using Spritely.Recipes;

    /// <summary>
    /// <see cref="File"/> focused implementation of <see cref="LogConfigurationBase" />.
    /// </summary>
    public class FileLogConfiguration : LogConfigurationBase, IEquatable<FileLogConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogConfiguration"/> class.
        /// </summary>
        /// <param name="contextsToLog">Contexts to log.</param>
        /// <param name="logFilePath">File path to write logs to.</param>
        /// <param name="createDirectoryStructureIfMissing">Optional value indicating whether to create the directory structure if it's missing; DEFAULT is true.</param>
        public FileLogConfiguration(LogContexts contextsToLog, string logFilePath, bool createDirectoryStructureIfMissing = true)
            : base(contextsToLog)
        {
            new { logFilePath }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();

            this.LogFilePath = logFilePath;
            this.CreateDirectoryStructureIfMissing = createDirectoryStructureIfMissing;
        }

        /// <summary>
        /// Gets the file path to write logs to.
        /// </summary>
        public string LogFilePath { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to create the directory structure if it's missing.
        /// </summary>
        public bool CreateDirectoryStructureIfMissing { get; private set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(FileLogConfiguration first, FileLogConfiguration second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.ContextsToLog == second.ContextsToLog && first.LogFilePath == second.LogFilePath && first.CreateDirectoryStructureIfMissing == second.CreateDirectoryStructureIfMissing;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(FileLogConfiguration first, FileLogConfiguration second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(FileLogConfiguration other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as FileLogConfiguration);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.ContextsToLog.ToString()).Hash(this.LogFilePath).Hash(this.CreateDirectoryStructureIfMissing).Value;
    }
}