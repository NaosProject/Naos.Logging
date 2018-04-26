﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="OBeautifulCode">
//   Copyright (c) OBeautifulCode. All rights reserved.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package. Will be overwritten with package update except in OBeautifulCode.Enum source.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Enum.Recipes
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Spritely.Recipes;

    /// <summary>
    /// Adds some convenient extension methods to enums.
    /// </summary>
#if !OBeautifulCodeEnumRecipesProject
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("OBeautifulCode.Enum", "See package version number")]
#endif
#if !OBeautifulCodeEnumRecipesProject
    internal
#else
    public
#endif
    static class EnumExtensions
    {
        /// <summary>
        /// Gets the members/values of a specified enum.
        /// </summary>
        /// <typeparam name="TEnum">The type of enum.</typeparam>
        /// <returns>
        /// The members/values of the specified enum.  
        /// For flags enums, returns all individual and combined flags.
        /// </returns>
        /// <exception cref="ArgumentException"><typeparamref name="TEnum"/> is not an enum.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", Justification = "This is a developer-facing string, not a user-facing string.")]
        public static IReadOnlyCollection<TEnum> GetEnumValues<TEnum>()
            where TEnum : struct
        {
            typeof(TEnum).IsEnum.Named($"typeof {nameof(TEnum)} is Enum").Must().BeTrue().OrThrow();
            var results = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList().AsReadOnly();
            return results;
        }

        /// <summary>
        /// Gets the members/values of a specified enum.
        /// </summary>
        /// <param name="enumType">The enum type.</param>
        /// <returns>
        /// The members/values of the specified enum.
        /// For flags enums, returns all individual and combined flags.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="enumType"/> is not an enum.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", Justification = "This is a developer-facing string, not a user-facing string.")]
        public static IReadOnlyCollection<Enum> GetEnumValues(
            this Type enumType)
        {
            enumType.IsEnum.Named($"{nameof(enumType)} is Enum").Must().BeTrue().OrThrow();
            var results = Enum.GetValues(enumType).Cast<Enum>().ToList().AsReadOnly();
            return results;
        }

        /// <summary>
        /// Gets the flags of a flags enum, with a preference for returning combined flags
        /// instead of individual flags where the enum value uses combined flags.
        /// </summary>
        /// <param name="value">The enum value to decompose into it's flags.</param>
        /// <remarks>
        /// Adapted from: <a href="http://stackoverflow.com/a/4171168/356790" />
        /// </remarks>
        /// <returns>
        /// The flags of the specified enum, with combined flags instead of individual flags where possible.
        /// No bit will be repeated.  Thus, if two combined flags are represented in the value and they
        /// have an overlapping individual flag, only one of those combined flags will be returned and 
        /// the other will be decomposed into it's non-overlapping individual flags.
        /// If value is 0, then a collection with only the 0 value is returned.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "'Flags' is the most appropriate term here.")]
        public static IReadOnlyCollection<Enum> GetFlagsCombinedWherePossible(this Enum value)
        {
            new { value }.Must().NotBeNull().OrThrow();

            var result = GetFlags(value, GetEnumValues(value.GetType()).ToArray()).ToList().AsReadOnly();
            return result;
        }

        /// <summary>
        /// Gets the flags of a flags enum, with a preference for returning combined flags
        /// instead of individual flags where the enum value uses combined flags.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value to decompose into it's flags.</param>
        /// <remarks>
        /// Adapted from: <a href="http://stackoverflow.com/a/4171168/356790" />
        /// </remarks>
        /// <returns>
        /// The flags of the specified enum, with combined flags instead of individual flags where possible.
        /// No bit will be repeated.  Thus, if two combined flags are represented in the value and they
        /// have an overlapping individual flag, only one of those combined flags will be returned and 
        /// the other will be decomposed into it's non-overlapping individual flags.
        /// If value is 0, then a collection with only the 0 value is returned.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException"><typeparamref name="TEnum"/> is not an enum.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "'Flags' is the most appropriate term here.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", Justification = "This is a developer-facing string, not a user-facing string.")]
        public static IReadOnlyCollection<TEnum> GetFlagsCombinedWherePossible<TEnum>(this Enum value)
            where TEnum : struct
        {
            new { value }.Must().NotBeNull().OrThrow();
            typeof(TEnum).IsEnum.Named($"typeof {nameof(TEnum)} is Enum").Must().BeTrue().OrThrow();

            var result = GetFlags(value, GetEnumValues(value.GetType()).ToArray()).Cast<TEnum>().ToList().AsReadOnly();
            return result;
        }

        /// <summary>
        /// Checks if there is any overlap between the two <see cref="FlagsAttribute" /> enumerations.
        /// </summary>
        /// <param name="first">First to check.</param>
        /// <param name="second">Second to check.</param>
        /// <returns>Value indicating whether there is any overlap.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag", Justification = "'Flag' is the most appropriate term here.")]
        public static bool HasFlagOverlap(this Enum first, Enum second)
        {
            new { first, second }.Must().NotBeNull().OrThrowFirstFailure();

            var ret = first.GetIndividualFlags().Intersect(second.GetIndividualFlags()).Any();
            return ret;
        }

        /// <summary>
        /// Gets the individual flags of a flags enum.
        /// </summary>
        /// <param name="value">The enum value to decompose into it's individual flags.</param>
        /// <remarks>
        /// Adapted from: <a href="http://stackoverflow.com/a/4171168/356790" />
        /// </remarks>
        /// <returns>
        /// The individuals flags of the specified flags enum.
        /// If value is 0, then a collection with only the 0 value is returned.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "'Flags' is the most appropriate term here.")]
        public static IReadOnlyCollection<Enum> GetIndividualFlags(this Enum value)
        {
            new { value }.Must().NotBeNull().OrThrow();

            var result = GetFlags(value, GetFlagValues(value.GetType()).ToArray()).ToList().AsReadOnly();
            return result;
        }

        /// <summary>
        /// Gets the individual flags of a flags enum.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value to decompose into it's individual flags.</param>
        /// <remarks>
        /// Adapted from: <a href="http://stackoverflow.com/a/4171168/356790" />
        /// </remarks>
        /// <returns>
        /// The individuals flags of the specified flags enum.
        /// If value is 0, then a collection with only the 0 value is returned.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException"><typeparamref name="TEnum"/> is not an enum.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "'Flags' is the most appropriate term here.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", Justification = "This is a developer-facing string, not a user-facing string.")]
        public static IReadOnlyCollection<TEnum> GetIndividualFlags<TEnum>(this Enum value)
            where TEnum : struct
        {
            new { value }.Must().NotBeNull().OrThrow();
            typeof(TEnum).IsEnum.Named($"typeof {nameof(TEnum)} is Enum").Must().BeTrue().OrThrow();

            var result = GetFlags(value, GetFlagValues(value.GetType()).ToArray()).Cast<TEnum>().ToList().AsReadOnly();
            return result;
        }

        private static IEnumerable<Enum> GetFlags(Enum value, IList<Enum> values)
        {
            ulong bits = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
            List<Enum> results = new List<Enum>();
            for (int i = values.Count - 1; i >= 0; i--)
            {
                ulong mask = Convert.ToUInt64(values[i], CultureInfo.InvariantCulture);
                if (i == 0 && mask == 0L)
                {
                    break;
                }

                if ((bits & mask) == mask)
                {
                    results.Add(values[i]);
                    bits -= mask;
                }
            }

            if (bits != 0L)
            {
                return Enumerable.Empty<Enum>();
            }

            if (Convert.ToUInt64(value, CultureInfo.InvariantCulture) != 0L)
            {
                return results.Reverse<Enum>();
            }

            if (bits == Convert.ToUInt64(value, CultureInfo.InvariantCulture) && values.Count > 0 &&
                Convert.ToUInt64(values[0], CultureInfo.InvariantCulture) == 0L)
            {
                return values.Take(1);
            }

            return Enumerable.Empty<Enum>();
        }

        private static IEnumerable<Enum> GetFlagValues(Type enumType)
        {
            ulong flag = 0x1;
            foreach (var value in Enum.GetValues(enumType).Cast<Enum>())
            {
                ulong bits = Convert.ToUInt64(value);
                if (bits == 0L)
                {
                    yield return value;
                }

                while (flag < bits)
                {
                    flag <<= 1;
                }

                if (flag == bits)
                {
                    yield return value;
                }
            }
        }
    }
}
