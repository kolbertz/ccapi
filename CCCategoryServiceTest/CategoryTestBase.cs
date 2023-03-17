using CCApiLibrary.Interfaces;
using CCApiTestLibrary;
using CCApiTestLibrary.BaseClasses;
using CCApiTestLibrary.PopulateQueries;
using CCCategoryService.Interface;
using CCCategoryService.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace CCCategoryServiceTest
{
    public class CategoryTestBase : ControllerTestBaseClass
    {
        public WebApplicationFactory<Program> GetWebApplication()
        {
            return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<ICategoryRepository, CategoryRepository>();
                    services.AddSingleton<ICategoryPoolRepository, CategoryPoolRepository>();
                    PrepareServiceCollectionForTest(services);
                });
            });
        }


        public async Task<(Guid catgoryId, Guid categoryPoolId)> PopulateCategory(IApplicationDbConnection dbConnection, string categoryName, int categoryKey, int type, string poolName, bool setSystemId = true)
        {
            Guid categoryPoolId = await PrepareDatabaseForTest(dbConnection, type, poolName, setSystemId);
            Guid categoryId = await dbConnection.ExecuteScalarAsync<Guid>(CategoryQueries.PopulateSingleCategory(categoryKey, categoryPoolId));
            await dbConnection.ExecuteAsync(CategoryQueries.PopulateCategoryStringsForSingleCategory(categoryId, categoryName));
            return (categoryId, categoryPoolId);
        }

        public async Task<Guid> PrepareDatabaseForTest(IApplicationDbConnection dbConnection, int type, string poolName, bool setSystemId = true)
        {
            if (setSystemId)
            {
                await SetSystemId(dbConnection);
            }
            Guid categoryPoolId = await dbConnection.ExecuteScalarAsync<Guid>(CategoryPoolQueries.PopulateSingleCategoryPool(poolName, type));
            return categoryPoolId;
        }

        public async Task ResetDatabaseAfterTesting(IServiceScope services)
        {
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                dbConnection.Init(databaseKey);
                await dbConnection.ExecuteAsync(CategoryQueries.DeleteCategories());
                await dbConnection.ExecuteAsync(CategoryPoolQueries.DeleteCategoryPools());
                await dbConnection.ExecuteAsync(SystemSettingsQueries.DeleteSystemSettingsQuery());
            }

        }

        public Task SetSystemId(IApplicationDbConnection dbConnection)
        {
            return dbConnection.ExecuteAsync(SystemSettingsQueries.PopulateSystemSettingsQuery(StaticTestGuids.SystemSettingsId));
        }
    }
}
