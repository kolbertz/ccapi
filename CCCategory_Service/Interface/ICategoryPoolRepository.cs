﻿using CCCategoryService.Data;
using CCCategoryService.Dtos;
using Microsoft.AspNetCore.JsonPatch;

namespace CCCategoryService.Interface
{
    public interface ICategoryPoolRepository : IDisposable
    {
        void Init(string database);
        Task<IEnumerable<CategoryPoolBase>> GetCategoryPoolsAsync();

        Task<CategoryPoolBase> GetCategoryPoolByIdAsync(Guid id, UserClaim userClaim);

        Task<Guid> AddCategoryPoolAsync(CategoryPoolBase categoryPool, UserClaim userClaim);

        Task<int> UpdateCategoryPoolAsync(CategoryPoolBase categoryPool, UserClaim userClaim);

        Task<CategoryPoolBase> PatchCategoryPoolAsync(Guid id, JsonPatchDocument jsonPatchDocument, UserClaim userClaim);

        Task<int> DeleteCategoryPoolAsync(Guid id);
    }
}