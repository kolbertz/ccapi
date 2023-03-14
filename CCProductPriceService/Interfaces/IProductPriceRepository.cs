using CCApiLibrary.Models;
using CCProductPriceService.DTOs;
using Microsoft.AspNetCore.JsonPatch;

namespace CCProductPriceService.Interfaces
{
    public interface IProductPriceRepository : IDisposable

    {
        void Init(string database);

        Task<IEnumerable<ProductPriceBase>> GetAllProductPricesAsync();

        Task<ProductPriceBase> GetProductPriceByIdAsync(Guid productPriceId);

        Task<Guid> AddProductPriceAsync(ProductPriceBase productPrice, UserClaim userClaim);
     
        Task<int> UpdateProductPriceAsync(ProductPriceBase productPrice, UserClaim userClaim);

        Task<ProductPriceBase> PatchProductPriceAsync(Guid id,JsonPatchDocument pricePool, UserClaim userClaim);

        Task<int> DeleteProductPriceAsync(Guid id);
    }
}
