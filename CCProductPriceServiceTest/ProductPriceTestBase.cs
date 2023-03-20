using CCApiLibrary.Interfaces;
using CCApiTestLibrary;
using CCApiTestLibrary.BaseClasses;
using CCApiTestLibrary.PopulateQueries;
using CCProductPriceService.Interfaces;
using CCProductPriceService.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace CCProductPriceServiceTest
{
    public class ProductPriceTestBase : ControllerTestBaseClass
    {
        public WebApplicationFactory<Program> GetWebApplication()
        {
            return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IProductPriceRepository, ProductPriceRepository>();
                    services.AddSingleton<IProductPricePoolRepository, ProductPricePoolRepository>();
                    services.AddSingleton<IProductPriceListRepository, ProductPriceListRepository>();
                    PrepareServiceCollectionForTest(services);
                });
            });
        }

        public Task<Guid> PopulateSingleProductPriceList(IApplicationDbConnection dbConnection, string name, int key, int priority)
        {
            return dbConnection.ExecuteScalarAsync<Guid>(ProductPriceQueries.PopulateSingleProductPriceList(name, key, priority));
        }

        public Task<Guid> PopulateSingleProductPricePool(IApplicationDbConnection dbConnection, string name, string description)
        {
            return dbConnection.ExecuteScalarAsync<Guid>(ProductPriceQueries.PopulateSingleProductPricePool(name, description));
        }
 
        public Task SetSystemSettingsId(IApplicationDbConnection dbConnection)
        {
            return dbConnection.ExecuteAsync(SystemSettingsQueries.PopulateSystemSettingsQuery(StaticTestGuids.SystemSettingsId));
        }

        public async Task ResetDatabaseAfterTesting(IServiceScope services)
        {
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                dbConnection.Init(databaseKey);
                await dbConnection.ExecuteAsync(ProductPriceQueries.DeleteProductPriceLists());
                await dbConnection.ExecuteAsync(ProductPriceQueries.DeleteProductPricePools());
                await dbConnection.ExecuteAsync(SystemSettingsQueries.DeleteSystemSettingsQuery());
            }
        }
    }
}
