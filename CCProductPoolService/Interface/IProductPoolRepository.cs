using CCProductPoolService.Data;
using CCProductPoolService.Dtos;
using Microsoft.AspNetCore.JsonPatch;

namespace CCProductPoolService.Interface
{
    public interface IProductPoolRepository : IDisposable
    {
        void Init(string database);
        Task<IEnumerable<ProductPoolDto>> GetProductPoolsAsync();

        Task<ProductPoolDto> GetProductPoolByIdAsync(Guid id);

        Task<Guid> AddProductPoolAsync(ProductPoolDto productPool, UserClaim userClaim);

        Task<int> UpdateProductPoolAsync(ProductPoolDto productPool, UserClaim userClaim);

        Task<ProductPoolDto> PatchProductPoolAsync(Guid id, JsonPatchDocument jsonPatchDocument, UserClaim userClaim);

        Task<int> DeleteProductPoolAsync(Guid id);
    }
}
