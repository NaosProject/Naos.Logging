﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionExtensions.cs" company="OBeautifulCode">
//   Copyright (c) OBeautifulCode 2018. All rights reserved.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package. Will be overwritten with package update except in OBeautifulCode.Error source.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Error.Recipes
{
    using System;
    using System.Collections;
    using System.Linq;

    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Extension methods on type <see cref="Exception"/>.
    /// </summary>
#if !OBeautifulCodeErrorRecipesProject
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("OBeautifulCode.Error", "See package version number")]
    internal
#else
    public
#endif
        static class ExceptionExtensions
    {
        /// <summary>
        /// Adds an error code to the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="dataKeyForErrorCode">
        /// Optional value for the key to use when storing the error code in the
        /// <paramref name="exception"/>'s <see cref="Exception.Data"/> dictionary.
        /// Default is use the constant <see cref="Constants.ExceptionDataKeyForErrorCode"/>.
        /// </param>
        /// <param name="dataKeyForErrorCodesVector">
        /// Optional value for the key to use when storing the error codes vector in the
        /// <paramref name="exception"/>'s <see cref="Exception.Data"/> dictionary.
        /// Default is use the constant <see cref="Constants.ExceptionDataKeyForErrorCodesVector"/>.
        /// </param>
        /// <returns>
        /// The specified <paramref name="exception"/>, with the error code added to the
        /// <see cref="Exception.Data"/> dictionary, keyed on <paramref name="dataKeyForErrorCode"/>
        /// and with all error codes (this one included) in the traversal of <see cref="Exception.InnerException"/>
        /// and <see cref="AggregateException.InnerExceptions"/> normalized into the same
        /// dictionary, keyed on <paramref name="dataKeyForErrorCodesVector"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="errorCode"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="errorCode"/> is white space.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="dataKeyForErrorCode"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="dataKeyForErrorCode"/> is white space.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="dataKeyForErrorCodesVector"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="dataKeyForErrorCodesVector"/> is white space.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="dataKeyForErrorCodesVector"/> is equal to <paramref name="dataKeyForErrorCode"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> <see cref="Exception.Data"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="exception"/> <see cref="Exception.Data"/> already contains the key <paramref name="dataKeyForErrorCode"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="exception"/> <see cref="Exception.Data"/> already contains the key <paramref name="dataKeyForErrorCodesVector"/>.</exception>
        public static Exception AddErrorCode(
            this Exception exception,
            string errorCode,
            string dataKeyForErrorCode = Constants.ExceptionDataKeyForErrorCode,
            string dataKeyForErrorCodesVector = Constants.ExceptionDataKeyForErrorCodesVector)
        {
            new { exception }.Must().NotBeNull();
            new { errorCode }.Must().NotBeNullNorWhiteSpace();
            new { dataKeyForErrorCode }.Must().NotBeNullNorWhiteSpace();
            new { dataKeyForErrorCodesVector }.Must().NotBeNullNorWhiteSpace().And().NotBeEqualTo(dataKeyForErrorCode, Invariant($"{nameof(dataKeyForErrorCode)} cannot equal {nameof(dataKeyForErrorCodesVector)}"));

            exception.Data.Named(Invariant($"{nameof(exception)}.{nameof(Exception.Data)}")).Must().NotBeNull();
            exception.Data.Keys.OfType<string>().Named(Invariant($"{nameof(exception)}.{nameof(Exception.Data)}.{nameof(IDictionary.Keys)}")).Must().NotContain(dataKeyForErrorCode);
            exception.Data.Keys.OfType<string>().Named(Invariant($"{nameof(exception)}.{nameof(Exception.Data)}.{nameof(IDictionary.Keys)}")).Must().NotContain(dataKeyForErrorCodesVector);

            exception.Data.Add(dataKeyForErrorCode, errorCode);

            var errorCodesVector = BuildErrorCodesVector(exception);
            exception.Data.Add(dataKeyForErrorCodesVector, errorCodesVector);

            return exception;
        }

        /// <summary>
        /// Gets the error code from the specified exception or null if no error code exists.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="dataKeyForErrorCode">
        /// Optional value for the key to use when retrieving the error code from the
        /// <paramref name="exception"/>'s <see cref="Exception.Data"/> dictionary.
        /// Default is use the constant <see cref="Constants.ExceptionDataKeyForErrorCode"/>.
        /// </param>
        /// <returns>
        /// The error code or null if no error code exists.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="dataKeyForErrorCode"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="dataKeyForErrorCode"/> is white space.</exception>
        public static string GetErrorCode(
            this Exception exception,
            string dataKeyForErrorCode = Constants.ExceptionDataKeyForErrorCode)
        {
            new { exception }.Must().NotBeNull();
            new { dataKeyForErrorCode }.Must().NotBeNullNorWhiteSpace();

            var result = GetErrorCodeString(exception, dataKeyForErrorCode);

            return result;
        }

        /// <summary>
        /// Gets the error codes vector from the specified exception or null if no error code
        /// exists on the specified exception and in the traversal of
        /// <see cref="Exception.InnerException"/> and <see cref="AggregateException.InnerExceptions"/>.
        /// </summary>
        /// <example>
        /// Here's what the vector looks like for the following exception tree:
        /// Exception(A)
        ///    .InnerException(B) (AggregateException)
        ///       .InnerExceptions[0](C)
        ///          .InnerException(D)
        ///       .InnerExceptions[1](E) (AggregateException)
        ///          .InnerExceptions[0](F)
        ///       .InnerExceptions[2](G)
        /// ErrorA -> ErrorB -> [ErrorC -> ErrorD, ErrorE -> ErrorF, ErrorG].
        /// </example>
        /// <param name="exception">The exception.</param>
        /// <param name="dataKeyForErrorCodesVector">
        /// Optional value for the key to use when retrieving the error codes vector from the
        /// <paramref name="exception"/>'s <see cref="Exception.Data"/> dictionary.
        /// Default is use the constant <see cref="Constants.ExceptionDataKeyForErrorCodesVector"/>.
        /// </param>
        /// <returns>
        /// The error code vector.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="dataKeyForErrorCodesVector"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="dataKeyForErrorCodesVector"/> is white space.</exception>
        public static string GetErrorCodesVector(
            this Exception exception,
            string dataKeyForErrorCodesVector = Constants.ExceptionDataKeyForErrorCodesVector)
        {
            new { exception }.Must().NotBeNull();
            new { dataKeyForErrorCodesVector }.Must().NotBeNullNorWhiteSpace();

            var result = GetErrorCodeString(exception, dataKeyForErrorCodesVector);

            return result;
        }

        private static string BuildErrorCodesVector(
            Exception exception)
        {
            var result = exception.GetErrorCode();
            if (exception is AggregateException aggregateException)
            {
                if (aggregateException.InnerExceptions != null)
                {
                    var innerExceptions = aggregateException.InnerExceptions.Where(_ => _ != null).ToArray();
                    if (innerExceptions.Any())
                    {
                        var innerExceptionsErrorCodeVectors = innerExceptions.Select(_ => Invariant($"{BuildErrorCodesVector(_)}")).Where(_ => !string.IsNullOrWhiteSpace(_)).ToArray();

                        var innerExceptionsErrorCodeVectorsCsv = string.Join(", ", innerExceptionsErrorCodeVectors);
                        if (!string.IsNullOrEmpty(innerExceptionsErrorCodeVectorsCsv))
                        {
                            if (result != null)
                            {
                                result = Invariant($"{result} -> ");
                            }

                            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                            if (innerExceptionsErrorCodeVectors.Length == 1)
                            {
                                result = Invariant($"{result}{innerExceptionsErrorCodeVectorsCsv}");
                            }
                            else
                            {
                                result = Invariant($"{result}[{innerExceptionsErrorCodeVectorsCsv}]");
                            }
                        }
                    }
                }
            }
            else
            {
                if (exception.InnerException != null)
                {
                    if (result != null)
                    {
                        result = Invariant($"{result} -> ");
                    }

                    result = Invariant($"{result}{BuildErrorCodesVector(exception.InnerException)}");
                }
            }

            return result;
        }

        private static string GetErrorCodeString(
            Exception exception,
            string dataKey)
        {
            string result = null;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (exception.Data != null)
            {
                if (exception.Data.Keys.OfType<string>().Contains(dataKey))
                {
                    result = exception.Data[dataKey] as string;
                }
            }

            return result;
        }
    }
}