using CCProductPoolService.Data;
using CCProductPoolService.Dtos;
using CCProductPoolService.Interface;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace CCProductPoolService.Repositories
{
    public class ProductPoolRepository : IProductPoolRepository
    {
        public AramarkDbProduction20210816Context _dbContext { get; }
        public IApplicationReadDbConnection _readDbContext { get; }
        public IApplicationWriteDbConnection _writeDbContext { get; }

        public ProductPoolRepository(AramarkDbProduction20210816Context dbContext, IApplicationReadDbConnection readDbConnection, IApplicationWriteDbConnection writeDbConnection)
        {
            _dbContext = dbContext;
            _readDbContext = readDbConnection;
            _writeDbContext = writeDbConnection;
        }

        public Task<IReadOnlyList<ProductPoolDto>> GetProductPoolsAsync()
        {

            var query = "SELECT Id, ProductPoolKey as [Key], [Name], Description, ParentProductPoolId as ParentProductPool, SystemSettingsId FROM ProductPool";
            return _readDbContext.QueryAsync<ProductPoolDto>(query);
        }

        public Task<ProductPoolDto> GetProductPoolByIdAsync(Guid id)
        {
            var query = "SELECT Id, ProductPoolKey as [Key], [Name], Description, ParentProductPoolId as ParentProductPool, SystemSettingsId FROM ProductPool " +
                "WHERE Id = @ProductPoolId";
            return _readDbContext.Connection.QuerySingleAsync<ProductPoolDto>(query, param: new { ProductPoolId = id });
        }

        public Task<Guid> AddProductPoolAsync(ProductPoolDto productPoolDto, UserClaim userClaim)
        {
            var query = "INSERT INTO ProductPool(ProductPoolKey, [Name], Description, ParentProductPoolId, SystemSettingsId, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser) " +
                "OUTPUT Inserted.Id " +
                "VALUES(@ProductPoolKey, @Name, @Description, @ParentProductPoolId, @SystemSettingsId, @CreatedDate, @CreatedUser, @LastUpdatedDate, @LastUpdatedUser);";
            ProductPool pool = new ProductPool(productPoolDto);
            pool.CreatedDate = pool.LastUpdatedDate = DateTimeOffset.Now;
            pool.CreatedUser = pool.LastUpdatedUser = userClaim.UserId;
            return _readDbContext.Connection.ExecuteScalarAsync<Guid>(query, param: pool);
        }

        public Task<int> UpdateProductPoolAsync(ProductPoolDto productPool, UserClaim userClaim)
        {
            ProductPool pool = new ProductPool(productPool);
            pool.LastUpdatedDate = DateTimeOffset.Now;
            pool.LastUpdatedUser = userClaim.UserId;
            return Update(pool);
        }

        public async Task<ProductPoolDto> PatchProductPoolAsync(Guid id, JsonPatchDocument jsonPatchDocument, UserClaim userClaim)
        {
            ProductPool pool = await _dbContext.ProductPools.Where(pool => pool.Id == id).FirstOrDefaultAsync();
            if (pool != null)
            {
                ProductPoolDto productPoolDto = new ProductPoolDto(pool);
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

        private Task<int> Update(ProductPool pool)
        {
            var query = "UPDATE ProductPool Set ProductPoolKey = @ProductPoolKey, [Name] = @Name, Description = @Description, ParentProductPoolId = @ParentProductPoolId, " +
                "SystemSettingsId = @SystemSettingsId, LastUpdatedDate = @LastUpdatedDate, LastUpdatedUser = @LastUpdatedUser WHERE Id = @Id";
            return _readDbContext.Connection.ExecuteAsync(query, param: pool);
        }

        public Task<int> DeleteProductPoolAsync(Guid id)
        {
            var query = "DELETE FROM ProductPool WHERE Id = @Id";
            return _readDbContext.Connection.ExecuteAsync(query, param: new { Id = id });
        }
    }
}
