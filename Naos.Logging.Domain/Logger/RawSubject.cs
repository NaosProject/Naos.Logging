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
                LogHelper.SubjectSerializationDescription,
                RawSubjectSerializerFactory.Instance,
                CompressorFactory.Instance,
                unregisteredTypeEncounteredStrategy: UnregisteredTypeEncounteredStrategy.Attempt);
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

    /// <summary>
    /// Implementation of <see cref="ISerializerFactory" /> for use with converting a <see cref="RawSubject" /> to a <see cref="Subject" />.
    /// </summary>
    /// <remarks>Caching is needed due to cost of creation with a serializer.</remarks>
    public class RawSubjectSerializerFactory : ISerializerFactory
    {
        private static readonly RawSubjectSerializerFactory InternalInstance = new RawSubjectSerializerFactory();

        /// <summary>
        /// Gets the singleton entry point to the code.
        /// </summary>
        public static ISerializerFactory Instance => InternalInstance;

        private readonly object sync = new object();

        private readonly IDictionary<Type, ISerializeAndDeserialize> configurationTypeToSerializerMap = new Dictionary<Type, ISerializeAndDeserialize>();

        private RawSubjectSerializerFactory()
        {
            /* no-op to make sure this can only be accessed via instance property */
        }

        /// <inheritdoc />
        public ISerializeAndDeserialize BuildSerializer(SerializationDescription serializationDescription, TypeMatchStrategy typeMatchStrategy = TypeMatchStrategy.NamespaceAndName, MultipleMatchStrategy multipleMatchStrategy = MultipleMatchStrategy.ThrowOnMultiple, UnregisteredTypeEncounteredStrategy unregisteredTypeEncounteredStrategy = UnregisteredTypeEncounteredStrategy.Default)
        {
            new { serializationDescription }.AsArg().Must().NotBeNull();

            if (serializationDescription == LogHelper.SubjectSerializationDescription)
            {
                return LogWriterBase.DefaultLogItemSerializer;
            }

            var configurationType = serializationDescription.ConfigurationTypeRepresentation?.ResolveFromLoadedTypes(typeMatchStrategy, multipleMatchStrategy);
            lock (this.sync)
            {
                switch (serializationDescription.SerializationKind)
                {
                    case SerializationKind.Json:
                    {
                        var configurationTypeForKeyCheck = configurationType ?? typeof(NullJsonConfiguration);
                        if (!this.configurationTypeToSerializerMap.ContainsKey(configurationTypeForKeyCheck))
                        {
                            var serializer = new ObcJsonSerializer(configurationTypeForKeyCheck, UnregisteredTypeEncounteredStrategy.Attempt);
                            this.configurationTypeToSerializerMap[configurationTypeForKeyCheck] = serializer;
                        }

                        var result = this.configurationTypeToSerializerMap[configurationTypeForKeyCheck];
                        return result;
                    }

                    default: throw new NotSupportedException(Invariant($"{nameof(serializationDescription)} from enumeration {nameof(SerializationKind)} of {serializationDescription.SerializationKind} is not supported."));
                }
            }
        }
    }
}
