using CCApiLibrary.Interfaces;
using CCApiLibrary.Models;
using CCProductPriceService.DTOs;
using CCProductPriceService.Interfaces;
using CCProductPriceService.InternalData;
using Microsoft.AspNetCore.JsonPatch;
using System.Reflection.Metadata.Ecma335;

namespace CCProductPriceService.Repositories
{
    public class ProductPriceListRepository  : IProductPriceListRepository
    {
        public IApplicationDbConnection _dbContext { get; }

        public ProductPriceListRepository(IApplicationDbConnection dbConnection)
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

        
        public Task<IEnumerable<ProductPriceList>> GetAllProductPriceLists()
        {
            var query = "SELECT Id, [Key], Priority,[Name], SystemSettingsId FROM ProductPriceList";

            return _dbContext.QueryAsync<InternalProductPriceList, Guid, ProductPriceList>(query, (internalProductPriceList, sysId) =>
            { 
                return new ProductPriceList(internalProductPriceList, sysId);
           }, splitOn: "[Name], SystemSettingsId");
            

        }

        public  Task<InternalProductPriceList> GetProductPriceListById(Guid id)
        {
            var query = "SELECT Id, [Name], [Key], Priority, SystemSettingsId FROM ProductPriceList " +
                "WHERE Id = @ProductPriceListId";
            return _dbContext.QueryFirstOrDefaultAsync<ProductPriceList>(query,param: new {ProductPriceListId = id });
        }

        public Task<Guid> AddProductPriceListAsync(ProductPriceList productPriceList, UserClaim userClaim)
        {
            var query = "INSERT INTO ProductPriceList(Id, [Name] ,[Key] , Priority, SystemSettingsId) " +
               "OUTPUT Inserted.Id " +
               "VALUES(@Id, @Name, @Key, @Priority, @SystemSettingsId);";
            return null;
        }
        public async Task<ProductPriceList> PatchProductPriceList(Guid id,JsonPatchDocument jsonPatchDocument, UserClaim userClaim)
        {
            var query = "SELECT * FROM ProductPriceList WHERE Id = @ProductPriceListId ";
            var p = new { ProductPriceListId = id };
            InternalProductPriceList productPriceList = await _dbContext.QueryFirstOrDefaultAsync<InternalProductPriceList>(query, param: p);
            if (productPriceList != null) { }
            { 
                ProductPriceList priceList = new ProductPriceList(productPriceList);
                jsonPatchDocument.ApplyTo(priceList);
                productPriceList.MergeProductPriceList(priceList);
                if (await Update(productPriceList).ConfigureAwait(false)>0)                 
                {
                    return priceList;
                }
            }
            return null;
            
        }
        private Task<int> Update(InternalProductPriceList priceList)
        {
            var query = "UPDATE ProductPriceList Set  [Name] = @Name, [Key] = @Key, Priority = @Priority " +
                "SystemSettingsId = @SystemSettingsId WHERE Id = @Id";
            return _dbContext.ExecuteAsync(query, param: priceList);
        }

        public Task<int> DeletePriceListAsync(Guid id)
        {
            var query = "DELETE FROM ProductPriceList WHERE Id = @Id";
            return _dbContext.ExecuteAsync(query, param: new { Id = id });
        }
    }

}
