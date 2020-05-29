// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Naos.Logging.Domain.Internal;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Serialization.Json;

    /// <summary>
    /// Helper to deal with correlations of <see cref="Exception" />'s.
    /// </summary>
    public static class Log
    {
        private static readonly object SyncSerializationSettings = new object();

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = NaosSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static Log()
        {
            SubjectSerializerRepresentation = new SerializerRepresentation(SerializationKind.Json,  typeof(LoggingJsonSerializationConfiguration).ToRepresentation());
            SubjectSerializerFactory = new JsonSerializerFactory();
        }

        /// <summary>
        /// Gets the <see cref="SerializerRepresentation" /> to use for serializing the subject object.
        /// </summary>
        public static SerializerRepresentation SubjectSerializerRepresentation { get; private set; }

        /// <summary>
        /// Gets the <see cref="ISerializerFactory" /> to use for serializing the subject object.
        /// </summary>
        public static ISerializerFactory SubjectSerializerFactory { get; private set; }

        /// <summary>
        /// Gets the singleton logger being used.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Keeping for preferred IL.")]
        public static ILog Instance { get; private set; } = new Logger();

        /// <summary>
        /// Setup the subject serialization to be used when packing and handing in payload object to a writer.
        /// </summary>
        /// <param name="subjectSerializerRepresentation">The subject serializer representation.</param>
        /// <param name="subjectSerializerFactory">The subject serializer factory.</param>
        public static void SetupSubjectSerialization(SerializerRepresentation subjectSerializerRepresentation, ISerializerFactory subjectSerializerFactory)
        {
            lock (SyncSerializationSettings)
            {
                SubjectSerializerRepresentation = subjectSerializerRepresentation;
                SubjectSerializerFactory = subjectSerializerFactory;
            }
        }

        /// <summary>
        /// Write a message.
        /// </summary>
        /// <param name="subjectFunc">Function to get the subject.</param>
        /// <param name="comment">Optional defaultCommentOverride.</param>
        /// <param name="origin">Optional origin override.</param>
        /// <param name="additionalCorrelations">Optional additional correlations.</param>
        public static void Write(Func<object> subjectFunc, string comment = null, string origin = null, IReadOnlyCollection<IHaveCorrelationId> additionalCorrelations = null)
        {
            Instance.Write(subjectFunc, comment, origin, additionalCorrelations);
        }

        /// <summary>
        /// With into a logged activity.
        /// </summary>
        /// <param name="correlatingSubjectFunc">Optional function to get the correlating subject.</param>
        /// <param name="defaultCommentOverride">Optional defaultCommentOverride.</param>
        /// <param name="defaultOriginOverride">Optional origin override.</param>
        /// <param name="correlationId">Optional correlation ID that will be used for each of the block correlations; DEFAULT is a different one for each.</param>
        /// <param name="additionalCorrelations">Optional additional correlations.</param>
        /// <returns>A configured <see cref="ICorrelatingActivityLogger" />.</returns>
        public static ILogDisposable With(Func<object> correlatingSubjectFunc = null, string defaultCommentOverride = null, string defaultOriginOverride = null, string correlationId = null, IReadOnlyCollection<IHaveCorrelationId> additionalCorrelations = null)
        {
            return Instance.With(correlatingSubjectFunc, defaultCommentOverride, defaultOriginOverride, correlationId, additionalCorrelations);
        }

        /// <summary>
        /// Set the <see cref="LogItemHandler" /> to send logged messages to.
        /// </summary>
        /// <param name="callback">Call back to send messages to.</param>
        public static void SetCallback(LogItemHandler callback)
        {
            Instance.SetCallback(callback);
        }
    }
}
