using CCApiLibrary.Interfaces;
using CCCategoryService.Interface;

namespace CCCategoryService.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public IApplicationDbConnection _dbContext { get; }

        public CategoryRepository(IApplicationDbConnection writeDbCoonection)

        {
            _dbContext = writeDbCoonection;
        }


        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        public async Task GetAllProducts()
        {
        }

        public async Task GetProductById()
        {
        }

        public async Task AddProductAsync()
        {
        }

        public Task<bool> UpdateProductAsync()
        {
            return  null;
        }

        public async Task PatchAsync()
        { 
        }

        public Task<int> DeleteProductAsync() 
        {
            return  null;
        }

    }



}
