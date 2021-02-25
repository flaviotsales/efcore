// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

#nullable enable

namespace Microsoft.EntityFrameworkCore.Sqlite.Query.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class SqliteByteArrayMethodTranslator : IMethodCallTranslator
    {
        private readonly ISqlExpressionFactory _sqlExpressionFactory;
        private readonly IRelationalTypeMappingSource _typeMappingSource;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public SqliteByteArrayMethodTranslator(
            [NotNull] ISqlExpressionFactory sqlExpressionFactory,
            [NotNull] IRelationalTypeMappingSource typeMappingSource)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
            _typeMappingSource = typeMappingSource;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual SqlExpression? Translate(
            SqlExpression? instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            Check.NotNull(method, nameof(method));
            Check.NotNull(arguments, nameof(arguments));
            Check.NotNull(logger, nameof(logger));

            if (method.IsGenericMethod
                && method.GetGenericMethodDefinition().Equals(EnumerableMethods.Contains)
                && arguments[0].Type == typeof(byte[]))
            {
                var source = arguments[0];

                var value = arguments[1] is SqlConstantExpression constantValue
                    ? (SqlExpression)_sqlExpressionFactory.Constant(new[] { (byte)constantValue.Value! }, source.TypeMapping)
                    : _sqlExpressionFactory.Function(
                        "char",
                        new[] { arguments[1] },
                        nullable: false,
                        argumentsPropagateNullability: new[] { false },
                        typeof(string));

                return _sqlExpressionFactory.GreaterThan(
                    _sqlExpressionFactory.Function(
                        "instr",
                        new[] { source, value },
                        nullable: true,
                        argumentsPropagateNullability: new[] { true, true },
                        typeof(int)),
                    _sqlExpressionFactory.Constant(0));
            }

            // See issue#16428
            //if (method.IsGenericMethod
            //    && method.GetGenericMethodDefinition().Equals(EnumerableMethods.FirstWithoutPredicate)
            //    && arguments[0].Type == typeof(byte[]))
            //{
            //    return _sqlExpressionFactory.Function(
            //        "unicode",
            //        new SqlExpression[]
            //        {
            //            _sqlExpressionFactory.Function(
            //                "substr",
            //                new SqlExpression[]
            //                {
            //                    arguments[0],
            //                    _sqlExpressionFactory.Constant(1),
            //                    _sqlExpressionFactory.Constant(1)
            //                },
            //                nullable: true,
            //                argumentsPropagateNullability: new[] { true, true, true },
            //                typeof(byte[]))
            //        },
            //        nullable: true,
            //        argumentsPropagateNullability: new[] { true },
            //        method.ReturnType);
            //}

            return null;
        }
    }
}
