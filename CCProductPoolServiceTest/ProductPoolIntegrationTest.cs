using CCApiLibrary.Interfaces;
using CCApiLibrary.Models;
using CCApiTestLibrary.BaseClasses;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using CCProductPoolService.Interface;
using CCProductPoolService.Repositories;
using CCApiTestLibrary.PopulateQueries;
using CCProductPoolService.Dtos;
using CCApiTestLibrary.Interfaces;
using CCApiTestLibrary;
using System.Net.Http;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace CCProductPoolServiceTest
{
    public class ProductPoolIntegrationTest : ControllerTestBaseClass, IServiceIntegrationTestBase, IClassFixture<CCApiTestStart>
    {
        private WebApplicationFactory<Program> GetWebApplication()
        {
            return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IProductPoolRepository, ProductPoolRepository>();
                    PrepareServiceCollectionForTest(services);
                });
            });
        }

        public ProductPoolIntegrationTest() { }

        [Fact]
        public async void Get_returns_401_Unauthorized_if_not_authenticated()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            HttpClient client = application.CreateClient();
            HttpResponseMessage respone = await client.GetAsync("/api/v2/productpool");
            Assert.Equal(HttpStatusCode.Unauthorized, respone.StatusCode);
        }

        [Fact]
        public async void Get_returns_200_Authorized()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            HttpClient client = application.CreateClient();
            CreateBasicClientWithAuth(client);
            HttpResponseMessage respone = await client.GetAsync("/api/v2/productpool");
            Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
        }

        [Fact]
        public async void Get_All_returns_200_And_List_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {

                try
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        // Populate DB
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.PopulateSystemSettingsQuery(StaticTestGuids.SystemSettingsId));
                        await dbConnection.ExecuteAsync(ProductPoolQueries.PopulateProductPoolList());
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage respone = await client.GetAsync("/api/v2/productpool");
                    string messsage = await respone.Content.ReadAsStringAsync();
                    dynamic pools = JArray.Parse(messsage);
                    Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
                    Assert.Equal(2, pools.Count);
                    Assert.Equal(1, (int)pools[0].key);
                    Assert.Equal("Pool 2", (string)pools[1].name);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
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
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);

                        // Populate DB with a systemSetting
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.PopulateSystemSettingsQuery(StaticTestGuids.SystemSettingsId));
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    ProductPool productPool = new ProductPool
                    {
                        Descriptions = new List<MultilanguageText>
                        { 
                            new MultilanguageText("de-DE","ApiController Test Pool")
                        },
                        Names = new List<MultilanguageText>
                        { 
                            new MultilanguageText("de-DE","ApiController Test Pool")
                        },
                        Key = 1,
                        SystemSettingsId = StaticTestGuids.SystemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("/api/v2/productpool/", httpContent);
                    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
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
                    Guid productPoolId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        // Populate DB
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.PopulateSystemSettingsQuery(StaticTestGuids.SystemSettingsId));
                        productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool(1, "Pool 1"));
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage respone = await client.GetAsync("/api/v2/productpool/" + productPoolId);
                    string messsage = await respone.Content.ReadAsStringAsync();
                    dynamic pool = JObject.Parse(messsage);
                    Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
                    Assert.Equal(1, (int)pool.key);
                    Assert.Equal("Pool 1", (string)pool.name);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.DeleteSystemSettingsQuery());
                    }
                }
            }
        }

        [Fact]
        public async void Put_Returns_204_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {

                try
                {
                    Guid productPoolId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        // Populate DB
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.PopulateSystemSettingsQuery(StaticTestGuids.SystemSettingsId));
                        productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool());
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    ProductPool productPool = new ProductPool
                    {
                        Id = productPoolId,
                        Descriptions = new List<MultilanguageText>
                        {
                            new MultilanguageText("de-DE","ApiController Put Test Pool")
                        },
                        Names = new List<MultilanguageText>
                        {
                            new MultilanguageText("de-DE","ApiController Put Test Pool")
                        },
                        Key = 2,
                        SystemSettingsId = StaticTestGuids.SystemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("/api/v2/productpool/" + productPoolId, httpContent);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                    // if PATCH return successful status code, proove it by sending a corresponding GET request
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        HttpResponseMessage getResponse = await client.GetAsync("/api/v2/productpool/" + productPoolId);
                        dynamic patchedPool = JObject.Parse(await getResponse.Content.ReadAsStringAsync());
                        Assert.Equal(2, (int)patchedPool.key);
                        Assert.Equal("ApiController Put Test Pool", (string)patchedPool.description);
                        Assert.Equal("ApiController Put Test Pool", (string)patchedPool.name);
                    }
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.DeleteSystemSettingsQuery());
                    }
                }
            }
        }

        //[Fact]
        public async void Patch_Returns_204_And_Item_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {

                try
                {
                    Guid productPoolId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        // Populate DB
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.PopulateSystemSettingsQuery(StaticTestGuids.SystemSettingsId));
                        productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool());
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var patch = new[]
                    {
                        new
                        {
                            op = "add",
                            path = "/description",
                            value = "Patch Test"
                        },
                        new
                        {
                            op = "replace",
                            path = "/key",
                            value = "99"
                        }
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(patch), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PatchAsync("/api/v2/productpool/" + productPoolId, httpContent);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                    // if PATCH return successful status code, proove it by sending a corresponding GET request
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        HttpResponseMessage getResponse = await client.GetAsync("/api/v2/productpool/" + productPoolId);
                        dynamic patchedPool = JObject.Parse(await getResponse.Content.ReadAsStringAsync());
                        Assert.Equal(99, (int)patchedPool.key);
                        Assert.Equal(StaticTestGuids.SystemSettingsId, (Guid)patchedPool.systemSettingsId);
                        Assert.Equal("Patch Test", (string)patchedPool.description);
                    }
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.DeleteSystemSettingsQuery());
                    }
                }
            }
        }

        [Fact]
        public async void Delete_Returns_204_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {
                    Guid productPoolId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        // Populate DB
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.PopulateSystemSettingsQuery(StaticTestGuids.SystemSettingsId));
                        productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool());
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage response = await client.DeleteAsync("/api/v2/productpool/" + productPoolId);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.DeleteSystemSettingsQuery());
                    }
                }
            }
        }

        [Fact]
        public async void Put_Returns_BadRequestErrorMessageResult_when_route_Id_and_Model_Id_are_different()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    ProductPool productPool = new ProductPool
                    {
                        Id = new Guid("6bbd2f72-94a9-453b-aa28-cff702e8fa4a"),
                        Descriptions = new List<MultilanguageText>
                        {
                            new MultilanguageText("de-DE","ApiController Put Test Pool")
                        },
                        Names = new List<MultilanguageText>
                        {
                            new MultilanguageText("de-DE","ApiController Put Test Pool")
                        },
                        Key = 2,
                        SystemSettingsId = StaticTestGuids.SystemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage response = await client.PutAsync("/api/v2/productpool/126e57fa-d9ec-40e3-ae51-8480e9cd9c05", httpContent);

                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                }
                finally
                {
                    dbConnection.Init(databaseKey);
                    await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                    await dbConnection.ExecuteAsync(SystemSettingsQueries.DeleteSystemSettingsQuery());
                }
            }
        }

        [Fact]
        public async void Post_Returns_422_if_required_prop_is_missing()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                ProductPool productPool = new ProductPool
                {
                    Key = 1,
                    SystemSettingsId = StaticTestGuids.SystemSettingsId
                };
                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");
                HttpClient client = application.CreateClient();
                CreateBasicClientWithAuth(client);

                HttpResponseMessage response = await client.PostAsync("/api/v2/productpool/", httpContent);

                Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
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
                HttpResponseMessage response = await client.GetAsync("/api/v2/productpool/" + "fab8c985-6147-4eba-b2c7-5f7012c4aeeb");
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
                HttpResponseMessage response = await client.GetAsync("/api/v2/productpool");
                string message = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                dynamic pools = JArray.Parse(message);
                Assert.Equal(0, pools.Count);
            }
        }

        [Fact]
        public async void Put_Returns_422_if_required_prop_is_missing()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                ProductPool productPool = new ProductPool
                {
                    Names = new List<MultilanguageText> { new MultilanguageText("de-DE", "ApiController Test Pool") },
                    SystemSettingsId = StaticTestGuids.SystemSettingsId
                };
                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");
                HttpClient client = application.CreateClient();
                CreateBasicClientWithAuth(client);

                HttpResponseMessage response = await client.PostAsync("/api/v2/productpool/", httpContent);

                Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            }
        }

        [Fact]
        public async void Put_Returns_404_If_given_Id_not_found()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {

                try
                {
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    ProductPool productPool = new ProductPool
                    {
                        Id = new Guid("82a4252e-c58f-49d0-8476-b7e1a5fa4b11"),
                        Descriptions = new List<MultilanguageText>
                        {
                            new MultilanguageText("de-DE","ApiController Put Test Pool")
                        },
                        Names = new List<MultilanguageText>
                        {
                            new MultilanguageText("de-DE","ApiController Put Test Pool")
                        },
                        Key = 2,
                        SystemSettingsId = StaticTestGuids.SystemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("/api/v2/productpool/82a4252e-c58f-49d0-8476-b7e1a5fa4b11", httpContent);
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                        await dbConnection.ExecuteAsync(SystemSettingsQueries.DeleteSystemSettingsQuery());
                    }
                }
            }
        }

        [Fact]
        public async void Delete_Returns_404_If_given_Id_not_found()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage response = await client.DeleteAsync("/api/v2/productpool/82a4252e-c58f-49d0-8476-b7e1a5fa4b11");
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }
}
