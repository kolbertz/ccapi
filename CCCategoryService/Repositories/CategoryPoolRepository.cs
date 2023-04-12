using CCApiLibrary.Interfaces;
using CCCategoryService.Data;
using CCCategoryService.Interface;
using CCCategoryService.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using CCApiLibrary.Models;
using System.Diagnostics;

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

        public Task<IEnumerable<CategoryPool>> GetCategoryPoolsAsync()
        {

            var query = "SELECT Id, ParentCategoryPoolId ,PoolType, SystemSettingsId, [Name], Description FROM CategoryPool";

            return _dbContext.QueryAsync<InternalCategoryPool, String, CategoryPool>(query, (internalPool, description) => {
                return new CategoryPool(internalPool, description);
            }, splitOn: "[Name], Description");
        }

        public async Task<CategoryPool> GetCategoryPoolByIdAsync(Guid id, UserClaim userClaim)
        {
            var query = "SELECT Id, [Name], Description, ParentCategoryPoolId ,PoolType, SystemSettingsId FROM CategoryPool " +
                "WHERE Id = @CategoryPoolId";
            InternalCategoryPool internalCategoryPool = await _dbContext.QueryFirstOrDefaultAsync<InternalCategoryPool>(query, param: new { CategoryPoolId = id }).ConfigureAwait(false);
            if (internalCategoryPool != null)
            {
                return new CategoryPool(internalCategoryPool);
            }
            return null;
        }

        public async Task<CategoryPoolWithCategoryList> GetCategoryPoolAsyncMoreInfo(Guid id, UserClaim userClaim)
        {
            var query = "SELECT  CategoryPoolId, Name AS CategoryPoolName,CategoryId,CategoryName AS CategoryNames FROM [CategoryString]  " +
                "LEFT JOIN Category ON CategoryString.CategoryId = Category.Id " +
                "LEFT JOIN CategoryPool ON Category.CategoryPoolId = CategoryPool.Id " +
                " WHERE CategoryPool.Id= @CategoryPoolId";
            
            CategoryPoolWithCategoryList withCategoryList = await _dbContext.QueryFirstOrDefaultAsync<CategoryPoolWithCategoryList>(query, param: new { CategoryPoolId = id }).ConfigureAwait(false);
            return withCategoryList;
        }

        public Task<Guid> AddCategoryPoolAsync(CategoryPoolBase categoryPoolDto, UserClaim userClaim)
        {
            var query = "INSERT INTO CategoryPool( [Name], Description, ParentCategoryPoolId, SystemSettingsId, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser) " +
                "OUTPUT Inserted.Id " +
                "VALUES( @Name, @Description, @ParentCategoryPoolId, @SystemSettingsId, @CreatedDate, @CreatedUser, @LastUpdatedDate, @LastUpdatedUser);";
            InternalCategoryPool pool = new InternalCategoryPool(categoryPoolDto);
            pool.CreatedDate = pool.LastUpdatedDate = DateTimeOffset.Now;
            pool.CreatedUser = pool.LastUpdatedUser = userClaim.UserId;
            return _dbContext.ExecuteScalarAsync<Guid>(query, pool);
        }

        public Task<int> UpdateCategoryPoolAsync(CategoryPool categoryPool, UserClaim userClaim)
        {
            InternalCategoryPool pool = new InternalCategoryPool(categoryPool);
            pool.LastUpdatedDate = DateTimeOffset.Now;
            pool.LastUpdatedUser = userClaim.UserId;
            return Update(pool);
        }

        public async Task<CategoryPoolBase> PatchCategoryPoolAsync(Guid id, JsonPatchDocument jsonPatchDocument, UserClaim userClaim)
        {
            var query = "SELECT * FROM CategoryPool WHERE Id = @CategoryPoolId";
            var p = new { CategoryPoolId = id };
            InternalCategoryPool pool = await _dbContext.QueryFirstOrDefaultAsync<InternalCategoryPool>(query, p);
            if (pool != null)
            {
                CategoryPoolBase categoryPoolDto = new CategoryPoolBase();
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

        private Task<int> Update(InternalCategoryPool pool)
        {
            var query = "UPDATE CategoryPool Set  [Name] = @Name, Description = @Description, ParentCategoryPoolId = @ParentCategoryPoolId, " +
                "SystemSettingsId = @SystemSettingsId, LastUpdatedDate = @LastUpdatedDate, LastUpdatedUser = @LastUpdatedUser, PoolType = @PoolType WHERE Id = @Id";
            return _dbContext.ExecuteAsync(query, param: pool);
        }

        public Task<int> DeleteCategoryPoolAsync(Guid id)
        {
            var query = "DELETE FROM CategoryPool WHERE Id = @Id";
            return _dbContext.ExecuteAsync(query, param: new { Id = id });
        }
        private static void ConvertCategoryPoolTypes() 
        { 

        
        }
    }
}
