using CCProductPoolService.Data;
using CCProductPoolService.Dtos;
using Microsoft.AspNetCore.JsonPatch;

namespace CCProductPoolService.Interface
{
    public interface IProductPoolRepository
    {
        Task<IReadOnlyList<ProductPoolDto>> GetProductPoolsAsync();

        Task<ProductPoolDto> GetProductPoolByIdAsync(Guid id);

        Task<Guid> AddProductPoolAsync(ProductPoolDto productPool, UserClaim userClaim);

        Task UpdateProductPoolAsync(ProductPoolDto productPool, UserClaim userClaim);

        Task<ProductPoolDto> PatchProductPoolAsync(Guid id, JsonPatchDocument jsonPatchDocument, UserClaim userClaim);

        Task DeleteProductPoolAsync(Guid id);
    }
}
