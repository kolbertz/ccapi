using CCApiLibrary.Models;
using CCProductPoolService.Data;
using CCProductPoolService.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections;

namespace CCProductPoolService.Interface
{
    public interface IProductPoolRepository : IDisposable
    {
        void Init(string database);
        Task<IEnumerable<ProductPool>> GetProductPoolsAsync(UserClaim userClaim);

        Task<ProductPool> GetProductPoolByIdAsync(Guid id, UserClaim userClaim);

        Task<Guid> AddProductPoolAsync(ProductPoolBase productPool, UserClaim userClaim);

        Task<int> UpdateProductPoolAsync(ProductPool productPool, UserClaim userClaim);

        Task<ProductPool> PatchProductPoolAsync(Guid id, JsonPatchDocument jsonPatchDocument, UserClaim userClaim);

        Task<int> DeleteProductPoolAsync(Guid id);
    }
}
