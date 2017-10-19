#region License

// Copyright (c) K2 Workflow (SourceCode Technology Holdings Inc.). All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;

namespace SourceCode.Clay.Collections.Generic
{
    /// <summary>
    /// Represents extensions for <see cref="ISet{T}"/>.
    /// </summary>
    public static class SetExtensions
    {
        #region Methods

        /// <summary>
        /// Performs an efficient item-by-item comparison, using a custom <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <param name="x">Set 1</param>
        /// <param name="y">Set 2</param>
        /// <param name="comparer">The comparer to use to test for equality.</param>
        /// <returns></returns>
        public static bool NullableSetEquals<T>(this IEnumerable<T> x, IEnumerable<T> y, IEqualityComparer<T> comparer)
        {
            if (x is null ^ y is null) return false; // (x, null) or (null, y)
            if (x is null) return true; // (null, null)
            if (ReferenceEquals(x, y)) return true; // (x, x)

            // ICollection is more common
            if (x is ICollection<T> xc)
            {
                var isEqual = CheckCount(xc.Count);
                if (isEqual.HasValue) return isEqual.Value;
            }
            // IReadOnlyCollection
            else if (x is IReadOnlyCollection<T> xrc)
            {
                var isEqual = CheckCount(xrc.Count);
                if (isEqual.HasValue) return isEqual.Value;
            }

            var cmpr = comparer ?? EqualityComparer<T>.Default;

            // ISet
            var xss = new HashSet<T>(x, cmpr);
            var yss = new HashSet<T>(y, cmpr);

            return xss.SetEquals(yss);

            // Local functions
            bool? CheckCount(int xCount)
            {
                // ICollection is more common
                if (y is ICollection<T> yc)
                {
                    if (xCount != yc.Count) return false; // (n, m)
                    if (xCount == 0) return true; // (0, 0)
                }
                // IReadOnlyCollection
                else if (y is IReadOnlyCollection<T> yrc)
                {
                    if (xCount != yrc.Count) return false; // (n, m)
                    if (xCount == 0) return true; // (0, 0)
                }

                return null;
            }
        }

        /// <summary>
        /// Performs an efficient item-by-item comparison, using the <see cref="IEqualityComparer{T}"/> from the first set.
        /// </summary>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <param name="x">Set 1</param>
        /// <param name="y">Set 2</param>
        /// <returns></returns>
        public static bool NullableSetEquals<T>(this IEnumerable<T> x, IEnumerable<T> y)
            => NullableSetEquals(x, y, null);

        #endregion
    }
}