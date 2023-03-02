using CCCategoryPoolService.Data;
using CCCategoryPoolService.DTOs;
using Microsoft.AspNetCore.JsonPatch;

namespace CCCategoryPoolService.Interface
{
    public interface ICategoryPoolRepository : IDisposable
    {
        void Init(string database);
        Task<IEnumerable<CategoryPoolDto>> GetProductPoolsAsync();

        Task<CategoryPoolDto> GetProductPoolByIdAsync(Guid id);

        Task<Guid> AddProductPoolAsync(CategoryPoolDto categoryPool, UserClaim userClaim);

        Task<int> UpdateProductPoolAsync(CategoryPoolDto productPool, UserClaim userClaim);

        Task<CategoryPoolDto> PatchProductPoolAsync(Guid id, JsonPatchDocument jsonPatchDocument, UserClaim userClaim);

        Task<int> DeleteProductPoolAsync(Guid id);
    }
}
