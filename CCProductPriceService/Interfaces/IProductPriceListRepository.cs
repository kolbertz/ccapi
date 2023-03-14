using CCApiLibrary.Models;
using CCProductPriceService.DTOs;
using Microsoft.AspNetCore.JsonPatch;

namespace CCProductPriceService.Interfaces
{
    public interface IProductPriceListRepository : IDisposable
    {
        void Init(string database);

        Task<IEnumerable<ProductPriceListBase>> GetAllProductPriceLists();

        Task<ProductPriceList> GetProductPriceListById(Guid productPriceListId);

        Task<Guid> AddProductPriceListAsync(ProductPriceList priceList, UserClaim userClaim);

        //Baustelle
        //Task<int> UpdatePricePool(ProductPricePool pricePool, UserClaim userClaim);

        //Task<int> PatchPricePool(JsonPatchDocument pricePool, UserClaim userClaim);

        Task<int> DeletePriceListAsync(Guid id);
    }
}
