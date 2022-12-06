using System.Data;

namespace CCProductService.Interface
{
    public interface IApplicationReadDbConnection
    {
        public IDbConnection Connection { get; }
        Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> QueryAsync<T>(string sql, Func<object[], T> map);

        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default);
        Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default);
    }
}
