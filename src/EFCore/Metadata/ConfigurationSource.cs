// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#nullable enable

namespace Microsoft.EntityFrameworkCore.Metadata
{
    /// <summary>
    ///     <para>
    ///         Indicates whether an element in the <see cref="IMutableModel" /> was specified explicitly
    ///         using the fluent API in <see cref="DbContext.OnModelCreating" />, through use of a
    ///         .NET attribute (data annotation), or by convention via the EF Core model building conventions.
    ///     </para>
    ///     <para>
    ///         This type is typically used by database providers (and other extensions). It is generally
    ///         not used in application code.
    ///     </para>
    /// </summary>
    public enum ConfigurationSource
    {
        /// <summary>
        ///     Indicates that the model element was explicitly specified using the fluent API in
        ///     <see cref="DbContext.OnModelCreating" />.
        /// </summary>
        Explicit,

        /// <summary>
        ///     Indicates that the model element was specified through use of a .NET attribute (data annotation).
        /// </summary>
        DataAnnotation,

        /// <summary>
        ///     Indicates that the model element was specified by convention via the EF Core model building conventions.
        /// </summary>
        Convention
    }
}
