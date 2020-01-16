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
        public virtual IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new DbQueryingAsyncEnumerator(this, cancellationToken);

        public virtual IEnumerator<T> GetEnumerator() => new DbQueryingEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private sealed class DbQueryingEnumerator : IEnumerator<T>
        {
            private DbDataReader _dataReader;

            public DbQueryingEnumerator(DbQueryingEnumerable<T> queryingEnumerable)
            {

            }

            public T Current { get; private set; }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                throw new NotImplementedException();
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
            private readonly CancellationToken _cancellationToken;

            private DbDataReader _dataReader;

            public DbQueryingAsyncEnumerator(
                DbQueryingEnumerable<T> queryingEnumerable,
                CancellationToken cancellationToken)
            {
                _cancellationToken = cancellationToken;
            }

            public T Current { get; private set; }

            public async ValueTask<bool> MoveNextAsync()
            {
                throw new NotImplementedException();
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
