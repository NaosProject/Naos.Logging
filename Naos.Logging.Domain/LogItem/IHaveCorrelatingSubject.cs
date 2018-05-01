// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHaveCorrelatingSubject.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System.ComponentModel;

    /// <summary>
    /// Correlates log-items based on a correlating subject.
    /// </summary>
    [Bindable(true)]
    public interface IHaveCorrelatingSubject : IHaveCorrelationId
    {
        /// <summary>
        /// Gets the correlating subject.
        /// </summary>
        Subject CorrelatingSubject { get; }
    }
}
