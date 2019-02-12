// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subject.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using Naos.Serialization.Domain;

    /// <summary>
    /// Some core piece of information that is being logged.
    /// </summary>
    public class Subject
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
    }
}
