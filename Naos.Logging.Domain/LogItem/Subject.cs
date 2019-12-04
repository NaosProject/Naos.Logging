// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subject.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using OBeautifulCode.Equality.Recipes;
    using OBeautifulCode.Serialization;

    /// <summary>
    /// Some core piece of information that is being logged.
    /// </summary>
    public class Subject : IEquatable<Subject>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subject"/> class.
        /// </summary>
        /// <param name="describedSerialization">A described serialization of the subject.</param>
        /// <param name="summary">A string summary of the subject.</param>
        public Subject(
            DescribedSerialization describedSerialization,
            string summary)
        {
            this.DescribedSerialization = describedSerialization ?? throw new ArgumentNullException(nameof(describedSerialization));
            this.Summary = summary ?? LogHelper.NullSubjectSummary;
        }

        /// <summary>
        /// Gets a described serialization of the subject.
        /// </summary>
        public DescribedSerialization DescribedSerialization { get; private set; }

        /// <summary>
        /// Gets a string summary of the subject.
        /// </summary>
        public string Summary { get; private set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(Subject first, Subject second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.DescribedSerialization == second.DescribedSerialization && string.Equals(first.Summary, second.Summary, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(Subject first, Subject second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(Subject other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as Subject);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.DescribedSerialization).Hash(this.Summary).Value;
    }
}
