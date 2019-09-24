// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Justifications.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package. Will be overwritten with package update except in Naos.Build source.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Standard justifications for analysis suppression.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class Justifications
    {
        public const string IntendToMakeMeaninfulDecisionFromGeneralException =
            "Do not mind having broad access to thrown exceptions because there is an intentions to make some meaningful decision or log and suppress for stability.";
    }
}
