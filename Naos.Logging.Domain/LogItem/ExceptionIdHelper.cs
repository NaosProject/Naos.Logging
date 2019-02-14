// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionIdHelper.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using static System.FormattableString;

    /// <summary>
    /// Helper to deal with correlations of <see cref="Exception" />'s.
    /// </summary>
    public static class ExceptionIdHelper
    {
        /// <summary>
        /// Key for the <see cref="Exception" />'s data dictionary to save the exception ID so if it's wrapped it will maintain the same exception correlation ID.
        /// </summary>
        public const string ExceptionIdKey = "__Naos_Log_ExceptionID__";

        /// <summary>
        /// Finds the exception the in the inner exception chain that contains a <see cref="ExceptionIdKey" /> in its Data dictionary, or null if not found.
        /// </summary>
        /// <param name="loggedException">Exception that is being logged.</param>
        /// <returns>The exception the in the inner exception chain that contains a <see cref="ExceptionIdKey" /> in its Data dictionary, or null if not found.</returns>
        public static Exception FindFirstExceptionInChainWithExceptionId(this Exception loggedException)
        {
            var iteratingException = loggedException;

            while (iteratingException != null)
            {
                if (iteratingException.Data.Contains(ExceptionIdKey))
                {
                    return iteratingException;
                }

                iteratingException = iteratingException.InnerException;
            }

            return null;
        }

        /// <summary>
        /// Generate and write an exception correlation ID into the provided exception's data dictionary with key <see cref="ExceptionIdKey" /> and return the new value.
        /// </summary>
        /// <param name="loggedException">Exception that is being logged.</param>
        /// <returns>New ID that is recorded to the exception's data dictionary with key <see cref="ExceptionIdKey" />.</returns>
        public static Guid GenerateAndWriteExceptionIdIntoExceptionData(this Exception loggedException)
        {
            if (loggedException == null)
            {
                return Guid.Empty;
            }

            if (loggedException.Data.Contains(ExceptionIdKey))
            {
                return Guid.Empty;
            }

            var result = Guid.NewGuid();
            loggedException.Data[ExceptionIdKey] = result;
            return result;
        }

        /// <summary>
        /// Extracts the exception ID from the exception's data dictionary if present, otherwise it will return <see cref="Guid.Empty" />.
        /// </summary>
        /// <param name="correlatingException">Exception to check.</param>
        /// <param name="searchInnerExceptionChain">Optional value to examine the inner exception chain for the value, DEFAULT is false.</param>
        /// <returns>Exception ID if found.</returns>
        public static Guid GetExceptionIdFromExceptionData(this Exception correlatingException, bool searchInnerExceptionChain = false)
        {
            var localException = correlatingException;

            do
            {
                if (localException.Data.Contains(ExceptionIdKey))
                {
                    var exceptionIdString = localException.Data[ExceptionIdKey]?.ToString();
                    Guid.TryParse(exceptionIdString, out var result);
                    return result;
                }
                else
                {
                    localException = localException.InnerException;
                }
            }
            while (localException != null && searchInnerExceptionChain);

            return Guid.Empty;
        }
    }
}