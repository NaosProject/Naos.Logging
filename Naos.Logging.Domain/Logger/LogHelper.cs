// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogHelper.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Naos.Diagnostics.Recipes;
    using Naos.Serialization.Domain;
    using Naos.Telemetry.Domain;
    using OBeautifulCode.Error.Recipes;
    using OBeautifulCode.TypeRepresentation;

    using static System.FormattableString;

    /// <summary>
    /// Helper methods for logging.
    /// </summary>
    public static class LogHelper
    {
        static LogHelper()
        {
            MachineName = Diagnostics.Recipes.MachineName.GetMachineName();
            ProcessName = ProcessHelpers.GetRunningProcess().GetName();
            ProcessFileVersion = ProcessHelpers.GetRunningProcess().GetFileVersion();
            SubjectSerializationDescription = new SerializationDescription(
                SerializationFormat.Json,
                SerializationRepresentation.String,
                SerializationKind.Compact);
        }

        /// <summary>
        /// Gets the machine name, initialized in static constructor.
        /// </summary>
        public static string MachineName { get; private set; }

        /// <summary>
        /// Gets the process name, initialized in static constructor.
        /// </summary>
        public static string ProcessName { get; private set; }

        /// <summary>
        /// Gets the process file version, initialized in static constructor.
        /// </summary>
        public static string ProcessFileVersion { get; private set; }

        /// <summary>
        /// Gets the <see cref="SerializationDescription" /> to use for serializing the subject object.
        /// </summary>
        public static SerializationDescription SubjectSerializationDescription { get; private set; }

        /// <summary>
        /// Determine the <see cref="LogItemKind" /> from the subject.
        /// </summary>
        /// <param name="subject">Subject to interrogate.</param>
        /// <returns><see cref="LogItemKind" /> of <paramref name="subject" />.</returns>
        public static LogItemKind DetermineKindFromSubject(object subject)
        {
            LogItemKind result;
            if (subject is string)
            {
                result = LogItemKind.String;
            }
            else if (subject is IAmTelemetryItem)
            {
                result = LogItemKind.Telemetry;
            }
            else if (subject is Exception)
            {
                result = LogItemKind.Exception;
            }
            else
            {
                result = LogItemKind.Object;
            }

            return result;
        }

        /// <summary>
        /// Build a log item from provided values.
        /// </summary>
        /// <param name="subjectFunc">Function that returns the subject.</param>
        /// <param name="comment">Optional comment.</param>
        /// <param name="originOverride">Optional origin override.</param>
        /// <param name="activityCorrelation">Optional activity correlation.</param>
        /// <param name="errorCodeKeyField">Optional error key for <see cref="Exception" />'s; DEFAULT is <see cref="Constants.ExceptionDataKeyForErrorCode" />.</param>
        /// <returns>Constructed <see cref="LogItem" />.</returns>
        public static LogItem BuildLogItem(
            Func<object> subjectFunc,
            string comment = null,
            string originOverride = null,
            ActivityCorrelation activityCorrelation = null,
            string errorCodeKeyField = null)
        {
            var correlations = new List<IHaveCorrelationId>();
            var activityCorrelationPosition = ActivityCorrelationPosition.Unknown;
            if (activityCorrelation != null)
            {
                correlations.Add(activityCorrelation);
                activityCorrelationPosition = activityCorrelation.CorrelationPosition;
            }

            var subject = subjectFunc?.Invoke();
            var kind = DetermineKindFromSubject(subject);
            var localErrorCodeKeyField = string.IsNullOrWhiteSpace(errorCodeKeyField)
                ? Constants.ExceptionDataKeyForErrorCode
                : errorCodeKeyField;
            string stackTrace = null;

            if (subject is Exception loggedException)
            {
                var correlatingException = loggedException.FindFirstExceptionInChainWithExceptionId();
                if (correlatingException == null)
                {
                    loggedException.GenerateAndWriteExceptionIdIntoExceptionData();
                    correlatingException = loggedException;
                }

                var correlationId = correlatingException.GetExceptionIdFromExceptionData().ToString();

                var exceptionCorrelatingSubject = new RawSubject(
                    correlatingException,
                    BuildSummaryFromSubjectObject(correlatingException, activityCorrelationPosition));

                var exceptionCorrelation = new ExceptionCorrelation(correlationId, exceptionCorrelatingSubject.ToSubject());
                correlations.Add(exceptionCorrelation);

                var errorCode = loggedException.GetErrorCode();
                if (!string.IsNullOrWhiteSpace(errorCode))
                {
                    var errorCodeCorrelation = new ErrorCodeCorrelation(localErrorCodeKeyField, errorCode);
                    correlations.Add(errorCodeCorrelation);
                }

                stackTrace = loggedException.StackTrace;
            }

            var rawSubject = new RawSubject(
                subject,
                BuildSummaryFromSubjectObject(subject, activityCorrelationPosition));

            var anonymousMethodInfo = subjectFunc != null ? new AnonymousMethodInfo(subjectFunc.Method) : null;
            var context = new LogItemContext(
                DateTime.UtcNow,
                originOverride,
                MachineName,
                ProcessName,
                ProcessFileVersion,
                anonymousMethodInfo?.MethodName,
                anonymousMethodInfo?.EnclosingType?.ToTypeDescription(),
                stackTrace);

            var result = new LogItem(rawSubject.ToSubject(), kind, context, comment, correlations);

            return result;
        }

        /// <summary>
        /// Build summary for <see cref="RawSubject" /> from the subject.
        /// </summary>
        /// <param name="subjectObject">Subject to use.</param>
        /// <param name="activityCorrelationPosition">Activity position correlation</param>
        /// <returns>A string summary of the subject.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object", Justification = "Spelling/name is correct.")]
        public static string BuildSummaryFromSubjectObject(
            object subjectObject,
            ActivityCorrelationPosition activityCorrelationPosition)
        {
            string result;
            if (subjectObject is Exception ex)
            {
                result = Invariant($"{ex.GetType().Name}: {ex.Message}");
            }
            else
            {
                result = subjectObject?.ToString() ?? "[null]";
            }

            if (activityCorrelationPosition != ActivityCorrelationPosition.Unknown)
            {
                result = Invariant($"{activityCorrelationPosition}: {result}");
            }

            return result;
        }
    }

    /// <summary>
    /// Gets information relating to an anonymous method.
    /// </summary>
    internal class AnonymousMethodInfo
    {
        private readonly MethodInfo anonymousMethod;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "AnonymousMethodInfo" /> class.
        /// </summary>
        /// <param name = "anonymousMethod">An anonymous method.</param>
        public AnonymousMethodInfo(MethodInfo anonymousMethod)
        {
            if (anonymousMethod == null)
            {
                throw new ArgumentNullException(nameof(anonymousMethod));
            }

            this.anonymousMethod = anonymousMethod;

            var declaringType = this.anonymousMethod.DeclaringType;

            var methodName = this.anonymousMethod.Name;
            var indexOfGt = methodName.IndexOf(">", StringComparison.OrdinalIgnoreCase);

            if (indexOfGt < 0)
            {
                this.EnclosingType = declaringType;
                this.EnclosingMethodName = this.anonymousMethod.Name;
                return;
            }

            this.EnclosingMethodName = methodName.Substring(0, indexOfGt).TrimStart('<');

            if (declaringType.IsCompilerGenerated() &&
                (declaringType
                     ?.DeclaringType
                     ?.IsGenericTypeDefinition ?? false))
            {
                this.EnclosingType = declaringType
                    .DeclaringType
                    ?.MakeGenericType(declaringType.GenericTypeArguments);
            }
            else
            {
                this.EnclosingType = declaringType;
            }

            while (this.EnclosingType?.DeclaringType != null &&
                   this.EnclosingType.IsCompilerGenerated())
            {
                this.EnclosingType = this.EnclosingType.DeclaringType;
            }
        }

        /// <summary>
        /// Gets the method name of the lambda.
        /// </summary>
        public string MethodName => this.anonymousMethod.Name;

        /// <summary>
        ///   Gets the name of the enclosing method.
        /// </summary>
        /// <value>The name of the method in which the anonymous method is declared.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Brought over code, keeping as is.")]
        public string EnclosingMethodName { get; }

        /// <summary>
        ///   Gets the type of the enclosing.
        /// </summary>
        /// <value>The name of the class within which the anonymous method is declared.</value>
        public Type EnclosingType { get; }
    }

    /// <summary>
    /// Extensions on <see cref="Type" />.
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Is the type anonymous.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns>A value indicating whether or not the type is anonymous.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Brought over code, keeping as is.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.StartsWith(System.String)", Justification = "Brought over code, keeping as is.")]
        public static bool IsAnonymous(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false) &&
                   type.IsGenericType && type.Name.Contains("AnonymousType") &&
                   (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$")) &&
                   (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        /// <summary>
        /// Is the type compiler generated.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns>A value indicating whether or not the type is compiler generated.</returns>
        public static bool IsCompilerGenerated(this Type type) =>
        Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false);

        /// <summary>
        /// Is the type async.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns>A value indicating whether or not the type is async.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Brought over code, keeping as is.")]
        public static bool IsAsync(this Type type) =>
            typeof(Task).IsAssignableFrom(type);
    }
}