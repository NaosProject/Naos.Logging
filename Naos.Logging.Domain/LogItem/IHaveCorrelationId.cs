// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHaveCorrelationId.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System.ComponentModel;

    /// <summary>
    /// Identifies an object that has a correlation identifier.
    /// </summary>
    [Bindable(true)]
    public interface IHaveCorrelationId
    {
        /// <summary>
        /// Gets an identifier used to correlate multiple log-items.
        /// </summary>
        string CorrelationId { get; }
    }
}
