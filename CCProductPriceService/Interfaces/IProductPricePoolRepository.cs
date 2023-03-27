using CCApiLibrary.Models;
using CCProductPriceService.DTOs;
using Microsoft.AspNetCore.JsonPatch;

namespace CCProductPriceService.Interfaces
{
    public interface IProductPricePoolRepository : IDisposable
    {
        void Init(string database);

        Task<IEnumerable<ProductPricePool>> GetAllPricePools(UserClaim userClaim);

        Task<ProductPricePool> GetPricePoolById(Guid pricePoolId, UserClaim userClaim);

        Task<Guid> AddPricePoolAsync(ProductPricePoolBase pricePoolBase, UserClaim userClaim);

        Task<int> UpdatePricePool(ProductPricePool pricePool, UserClaim userClaim);

        Task<ProductPricePool> PatchPricePoolAsync(Guid id, JsonPatchDocument jsonPatchDocument, UserClaim userClaim);

        Task<int> DeletePricePool(Guid id);
    }
}
