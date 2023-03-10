using CCApiLibrary.Interfaces;
using CCApiLibrary.Models;
using CCProductPriceService.DTOs;
using CCProductPriceService.Interfaces;
using CCProductPriceService.InternalData;
using Microsoft.AspNetCore.JsonPatch;
using System.Dynamic;

namespace CCProductPriceService.Repositories
{
    public class ProductPricePoolRepository : IProductPricePoolRepository
    {
        private IApplicationDbConnection _dbContext { get; }

        public ProductPricePoolRepository(IApplicationDbConnection dbConnection)
        {
            _dbContext = dbConnection;
        }

        public void Init(string database)
        {
            _dbContext.Init(database);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        public async Task<IEnumerable<ProductPricePoolBase>> GetAllPricePools(UserClaim userClaim)
        {
            (string sysIdQuery, ExpandoObject paramObj) = GetClaimsQuery(userClaim);

            string query = $"SELECT Id, Name, Description, ParentProductPricePoolId, SystemSettingsId FROM ProductPricePool{sysIdQuery}";
            IEnumerable<InternalProductPricePool> pricePools = await _dbContext.QueryAsync<InternalProductPricePool>(query, paramObj).ConfigureAwait(false);
            List<ProductPricePoolBase> poolBases= new List<ProductPricePoolBase>();
            foreach (var pricePool in pricePools)
            {
                ProductPricePoolBase pricePoolBase = new ProductPricePoolBase();
                pricePoolBase.Id = pricePool.Id;
                pricePoolBase.Name.Add(new MultilanguageText { Culture = "de-DE", Text = pricePool.Name });
                if (!string.IsNullOrEmpty(pricePool.Description))
                {
                    pricePoolBase.Description.Add(new MultilanguageText { Culture = "de-DE", Text = pricePool.Description });
                }
                pricePoolBase.ParentPoolId = pricePool.ParentProductPricePoolId;
                pricePoolBase.SystemSettingsId= pricePool.SystemSettingsId;
                poolBases.Add(pricePoolBase);
            }
            return poolBases;
        }

        public async Task<ProductPricePool> GetPricePoolById(Guid pricePoolId, UserClaim userClaim)
        {
            (string sysIdQuery, ExpandoObject paramObj) = GetClaimsQuery(userClaim);
            string poolIdQuery = string.IsNullOrEmpty(sysIdQuery) ? " where" : " and" + " Id = @Id";
            string query = $"SELECT * FROM ProductPricePool{sysIdQuery}{poolIdQuery}";
            paramObj.TryAdd("Id", pricePoolId);
            InternalProductPricePool internalPool = await _dbContext.QuerySingleAsync<InternalProductPricePool>(query, paramObj).ConfigureAwait(false);
            ProductPricePool pricePool = new ProductPricePool(internalPool);
            return pricePool;
        }

        public Task<int> UpdatePricePool(ProductPricePool pricePool, UserClaim userClaim)
        {
            (string sysIdQuery, ExpandoObject paramObj) = GetClaimsQuery(userClaim);
            throw new NotImplementedException();
        }

        public Task<int> PatchPricePool(JsonPatchDocument pricePool, UserClaim userClaim)
        {
            (string sysIdQuery, ExpandoObject paramObj) = GetClaimsQuery(userClaim);
            throw new NotImplementedException();
        }

        public Task<int> DeletePricePool(Guid pricePoolId, UserClaim userClaim)
        {
            (string sysIdQuery, ExpandoObject paramObj) = GetClaimsQuery(userClaim);
            throw new NotImplementedException();
        }

        private static (string sysId, ExpandoObject paramObj) GetClaimsQuery(UserClaim userClaim)
        {
            string sysIdQuery = string.Empty;
            var paramObj = new ExpandoObject();

            if (userClaim != null)
            {
                sysIdQuery = " where SystemSettingsId = @sysId";
                paramObj.TryAdd("sysId", userClaim.SystemId);
            }
            return (sysIdQuery, paramObj);
        }
    }
}
