using System;
using System.Collections.Generic;

namespace HEF.Repository.Dapper
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

        internal static Type UnwrapNullableType(this Type type) => Nullable.GetUnderlyingType(type) ?? type;

        internal static bool IsNullableValueType(this Type type)
            => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }
}
