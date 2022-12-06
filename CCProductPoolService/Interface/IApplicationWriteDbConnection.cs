using System.Data;

namespace CCProductPoolService.Interface
{
    public interface IApplicationWriteDbConnection
    {
        Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default);
    }
}
