using CCApiLibrary.Interfaces;
using CCApiLibrary.Models;
using CCProductPriceService.DTOs;
using CCProductPriceService.Interfaces;
using CCProductPriceService.InternalData;
using Microsoft.AspNetCore.JsonPatch;
using System.Dynamic;

namespace CCProductPriceService.Repositories
{
    public class ProductPriceRepository : IProductPriceRepository
    {
        public IApplicationDbConnection _dbContext { get; }

        public ProductPriceRepository (IApplicationDbConnection dbConnection) 
        { 
            _dbContext= dbConnection;
        }

        public void Init(string database)
        {
            _dbContext.Init(database);
        }
        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        public Task<IEnumerable<ProductPriceBase>> GetAllProductPricesAsync()
        {
            var query = "SELECT Id, ProductId, ProductPricePoolId, ProductPriceListId, ManualPrice FROM ProductPrice";
            return _dbContext.QueryAsync<ProductPriceBase>(query);
        }

        public Task<ProductPriceBase> GetProductPriceByIdAsync(Guid id)
        {
            var query = "SELECT Id, ProductId, ProductPricePoolId, ProductPriceListId, ManualPrice FROM ProductPrice " +
                "WHERE Id = @ProductPriceId";
            return _dbContext.QuerySingleAsync<ProductPriceBase>(query, param: new { ProductPriceId = id });
        }


        public Task<Guid> AddProductPriceAsync(ProductPriceBase productPriceBase, UserClaim userClaim)
        {           
            var query = "INSERT INTO ProductPrice(Id, ProductId,ProductPricePoolId, ProductPriceListId, ManualPrice) " +
                "OUTPUT Inserted.Id " +
                "VALUES(@ProductId, @ProductPricePoolId, @ProductPriceListId, @ManualPrice);";
            InternalProductPrice  price = new InternalProductPrice(productPriceBase);            
            return _dbContext.ExecuteScalarAsync<Guid>(query, price);            
            
        }

        public Task <int> UpdateProductPriceAsync(ProductPriceBase productPrice, UserClaim userClaim) 
        {
            InternalProductPrice price = new InternalProductPrice(productPrice);

            return Update(price);
        }

        private Task<int> Update (InternalProductPrice price) 
        {
            var query = "UPDATE ProductPrice Set ProductId = @ProductId, ProductId = @ProductId, ProductPriceListId = @ProductPriceListId, ManualPrice = @ManualPrice, " +
                "WHERE Id = @Id";
            return _dbContext.ExecuteAsync(query, param:price);
        }


        public async Task<ProductPriceBase> PatchProductPriceAsync(Guid id, JsonPatchDocument jsonPatchDocument, UserClaim userClaim)
        {
            var query = "SELECT * FROM ProductPrice WHERE Id = @ProductPriceId";
            var p = new { ProductPriceId = id };
            InternalProductPrice price = await _dbContext.QuerySingleAsync<InternalProductPrice>(query, param: p);
            if (price != null)
            {
                ProductPrice productPrice = new ProductPrice(price);
                jsonPatchDocument.ApplyTo(productPrice);
                price.MergeProductPrice(productPrice);
                if (await Update(price).ConfigureAwait(false) > 0)
                {
                    return productPrice;
                }
            }
            return null;
        }

        public Task<int> DeleteProductPriceAsync(Guid id)
        {
            var query = "DELETE FROM ProductPrice WHERE Id = @Id";
            return _dbContext.ExecuteAsync(query, param: new { Id = id });
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
