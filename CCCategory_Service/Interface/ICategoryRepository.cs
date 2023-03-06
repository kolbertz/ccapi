using CCCategoryService.Data;
using CCCategoryService.Dtos;

namespace CCCategoryService.Interface
{
    public interface ICategoryRepository : IDisposable
    {
        void Init(string database);
        Task<IEnumerable<CategoryDto>> GetAllCategorys(int? take, int? skip, UserClaim userClaim);
        Task<CategoryDto> GetCategoryById(Guid id, UserClaim userClaim);
        Task<Guid> AddCategoryAsync(CategoryDto categoryDto, UserClaim userClaim);
        Task<bool> UpdateCategoryAsync(CategoryDto categoryDto, UserClaim userClaim);
        Task<CategoryDto> PatchCategoryAsync(Guid id, UserClaim userClaim);
        Task<int> DeleteCategoryAsync(Guid id, UserClaim userClaim);
    }
}
