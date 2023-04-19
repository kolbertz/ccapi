using CCApiLibrary.Interfaces;
using CCApiLibrary.Models;
using CCProductPoolService.Data;
using CCProductPoolService.Dtos;
using CCProductPoolService.Interface;
using Microsoft.AspNetCore.JsonPatch;
using System.Dynamic;

namespace CCProductPoolService.Repositories
{
    public class ProductPoolRepository : IProductPoolRepository
    {
        public IApplicationDbConnection _dbContext { get; }

        public ProductPoolRepository(IApplicationDbConnection writeDbConnection)
        {
            _dbContext = writeDbConnection;
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }


        public void Init(string database)
        {
            _dbContext.Init(database);
        }

        public async Task<IEnumerable<ProductPool>> GetProductPoolsAsync(UserClaim userClaim)
        {

            var query = "SELECT Id, ProductPoolKey, [Name], Description, ParentProductPoolId, SystemSettingsId FROM ProductPool";
            IEnumerable<InternalProductPool> internalProductPool = await _dbContext.QueryAsync<InternalProductPool>(query);
            List<ProductPool> pools = new List<ProductPool>();
            foreach (var item in internalProductPool)
            {
                pools.Add(new ProductPool(item));
            }
            return pools;
        }

        //public Task<ProductPool> GetProductPoolByIdAsync(Guid id, UserClaim userClaim)
        //{
        //    var query = "SELECT Id, ProductPoolKey as [Key], [Name], Description, ParentProductPoolId as ParentProductPool, SystemSettingsId FROM ProductPool " +
        //        "WHERE Id = @ProductPoolId";
        //    return _dbContext.QueryFirstOrDefaultAsync<ProductPool>(query, param: new { ProductPoolId = id });
        //}

        public Task<ProductPool> GetProductPoolByIdAsync(Guid id, UserClaim userClaim)
        {
            (string sysIdQuery, ExpandoObject paramObj) = GetClaimsQuery(userClaim);
            string poolIdQuery = (string.IsNullOrEmpty(sysIdQuery) ? " where" : " and") + " Id = @Id";
            var query = $"SELECT Id, ProductPoolKey as [Key], [Name], Description, ParentProductPoolId as ParentProductPool, SystemSettingsId FROM ProductPool{sysIdQuery}{poolIdQuery}";
            paramObj.TryAdd("Id", id);

            return _dbContext.QueryFirstOrDefaultAsync<ProductPool>(query, paramObj);
            
            //IEnumerable<InternalProductPool> internalProductPool = await _dbContext.QueryAsync<InternalProductPool>(query, paramObj);
            //List<ProductPool> pools = new List<ProductPool>();
            //foreach (var item in internalProductPool)
            //{
            //    pools.Add(new ProductPool(item));
            //}
            //return pools;
        }

        public Task<Guid> AddProductPoolAsync(ProductPoolBase productPoolDto, UserClaim userClaim)
        {
            var query = "INSERT INTO ProductPool(ProductPoolKey, [Name], Description, ParentProductPoolId, SystemSettingsId, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser) " +
                "OUTPUT Inserted.Id " +
                "VALUES(@ProductPoolKey, @Name, @Description, @ParentProductPoolId, @SystemSettingsId, @CreatedDate, @CreatedUser, @LastUpdatedDate, @LastUpdatedUser);";
            InternalProductPool pool = new InternalProductPool(productPoolDto);
            pool.CreatedDate = pool.LastUpdatedDate = DateTimeOffset.Now;
            pool.CreatedUser = pool.LastUpdatedUser = userClaim.UserId;
            return _dbContext.ExecuteScalarAsync<Guid>(query, pool);
        }

        public Task<int> UpdateProductPoolAsync(ProductPool productPool, UserClaim userClaim)
        {
            InternalProductPool pool = new InternalProductPool(productPool);
            pool.LastUpdatedDate = DateTimeOffset.Now;
            pool.LastUpdatedUser = userClaim.UserId;
            return Update(pool);
        }

        public async Task<ProductPool> PatchProductPoolAsync(Guid id, JsonPatchDocument jsonPatchDocument, UserClaim userClaim)
        {
            var query = "SELECT * FROM ProductPool WHERE Id = @ProductPoolId";
            var p = new {ProductPoolId = id };
            InternalProductPool pool = await _dbContext.QuerySingleAsync<InternalProductPool>(query, p);
            if (pool != null)
            {
                ProductPool productPoolDto = new ProductPool(pool);
                jsonPatchDocument.ApplyTo(productPoolDto);
                pool.MergeProductPool(productPoolDto);
                pool.LastUpdatedDate= DateTimeOffset.Now;
                pool.LastUpdatedUser = userClaim.UserId;
                if (await Update(pool).ConfigureAwait(false) > 0)
                {
                    return productPoolDto;
                }
            }
            return null;
        }

        private Task<int> Update(InternalProductPool pool)
        {
            var query = "UPDATE ProductPool Set ProductPoolKey = @ProductPoolKey, [Name] = @Name, Description = @Description, ParentProductPoolId = @ParentProductPoolId, " +
                "SystemSettingsId = @SystemSettingsId, LastUpdatedDate = @LastUpdatedDate, LastUpdatedUser = @LastUpdatedUser WHERE Id = @Id";
            return _dbContext.ExecuteAsync(query, param: pool);
        }

        public Task<int> DeleteProductPoolAsync(Guid id)
        {
            var query = "DELETE FROM ProductPool WHERE Id = @Id";
            return _dbContext.ExecuteAsync(query, param: new { Id = id });
        }

        private static (string sysId, ExpandoObject paramObj) GetClaimsQuery(UserClaim userClaim)
        {
            string sysIdQuery = string.Empty;
            var paramObj = new ExpandoObject();

            if (userClaim != null && userClaim.SystemId != null)
            {
                sysIdQuery = " where SystemSettingsId = @sysId";
                paramObj.TryAdd("sysId", userClaim.SystemId);
            }
            return (sysIdQuery, paramObj);
        }
    }
}
