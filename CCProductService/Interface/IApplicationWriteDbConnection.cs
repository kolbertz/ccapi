using System.Data;

namespace CCProductService.Interface
{
    public interface IApplicationWriteDbConnection
    {
        Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default);
    }
}
