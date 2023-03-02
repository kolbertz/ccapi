using CCApiLibrary.Interfaces;
using CCCategoryPoolService.Interface;

namespace CCCategoryPoolService.Repositories
{
    public class CategoryServiceRepository : ICategoryRepository
    {
        public IApplicationDbConnection _dbContext { get; }

        public CategoryServiceRepository(IApplicationDbConnection writeDbCoonection)

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
            return  ;
        }

        public async Task PatchAsync()
        { 
        }

        public Task<int> DeleteProductAsync() 
        {
            return  ;
        }

    }



}
