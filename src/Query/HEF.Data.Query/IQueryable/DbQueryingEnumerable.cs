using HEF.Entity.Mapper;
using HEF.Sql;
using HEF.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HEF.Data.Query
{
    public class DbQueryingEnumerable<T> : IEnumerable<T>, IAsyncEnumerable<T>
    {
        private readonly IDbCommandBuilder _commandBuilder;
        private readonly SqlSentence _querySql;
        private readonly IReadOnlyList<IPropertyMap> _selectProperties;
        private readonly Func<DbDataReader, IDictionary<string, int>, T> _elementFactory;
        private readonly IConcurrencyDetector _concurrencyDetector;

        public DbQueryingEnumerable(IDbCommandBuilder commandBuilder,
            SqlSentence querySql,
            IReadOnlyList<IPropertyMap> selectProperties,
            Func<DbDataReader, IDictionary<string, int>, T> elementFactory,
            IConcurrencyDetector concurrencyDetector)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
            _querySql = querySql ?? throw new ArgumentNullException(nameof(querySql));
            _selectProperties = selectProperties ?? throw new ArgumentNullException(nameof(selectProperties));
            _elementFactory = elementFactory ?? throw new ArgumentNullException(nameof(elementFactory));
            _concurrencyDetector = concurrencyDetector ?? throw new ArgumentNullException(nameof(concurrencyDetector));
        }

        public virtual IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new DbQueryingAsyncEnumerator(this, cancellationToken);

        public virtual IEnumerator<T> GetEnumerator() => new DbQueryingEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static IDictionary<string, int> BuildSelectPropertyIndexMap(
            IReadOnlyList<IPropertyMap> selectProperties, DbDataReader dataReader)
        {
            if (selectProperties.IsEmpty())            
                return null;
            
            var readerColumns = Enumerable.Range(0, dataReader.FieldCount)
                .ToDictionary(dataReader.GetName, i => i, StringComparer.OrdinalIgnoreCase);

            var propertyIndexMap = new Dictionary<string, int>();
            for (var i = 0; i < selectProperties.Count; i++)
            {
                var columnName = selectProperties[i].Name;  //select sql has alias property name
                if (!readerColumns.TryGetValue(columnName, out var ordinal))
                {
                    throw new InvalidOperationException($"not found column '{columnName}' from dataReader");
                }

                propertyIndexMap.Add(columnName, ordinal);
            }

            return propertyIndexMap;
        }

        #region Enumerator
        private sealed class DbQueryingEnumerator : IEnumerator<T>
        {
            private readonly IDbCommandBuilder _commandBuilder;
            private readonly SqlSentence _querySql;
            private readonly IReadOnlyList<IPropertyMap> _selectProperties;
            private readonly Func<DbDataReader, IDictionary<string, int>, T> _elementFactory;
            private readonly IConcurrencyDetector _concurrencyDetector;

            private DbDataReader _dataReader;
            private IDictionary<string, int> _propertyIndexMap;

            public DbQueryingEnumerator(DbQueryingEnumerable<T> queryingEnumerable)
            {
                _commandBuilder = queryingEnumerable._commandBuilder;
                _querySql = queryingEnumerable._querySql;
                _selectProperties = queryingEnumerable._selectProperties;
                _elementFactory = queryingEnumerable._elementFactory;
                _concurrencyDetector = queryingEnumerable._concurrencyDetector;
            }

            public T Current { get; private set; }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                using (_concurrencyDetector.EnterCriticalSection())
                {
                    if (_dataReader == null)
                        InitializeReader();

                    var hasNext = _dataReader.Read();
                    Current = default;

                    if (hasNext)
                    {
                        Current = _elementFactory(_dataReader, _propertyIndexMap);
                    }

                    return hasNext;
                }
            }

            private void InitializeReader()
            {
                var command = _commandBuilder.Append(_querySql.SqlText)
                    .AddParameters(_querySql.Parameters)
                    .Build();

                if (command.Connection.State != ConnectionState.Open)
                    command.Connection.Open();

                _dataReader = command.ExecuteReader() as DbDataReader;

                _propertyIndexMap = BuildSelectPropertyIndexMap(_selectProperties, _dataReader);
            }

            public void Dispose()
            {
                _dataReader?.Dispose();
                _dataReader = null;
            }

            public void Reset() => throw new NotImplementedException();
        }

        private sealed class DbQueryingAsyncEnumerator : IAsyncEnumerator<T>
        {
            private readonly IDbCommandBuilder _commandBuilder;
            private readonly SqlSentence _querySql;
            private readonly IReadOnlyList<IPropertyMap> _selectProperties;
            private readonly Func<DbDataReader, IDictionary<string, int>, T> _elementFactory;
            private readonly IConcurrencyDetector _concurrencyDetector;

            private readonly CancellationToken _cancellationToken;

            private DbDataReader _dataReader;
            private IDictionary<string, int> _propertyIndexMap;

            public DbQueryingAsyncEnumerator(
                DbQueryingEnumerable<T> queryingEnumerable,
                CancellationToken cancellationToken)
            {
                _commandBuilder = queryingEnumerable._commandBuilder;
                _querySql = queryingEnumerable._querySql;
                _selectProperties = queryingEnumerable._selectProperties;
                _elementFactory = queryingEnumerable._elementFactory;
                _concurrencyDetector = queryingEnumerable._concurrencyDetector;

                _cancellationToken = cancellationToken;
            }

            public T Current { get; private set; }

            public async ValueTask<bool> MoveNextAsync()
            {
                using (_concurrencyDetector.EnterCriticalSection())
                {
                    if (_dataReader == null)
                        await InitializeReaderAsync(_cancellationToken);

                    var hasNext = await _dataReader.ReadAsync(_cancellationToken);
                    Current = default;

                    if (hasNext)
                    {
                        Current = _elementFactory(_dataReader, _propertyIndexMap);
                    }

                    return hasNext;
                }
            }

            private async Task InitializeReaderAsync(CancellationToken cancellationToken)
            {
                var command = _commandBuilder.Append(_querySql.SqlText)
                    .AddParameters(_querySql.Parameters)
                    .Build() as DbCommand;

                if (command.Connection.State != ConnectionState.Open)
                    await command.Connection.OpenAsync(cancellationToken);

                _dataReader = await command.ExecuteReaderAsync(cancellationToken);

                _propertyIndexMap = BuildSelectPropertyIndexMap(_selectProperties, _dataReader);
            }

            public ValueTask DisposeAsync()
            {
                if (_dataReader != null)
                {
                    var dataReader = _dataReader;
                    _dataReader = null;

                    return dataReader.DisposeAsync();
                }

                return default;
            }
        }
        #endregion
    }
}
