using CCApiLibrary.Interfaces;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace CCApiLibrary.DbConnection
{
    public class ApplicationDbConnection : IApplicationDbConnection
    {
        private IDbConnection _connection;
        private IConfiguration _configuration;
        private IDbTransaction _transaction;
        private bool _disposed;

        public ApplicationDbConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                        _transaction = null;
                    }
                    if (_connection != null)
                    {
                        _connection.Dispose();
                        _connection = null;
                    }
                }
                _disposed = true;
            }
        }


        public void Init(string database)
        {
            _connection = new SqlConnection(_configuration.GetConnectionString(database));
            _connection.Open();
        }

        public void BeginTransaction()
        {
            _transaction = _connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            try
            {
                _transaction?.Commit();
            }
            catch (Exception ex)
            {
                _transaction?.Rollback();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task<int> ExecuteAsync(string sql, object param = null, CancellationToken cancellationToken = default)
        {
            return _connection.ExecuteAsync(sql, param, _transaction);
        }

        public Task<T> ExecuteScalarAsync<T>(string sql, object param, CancellationToken cancellationToken = default)
        {
            return _connection.ExecuteScalarAsync<T>(sql, param, _transaction);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, CancellationToken cancellationToken = default)
        {
            return _connection.QueryAsync<T>(sql, param, _transaction);
        }

        public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null, string splitOn = null)
        {
            return _connection.QueryAsync<TFirst, TSecond, TReturn>(sql, map, param, _transaction, splitOn: splitOn);
        }

        public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, string splitOn = null)
        {
            return _connection.QueryAsync<TFirst, TSecond, TThird, TReturn>(sql, map, param, _transaction, splitOn: splitOn);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, CancellationToken cancellationToken = default)
        {
            return _connection.QueryFirstOrDefaultAsync<T>(sql, param, _transaction);
        }
        public Task<T> QuerySingleAsync<T>(string sql, object param = null, CancellationToken cancellationToken = default)
        {
            return _connection.QuerySingleAsync<T>(sql, param, _transaction);
        }

        public Task QueryMultipleAsync(string sql, object param = null)
        {
            return _connection.QueryMultipleAsync(sql, param, _transaction);
        }
    }
}
