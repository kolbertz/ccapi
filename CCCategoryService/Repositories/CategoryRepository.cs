using CCApiLibrary.Interfaces;
using CCApiLibrary.Models;
using CCCategoryService.Data;
using CCCategoryService.Dtos;
using CCCategoryService.Helper;
using CCCategoryService.Interface;
using System.Collections;
using System.Dynamic;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;


namespace CCCategoryService.Repositories
{


    public class CategoryRepository : ICategoryRepository

    {      
        public IApplicationDbConnection _dbContext { get; }

        public CategoryRepository(IApplicationDbConnection writeDbCoonection)

        {
            _dbContext = writeDbCoonection;
        }

        public void Init(string database)
        {
            _dbContext.Init(database);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        public async Task<IEnumerable<Category>> GetAllCategorys(int? take, int? skip, UserClaim userClaim)
        {
            string query;
            string categoryPoolQuery = string.Empty;
            string sysIdQuery = string.Empty;
            var paramObj = new ExpandoObject();

            if (userClaim.SystemId.HasValue) 
            {
                sysIdQuery = " WHERE CategoryPool.SystemSettingsId = @SysId";
                paramObj.TryAdd("SysId", userClaim.SystemId);
            }

            if (userClaim.CategoryPoolIds != null && userClaim.CategoryPoolIds.Count() > 0)
            {
                categoryPoolQuery = " where Category.CategoryPoolId in @poolIds";
                paramObj.TryAdd("poolIds", userClaim.CategoryPoolIds.ToArray());
            }

            if (take.HasValue && skip.HasValue)
            {
                query = $"Select t.Id, t.CategoryPoolId, t.CategoryKey,  CategoryString.CategoryId, CategoryString.Culture, CategoryString.CategoryName, CategoryString.Comment, CategoryString.Description " +
                    $"				FROM (Select Id, CategoryKey, CategoryPoolId	From Category{sysIdQuery}{categoryPoolQuery}	" +
                    $"				ORDER BY CategoryKey      " +
                    $"              OFFSET @offset ROWS FETCH NEXT @fetch ROWS ONLY) as t 		" +
                    $"			    LEFT JOIN CategoryString on t.Id = CategoryString.CategoryId";

                paramObj.TryAdd("offset", skip.Value);
                paramObj.TryAdd("fetch", take.Value);
            }
            else
            {
                query = "SELECT Category.Id, Category.CategoryKey, Category.CategoryPoolId, CategoryString.Id AS CategoryStringID, CategoryString.CategoryId, CategoryString.Culture, CategoryString.CategoryName, CategoryString.Comment, CategoryString.Description" +
                    $" from Category " +
                    $"JOIN CategoryString on Category.Id = CategoryString.CategoryId{sysIdQuery}{categoryPoolQuery}" +
                    $" ORDER BY CategoryKey";
            }

            var stringMap = new Dictionary<Guid, Category>();
            IEnumerable<Category> result = await _dbContext.QueryAsync<InternalCategory, InternalCategoryString, Category>(query, (c, cs) =>
            {
                Category dto;
                if (!stringMap.TryGetValue(c.Id, out dto))
                {
                    dto = new Category(c);
                    CategoryHelper.ParseCategoryToDto(c, dto);
                    stringMap.Add(c.Id, dto);
                }
                if (cs != null)
                {
                    dto.SetMultilanguageText(cs);
                }
                
                return dto;
            }, splitOn: "Id, CategoryId", param: paramObj).ConfigureAwait(false);
            return stringMap.Values.ToList().AsReadOnly();
            
        }

        public async Task<Category> GetCategoryById(Guid id, UserClaim userClaim)
        {
            Category dto = null;
            var paramObj = new ExpandoObject();
            string categoryPoolQuery = string.Empty;
            string sysIdQuery = string.Empty;

            if (userClaim.SystemId.HasValue)
            {
                sysIdQuery = " AND CategoryPool.SystemSettingsId = @SysId";
                paramObj.TryAdd("SysId", userClaim.SystemId);
            }

            if (userClaim.CategoryPoolIds != null && userClaim.CategoryPoolIds.Count() > 0)
            {
                categoryPoolQuery = " and Category.CategoryPoolId in @poolIds";
                paramObj.TryAdd("poolIds", userClaim.CategoryPoolIds.ToArray());
            }

            string query = $"SELECT Category.Id, CategoryKey, CategoryPoolId, CategoryString.CategoryId,CategoryString.Id AS CategoryStringID, CategoryString.Culture, CategoryString.CategoryName, CategoryString.Comment, CategoryString.Description " +
                 $"from Category JOIN CategoryString on Category.Id = CategoryString.CategoryId " +
                 $"WHERE Category.Id = @CategoryId{sysIdQuery}{categoryPoolQuery}";

            paramObj.TryAdd("CategoryId", id);

            return (await _dbContext.QueryAsync<InternalCategory, InternalCategoryString, Category>(query, (p, ps) =>
            {
                if (dto == null)
                {
                    dto = new Category(p);
                    CategoryHelper.ParseCategoryToDto(p, dto);
                }
                 dto.SetMultilanguageText(ps);
                return dto;
            }, splitOn: "Id, CategoryId", param: paramObj).ConfigureAwait(false)).FirstOrDefault();

        }      
     

        public async Task<Guid> AddCategoryAsync(CategoryBase categoryDto, UserClaim userClaim)
        {
            string categoryInsertQuery = $"INSERT INTO [dbo].Category (CategoryKey,[CreatedDate],[CreatedUser],[LastUpdatedUser],[LastUpdatedDate],CategoryPoolId) " +
                 $"OUTPUT INSERTED.Id " +
                 $"VALUES(@CategoryKey,@CreatedDate,@CreatedUser,@LastUpdatedUser,@LastUpdatedDate,@CategoryPoolId)";

            InternalCategory category = new InternalCategory();
            CategoryHelper.ParseDtoToCategory(categoryDto, category);
            
            category.CreatedUser = category.LastUpdatedUser = userClaim.UserId;
            category.CreatedDate = category.LastUpdatedDate = DateTimeOffset.Now;          

            try
            {
                _dbContext.BeginTransaction();
                Guid id = await _dbContext.ExecuteScalarAsync<Guid>(categoryInsertQuery, category);
                await InsertCategoryString(category, userClaim);
                _dbContext.CommitTransaction();
                return id;
            }
            catch (Exception)
            {
                //Rollback
                throw;
            }
        }

        public Task<int> UpdateCategoryAsync(Category categoryDto, UserClaim userClaim)
        {

            InternalCategory category = new InternalCategory(categoryDto);
            category.LastUpdatedDate = DateTimeOffset.Now;
            category.LastUpdatedUser = userClaim.UserId;
            return Update(category, categoryDto, userClaim);
        }

        public async Task<CategoryBase> PatchCategoryAsync(Guid id, UserClaim userClaim)
        {
            var query = "SELECT * FROM Product WHERE Id = @CategoryId";
            var p = new { CategoryId = id };
            InternalCategory category = await _dbContext.QueryFirstOrDefaultAsync<InternalCategory>(query, p);
            if (category != null)
            {
                CategoryBase categoryDto = new CategoryBase();
                category.LastUpdatedDate = DateTimeOffset.Now;
                category.LastUpdatedUser = userClaim.UserId;
                if (await Update(category, categoryDto, userClaim).ConfigureAwait(false) > 0)
                {
                    return categoryDto;
                }
            }
            return new CategoryBase();
        }

        public async Task<int> DeleteCategoryAsync(Guid id, UserClaim userClaim)
        {
            await DeleteCategoryStringAsync(id, userClaim).ConfigureAwait(false);
            var query = "DELETE FROM [dbo].[Category] WHERE Id = @Id";

            return await _dbContext.ExecuteAsync(query, param: new { Id = id });
        }

        public Task<int> DeleteCategoryStringAsync(Guid id, UserClaim userClaim)
        {
            var query = "DELETE FROM [dbo].[CategoryString] WHERE CategoryId = @Id";

            return _dbContext.ExecuteAsync(query, param: new { Id = id });
        }

        public async Task<int> Update(InternalCategory category, CategoryBase categoryBase, UserClaim userClaim)
        {
            var categoryUpdateQuery = "Update Category Set CategoryKey = @CategoryKey, CreatedUser = @CreatedUser, CategoryPoolId = @CategoryPoolId WHERE Id = @Id ";
            try
            {
                _dbContext.BeginTransaction();
                if (await _dbContext.ExecuteAsync(categoryUpdateQuery, category) >0 )
                {
                    await DeleteCategoryStringAsync(category.Id, userClaim);
                    await InsertCategoryString(category, userClaim);
                }
                _dbContext.CommitTransaction();
                return await _dbContext.ExecuteAsync(categoryUpdateQuery, param: category);
                   
            }
            catch (Exception)
            {
                //Rollback
                throw;
            }

        }


        private async Task<bool> InsertCategoryString(InternalCategory category, UserClaim userClaim)
        {
            string categoryStringInsertQuery = "INSERT INTO [dbo].[CategoryString]([Culture],[CategoryName],[Comment],[Description],[CategoryId])" +
               " VALUES(@Culture,@CategoryName,@Comment,@Description,@CategoryId)";
           
            try
            {
                await _dbContext.ExecuteAsync(categoryStringInsertQuery, category.CategoryStrings);

                return true;
            }
            catch (Exception)
            {

                throw;
            }

        }

    }

}
