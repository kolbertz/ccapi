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
                // TODO: Change when multilanguage for ProcePool is available
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
               
        public Task<Guid> AddPricePoolAsync(ProductPricePoolBase pricePoolBase, UserClaim userClaim) 
        {
            var query = "INSERT INTO ProductPricePool( [Name], Description, ParentProductPricePoolId, CurrencyId, SystemSettingsId, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser) " +
                "OUTPUT Inserted.Id " +
                "VALUES( @Name, @Description, @ParentProductPricePoolId,@CurrencyId, @SystemSettingsId, @CreatedDate, @CreatedUser, @LastUpdatedDate, @LastUpdatedUser);";
            InternalProductPricePool pricePool = new InternalProductPricePool(pricePoolBase);
            pricePool.CreatedDate = pricePool.LastUpdatedDate = DateTimeOffset.Now;
            pricePool.CreatedUser = pricePool.LastUpdatedUser = userClaim.UserId;
            return _dbContext.ExecuteScalarAsync<Guid>(query, pricePool);
        }
        public Task<int> UpdatePricePool(ProductPricePool productPricePool, UserClaim userClaim)
        {
            InternalProductPricePool pricePool = new InternalProductPricePool(productPricePool);
            pricePool.LastUpdatedDate = DateTimeOffset.Now;
            pricePool.LastUpdatedUser = userClaim.UserId;
            return Update(pricePool);
        }

        private Task<int> Update (InternalProductPricePool pricePool)
        {
            var query = "UPDATE ProductPricePool Set  [Name] = @Name, Description = @Description, ParentProductPricePoolId = @ParentProductPricePoolId, CurrencyId = @CurrencyId " +
                "SystemSettingsId = @SystemSettingsId, LastUpdatedDate = @LastUpdatedDate, LastUpdatedUser = @LastUpdatedUser WHERE Id = @Id";
            return _dbContext.ExecuteAsync(query, param: pricePool);
        }

        public async Task<ProductPricePool> PatchPricePoolAsync(Guid id,JsonPatchDocument jsonPatchDocument, UserClaim userClaim)
        {
            var query = "SELECT * FROM ProductPricePool WHERE Id = @ProductPricePoolId ";
            var p = new { ProductPricePoolId = id };
            InternalProductPricePool pricePool = await _dbContext.QueryFirstOrDefaultAsync<InternalProductPricePool>(query, param: p);
            if (pricePool == null) 
            {
                ProductPricePool pricePoolBase = new ProductPricePool();
                jsonPatchDocument.ApplyTo(pricePoolBase);
                pricePool.MergeProductPricePool(pricePoolBase);
                pricePool.LastUpdatedDate = DateTimeOffset.Now;
                pricePool.LastUpdatedUser = userClaim.UserId;
                if (await Update(pricePool).ConfigureAwait(false) > 0)
                {
                    return pricePoolBase;
                }
            }
            return null;
        }

        public Task<int> DeletePricePool(Guid id)
        {
            var query = "DELETE FROM ProductPricePool WHERE Id = @Id";
            return _dbContext.ExecuteAsync(query, param: new {Id = id });
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
