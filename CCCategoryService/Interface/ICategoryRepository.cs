using CCApiLibrary.Models;
using CCCategoryService.Data;
using CCCategoryService.Dtos;

namespace CCCategoryService.Interface
{
    public interface ICategoryRepository : IDisposable
    {
        void Init(string database);
        Task<IEnumerable<Category>> GetAllCategorys(int? take, int? skip, UserClaim userClaim);
        Task<Category> GetCategoryById(Guid id, UserClaim userClaim);
        Task<Guid> AddCategoryAsync(CategoryBase categoryDto, UserClaim userClaim);
        Task<bool> UpdateCategoryAsync(Category categoryDto, UserClaim userClaim);
        Task<CategoryBase> PatchCategoryAsync(Guid id, UserClaim userClaim);
        Task<int> DeleteCategoryAsync(Guid id, UserClaim userClaim);
    }
}
