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

        public async Task UpdateProductPoolAsync(ProductPoolDto productPool, UserClaim userClaim)
        {
            ProductPool pool = await _dbContext.ProductPools.Where(pool => pool.Id == productPool.Id).FirstOrDefaultAsync();
            if (pool != null)
            {
                pool.MergeProductPool(productPool);
                pool.LastUpdatedDate= DateTimeOffset.Now;
                pool.LastUpdatedUser = userClaim.UserId;
                await _dbContext.SaveChangesAsync(new CancellationToken()).ConfigureAwait(false);
            }
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
                await _dbContext.SaveChangesAsync(new CancellationToken()).ConfigureAwait(false);
                return productPoolDto;
            }
            return null;
        }

        public async Task DeleteProductPoolAsync(Guid id)
        {
            ProductPool productPool = await _dbContext.ProductPools.Where(pool => pool.Id == id).FirstOrDefaultAsync().ConfigureAwait(false);
            if (productPool != null)
            {
                _dbContext.ProductPools.Remove(productPool);
                await _dbContext.SaveChangesAsync(new CancellationToken()).ConfigureAwait(!false);
            }
        }
    }
}
