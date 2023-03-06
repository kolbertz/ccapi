using CCApiLibrary.Interfaces;
using CCCategoryService.Data;
using CCCategoryService.Dtos;
using CCCategoryService.Interface;
using System.Collections;
using System.Dynamic;
using System.Net.Http.Headers;

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

        public async Task<IEnumerable<CategoryDto>> GetAllCategorys(int? take, int? skip, UserClaim userClaim)
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
                query = "SELECT Category.Id, CategoryKey, Category.CategoryPoolId, CategoryString.CategoryId, CategoryString.Culture, CategoryString.CategoryName, CategoryString.Comment, CategoryString.Description" +
                    $" from Category " +
                    $"JOIN CategoryString on Category.Id = CategoryString.CategoryId{categoryPoolQuery}" +
                    $" ORDER BY CategoryKey";
            }           
            var stringMap = new Dictionary<Guid, CategoryDto> ();

            //Baustelle
            //IEnumerable<CategoryDto> result = await _dbContext.QueryAsync <Category, CategoryString CategoryDto > (query, (c, cs) =>
            //{
            //    CategoryDto dto;
            //    if (!stringMap.TryGetValue(p.Id, out dto))
            //    {
            //        dto = new CategoryDto(p);
            //        stringMap.Add(p.Id, dto);
            //    }
            //    //dto.SetMultilanguageText(ps);
            //    return dto;
            //}
            return null;
            
        }

        public async Task<CategoryDto> GetCategoryById(Guid id, UserClaim userClaim)
        {
            CategoryDto dto = null;
            var paramObj = new ExpandoObject();
            string categoryPoolQuery = string.Empty;

            if (userClaim.CategoryPoolIds != null && userClaim.CategoryPoolIds.Count() > 0)
            {
                categoryPoolQuery = " and Category.CategoryPoolId in @poolIds";
                paramObj.TryAdd("poolIds", userClaim.CategoryPoolIds.ToArray());
            }

            //Baustelle
            string query = "$SELECT Category.Id, CategoryKey, CategoryPoolId, CategoryString.CategoryId, CategoryString.Culture, CategoryString.CategoryName, CategoryString.Comment, CategoryString.Description" +
                "$ from Category JOIN CategoryString on Category.Id = CategoryString.CategoryId " +
                "$WHERE Category.Id = @CategoryId and Category.CategoryPoolId{productPoolQuery}";

            paramObj.TryAdd("CategoryId", id);

            return (await _dbContext.QueryAsync<Category, CategoryString, CategoryDto>(query, (p, ps) =>
            {
                if (dto == null)
                {
                    dto = new CategoryDto(p);
                }
                // dto.SetMultilanguageText(ps);
                return dto;
            }, splitOn: "Id, CategoryId", param: paramObj).ConfigureAwait(false)).FirstOrDefault();

        }
            //return _dbContext.QuerySingleAsync<CategoryDto>(query, paramObj);

        

        public async Task<Guid> AddCategoryAsync(CategoryDto categoryDto, UserClaim userClaim)
        {

            string categoryInsertQuery = "$INSERT INTO [dbo].Category (CategoryKey,[CreatedDate],[CreatedUser],[LastUpdatedUser],[LastUpdatedDate],CategoryPoolId) " +
                "OUTPUT INSERTED.Id " +
                "$VALUES(@CategoryKey,@CreatedDate,@CreatedUser,@LastUpdatedUser,@LastUpdatedDate,@CategoryPoolId)";
            Category category = new Category();
            category.CreatedUser = category.LastUpdateUser = userClaim.UserId;
            category.CreatedDate = category.LastUpdatedDate = DateTimeOffset.Now;
            try
            {
                _dbContext.BeginTransaction();
                Guid id = await _dbContext.ExecuteScalarAsync<Guid>(categoryInsertQuery, category);
                _dbContext.CommitTransaction();
                return id;
            }
            catch (Exception)
            {
                //Rollback
                throw;
            }
        }

        public Task<bool> UpdateCategoryAsync(CategoryDto categoryDto, UserClaim userClaim)
        {
            Category category = new Category();
            category.LastUpdatedDate = DateTimeOffset.Now;
            category.LastUpdateUser = userClaim.UserId;
            return Update(category, categoryDto, userClaim);
        }

        public async Task<CategoryDto> PatchCategoryAsync(Guid id, UserClaim userClaim)
        {
            var query = "SELECT * FROM Product WHERE Id = @CategoryId";
            var p = new {CategoryId= id};
            Category category = await _dbContext.QuerySingleAsync<Category>(query, p);
            if (category != null) 
            { 
             CategoryDto categoryDto = new CategoryDto();
                category.LastUpdatedDate = DateTimeOffset.Now;
                category.LastUpdateUser = userClaim.UserId;
                if (await Update(category, categoryDto, userClaim).ConfigureAwait(false))
                {
                    return categoryDto;
                }
            }
            return new CategoryDto();
         }

        public Task<int> DeleteCategoryAsync(Guid id, UserClaim userClaim)
        {
            var query = "DELETE FROM [dbo].[Category] WHERE Id = @Id";

            return _dbContext.ExecuteAsync(query, param : new {Id = id });
        }

        public async Task<bool> Update(Category category, CategoryDto categoryDto, UserClaim userClaim)
        {
            var categoryUpdateQuery = "Update Category Set CategoryKey = @CategoryKey, CreatedUser = @CreatedUser, CategoryPoolId = @CategoryPoolId WHERE Id = @Id ";
            try
            {
                _dbContext.BeginTransaction();
                if (await _dbContext.ExecuteAsync(categoryUpdateQuery, category) > 0)
                {
                    await DeleteCategoryAsync(category.Id, userClaim);
                    await InsertCategoryString(categoryDto, userClaim);
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

        private async Task<bool> InsertCategoryString(CategoryDto categoryDto, UserClaim userClaim)
        {
            string categoryStringInsertQuery = "INSERT INTO [dbo].[CategoryString]([Culture],[Name],[Comment],[Description],[ProductId])" +
               " VALUES(@Culture,@Name,@Comment,@Description,@ProductId)";
            Category category = new Category();

            category.CreatedUser = category.LastUpdateUser = userClaim.UserId;
            category.CreatedDate = category.LastUpdatedDate= DateTime.Now;
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
