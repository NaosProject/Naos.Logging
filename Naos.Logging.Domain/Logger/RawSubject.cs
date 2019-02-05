// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RawSubject.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using Naos.Compression.Domain;
    using Naos.Serialization.Domain.Extensions;
    using Naos.Serialization.Json;

    /// <summary>
    /// Model object to encapsulate the conversion of a subject object into a payload for logging.
    /// </summary>
    public class RawSubject
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
                LogHelper.SubjectSerializationDescription,
                JsonSerializerFactory.Instance,
                CompressorFactory.Instance);
            var result = new Subject(describedSerialization, this.Summary);
            return result;
        }
    }
}