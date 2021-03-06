#region License

// Copyright (c) K2 Workflow (SourceCode Technology Holdings Inc.). All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

#endregion

namespace SourceCode.Clay.OpenApi
{
    /// <summary>
    /// Represents the different locations a parameter can occur.
    /// </summary>
#pragma warning disable CA1028 // Enum Storage should be Int32

    public enum ParameterLocation : byte
#pragma warning restore CA1028 // Enum Storage should be Int32
    {
        /// <summary>
        /// The parameter occurs in the query.
        /// </summary>
        Query = 0,

        /// <summary>
        /// The parameter occurs in the header.
        /// </summary>
        Header = 1,

        /// <summary>
        /// The parameter occurs in the path.
        /// </summary>
        Path = 2,

        /// <summary>
        /// The parameter occurs in a cookie.
        /// </summary>
        Cookie = 3
    }
}
