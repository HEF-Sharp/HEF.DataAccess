﻿using HEF.Data;
using System;
using System.Collections.Generic;

namespace HEF.Sql.Entity
{
    internal static class TypeExtensions
    {
        private static List<Type> _simpleTypes = new List<Type>
                               {
                                   typeof(byte),
                                   typeof(sbyte),
                                   typeof(short),
                                   typeof(ushort),
                                   typeof(int),
                                   typeof(uint),
                                   typeof(long),
                                   typeof(ulong),
                                   typeof(float),
                                   typeof(double),
                                   typeof(decimal),
                                   typeof(bool),
                                   typeof(string),
                                   typeof(char),
                                   typeof(Guid),
                                   typeof(DateTime),
                                   typeof(DateTimeOffset),
                                   typeof(byte[])
                               };

        internal static bool IsSimpleType(this Type type)
        {
            var unwrapType = type.UnwrapNullableType();

            return _simpleTypes.Contains(unwrapType);
        }
    }
}
