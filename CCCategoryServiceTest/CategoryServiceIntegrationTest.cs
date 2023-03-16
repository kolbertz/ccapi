using CCApiLibrary.Interfaces;
using CCApiLibrary.Models;
using CCApiTestLibrary;
using CCApiTestLibrary.BaseClasses;
using CCApiTestLibrary.Interfaces;
using CCApiTestLibrary.PopulateQueries;
using CCCategoryService.Dtos;
using CCCategoryService.Interface;
using CCCategoryService.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using System.Net;
using System.Text;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace CCCategoryServiceTest
{
    public class CategoryServiceIntegrationTest : ControllerTestBaseClass, IServiceIntegrationTestBase, IClassFixture<CCApiTestStart>
    {
        private WebApplicationFactory<Program> GetWebApplication()
        {
            return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<ICategoryRepository, CategoryRepository>();
                    PrepareServiceCollectionForTest(services);
                });
            });
        }

        [Fact]
        public async void Delete_Returns_204_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {
                    Guid categoryId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        categoryId = await PopulateCategory(dbConnection, "Glutenfrei", 1, 0, "Allergene");
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var response = await client.DeleteAsync("/api/v2/category/" + categoryId);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(CategoryQueries.DeleteCategories());
                        await dbConnection.ExecuteAsync(CategoryPoolQueries.DeleteCategoryPools());
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.DeleteSystemSettingsQuery());
                    }
                }
            }
        }

        [Fact]
        public async void GetByID_returns_404_If_given_Id_not_found()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                HttpClient client = application.CreateClient();
                CreateBasicClientWithAuth(client);
                var response = await client.GetAsync("/api/v2/category/" + "fab8c985-6147-4eba-b2c7-5f7012c4aeeb");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async void Get_All_204_No_Content()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                HttpClient client = application.CreateClient();
                CreateBasicClientWithAuth(client);
                HttpResponseMessage response = await client.GetAsync("/api/v2/category");
                string message = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                dynamic pools = JArray.Parse(message);
                Assert.Equal(0, pools.Count);
            }
        }

        [Fact]
        public async void Get_All_returns_200_And_List_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {
                    Guid categoryOne, categoryTwo;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        categoryOne = await PopulateCategory(dbConnection, "Glutenfrei", 1, 0, "Allergene");
                        categoryTwo = await PopulateCategory(dbConnection, "Heißgetränke", 2, 0, "Getränke", false);
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage respone = await client.GetAsync("/api/v2/category");
                    string messsage = await respone.Content.ReadAsStringAsync();
                    dynamic categories = JArray.Parse(messsage);
                    Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
                    Assert.Equal(2, categories.Count);
                    Assert.Equal("Glutenfrei", (string)categories[0].categoryNames[0].text);
                    Assert.Equal(2, (int)categories[1].categoryKey);
                    Assert.Equal(categoryOne, (Guid)categories[0].id);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(CategoryQueries.DeleteCategories());
                        await dbConnection.ExecuteAsync(CategoryPoolQueries.DeleteCategoryPools());
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.DeleteSystemSettingsQuery());
                    }
                }
            }
        }

        [Fact]
        public async void Get_By_Id_returns_200_And_Item_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {
                    Guid categoryId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        categoryId = await PopulateCategory(dbConnection, "Glutenfrei", 1, 0, "Allergene");
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage respone = await client.GetAsync("/api/v2/category/" + categoryId);
                    string messsage = await respone.Content.ReadAsStringAsync();
                    dynamic category = JObject.Parse(messsage);
                    Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
                    Assert.Equal(1, (int)category.key);
                    Assert.Equal("Glutenfrei", (string)category.categoryNames[0].text);
                    Assert.Equal(categoryId, (Guid)category.id);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(CategoryQueries.DeleteCategories());
                        await dbConnection.ExecuteAsync(CategoryPoolQueries.DeleteCategoryPools());
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.DeleteSystemSettingsQuery());
                    }
                }
            }
        }

        [Fact]
        public async void Get_returns_200_Authorized()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            HttpClient client = application.CreateClient();
            CreateBasicClientWithAuth(client);
            HttpResponseMessage respone = await client.GetAsync("/api/v2/category");
            Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
        }

        [Fact]
        public async void Get_returns_401_Unauthorized_if_not_authenticated()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            HttpClient client = application.CreateClient();
            HttpResponseMessage respone = await client.GetAsync("/api/v2/category");
            Assert.Equal(HttpStatusCode.Unauthorized, respone.StatusCode);
        }

        public async void Patch_Returns_204_And_Item_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {

                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(CategoryQueries.DeleteCategories());
                        await dbConnection.ExecuteAsync(CategoryPoolQueries.DeleteCategoryPools());
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.DeleteSystemSettingsQuery());
                    }
                }
            }
        }

        [Fact]
        public async void Post_returns_201_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {
                    Guid categoryPoolId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        categoryPoolId = await PrepareDatabaseForTest(dbConnection, 0, "Allergene");
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    CategoryBase category = new CategoryBase
                    {
                        CategoryNames = new List<MultilanguageText>()
                        {
                            new MultilanguageText("de-DE", "Glutenfrei")
                        },
                        CategoryKey = 15,
                        CategoryPoolId = categoryPoolId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("/api/v2/category/", httpContent);
                    var message = await response.Content.ReadAsStringAsync();
                    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(CategoryQueries.DeleteCategories());
                        await dbConnection.ExecuteAsync(CategoryPoolQueries.DeleteCategoryPools());
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.DeleteSystemSettingsQuery());
                    }
                }
            }
        }

        public async void Post_Returns_422_if_required_prop_is_missing()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {

                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(CategoryQueries.DeleteCategories());
                        await dbConnection.ExecuteAsync(CategoryPoolQueries.DeleteCategoryPools());
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.DeleteSystemSettingsQuery());
                    }
                }
            }
        }

        public async void Put_Returns_204_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {

                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(CategoryQueries.DeleteCategories());
                        await dbConnection.ExecuteAsync(CategoryPoolQueries.DeleteCategoryPools());
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.DeleteSystemSettingsQuery());
                    }
                }
            }
        }

        public async void Put_Returns_422_if_required_prop_is_missing()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {

                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(CategoryQueries.DeleteCategories());
                        await dbConnection.ExecuteAsync(CategoryPoolQueries.DeleteCategoryPools());
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.DeleteSystemSettingsQuery());
                    }
                }
            }
        }

        public async void Returns_BadRequestErrorMessageResult_when_route_Id_and_Model_Id_are_different()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {

                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(CategoryQueries.DeleteCategories());
                        await dbConnection.ExecuteAsync(CategoryPoolQueries.DeleteCategoryPools());
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.DeleteSystemSettingsQuery());
                    }
                }
            }
        }

        private async Task<Guid> PopulateCategory(IApplicationDbConnection dbConnection, string categoryName, int categoryKey, int type, string poolName, bool setSystemId = true)
        {
            Guid categoryPoolId = await PrepareDatabaseForTest(dbConnection, type, poolName, setSystemId);
            Guid categoryId = await dbConnection.ExecuteScalarAsync<Guid>(CategoryQueries.PopulateSingleCategory(categoryKey, categoryPoolId));
            await dbConnection.ExecuteAsync(CategoryQueries.PopulateCategoryStringsForSingleCategory(categoryId, categoryName));
            return categoryId;
        }

        private async Task<Guid> PrepareDatabaseForTest(IApplicationDbConnection dbConnection, int type, string poolName, bool setSystemId = true)
        {
            if (setSystemId)
            {
                await dbConnection.ExecuteAsync(SystemSettingsQueries.PopulateSystemSettingsQuery(StaticTestGuids.SystemSettingsId));
            }
            Guid categoryPoolId = await dbConnection.ExecuteScalarAsync<Guid>(CategoryPoolQueries.PopulateSingleCategoryPool(poolName, type));
            return categoryPoolId;
        }
    }
}
