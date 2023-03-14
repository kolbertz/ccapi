using CCApiLibrary.Interfaces;
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
            var paramObj = new ExpandoObject();

            if (userClaim.CategoryPoolIds != null && userClaim.CategoryPoolIds.Count() > 0)
            {
                categoryPoolQuery = " where Category.CategoryPoolId in @poolIds";
                paramObj.TryAdd("poolIds", userClaim.CategoryPoolIds.ToArray());
            }

            if (take.HasValue && skip.HasValue)
            {
                query = $"Select t.Id, t.CategoryPoolId, t.CategoryKey,  CategoryString.CategoryId, CategoryString.Culture, CategoryString.CategoryName, CategoryString.Comment, CategoryString.Description " +
                    $"				FROM (Select Id, CategoryKey, CategoryPoolId	From Category{categoryPoolQuery}	" +
                    $"				ORDER BY CategoryKey      " +
                    $"              OFFSET @offset ROWS FETCH NEXT @fetch ROWS ONLY) as t 		" +
                    $"			LEFT JOIN CategoryString on t.Id = CategoryString.CategoryId";

                paramObj.TryAdd("offset", skip.Value);
                paramObj.TryAdd("fetch", skip.Value);
            }
            else
            {
                query = "SELECT Category.Id, CategoryKey, Category.CategoryPoolId, CategoryString.Id AS CategoryStringID, CategoryString.CategoryId, CategoryString.Culture, CategoryString.CategoryName, CategoryString.Comment, CategoryString.Description" +
                    $" from Category " +
                    $"JOIN CategoryString on Category.Id = CategoryString.CategoryId{categoryPoolQuery}" +
                    $" ORDER BY CategoryKey";
            }

            var stringMap = new Dictionary<Guid, Category>();
            IEnumerable<Category> result = await _dbContext.QueryAsync<InternalCategory, InternalCategoryString, Category>(query, (c, cs) =>
            {
                Category dto;
                if (!stringMap.TryGetValue(c.Id, out dto))
                {
                    dto = new Category(c);
                    //CategoryHelper.ParseCategoryToDto(c,dto);
                    stringMap.Add(c.Id, dto);
                }
                
                dto.SetMultilanguageText(cs);
                return dto;
            }, splitOn: "Id, CategoryId", param: paramObj).ConfigureAwait(false);
            return stringMap.Values.ToList().AsReadOnly();
            
        }

        public async Task<Category> GetCategoryById(Guid id, UserClaim userClaim)
        {
            Category dto = null;
            var paramObj = new ExpandoObject();
            string categoryPoolQuery = string.Empty;

            if (userClaim.CategoryPoolIds != null && userClaim.CategoryPoolIds.Count() > 0)
            {
                categoryPoolQuery = " and Category.CategoryPoolId in @poolIds";
                paramObj.TryAdd("poolIds", userClaim.CategoryPoolIds.ToArray());
            }

            string query = $"SELECT Category.Id, CategoryKey, CategoryPoolId, CategoryString.CategoryId,CategoryString.Id AS CategoryStringID, CategoryString.Culture, CategoryString.CategoryName, CategoryString.Comment, CategoryString.Description " +
                 $"from Category JOIN CategoryString on Category.Id = CategoryString.CategoryId " +
                 $"WHERE Category.Id = @CategoryId{categoryPoolQuery}";

            paramObj.TryAdd("CategoryId", id);

            return (await _dbContext.QueryAsync<InternalCategory, InternalCategoryString, Category>(query, (p, ps) =>
            {
                if (dto == null)
                {
                    dto = new Category(p);
                    CategoryHelper.ParseCategoryToDto(p, dto);
                }
                 dto.SetMultilanguageText(ps);
                return (Category)dto;
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

        public Task<bool> UpdateCategoryAsync(CategoryBase categoryDto, UserClaim userClaim)
        {

            InternalCategory category = new InternalCategory();
            CategoryHelper.ParseDtoToCategory(categoryDto, category);
            category.LastUpdatedDate = DateTimeOffset.Now;
            category.LastUpdatedUser = userClaim.UserId;
            return Update(category, categoryDto, userClaim);
        }

        public async Task<CategoryBase> PatchCategoryAsync(Guid id, UserClaim userClaim)
        {
            var query = "SELECT * FROM Product WHERE Id = @CategoryId";
            var p = new { CategoryId = id };
            InternalCategory category = await _dbContext.QuerySingleAsync<InternalCategory>(query, p);
            if (category != null)
            {
                CategoryBase categoryDto = new CategoryBase();
                category.LastUpdatedDate = DateTimeOffset.Now;
                category.LastUpdatedUser = userClaim.UserId;
                if (await Update(category, categoryDto, userClaim).ConfigureAwait(false))
                {
                    return categoryDto;
                }
            }
            return new CategoryBase();
        }

        public Task<int> DeleteCategoryAsync(Guid id, UserClaim userClaim)
        {
            var query = "DELETE FROM [dbo].[Category] WHERE Id = @Id";

            return _dbContext.ExecuteAsync(query, param: new { Id = id });
        }


        public async Task<bool> Update(InternalCategory category, CategoryBase categoryBase, UserClaim userClaim)
        {
            var categoryUpdateQuery = "Update Category Set CategoryKey = @CategoryKey, CreatedUser = @CreatedUser, CategoryPoolId = @CategoryPoolId WHERE Id = @Id ";
            try
            {
                _dbContext.BeginTransaction();
                if (await _dbContext.ExecuteAsync(categoryUpdateQuery, category) > 0)
                {
                    await DeleteCategoryAsync(category.Id, userClaim);
                    await InsertCategoryString(category, userClaim);
                }

                _dbContext.CommitTransaction();
                return false;
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
