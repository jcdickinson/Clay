#region License

// Copyright (c) K2 Workflow (SourceCode Technology Holdings Inc.). All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SourceCode.Clay
{
    /// <summary>
    /// Represents a <see cref="Sha256"/> value.
    /// </summary>
    /// <seealso cref="System.Security.Cryptography.SHA256" />
    /// <seealso cref="System.IEquatable{T}" />
    /// <seealso cref="System.IComparable{T}" />
    [DebuggerDisplay("{ToString(\"D\"),nq,ac}")]
    [StructLayout(LayoutKind.Sequential, Size = ByteLength)]
    public readonly struct Sha256 : IEquatable<Sha256>, IComparable<Sha256>
    {
        /// <summary>
        /// The standard byte length of a <see cref="Sha256"/> value.
        /// </summary>
        public const byte ByteLength = 32;

        /// <summary>
        /// The number of hex characters required to represent a <see cref="Sha256"/> value.
        /// </summary>
        public const byte HexLength = ByteLength * 2;

        // We choose to use value types for primary storage so that we can live on the stack
        // TODO: In C#8 we can use 'readonly fixed byte'
        // https://github.com/dotnet/csharplang/issues/1502

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = ByteLength)]
        private unsafe struct Block // Avoids making main struct unsafe
        {
#pragma warning disable IDE0044 // Add readonly modifier
            public fixed byte Bytes[ByteLength];
#pragma warning restore IDE0044 // Add readonly modifier
        }

        private readonly Block _block;

        /// <summary>
        /// Deserializes a <see cref="Sha256"/> value from the provided <see cref="ReadOnlyMemory{T}"/>.
        /// </summary>
        /// <param name="source">The buffer.</param>
        public Sha256(ReadOnlySpan<byte> source)
            : this() // Compiler doesn't know we're indirectly setting all the fields
        {
            if (source.Length < ByteLength) throw new ArgumentOutOfRangeException(nameof(source));

            ReadOnlySpan<byte> src = source.Slice(0, ByteLength);

            unsafe
            {
                fixed (byte* ptr = _block.Bytes)
                {
                    var dst = new Span<byte>(ptr, ByteLength);
                    src.CopyTo(dst);
                }
            }
        }

#if NETSTANDARD2_0
        /// <summary>
        /// Deserializes a <see cref="Sha1"/> value from the provided array.
        /// </summary>
        /// <param name="source">The buffer.</param>
        public Sha256(byte[] source)
            : this(new ReadOnlySpan<byte>(source))
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
        }
#endif

        /// <summary>
        /// Copies the <see cref="Sha256"/> value to the provided buffer.
        /// </summary>
        /// <param name="destination">The buffer to copy to.</param>
        public void CopyTo(Span<byte> destination)
        {
            unsafe
            {
                fixed (byte* ptr = _block.Bytes)
                {
                    var src = new ReadOnlySpan<byte>(ptr, ByteLength);
                    src.CopyTo(destination);
                }
            }
        }

        /// <summary>
        /// Tries to copy the <see cref="Sha256"/> value to the provided buffer.
        /// </summary>
        /// <param name="destination">The buffer to copy to.</param>
        /// <returns>True if successful</returns>
        public bool TryCopyTo(Span<byte> destination)
        {
            unsafe
            {
                fixed (byte* ptr = _block.Bytes)
                {
                    var src = new ReadOnlySpan<byte>(ptr, ByteLength);
                    return src.TryCopyTo(destination);
                }
            }
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Sha256"/> instance using the 'n' format.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToString("n");

        /// <summary>
        /// Returns a string representation of the <see cref="Sha256"/> instance.
        /// n: cdc76e5c9914fb9281a1c7e284d73e67f1809a48a497200e046d39ccc7112cd0,
        /// d: cdc76e5c-9914fb92-81a1c7e2-84d73e67-f1809a48-a497200e-046d39cc-c7112cd0,
        /// s: cdc76e5c 9914fb92 81a1c7e2 84d73e67 f1809a48 a497200e 046d39cc c7112cd0
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
                throw new FormatException($"Empty format specification");

            if (format.Length != 1)
                throw new FormatException($"Invalid format specification length {format.Length}");

            unsafe
            {
                fixed (byte* ptr = _block.Bytes)
                {
                    var sha = new ReadOnlySpan<byte>(ptr, ByteLength);

                    switch (format[0])
                    {
                        // cdc76e5c9914fb9281a1c7e284d73e67f1809a48a497200e046d39ccc7112cd0
                        case 'n': return ShaUtil.ToString(sha, ShaUtil.HexCasing.Lower);
                        case 'N': return ShaUtil.ToString(sha, ShaUtil.HexCasing.Upper);

                        // cdc76e5c-9914fb92-81a1c7e2-84d73e67-f1809a48-a497200e-046d39cc-c7112cd0
                        case 'd': return ShaUtil.ToString(sha, '-', ShaUtil.HexCasing.Lower);
                        case 'D': return ShaUtil.ToString(sha, '-', ShaUtil.HexCasing.Upper);

                        // cdc76e5c 9914fb92 81a1c7e2 84d73e67 f1809a48 a497200e 046d39cc c7112cd0
                        case 's': return ShaUtil.ToString(sha, ' ', ShaUtil.HexCasing.Lower);
                        case 'S': return ShaUtil.ToString(sha, ' ', ShaUtil.HexCasing.Upper);
                    }
                }
            }

            throw new FormatException($"Invalid format specification '{format}'");
        }

        /// <summary>
        /// Converts the <see cref="Sha256"/> instance to a string using the 'n' or 'N' format,
        /// and returns the value split into two tokens.
        /// </summary>
        /// <param name="prefixLength">The length of the first token.</param>
        /// <param name="uppercase">If True, output uppercase, else output lowercase.</param>
        public KeyValuePair<string, string> Split(int prefixLength, bool uppercase = false)
        {
            ShaUtil.HexCasing casing = uppercase ? ShaUtil.HexCasing.Upper : ShaUtil.HexCasing.Lower;

            unsafe
            {
                fixed (byte* ptr = _block.Bytes)
                {
                    var sha = new ReadOnlySpan<byte>(ptr, ByteLength);

                    KeyValuePair<string, string> kvp = ShaUtil.Split(sha, prefixLength, casing);
                    return kvp;
                }
            }
        }

        /// <summary>
        /// Tries to parse the specified hexadecimal.
        /// </summary>
        /// <param name="hex">The hexadecimal.</param>
        /// <param name="value">The value.</param>
        public static bool TryParse(ReadOnlySpan<char> hex, out Sha256 value)
        {
            value = default;

            Span<byte> sha = stackalloc byte[ByteLength];
            if (!ShaUtil.TryParse(hex, sha))
                return false;

            value = new Sha256(sha);
            return true;
        }

        /// <summary>
        /// Tries to parse the specified hexadecimal.
        /// </summary>
        /// <param name="hex">The hexadecimal.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool TryParse(string hex, out Sha256 value)
        {
            value = default;

            if (hex is null || hex.Length < HexLength)
                return false;

            Span<byte> sha = stackalloc byte[ByteLength];
            if (!ShaUtil.TryParse(hex.AsSpan(), sha))
                return false;

            value = new Sha256(sha);
            return true;
        }

        /// <summary>
        /// Parses the specified hexadecimal.
        /// </summary>
        /// <param name="hex">The hexadecimal.</param>
        /// <returns></returns>
        /// <exception cref="FormatException">Sha256</exception>
        public static Sha256 Parse(ReadOnlySpan<char> hex)
        {
            Span<byte> sha = stackalloc byte[ByteLength];
            if (!ShaUtil.TryParse(hex, sha))
                throw new FormatException($"Input was not recognized as a valid {nameof(Sha256)}");

            var value = new Sha256(sha);
            return value;
        }

        /// <summary>
        /// Parses the specified hexadecimal.
        /// </summary>
        /// <param name="hex">The hexadecimal.</param>
        /// <returns></returns>
        /// <exception cref="FormatException">Sha256</exception>
        public static Sha256 Parse(string hex)
        {
            if (hex is null)
                throw new ArgumentNullException(nameof(hex));

            if (hex.Length < HexLength)
                throw new FormatException($"Input was not recognized as a valid {nameof(Sha256)}");

            Span<byte> sha = stackalloc byte[ByteLength];
            if (!ShaUtil.TryParse(hex.AsSpan(), sha))
                throw new FormatException($"Input was not recognized as a valid {nameof(Sha256)}");

            var value = new Sha256(sha);
            return value;
        }

        public bool Equals(Sha256 other)
            => CompareTo(other) == 0;

        public override bool Equals(object obj)
            => obj is Sha256 other
            && Equals(other);

        public override int GetHashCode()
        {
            unsafe
            {
                fixed (byte* b = _block.Bytes)
                {
#if NETCOREAPP2_2
                    int hc = HashCode.Combine(b[00], b[01], b[02], b[03], b[04], b[05], b[06], b[07]);
                    hc = HashCode.Combine(hc, b[08], b[09], b[10], b[11], b[12], b[13], b[14]);
                    hc = HashCode.Combine(hc, b[15], b[16], b[17], b[18], b[19], b[20], b[21]);
                    hc = HashCode.Combine(hc, b[22], b[23], b[24], b[25], b[26], b[27], b[28]);
                    hc = HashCode.Combine(hc, b[29], b[30], b[31]);
#else
                    int hc = 11;

                    unchecked
                    {
                        hc = hc * 7 + b[00];
                        hc = hc * 7 + b[01];
                        hc = hc * 7 + b[02];
                        hc = hc * 7 + b[03];
                        hc = hc * 7 + b[04];
                        hc = hc * 7 + b[05];
                        hc = hc * 7 + b[06];
                        hc = hc * 7 + b[07];

                        hc = hc * 7 + b[08];
                        hc = hc * 7 + b[09];
                        hc = hc * 7 + b[10];
                        hc = hc * 7 + b[11];
                        hc = hc * 7 + b[12];
                        hc = hc * 7 + b[13];
                        hc = hc * 7 + b[14];

                        hc = hc * 7 + b[15];
                        hc = hc * 7 + b[16];
                        hc = hc * 7 + b[17];
                        hc = hc * 7 + b[18];
                        hc = hc * 7 + b[19];
                        hc = hc * 7 + b[20];
                        hc = hc * 7 + b[21];

                        hc = hc * 7 + b[22];
                        hc = hc * 7 + b[23];
                        hc = hc * 7 + b[24];
                        hc = hc * 7 + b[25];
                        hc = hc * 7 + b[26];
                        hc = hc * 7 + b[27];
                        hc = hc * 7 + b[28];

                        hc = hc * 7 + b[29];
                        hc = hc * 7 + b[30];
                        hc = hc * 7 + b[31];
                    }
#endif
                    return hc;
                }
            }
        }

        public int CompareTo(Sha256 other)
        {
            unsafe
            {
                fixed (Block* src = &_block)
                {
                    Block* dst = &other._block;

                    int cmp = ShaUtil.NativeMethods.MemCompare((byte*)src, (byte*)dst, ByteLength);
                    return cmp;
                }
            }
        }

        public static bool operator ==(Sha256 x, Sha256 y) => x.Equals(y);

        public static bool operator !=(Sha256 x, Sha256 y) => !(x == y);

        public static bool operator >=(Sha256 x, Sha256 y) => x.CompareTo(y) >= 0;

        public static bool operator >(Sha256 x, Sha256 y) => x.CompareTo(y) > 0;

        public static bool operator <=(Sha256 x, Sha256 y) => x.CompareTo(y) <= 0;

        public static bool operator <(Sha256 x, Sha256 y) => x.CompareTo(y) < 0;
    }
}
