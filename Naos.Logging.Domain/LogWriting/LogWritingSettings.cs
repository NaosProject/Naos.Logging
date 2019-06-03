// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWritingSettings.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Math.Recipes;

    /// <summary>
    /// Settings to use when configuring log writing.
    /// </summary>
    public class LogWritingSettings : IEquatable<LogWritingSettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogWritingSettings"/> class.
        /// </summary>
        /// <param name="configs">Configurations to setup; DEFAULT is null but only to be used in null logging scenarios or custom provided log writers.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "configs", Justification = "Spelling/name is correct.")]
        public LogWritingSettings(
            IReadOnlyCollection<LogWriterConfigBase> configs = null)
        {
            this.Configs = configs ?? new List<LogWriterConfigBase>();
        }

        /// <summary>
        /// Gets the configurations to use.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Configs", Justification = "Spelling/name is correct.")]
        public IReadOnlyCollection<LogWriterConfigBase> Configs { get; private set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(LogWritingSettings first, LogWritingSettings second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.Configs.SequenceEqualHandlingNulls(second.Configs);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(LogWritingSettings first, LogWritingSettings second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(LogWritingSettings other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as LogWritingSettings);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize().HashElements(this.Configs).Value;
    }
}