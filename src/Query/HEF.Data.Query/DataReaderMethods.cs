using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace HEF.Data.Query
{
    internal static class DataReaderMethods
    {
        private static readonly IDictionary<Type, MethodInfo> _getValueMethodMapping = new Dictionary<Type, MethodInfo>();

        private static readonly MethodInfo _getFieldValueMethod = GetDataReaderGetValueMethod(nameof(DbDataReader.GetFieldValue));
        
        internal static readonly MethodInfo IsDbNullMethod =
            typeof(DbDataReader).GetRuntimeMethod(nameof(DbDataReader.IsDBNull), new[] { typeof(int) });        

        static DataReaderMethods()
        {
            _getValueMethodMapping.Add(typeof(bool), GetDataReaderGetValueMethod(nameof(DbDataReader.GetBoolean)));
            _getValueMethodMapping.Add(typeof(byte), GetDataReaderGetValueMethod(nameof(DbDataReader.GetByte)));
            _getValueMethodMapping.Add(typeof(char), GetDataReaderGetValueMethod(nameof(DbDataReader.GetChar)));
            _getValueMethodMapping.Add(typeof(DateTime), GetDataReaderGetValueMethod(nameof(DbDataReader.GetDateTime)));
            _getValueMethodMapping.Add(typeof(decimal), GetDataReaderGetValueMethod(nameof(DbDataReader.GetDecimal)));
            _getValueMethodMapping.Add(typeof(double), GetDataReaderGetValueMethod(nameof(DbDataReader.GetDouble)));
            _getValueMethodMapping.Add(typeof(float), GetDataReaderGetValueMethod(nameof(DbDataReader.GetFloat)));
            _getValueMethodMapping.Add(typeof(Guid), GetDataReaderGetValueMethod(nameof(DbDataReader.GetGuid)));
            _getValueMethodMapping.Add(typeof(short), GetDataReaderGetValueMethod(nameof(DbDataReader.GetInt16)));
            _getValueMethodMapping.Add(typeof(int), GetDataReaderGetValueMethod(nameof(DbDataReader.GetInt32)));
            _getValueMethodMapping.Add(typeof(long), GetDataReaderGetValueMethod(nameof(DbDataReader.GetInt64)));
            _getValueMethodMapping.Add(typeof(string), GetDataReaderGetValueMethod(nameof(DbDataReader.GetString)));
        }

        private static MethodInfo GetDataReaderGetValueMethod(string name)
            => typeof(DbDataReader).GetRuntimeMethod(name, new[] { typeof(int) });

        internal static MethodInfo GetDataReaderGetValueMethod(Type type)
        {
            var unwrapType = type.UnwrapNullableType();

            return _getValueMethodMapping.TryGetValue(unwrapType, out var method)
                ? method
                : _getFieldValueMethod.MakeGenericMethod(unwrapType);
        }
    }
}
