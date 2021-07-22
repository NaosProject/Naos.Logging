// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RawSubject.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Compression;
    using OBeautifulCode.Compression.Recipes;
    using OBeautifulCode.Equality.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Serialization.Json;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// Model object to encapsulate the conversion of a subject object into a payload for logging.
    /// </summary>
    public class RawSubject : IEquatable<RawSubject>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RawSubject"/> class.
        /// </summary>
        /// <param name="originalSubject">Subject object.</param>
        /// <param name="summary">Optional summary.</param>
        public RawSubject(object originalSubject, string summary)
        {
            this.OriginalSubject = originalSubject;
            this.Summary = summary;
        }

        /// <summary>
        /// Gets the original subject object.
        /// </summary>
        public object OriginalSubject { get; private set; }

        /// <summary>
        /// Gets the summary if provided.
        /// </summary>
        public string Summary { get; private set; }

        /// <summary>
        /// Convert to a <see cref="Subject" />.
        /// </summary>
        /// <returns>Converted <see cref="Subject" />.</returns>
        public Subject ToSubject()
        {
            var describedSerialization = this.OriginalSubject.ToDescribedSerializationUsingSpecificFactory(
                Log.SubjectSerializerRepresentation,
                Log.SubjectSerializerFactory,
                SerializationFormat.String,
                VersionMatchStrategy.AnySingleVersion);
            var result = new Subject(describedSerialization, this.Summary);
            return result;
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(RawSubject first, RawSubject second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            var originalSubjectEqual = ReferenceEquals(first.OriginalSubject, second.OriginalSubject);
            if (!originalSubjectEqual)
            {
                if (ReferenceEquals(first.OriginalSubject, null) || ReferenceEquals(second.OriginalSubject, null))
                {
                    originalSubjectEqual = false;
                }

                if (!originalSubjectEqual)
                {
                    originalSubjectEqual = first?.OriginalSubject.Equals(second.OriginalSubject) ?? false;
                }
            }

            return originalSubjectEqual &&
                   string.Equals(first.Summary, second.Summary, StringComparison.Ordinal);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(RawSubject first, RawSubject second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(RawSubject other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as RawSubject);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.OriginalSubject)
            .Hash(this.Summary)
            .Value;
    }
}
