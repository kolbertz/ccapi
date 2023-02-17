using CCProductPoolService.Interface;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace CCProductPoolService.DapperDbConnection
{
    public class ApplicationReadDbConnection : IApplicationReadDbConnection, IDisposable
    {
        public IDbConnection Connection { get; private set; }
        private IConfiguration _configuration;

        public ApplicationReadDbConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }
         
        public void Init(string database)
        {
            Connection = new SqlConnection(_configuration.GetConnectionString(database));
        }

        public void Dispose()
        {
            Connection.Dispose();
        }

        public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return (await Connection.QueryAsync<T>(sql, param, transaction)).AsList();
        }

        public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, Func<object[], T> map)
        {
            return (await Connection.QueryAsync<T>(sql, map)).AsList();
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
