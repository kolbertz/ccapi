using CCApiLibrary.Models;
using CCProductPriceService.DTOs;
using Microsoft.AspNetCore.JsonPatch;

namespace CCProductPriceService.Interfaces
{
    public interface IProductPriceListRepository : IDisposable
    {
        void Init(string database);

        Task<IEnumerable<ProductPriceList>> GetAllProductPriceLists(UserClaim userClaim);

        Task<ProductPriceList> GetProductPriceListById(Guid productPriceListId, UserClaim userClaim);

        Task<Guid> AddProductPriceListAsync(ProductPriceListBase priceListBase, UserClaim userClaim);
                
        Task<int> UpdateProductPriceListAsync(ProductPriceList priceList, UserClaim userClaim);

        Task<ProductPriceList> PatchProductPriceList(Guid id, JsonPatchDocument pricePool, UserClaim userClaim);

        Task<int> DeletePriceListAsync(Guid id);
    }
}
