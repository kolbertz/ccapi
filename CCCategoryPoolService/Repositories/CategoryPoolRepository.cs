using CCApiLibrary.Interfaces;
using CCCategoryPoolService.Data;
using CCCategoryPoolService.DTOs;
using CCCategoryPoolService.Interface;

using CCProductPoolService.Interface;
using Microsoft.AspNetCore.JsonPatch;

namespace CCCategoryPoolService.Repositories
{
    public class CategoryPoolRepository : ICategoryPoolRepository
    {
        public IApplicationDbConnection _dbContext { get; }

        public CategoryPoolRepository(IApplicationDbConnection writeDbConnection)
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

        public Task<IEnumerable<CategoryPoolDto>> GetProductPoolsAsync()
        {

            var query = "SELECT Id, ProductPoolKey as [Key], [Name], Description, ParentProductPoolId as ParentProductPool, SystemSettingsId FROM ProductPool";
            return _dbContext.QueryAsync<CategoryPoolDto>(query);
        }

        public Task<CategoryPoolDto> GetProductPoolByIdAsync(Guid id)
        {
            var query = "SELECT Id, ProductPoolKey as [Key], [Name], Description, ParentProductPoolId as ParentProductPool, SystemSettingsId FROM ProductPool " +
                "WHERE Id = @ProductPoolId";
            return _dbContext.QuerySingleAsync<CategoryPoolDto>(query, param: new { ProductPoolId = id });
        }

        public Task<Guid> AddProductPoolAsync(CategoryPoolDto productPoolDto, UserClaim userClaim)
        {
            var query = "INSERT INTO ProductPool(ProductPoolKey, [Name], Description, ParentProductPoolId, SystemSettingsId, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser) " +
                "OUTPUT Inserted.Id " +
                "VALUES(@ProductPoolKey, @Name, @Description, @ParentProductPoolId, @SystemSettingsId, @CreatedDate, @CreatedUser, @LastUpdatedDate, @LastUpdatedUser);";
            ProductPool pool = new ProductPool(CategoryPoolDto);
            pool.CreatedDate = pool.LastUpdatedDate = DateTimeOffset.Now;
            pool.CreatedUser = pool.LastUpdatedUser = userClaim.UserId;
            return _dbContext.ExecuteScalarAsync<Guid>(query, pool);
        }

        public Task<int> UpdateProductPoolAsync(CategoryPoolDto productPool, UserClaim userClaim)
        {
            ProductPool pool = new ProductPool(productPool);
            pool.LastUpdatedDate = DateTimeOffset.Now;
            pool.LastUpdatedUser = userClaim.UserId;
            return Update(pool);
        }

        public async Task<CategoryPoolDto> PatchProductPoolAsync(Guid id, JsonPatchDocument jsonPatchDocument, UserClaim userClaim)
        {
            var query = "SELECT * FROM ProductPool WHERE Id = @ProductPoolId";
            var p = new {ProductPoolId = id };
            ProductPool pool = await _dbContext.QuerySingleAsync<ProductPool>(query, p);
            if (pool != null)
            {
                CategoryPoolDto productPoolDto = new CategoryPoolDto(pool);
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
            return _dbContext.ExecuteAsync(query, param: pool);
        }

        public Task<int> DeleteProductPoolAsync(Guid id)
        {
            var query = "DELETE FROM ProductPool WHERE Id = @Id";
            return _dbContext.ExecuteAsync(query, param: new { Id = id });
        }
               
    }
}
