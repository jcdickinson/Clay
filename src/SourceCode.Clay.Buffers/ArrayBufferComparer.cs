﻿using System;

namespace SourceCode.Clay.Buffers
{
    /// <summary>
    /// Represents a way to compare the contents of <see cref="Byte[]"/>.
    /// </summary>
    public sealed class ArrayBufferComparer : BufferComparer<byte[]>
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="ArrayBufferComparer"/> class, that considers the full
        /// buffer when calculating the hashcode.
        /// </summary>
        public ArrayBufferComparer()
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="ArrayBufferComparer"/> class.
        /// </summary>
        /// <param name="hashCodeFidelity">
        /// The maximum number of octets that processed when calculating a hashcode. Pass zero or a negative value to
        /// disable the limit.
        /// </param>
        public ArrayBufferComparer(int hashCodeFidelity)
            : base(hashCodeFidelity)
        { }

        #endregion

        #region IComparer

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.
        /// </returns>
        public override int Compare(byte[] x, byte[] y)
            => BufferComparer.CompareArray(x, y);

        #endregion

        #region IEqualityComparer

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public override bool Equals(byte[] x, byte[] y)
            => BufferComparer.CompareArray(x, y) == 0;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode(byte[] obj)
        {
            if (obj == null) return HashCode.Fnv((byte[])null);

            var span = new ReadOnlySpan<byte>(obj);
            if (HashCodeFidelity > 0 && span.Length > HashCodeFidelity)
                span = span.Slice(0, HashCodeFidelity);

            var fnv = HashCode.Fnv(span);
            return fnv;
        }

        #endregion
    }
}
