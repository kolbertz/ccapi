using CCProductPoolService.Interface;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CCProductPoolService.DapperDbConnection
{
    public class ApplicationWriteDbConnection : IApplicationDbConnection
    {
        private IDbConnection _connection { get; set; }
        private IConfiguration _configuration;

        public ApplicationWriteDbConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Init(string database)
        {
            _connection = new SqlConnection(_configuration.GetConnectionString(database));
        }

        public Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return _connection.ExecuteAsync(sql, param, transaction);
        }

        public Task<T> ExecuteScalarAsync<T>(string sql, object param, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return _connection.ExecuteScalarAsync<T>(sql, param, transaction);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return _connection.QueryAsync<T>(sql, param, transaction);
        }

        public Task<IReadOnlyList<T>> QueryAsync<T>(string sql, Func<object[], T> map)
        {
            throw new NotImplementedException();
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return _connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
        }
        public Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return _connection.QuerySingleAsync<T>(sql, param, transaction);
        }

        public IDbTransaction BeginTransaction()
        {
            _connection.Open();
            return _connection.BeginTransaction();
        }
    }
}
