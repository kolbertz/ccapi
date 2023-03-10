using CCApiLibrary.Interfaces;
using CCCategoryService.Data;
using CCCategoryService.Interface;
using CCCategoryService.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using CCApiLibrary.Models;

namespace CCCategoryService.Repositories
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

        public Task<IEnumerable<CategoryPoolDto>> GetCategoryPoolsAsync()
        {

            var query = "SELECT Id, [Name], Description, ParentCategoryPoolId ,PoolType, SystemSettingsId FROM CategoryPool";
            return _dbContext.QueryAsync<CategoryPoolDto>(query);
        }

        public Task<CategoryPoolDto> GetCategoryPoolByIdAsync(Guid id)
        {
            var query = "SELECT Id, [Name], Description, ParentCategoryPoolId ,PoolType, SystemSettingsId FROM CategoryPool " +
                "WHERE Id = @CategoryPoolId";
            return _dbContext.QuerySingleAsync<CategoryPoolDto>(query, param: new { CategoryPoolId = id });
        }

        public Task<Guid> AddCategoryPoolAsync(CategoryPoolDto categoryPoolDto, UserClaim userClaim)
        {
            var query = "INSERT INTO CategoryPool( [Name], Description, ParentProductPoolId, SystemSettingsId, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser) " +
                "OUTPUT Inserted.Id " +
                "VALUES( @Name, @Description, @ParentCategeoryPoolId, @SystemSettingsId, @CreatedDate, @CreatedUser, @LastUpdatedDate, @LastUpdatedUser);";
            CategoryPool pool = new CategoryPool();
            pool.CreatedDate = pool.LastUpdatedDate = DateTimeOffset.Now;
            pool.CreatedUser = pool.LastUpdatedUser = userClaim.UserId;
            return _dbContext.ExecuteScalarAsync<Guid>(query, pool);
        }

        public Task<int> UpdateCategoryPoolAsync(CategoryPoolDto categoryPool, UserClaim userClaim)
        {
            CategoryPool pool = new CategoryPool();
            pool.LastUpdatedDate = DateTimeOffset.Now;
            pool.LastUpdatedUser = userClaim.UserId;
            return Update(pool);
        }

        public async Task<CategoryPoolDto> PatchCategoryPoolAsync(Guid id, JsonPatchDocument jsonPatchDocument, UserClaim userClaim)
        {
            var query = "SELECT * FROM CategoryPool WHERE Id = @CategoryPoolId";
            var p = new { CategoryPoolId = id };
            CategoryPool pool = await _dbContext.QuerySingleAsync<CategoryPool>(query, p);
            if (pool != null)
            {
                CategoryPoolDto categoryPoolDto = new CategoryPoolDto();
                jsonPatchDocument.ApplyTo(categoryPoolDto);
                //pool.MergeProductPool(productPoolDto);
                pool.LastUpdatedDate = DateTimeOffset.Now;
                pool.LastUpdatedUser = userClaim.UserId;
                if (await Update(pool).ConfigureAwait(false) > 0)
                {
                    return categoryPoolDto;
                }
            }
            return null;
        }

        private Task<int> Update(CategoryPool pool)
        {
            var query = "UPDATE CategoryPool Set  [Name] = @Name, Description = @Description, ParentCategoryPoolId = @ParentCategoryPoolId, " +
                "SystemSettingsId = @SystemSettingsId, LastUpdatedDate = @LastUpdatedDate, LastUpdatedUser = @LastUpdatedUser WHERE Id = @Id";
            return _dbContext.ExecuteAsync(query, param: pool);
        }

        public Task<int> DeleteCategoryPoolAsync(Guid id)
        {
            var query = "DELETE FROM CategoryPool WHERE Id = @Id";
            return _dbContext.ExecuteAsync(query, param: new { Id = id });
        }

    }
}
