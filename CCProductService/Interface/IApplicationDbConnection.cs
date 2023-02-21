﻿using System.Data;

namespace CCProductService.Interface
{
    public interface IApplicationDbConnection : IDisposable
    {
        void Init(string database);
        void BeginTransaction();
        void CommitTransaction();
        Task<int> ExecuteAsync(string sql, object param = null, CancellationToken cancellationToken = default);
        Task<T> ExecuteScalarAsync<T>(string sql, object param = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null, string splitOn = null);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, CancellationToken cancellationToken = default);
        Task<T> QuerySingleAsync<T>(string sql, object param = null, CancellationToken cancellationToken = default);
        Task QueryMultipleAsync(string sql, object param = null);
    }
}