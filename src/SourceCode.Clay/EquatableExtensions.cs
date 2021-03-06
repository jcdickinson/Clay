#region License

// Copyright (c) K2 Workflow (SourceCode Technology Holdings Inc.). All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

#endregion

using System;
using System.Runtime.CompilerServices;

namespace SourceCode.Clay
{
    /// <summary>
    /// Represents <see cref="IEquatable{T}"/> extensions.
    /// </summary>
    public static class EquatableExtensions
    {
        #region Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NullableEquals<T>(this T x, T y)
            where T : IEquatable<T>
        {
            if (ReferenceEquals(x, null)) return ReferenceEquals(y, null); // (null, null) or (null, y)
            if (ReferenceEquals(y, null)) return false; // (x, null)
            if (ReferenceEquals(x, y)) return true; // (x, x)

            return x.Equals(y);
        }

        #endregion
    }
}
