using HEF.Sql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace HEF.Data.Query
{
    public class DbQueryingEnumerable<T> : IEnumerable<T>, IAsyncEnumerable<T>
    {
        private readonly IDbCommandBuilder _commandBuilder;
        private readonly SqlSentence _querySql;
        private readonly Func<DbDataReader, T> _elementFactory;

        public DbQueryingEnumerable(IDbCommandBuilder commandBuilder,
            SqlSentence querySql, Func<DbDataReader, T> elementFactory)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
            _querySql = querySql ?? throw new ArgumentNullException(nameof(querySql));
            _elementFactory = elementFactory ?? throw new ArgumentNullException(nameof(elementFactory));
        }

        public virtual IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new DbQueryingAsyncEnumerator(this, cancellationToken);

        public virtual IEnumerator<T> GetEnumerator() => new DbQueryingEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private sealed class DbQueryingEnumerator : IEnumerator<T>
        {
            private readonly IDbCommandBuilder _commandBuilder;
            private readonly SqlSentence _querySql;
            private readonly Func<DbDataReader, T> _elementFactory;

            private DbDataReader _dataReader;

            public DbQueryingEnumerator(DbQueryingEnumerable<T> queryingEnumerable)
            {
                _commandBuilder = queryingEnumerable._commandBuilder;
                _querySql = queryingEnumerable._querySql;
                _elementFactory = queryingEnumerable._elementFactory;
            }

            public T Current { get; private set; }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_dataReader == null)
                    InitializeReader();

                var hasNext = _dataReader.Read();
                Current = default;

                if (hasNext)
                {
                    Current = _elementFactory(_dataReader);
                }

                return hasNext;
            }

            private void InitializeReader()
            {
                var command = _commandBuilder.Append(_querySql.SqlText)
                    .AddParameters(_querySql.Parameters)
                    .Build();

                command.Connection.Open();

                _dataReader = command.ExecuteReader() as DbDataReader;
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
            private readonly Func<DbDataReader, T> _elementFactory;

            private readonly CancellationToken _cancellationToken;

            private DbDataReader _dataReader;

            public DbQueryingAsyncEnumerator(
                DbQueryingEnumerable<T> queryingEnumerable,
                CancellationToken cancellationToken)
            {
                _commandBuilder = queryingEnumerable._commandBuilder;
                _querySql = queryingEnumerable._querySql;
                _elementFactory = queryingEnumerable._elementFactory;

                _cancellationToken = cancellationToken;
            }

            public T Current { get; private set; }

            public async ValueTask<bool> MoveNextAsync()
            {
                if (_dataReader == null)
                   await InitializeReaderAsync(_cancellationToken);

                var hasNext = await _dataReader.ReadAsync(_cancellationToken);
                Current = default;

                if (hasNext)
                {
                    Current = _elementFactory(_dataReader);
                }

                return hasNext;
            }

            private async Task InitializeReaderAsync(CancellationToken cancellationToken)
            {
                var command = _commandBuilder.Append(_querySql.SqlText)
                    .AddParameters(_querySql.Parameters)
                    .Build() as DbCommand;

                await command.Connection.OpenAsync(cancellationToken);

                _dataReader = await command.ExecuteReaderAsync(cancellationToken);
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
    }
}
