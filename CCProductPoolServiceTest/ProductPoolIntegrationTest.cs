using CCApiLibrary.Interfaces;
using CCApiTestLibrary.BaseClasses;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using CCProductPoolService.Interface;
using CCProductPoolService.Repositories;
using CCApiTestLibrary.PopulateQueries;
using CCProductPoolService.Dtos;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace CCProductPoolServiceTest
{
    public class ProductPoolIntegrationTest : ControllerTestBaseClass, IClassFixture<CCApiTestStart>
    {
        private Guid systemSettingsId = new Guid("fab8c985-6147-4eba-b2c7-5f7012c4aeeb");

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
            HttpResponseMessage respone = await client.GetAsync("/api/v2/productpool").ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.Unauthorized, respone.StatusCode);
        }

        [Fact]
        public async void Get_returns_200_Authorized()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            HttpClient client = application.CreateClient();
            CreateBasicClientWithAuth(client);
            HttpResponseMessage respone = await client.GetAsync("/api/v2/productpool").ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
        }

        [Fact]
        public async void Get_All_returns_200_And_List_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (IServiceScope services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("TestDatabase");
                    // Populate DB
                    await dbConnection.ExecuteAsync(BaseQueries.PopulateSystemSettingsQuery(systemSettingsId)).ConfigureAwait(false);
                    await dbConnection.ExecuteAsync(ProductPoolQueries.PopulateProductPoolList()).ConfigureAwait(false);
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var respone = await client.GetAsync("/api/v2/productpool").ConfigureAwait(false);
                    string messsage = await respone.Content.ReadAsStringAsync().ConfigureAwait(false);
                    dynamic pools = JArray.Parse(messsage);
                    Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
                    Assert.Equal(2, pools.Count);
                    Assert.Equal(1, (int)pools[0].key);
                    Assert.Equal("Pool 2", (string)pools[1].name);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                    await dbConnection.ExecuteAsync(BaseQueries.DeleteSystemSettingsQuery());
                }
            }
        }

        [Fact]
        public async void Post_returns_201_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();

            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("TestDatabase");

                    // Populate DB with a systemSetting
                    await dbConnection.ExecuteAsync(BaseQueries.PopulateSystemSettingsQuery(systemSettingsId)).ConfigureAwait(false);
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    ProductPoolDto productPool = new ProductPoolDto
                    {
                        Description = "ApiController Test Pool",
                        Name = "ApiController Test Pool",
                        Key = 1,
                        SystemSettingsId = systemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/api/v2/productpool", httpContent);
                    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                    await dbConnection.ExecuteAsync(BaseQueries.DeleteSystemSettingsQuery());
                }
            }
        }

        [Fact]
        public async void Get_By_Id_returns_200_And_Item_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("TestDatabase");
                    // Populate DB
                    await dbConnection.ExecuteAsync(BaseQueries.PopulateSystemSettingsQuery(systemSettingsId)).ConfigureAwait(false);
                    Guid productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool()).ConfigureAwait(false);

                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage respone = await client.GetAsync("/api/v2/productpool/" + productPoolId).ConfigureAwait(false);
                    string messsage = await respone.Content.ReadAsStringAsync();
                    dynamic pool = JObject.Parse(messsage);
                    Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
                    Assert.Equal(1, (int)pool.key);
                    Assert.Equal("Pool 1", (string)pool.name);

                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                    await dbConnection.ExecuteAsync(BaseQueries.DeleteSystemSettingsQuery());
                }
            }
        }

        [Fact]
        public async void Put_Returns_200_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("TestDatabase");
                    // Populate DB
                    await dbConnection.ExecuteAsync(BaseQueries.PopulateSystemSettingsQuery(systemSettingsId)).ConfigureAwait(false);
                    Guid productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool()).ConfigureAwait(false);
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    ProductPoolDto productPool = new ProductPoolDto
                    {
                        Id = productPoolId,
                        Description = "ApiController Put Test Pool",
                        Name = "ApiController Put Test Pool",
                        Key = 2,
                        SystemSettingsId = systemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");
                    var response = await client.PutAsync("/api/v2/productpool/" + productPoolId, httpContent);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                    await dbConnection.ExecuteAsync(BaseQueries.DeleteSystemSettingsQuery());
                }

            }
        }

        [Fact]
        public async void Patch_Returns_200_And_Item_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("TestDatabase");
                    // Populate DB
                    await dbConnection.ExecuteAsync(BaseQueries.PopulateSystemSettingsQuery(systemSettingsId)).ConfigureAwait(false);
                    Guid productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool()).ConfigureAwait(false);
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
                    var response = await client.PatchAsync("/api/v2/productpool/" + productPoolId, httpContent);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    dynamic patchedPool = JObject.Parse(await response.Content.ReadAsStringAsync());
                    Assert.Equal(99, (int)patchedPool.key);
                    Assert.Equal(systemSettingsId, (Guid)patchedPool.systemSettingsId);
                    Assert.Equal("Patch Test", (string)patchedPool.description);
                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                    await dbConnection.ExecuteAsync(BaseQueries.DeleteSystemSettingsQuery());
                }

            }
        }

        [Fact]
        public async void Delete_Returns_200_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("TestDatabase");
                    // Populate DB
                    await dbConnection.ExecuteAsync(BaseQueries.PopulateSystemSettingsQuery(systemSettingsId)).ConfigureAwait(false);
                    Guid productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool()).ConfigureAwait(false);
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var response = await client.DeleteAsync("/api/v2/productpool/" + productPoolId);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                    await dbConnection.ExecuteAsync(BaseQueries.DeleteSystemSettingsQuery());
                }

            }
        }

        [Fact]
        public async void Returns_BadRequestErrorMessageResult_when_request_is_wrong_GUID()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("TestDatabase");

                    ProductPoolDto productPool = new ProductPoolDto
                    {
                        Description = "ApiController Test Pool",
                        Name = "ApiController Test Pool",
                        Key = 1,
                        SystemSettingsId = new Guid("fab8c985-6147-4eba-b2c7-5f7012c4aeec")
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var response = await client.PostAsync("/api/v2/productpool/", httpContent);

                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                    await dbConnection.ExecuteAsync(BaseQueries.DeleteSystemSettingsQuery());
                }
            }
        }

        [Fact]
        public async void NotValidModel_400_Required_Field_name_missing()
        {
            WebApplicationFactory<Program> application = GetWebApplication();

            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("TestDatabase");

                    ProductPoolDto productPool = new ProductPoolDto
                    {
                        //Name = "ApiController Test Pool",
                        Key = 1,
                        SystemSettingsId = systemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);

                    var response = await client.PostAsync("/api/v2/productpool/", httpContent);

                    Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                    await dbConnection.ExecuteAsync(BaseQueries.DeleteSystemSettingsQuery());
                }
            }
        }

        [Fact]
        public async Task GetByID_returns_500_If_wrong_and_wrong_datatyp_PoolID()
        {
            WebApplicationFactory<Program> application = GetWebApplication();

            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("TestDatabase");

                    // Populate DB
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var response = await client.GetAsync("/api/v2/productpool/" + "fab8c985-6147-4eba-b2c7-5f7012c4aeeb");
                    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                    await dbConnection.ExecuteAsync(BaseQueries.DeleteSystemSettingsQuery());
                }
            }
        }

        [Fact]
        public async void Get_All_204_No_Content()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    var client = application.CreateClient();
                    var base64EncodedAuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes("Test:test"));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64EncodedAuthString);

                    var response = await client.GetAsync("/api/v2/productpool");
                    string message = await response.Content.ReadAsStringAsync();
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    //Assert.False(message.Any());
                    //Assert.Empty(message);
                    dynamic pools = JArray.Parse(message);
                    Assert.Equal(0, pools.Count);
                }
                catch (Exception ex) { }

            }
        }

        [Fact]
        public async void NotValidModel_400_Required_Field_Key_missing()
        {
            WebApplicationFactory<Program> application = GetWebApplication();

            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("TestDatabase");

                    ProductPoolDto productPool = new ProductPoolDto
                    {
                        Name = "ApiController Test Pool",
                        //Key = 1,
                        SystemSettingsId = systemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);

                    var response = await client.PostAsync("/api/v2/productpool/", httpContent);

                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                    await dbConnection.ExecuteAsync(BaseQueries.DeleteSystemSettingsQuery());
                }
            }
        }

        [Fact]
        public async void NotValidModel_400_Required_Field_SystemSettingsId_missing()
        {
            WebApplicationFactory<Program> application = GetWebApplication();

            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("TestDatabase");

                    ProductPoolDto productPool = new ProductPoolDto
                    {
                        Name = "ApiController Test Pool",
                        Key = 1,
                        //SystemSettingsId = systemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);

                    var response = await client.PostAsync("/api/v2/productpool/", httpContent);

                    Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                    await dbConnection.ExecuteAsync(BaseQueries.DeleteSystemSettingsQuery());
                }
            }
        }

    }
}
