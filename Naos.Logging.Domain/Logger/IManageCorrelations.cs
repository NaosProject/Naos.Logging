// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManageCorrelations.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using OBeautifulCode.Error.Recipes;

    /// <summary>
    /// Interface for a client facing logger.
    /// </summary>
    public interface IManageCorrelations
    {
        /// <summary>
        /// Make a shallow clone that can have new correlations but will also persist the existing correlations in sync with the configured ones.
        /// </summary>
        /// <returns>Cloned <see cref="IManageCorrelations" />.</returns>
        IManageCorrelations ShallowClone();

        /// <summary>
        /// Add a subject correlation to be put on each message.
        /// </summary>
        /// <param name="correlatingSubjectFunc">Subject to use.</param>
        /// <param name="correlationId">Optional correlation id; DEFAULT is new Guid.</param>
        void AddSubjectCorrelation(Func<object> correlatingSubjectFunc, string correlationId = null);

        /// <summary>
        /// Add a new elapsed time correlation to be added to each message.
        /// </summary>
        /// <param name="correlationId">Optional correlation id; DEFAULT is new Guid.</param>
        void AddElapsedCorrelation(string correlationId = null);

        /// <summary>
        /// Add a new order correlation that adds position to each message.
        /// </summary>
        /// <param name="startingIndex">Optional starting index; DEFAULT is 0.</param>
        /// <param name="correlationId">Optional correlation id; DEFAULT is new Guid.</param>
        void AddOrderCorrelation(int startingIndex = 0, string correlationId = null);

        /// <summary>
        /// Prepare for exception correlations by setting the error code key or a specific correlation ID.
        /// </summary>
        /// <param name="errorCodeKey">Error code key to use to extract error code if any; DEFAULT is OBC Error Code Key, empty collection will disable feature.</param>
        /// <param name="correlationId">Optional correlation id; DEFAULT is new Guid.</param>
        void PrepareExceptionCorrelations(
            IReadOnlyCollection<string> errorCodeKey = null,
            string correlationId = null);

        /// <summary>
        /// Add additional arbitrary correlations to be used for each message.
        /// </summary>
        /// <param name="newAdditionalCorrelations">Additional Correlations that have already been prepared.</param>
        void AddAdditionalCorrelations(IReadOnlyCollection<IHaveCorrelationId> newAdditionalCorrelations);

        /// <summary>
        /// Gets the next correlations and advances any that require.
        /// </summary>
        /// <returns>The set of correlations to be logged next.</returns>
        IReadOnlyCollection<IHaveCorrelationId> GetNextCorrelations();

        /// <summary>
        /// Gets the correlations that are derived from the exception provided, if any.
        /// </summary>
        /// <param name="exception">Exception to examine.</param>
        /// <returns>The set of correlations associated with the exception; if none then empty collection.</returns>
        IReadOnlyCollection<IHaveCorrelationId> GetExceptionCorrelations(Exception exception);
    }
}