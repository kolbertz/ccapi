﻿using CCApiLibrary.Models;
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
        Task<bool> UpdateProductAsync(ProductBase productDto, UserClaim userClaim);
        Task<ProductBase> PatchProductAsync(Guid id,UserClaim userClaim);
        Task <int>DeleteProductAsync(Guid id, UserClaim userClaim);
        Task<IEnumerable<ProductCategoryDto>> GetCategoriesAsnyc(Guid id, UserClaim userClaim);
        Task<IEnumerable<string>> GetBarcodesAsync(Guid id, UserClaim userClaim);
        Task<IEnumerable<ProductPriceDto>> GetProductPrices(Guid id, UserClaim userClaim);

    }
}
 