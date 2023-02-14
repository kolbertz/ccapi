using System.Data;

namespace CCProductPoolService.Interface
{
    public interface IApplicationDbConnection
    {
        void Init(string database);
        Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default);
        Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> QueryAsync<T>(string sql, Func<object[], T> map);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default);
        Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default);
        IDbTransaction BeginTransaction();
    }
}
