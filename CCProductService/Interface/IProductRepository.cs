using CCProductService.Data;
using CCProductService.DTOs;
using Microsoft.AspNetCore.JsonPatch;
using System.Security.Claims;

namespace CCProductService.Interface
{
    public interface IProductRepository
    {
        Task<IReadOnlyList<ProductDto>> GetAllProducts(int? take, int? skip, UserClaim userClaim);
        Task<ProductDto> GetProductById(Guid id, UserClaim userClaim);
        Task<Guid> AddProductAsync(ProductDto productDto, UserClaim userClaim);
        Task UpdateProductAsync(ProductDto productDto, UserClaim userClaim);
        Task<ProductDto> PatchProductAsync(Guid id, JsonPatchDocument productPatch);
        Task DeleteProductAsync(Guid id, UserClaim userClaim);
        Task<IEnumerable<ProductCategoryDto>> GetCategoriesAsnyc(Guid id, UserClaim userClaim);
        Task<IReadOnlyList<string>> GetBarcodesAsync(Guid id, UserClaim userClaim);
        Task<IEnumerable<ProductPriceDto>> GetProductPrices(Guid id, UserClaim userClaim);

    }
}
