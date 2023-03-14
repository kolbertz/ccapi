using CCApiLibrary.Interfaces;
using CCApiLibrary.Models;
using CCProductPriceService.DTOs;
using CCProductPriceService.Interfaces;

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

        
        public Task<IEnumerable<ProductPriceListBase>> GetAllProductPriceLists()
        {
            var query = "SELECT Id, [Name], Key,Priority,SystemSettingsId FROM ProductPriceList";
            return _dbContext.QueryAsync<ProductPriceListBase>(query);            
        }

        public  Task<ProductPriceList> GetProductPriceListById(Guid id)
        {
            var query = "SELECT Id, [Name], Key,Priority,SystemSettingsId FROM ProductPriceList " +
                "WHERE Id = @ProductPriceListId";
            return _dbContext.QuerySingleAsync< ProductPriceList>(query,param: new {ProductPriceListId = id });
        }

        public Task<Guid> AddProductPriceListAsync(ProductPriceList productPriceList, UserClaim userClaim)
        {
            var query = "INSERT INTO ProductPriceList(Id, [Name],Key, Priority, SystemSettingsId) " +
               "OUTPUT Inserted.Id " +
               "VALUES(@Id, @Name, @Key, @Priority, @SystemSettingsId);";
            return null;
        }

        public Task<int> DeletePriceListAsync(Guid id)
        {
            var query = "DELETE FROM ProductPriceList WHERE Id = @Id";
            return _dbContext.ExecuteAsync(query, param: new { Id = id });
        }
    }

}
