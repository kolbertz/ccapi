using CCProductPoolService.Interface;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CCProductPoolService.DapperDbConnection
{
    public class ApplicationWriteDbConnection : IApplicationWriteDbConnection
    {
        public IDbConnection Connection { get; set; }
        public ApplicationWriteDbConnection(IConfiguration configuration)
        {
            Connection = new SqlConnection(configuration.GetConnectionString("AramarkStaging"));
        }
        public async Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return await Connection.ExecuteAsync(sql, param, transaction);
        }
        public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return (await Connection.QueryAsync<T>(sql, param, transaction)).AsList();
        }

        public Task<IReadOnlyList<T>> QueryAsync<T>(string sql, Func<object[], T> map)
        {
            throw new NotImplementedException();
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return await Connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
        }
        public async Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return await Connection.QuerySingleAsync<T>(sql, param, transaction);
        }
    }
}
