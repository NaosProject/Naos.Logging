// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluatedSubject.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using OBeautifulCode.Math.Recipes;

    /// <summary>
    /// Some core piece of information that is being logged.
    /// </summary>
    public class EvaluatedSubject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluatedSubject"/> class.
        /// </summary>
        /// <param name="correlatingSubjectFunc">Correlating subject to use.</param>
        public EvaluatedSubject(Func<object> correlatingSubjectFunc)
        {
            this.Subject = correlatingSubjectFunc?.Invoke();
            this.SubjectToString = this.Subject?.ToString();
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
    }
}
