using CCApiLibrary.Enums;
using CCApiLibrary.Models;
using CCProductService.Data;
using CCProductService.DTOs;
using Microsoft.AspNetCore.JsonPatch;
using System.Security.Claims;

namespace CCProductService.Interface
{
    public interface IProductRepository : IDisposable
    {
        void Init(string database);
        Task<IEnumerable<ProductStandardPrice>> GetAllProducts(int? take, int? skip, UserClaim userClaim);
        Task<ProductBase> GetProductById(Guid id, UserClaim userClaim);
        Task<Guid> AddProductAsync(ProductBase productDto, UserClaim userClaim);
        Task<int> UpdateProductAsync(Product productDto, UserClaim userClaim);
        Task<ProductBase> PatchProductAsync(Guid id, JsonPatchDocument jsonPatchDocument, UserClaim userClaim);
        Task<int> DeleteProductAsync(Guid id, UserClaim userClaim);
        Task<IEnumerable<ProductCategory>> GetCategoriesAsync(Guid id,CategoryPoolType categoryPoolType, UserClaim userClaim);
        Task<IEnumerable<string>> GetBarcodesAsync(Guid id, UserClaim userClaim);        
        Task<IEnumerable<ProductPrice>> GetProductPrices(Guid id, DateTimeOffset currentDate, UserClaim userClaim);
        Task<Guid> AddProductPrices(Guid id, List<ProductPriceBase> productPriceBases, UserClaim userClaim);
        Task<int> UpdateProductPrice(Guid id, ProductPriceBase productPriceBase, UserClaim userClaim);
        Task<int> DeleteProductPrice(Guid id);
        Task<Guid> SetCategoryByProductId(Guid id, ProductCategory productCategory, CategoryPoolType categoryPoolType, UserClaim userClaim);
        Task<int> UpdateCategoryByProductId(Guid id, ProductCategory proCategory, CategoryPoolType categoryPoolType, UserClaim userClaim);
        Task<IEnumerable<ProductPrice>> GetPricingHistory(Guid id,string start, string end, UserClaim userClaim);
    }
}
