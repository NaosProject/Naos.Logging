// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessHelpers.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Diagnostics.Domain
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Various helper methods related to a processes.
    /// </summary>
#if !NaosDiagnosticsRecipes
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("Naos.Diagnostics", "See package version number")]
#endif
    internal static class ProcessHelpers
    {
        /// <summary>
        /// Gets the currently running process.
        /// </summary>
        /// <returns>
        /// The currently running process.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "The caller will be the consumer of this recipe.")]
        public static Process GetRunningProcess()
        {
            var result = Process.GetCurrentProcess();
            return result;
        }

        /// <summary>
        /// Gets the process' name.
        /// </summary>
        /// <param name="process">The process</param>
        /// <returns>
        /// The process' name.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "The caller will be the consumer of this recipe.")]
        public static string Name(
            this Process process)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            var result = process.ProcessName;
            return result;
        }

        /// <summary>
        /// Gets the file path of a process.
        /// </summary>
        /// <param name="process">The process</param>
        /// <returns>
        /// The file path of the a process.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "The caller will be the consumer of this recipe.")]
        public static string FilePath(
            this Process process)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            var result = process.MainModule.FileName;
            return result;
        }

        /// <summary>
        /// Gets the file version of a process.
        /// </summary>
        /// <param name="process">The process</param>
        /// <returns>
        /// The file version of a process.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "The caller will be the consumer of this recipe.")]
        public static string FileVersion(
            this Process process)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            var result = process.MainModule.FileVersionInfo.FileVersion;
            return result;
        }

        /// <summary>
        /// Gets the product version of a process.
        /// </summary>
        /// <param name="process">The process</param>
        /// <returns>
        /// The product version of a process.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "The caller will be the consumer of this recipe.")]
        public static string ProductVersion(
            this Process process)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            var result = process.MainModule.FileVersionInfo.ProductVersion;
            return result;
        }
    }
}
