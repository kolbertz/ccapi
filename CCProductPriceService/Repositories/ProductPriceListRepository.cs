using CCApiLibrary.Interfaces;
using CCApiLibrary.Models;
using CCProductPriceService.DTOs;
using CCProductPriceService.Interfaces;
using CCProductPriceService.InternalData;
using Microsoft.AspNetCore.JsonPatch;
using System.Dynamic;
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

        
        public Task<IEnumerable<ProductPriceList>> GetAllProductPriceLists(UserClaim userClaim)
        {            
            string sysIdQuery = string.Empty;
            var paramObj = new ExpandoObject();

            if (userClaim.SystemId.HasValue)
            {
                sysIdQuery = " WHERE ProductPriceList.SystemSettingsId = @SysId";
                paramObj.TryAdd("SysId", userClaim.SystemId);
            }
            var query = $"SELECT Id, [Key], Priority,[Name], SystemSettingsId FROM ProductPriceList{sysIdQuery}";       
            
            return _dbContext.QueryAsync<InternalProductPriceList, Guid, ProductPriceList>(query, (internalProductPriceList, sysId) =>
            { 
                return new ProductPriceList(internalProductPriceList, sysId);
           }, splitOn: "[Name], SystemSettingsId");            

        }

        public  async Task<ProductPriceList> GetProductPriceListById(Guid id, UserClaim userClaim)
        {
            var paramObj = new ExpandoObject();
            string sysIdQuery = string.Empty;

            if (userClaim.SystemId.HasValue)
            {
                sysIdQuery = " AND SystemSettingsId = @SysId";
                paramObj.TryAdd("SysId", userClaim.SystemId);
            }
            var query = "SELECT Id, [Name], [Key], Priority, SystemSettingsId FROM ProductPriceList " +
                $"WHERE Id = @ProductPriceListId{sysIdQuery}";

            InternalProductPriceList productPriceList = await _dbContext.QueryFirstOrDefaultAsync<InternalProductPriceList>(query, param: new { ProductPriceListId = id });
            if (productPriceList != null)
            {
                ProductPriceList priceList = new ProductPriceList(productPriceList);
                productPriceList.MergeProductPriceList(priceList);
                return priceList;
            }

            return null;
        }

        public Task<Guid> AddProductPriceListAsync(ProductPriceListBase productPriceList, UserClaim userClaim)
        {
            var query = "INSERT INTO ProductPriceList( [Name] ,[Key] , Priority, SystemSettingsId) " +
               "OUTPUT Inserted.Id " +
               "VALUES(@Name, @Key, @Priority, @SystemSettingsId);";
            InternalProductPriceList priceList = new InternalProductPriceList(productPriceList);
            return _dbContext.ExecuteScalarAsync<Guid>(query, priceList);
           
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
            return new ProductPriceList();
            
        }
        private Task<int> Update(InternalProductPriceList priceList)
        {
            var query = "UPDATE ProductPriceList Set  [Name] = @Name, [Key] = @Key, Priority = @Priority, " +
                "SystemSettingsId = @SystemSettingsId WHERE Id = @Id";
            return _dbContext.ExecuteAsync(query, param: priceList);
        }

        public Task<int> DeletePriceListAsync(Guid id)
        {
            var query = "DELETE FROM ProductPriceList WHERE Id = @Id";
            return _dbContext.ExecuteAsync(query, param: new { Id = id });
        }

        public Task<int> UpdateProductPriceListAsync(ProductPriceList priceList, UserClaim userClaim)
        {
            InternalProductPriceList productPriceList = new InternalProductPriceList(priceList);

            return Update(productPriceList);
        }
    }

}
