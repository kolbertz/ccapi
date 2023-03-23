using CCApiLibrary.Models;
using CCProductPriceService.DTOs;
using Microsoft.AspNetCore.JsonPatch;

namespace CCProductPriceService.Interfaces
{
    public interface IProductPriceRepository : IDisposable

    {
        void Init(string database);

        Task<IEnumerable<ProductPriceBase>> GetAllProductPricesAsync(UserClaim userClaim);

        Task<ProductPriceBase> GetProductPriceByIdAsync(Guid productPriceId, UserClaim userClaim);

        Task<Guid> AddProductPriceAsync(ProductPriceBase productPrice, UserClaim userClaim);
     
        Task<int> UpdateProductPriceAsync(ProductPriceBase productPrice, UserClaim userClaim);

        Task<ProductPriceBase> PatchProductPriceAsync(Guid id,JsonPatchDocument pricePool, UserClaim userClaim);

        Task<int> DeleteProductPriceAsync(Guid id);
    }
}
