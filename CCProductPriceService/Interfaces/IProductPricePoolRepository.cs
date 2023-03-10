using CCApiLibrary.Models;
using CCProductPriceService.DTOs;
using Microsoft.AspNetCore.JsonPatch;

namespace CCProductPriceService.Interfaces
{
    public interface IProductPricePoolRepository : IDisposable
    {
        void Init(string database);

        Task<IEnumerable<ProductPricePoolBase>> GetAllPricePools(UserClaim userClaim);

        Task<ProductPricePool> GetPricePoolById(Guid pricePoolId, UserClaim userClaim);

        Task<int> UpdatePricePool(ProductPricePool pricePool, UserClaim userClaim);

        Task<int> PatchPricePool(JsonPatchDocument pricePool, UserClaim userClaim);

        Task<int> DeletePricePool(Guid pricePoolId, UserClaim userClaim);
    }
}
