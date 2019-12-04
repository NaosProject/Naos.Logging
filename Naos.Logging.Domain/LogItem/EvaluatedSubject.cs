// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluatedSubject.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Runtime.CompilerServices;
    using OBeautifulCode.Equality.Recipes;
    using OBeautifulCode.Math.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Type.Recipes;

    /// <summary>
    /// Some core piece of information that is being logged.
    /// </summary>
    public class EvaluatedSubject : IEquatable<EvaluatedSubject>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluatedSubject"/> class.
        /// </summary>
        /// <param name="correlatingSubjectFunc">Correlating subject to use.</param>
        public EvaluatedSubject(Func<object> correlatingSubjectFunc)
        {
            this.Subject = correlatingSubjectFunc?.Invoke();
            this.SubjectToString = this.Subject?.GetType() == typeof(Type) ? ((Type)this.Subject).ToStringReadable() : this.Subject?.ToString();
            this.SubjectHashCode = this.Subject?.GetHashCode().ToGuid().ToString().ToUpperInvariant();
        }

        /// <summary>
        /// Gets the subject object.
        /// </summary>
        public object Subject { get; private set; }

        /// <summary>
        /// Gets the subject's ToString() value.
        /// </summary>
        public string SubjectToString { get; private set; }

        /// <summary>
        /// Gets the subject's hash code a Guid in string format (upper case).
        /// </summary>
        public string SubjectHashCode { get; private set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(EvaluatedSubject first, EvaluatedSubject second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.Subject == second.Subject &&
                   string.Equals(first.SubjectToString, second.SubjectToString, StringComparison.Ordinal) &&
                   string.Equals(first.SubjectHashCode, second.SubjectHashCode, StringComparison.Ordinal);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(EvaluatedSubject first, EvaluatedSubject second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(EvaluatedSubject other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as EvaluatedSubject);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.Subject)
            .Hash(this.SubjectToString)
            .Hash(this.SubjectHashCode)
            .Value;
    }
}
