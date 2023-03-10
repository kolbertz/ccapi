using CCApiLibrary.Models;
using CCCategoryService.Data;
using CCCategoryService.Dtos;
using Microsoft.AspNetCore.JsonPatch;

namespace CCCategoryService.Interface
{
    public interface ICategoryPoolRepository : IDisposable
    {
        void Init(string database);
        Task<IEnumerable<CategoryPoolDto>> GetCategoryPoolsAsync();

        Task<CategoryPoolDto> GetCategoryPoolByIdAsync(Guid id);

        Task<Guid> AddCategoryPoolAsync(CategoryPoolDto categoryPool, UserClaim userClaim);

        Task<int> UpdateCategoryPoolAsync(CategoryPoolDto categoryPool, UserClaim userClaim);

        Task<CategoryPoolDto> PatchCategoryPoolAsync(Guid id, JsonPatchDocument jsonPatchDocument, UserClaim userClaim);

        Task<int> DeleteCategoryPoolAsync(Guid id);
    }
}
